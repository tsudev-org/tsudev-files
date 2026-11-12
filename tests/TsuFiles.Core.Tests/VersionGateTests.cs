// TsuFiles: bắt buộc cập nhật (phien-ban.json, chuẩn hệ sinh thái - phiếu 20261112-04): đọc tệp, quyết định, tải tệp.

using System.Net;
using System.Net.Sockets;
using System.Text;
using TsuFiles.Core;
using TsuFiles.Core.Updates;

namespace TsuFiles.Core.Tests;

public sealed class VersionGateTests
{
    private static byte[] Bytes(string text) => Encoding.UTF8.GetBytes(text);

    [Fact]
    public void A_well_formed_file_is_read()
    {
        var file = VersionGate.Parse(Bytes("""{"schema":1,"phienBan":"2.4.1","moiNhat":2,"toiThieu":1}"""));
        Assert.Equal(new VersionFile("2.4.1", 2, 1), file);
    }

    [Theory]
    [InlineData("")]
    [InlineData("không phải JSON")]
    [InlineData("[]")]
    [InlineData("""{"schema":2,"phienBan":"2.4.1","moiNhat":2,"toiThieu":1}""")]
    [InlineData("""{"phienBan":"2.4.1","moiNhat":2,"toiThieu":1}""")]
    [InlineData("""{"schema":1,"moiNhat":2,"toiThieu":1}""")]
    [InlineData("""{"schema":1,"phienBan":"2.4","moiNhat":2,"toiThieu":1}""")]
    [InlineData("""{"schema":1,"phienBan":"https://x/","moiNhat":2,"toiThieu":1}""")]
    [InlineData("""{"schema":1,"phienBan":"2.4.1","moiNhat":"2","toiThieu":1}""")]
    [InlineData("""{"schema":1,"phienBan":"2.4.1","moiNhat":2,"toiThieu":0}""")]
    [InlineData("""{"schema":1,"phienBan":"2.4.1","moiNhat":1,"toiThieu":2}""")]
    [InlineData("""{"schema":1,"phienBan":"2.4.1","moiNhat":2.5,"toiThieu":1}""")]
    [InlineData("""{"schema":1,"phienBan":"2.4.1","moiNhat":99999999999,"toiThieu":1}""")]
    public void A_malformed_file_is_ignored(string json) => Assert.Null(VersionGate.Parse(Bytes(json)));

    [Fact]
    public void An_oversized_file_is_ignored()
    {
        var padding = new string(' ', VersionGate.MaxBytes);
        Assert.Null(VersionGate.Parse(Bytes("""{"schema":1,"phienBan":"2.4.1","moiNhat":2,"toiThieu":1}""" + padding)));
    }

    [Theory]
    [InlineData(5, 5, 1, 0, VersionGateResult.UpToDate)]
    [InlineData(5, 7, 1, 0, VersionGateResult.Optional)]
    [InlineData(5, 7, 5, 0, VersionGateResult.Optional)]
    [InlineData(5, 7, 6, 0, VersionGateResult.Required)]
    [InlineData(6, 6, 1, 0, VersionGateResult.UpToDate)]
    [InlineData(9, 7, 6, 0, VersionGateResult.UpToDate)]
    [InlineData(5, 7, 1, 6, VersionGateResult.Required)]   // tệp bị sửa lùi: mức đã lưu vẫn chặn
    public void Decides_from_the_build_numbers(int current, int latest, int minimum, int stored, VersionGateResult expected) =>
        Assert.Equal(expected, VersionGate.Decide(current, new VersionFile("9.9.9", latest, minimum), stored));

    [Fact]
    public void Offline_the_stored_minimum_still_blocks()
    {
        Assert.Equal(VersionGateResult.Unknown, VersionGate.Decide(5, null, 0));
        Assert.Equal(VersionGateResult.Unknown, VersionGate.Decide(5, null, 5));
        Assert.Equal(VersionGateResult.Required, VersionGate.Decide(5, null, 6));
    }

    [Fact]
    public void The_stored_minimum_only_rises()
    {
        Assert.Equal(3, VersionGate.RaiseMinimum(0, new VersionFile("1.0.0", 4, 3)));
        Assert.Equal(5, VersionGate.RaiseMinimum(5, new VersionFile("1.0.0", 4, 3)));
        Assert.Equal(5, VersionGate.RaiseMinimum(5, null));
    }

    [Fact]
    public void The_addresses_are_fixed_on_the_product_site()
    {
        Assert.Equal("https://files.tsudev.com/phien-ban.json", VersionGate.Url.ToString());
        Assert.Equal("https://files.tsudev.com/tai-ve/", VersionGate.DownloadPageUrl);
        Assert.True(ForkDefaults.BuildNumber >= 2);
    }

    private sealed class Stub(Func<HttpRequestMessage, HttpResponseMessage> answer) : HttpMessageHandler
    {
        public int Calls { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(answer(request));
        }
    }

    private static HttpResponseMessage Ok(string body) =>
        new(HttpStatusCode.OK) { Content = new ByteArrayContent(Bytes(body)) };

    [Fact]
    public async Task Fetch_reads_a_good_answer()
    {
        var stub = new Stub(_ => Ok("""{"schema":1,"phienBan":"2.4.1","moiNhat":2,"toiThieu":1}"""));
        Assert.Equal(new VersionFile("2.4.1", 2, 1), await VersionGate.FetchAsync(stub, cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(1, stub.Calls);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    [InlineData(HttpStatusCode.Found)]
    [InlineData(HttpStatusCode.MovedPermanently)]
    public async Task Fetch_ignores_anything_but_200(HttpStatusCode status)
    {
        var stub = new Stub(_ =>
        {
            var response = new HttpResponseMessage(status) { Content = new ByteArrayContent(Bytes("""{"schema":1,"phienBan":"2.4.1","moiNhat":2,"toiThieu":1}""")) };
            response.Headers.Location = new Uri("https://example.invalid/phien-ban.json");
            return response;
        });
        Assert.Null(await VersionGate.FetchAsync(stub, cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(1, stub.Calls);
    }

    [Fact]
    public async Task Fetch_ignores_an_oversized_answer()
    {
        var big = """{"schema":1,"phienBan":"2.4.1","moiNhat":2,"toiThieu":1}""" + new string(' ', VersionGate.MaxBytes * 2);
        Assert.Null(await VersionGate.FetchAsync(new Stub(_ => Ok(big)), cancellationToken: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Fetch_survives_a_network_error() =>
        Assert.Null(await VersionGate.FetchAsync(new Stub(_ => throw new HttpRequestException("no route")),
            cancellationToken: TestContext.Current.CancellationToken));

    [Fact]
    public async Task Fetch_survives_an_unreachable_server()
    {
        // Một cổng cục bộ vừa đóng: kết nối bị từ chối ngay.
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        Assert.Null(await VersionGate.FetchAsync(url: new Uri($"http://127.0.0.1:{port}/phien-ban.json"),
            cancellationToken: TestContext.Current.CancellationToken));
    }
}

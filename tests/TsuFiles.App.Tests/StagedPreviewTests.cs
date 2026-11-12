// The Home list draws a picture of each PDF's first page, as its card appears.

using Avalonia.Headless.XUnit;
using PdfSharp.Pdf;
using TsuFiles.App.Services;
using TsuFiles.App.ViewModels.Pages;
using Xunit;

namespace TsuFiles.App.Tests;

public class StagedPreviewTests
{
    [AvaloniaFact]
    public async Task A_pdf_added_to_the_list_gets_a_picture_of_its_first_page()
    {
        TestServices.EnsureInitialized();
        var folder = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "filee-preview-" + Guid.NewGuid().ToString("N")[..8])).FullName;
        var path = Path.Combine(folder, "page.pdf");
        using (var document = new PdfDocument())
        {
            document.AddPage();
            document.Save(path);
        }

        var staged = AppHost.Get<StagedFiles>();
        staged.Clear();
        staged.Add([path]);
        var card = Assert.Single(staged.Cards);

        await card.LoadPreviewAsync();

        Assert.False(card.Oversized);
        Assert.NotNull(card.Thumbnail);
        staged.Clear();
    }
}

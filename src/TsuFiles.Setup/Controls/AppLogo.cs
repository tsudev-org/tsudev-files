// The TsuFiles logo (assets/brand, generated into src/TsuFiles.App/Assets/Icons/filee.png by
// tools/thuong-hieu/tao-tai-san.py), with a short entrance: it springs up from a smaller size, fades in and turns a
// little into place. Without animations (or after 0.9 s) it is simply the logo.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace TsuFiles.Setup.Controls;

public sealed class AppLogo : Control
{
    /// <summary>Plays the entrance when shown; off draws the finished logo.</summary>
    public static readonly StyledProperty<bool> AnimateProperty =
        AvaloniaProperty.Register<AppLogo, bool>(nameof(Animate), true);

    private static readonly TimeSpan Duration = TimeSpan.FromMilliseconds(900);
    private static readonly Lazy<Bitmap> Image =
        new(() => new Bitmap(AssetLoader.Open(new Uri("avares://TsuFiles.Setup/Assets/Icons/filee.png"))));

    private readonly FrameLoop _loop;
    private double _progress;

    static AppLogo()
    {
        AffectsRender<AppLogo>(AnimateProperty);
    }

    public AppLogo()
    {
        // The logo is never mirrored, also in Arabic.
        FlowDirection = FlowDirection.LeftToRight;
        _progress = SetupMotion.Enabled ? 0 : 1;
        _loop = new FrameLoop(this, elapsed =>
        {
            _progress = Math.Min(1, elapsed / Duration);
            InvalidateVisual();
            return _progress < 1;
        });
    }

    public bool Animate { get => GetValue(AnimateProperty); set => SetValue(AnimateProperty, value); }

    /// <summary>Plays the entrance again.</summary>
    public void Replay()
    {
        if (!SetupMotion.Enabled || !Animate)
            return;
        _progress = 0;
        _loop.Restart();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var side = Math.Min(double.IsInfinity(availableSize.Width) ? 96 : availableSize.Width,
            double.IsInfinity(availableSize.Height) ? 96 : availableSize.Height);
        return new Size(side, side);
    }

    public override void Render(DrawingContext context)
    {
        var progress = Animate && SetupMotion.Enabled ? _progress : 1;
        var side = Math.Min(Bounds.Width, Bounds.Height);
        if (side <= 0 || progress <= 0)
            return;
        var center = new Point(Bounds.Width / 2, Bounds.Height / 2);
        var grow = 0.6 + 0.4 * BackOut(progress);
        var turn = (1 - EaseOut(progress)) * -0.35;
        var transform = Matrix.CreateTranslation(-center.X, -center.Y)
                        * Matrix.CreateScale(grow, grow)
                        * Matrix.CreateRotation(turn)
                        * Matrix.CreateTranslation(center.X, center.Y);
        var rect = new Rect(center.X - side / 2, center.Y - side / 2, side, side);
        using (context.PushTransform(transform))
        using (context.PushOpacity(Math.Min(1, progress * 2)))
            context.DrawImage(Image.Value, rect);
    }

    internal static double EaseOut(double t) => 1 - Math.Pow(1 - t, 3);

    internal static double BackOut(double t)
    {
        const double c1 = 1.70158;
        const double c3 = c1 + 1;
        return 1 + c3 * Math.Pow(t - 1, 3) + c1 * Math.Pow(t - 1, 2);
    }
}

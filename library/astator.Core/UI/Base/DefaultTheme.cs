using Android.Graphics;

namespace astator.Core.UI.Base;

/// <summary>
/// view默认值
/// </summary>
public struct DefaultTheme
{
    public static Color LayoutBackgroundColor { get; set; } = Color.Transparent;
    public static Color ColorPrimary { get; set; } = Color.ParseColor("#2b0b98");
    public static Color ColorHint { get; set; } = Color.ParseColor("#80808080");
    public static Color TextColor { get; set; } = Color.ParseColor("#4a4a4d");
    public static int TextSize { get; set; } = 12;
}

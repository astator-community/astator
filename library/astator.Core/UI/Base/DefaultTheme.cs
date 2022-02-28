using Android.Graphics;

namespace astator.Core.UI.Base;

/// <summary>
/// view默认值
/// </summary>
public struct DefaultTheme
{
    public static int TextSize { get; set; } = 12;
    public static Color ColorPrimary { get; set; } = Color.ParseColor("#2b0b98");
    public static Color ColorPrimaryDark { get; set; } = Color.ParseColor("#2b0b98");
    public static Color ColorAccent { get; set; } = Color.ParseColor("#2b0b98");
    public static Color TextColorPrimary { get; set; } = Color.ParseColor("#4a4a4d");
    public static Color LayoutBackground { get; set; } = Color.Transparent;
    public static Color ColorHint { get; set; } = Color.ParseColor("#80808080");
}

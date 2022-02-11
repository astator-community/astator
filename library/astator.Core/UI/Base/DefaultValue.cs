using Android.Graphics;

namespace astator.Core.UI.Base;

/// <summary>
/// view默认值
/// </summary>
public struct DefaultValue
{
    public static int TextSize { get; set; } = 12;
    public static Color TextColor { get; set; } = Color.ParseColor("#4a4a4d");
    public static Color BackgroundColor { get; set; } = Color.Transparent;
    public static Color TitleColor { get; set; } = Color.ParseColor("#2a2a2d");
    public static Color HintColor { get; set; } = Color.ParseColor("#80808080");
    public static Color AccentColor { get; set; } = Color.ParseColor("#2b0b98");
}

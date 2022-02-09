using Android.Graphics;

namespace astator.Core.UI.Base;

/// <summary>
/// view默认值
/// </summary>
public struct DefaultValue
{
    public static int TextSize { get; set; } = 12;
    public static Color TextColor { get; set; } = Color.ParseColor("#4a4a4d");
    public static Color BackgroundColor { get; set; } = Color.ParseColor("#ffffff");
}

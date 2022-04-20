using astator.Core.Graphics;
using System.Collections.Generic;

namespace astator.Core.Accessibility;

public class SearcherArgs
{
    public string Id { get; set; } = null;
    public string PackageName { get; set; } = null;
    public string PackageNameStartsWith { get; set; } = null;
    public string PackageNameEndsWith { get; set; } = null;
    public string ClassName { get; set; } = null;
    public string Text { get; set; } = null;
    public string Description { get; set; } = null;
    public Rect? Bounds { get; set; } = null;
    public bool? Checkable { get; set; } = null;
    public bool? Clickable { get; set; } = null;
    public bool? Editable { get; set; } = null;
    public bool? Enabled { get; set; } = null;
    public bool? Focusable { get; set; } = null;
    public int? Depth { get; set; } = null;
    public int? DrawingOrder { get; set; } = null;

    public Dictionary<string, object> GetEnabledArgs()
    {
        var result = new Dictionary<string, object>();
        foreach (var prop in GetType().GetProperties())
        {
            var value = prop.GetValue(this);
            if (value is not null)
            {
                result.Add(prop.Name, value);
            }
        }
        return result;
    }
}

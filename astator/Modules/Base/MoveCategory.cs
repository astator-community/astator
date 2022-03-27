using System.Collections.ObjectModel;


namespace astator.Modules.Base;



public class MoveCategory
{
#nullable enable
    public bool? AccessibilityFocused { get; set; }
    public Core.Graphics.Rect? Bounds { get; set; }
    public bool? Checkable { get; set; }
    public bool? Checked { get; set; }
    public string? ClassName { get; set; }
    public bool? Clickable { get; set; }
    public int? Column { get; set; }
    public int? ColumnCount { get; set; }
    public int? ColumnSpan { get; set; }
    public bool? ContextClickable { get; set; }
    public int? Depth { get; set; }
    public string? Desc { get; set; }
    public bool? Dismissable { get; set; }
    public int? DrawingOrder { get; set; }
    public bool? Editable { get; set; }
    public bool? Enabled { get; set; }
    public bool? Focused { get; set; }
    public int? IndexInParent { get; set; }

    private string? id;
    public string? Id
    {
        get => this.id;
        set
        {
            this.id = value?[(value.LastIndexOf("/") + 1)..];
        }
    }
    public bool? LongClickable { get; set; }
    public string? PackageName { get; set; }
    public int? Row { get; set; }
    public int? RowCount { get; set; }
    public int? RowSpan { get; set; }
    public bool? Scrollable { get; set; }
    public bool? Selected { get; set; }
    public string? Text { get; set; }
    public bool? VisibleToUser { get; set; }
    public string Title
    {
        get
        {
            if (this.Id == null)
            {
                return $"{ this.ClassName?[(this.ClassName.LastIndexOf(".") + 1)..] }";
            }
            return $"{ this.ClassName?[(this.ClassName.LastIndexOf(".") + 1)..] }  [id: { this.Id }]";

        }
    }
    public ObservableCollection<MoveCategory>? Children { get; set; }
#nullable disable
}

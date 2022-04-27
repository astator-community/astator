using astator.Core.UI.Base;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Platform;

namespace astator.Popups;

public partial class PathRename : Popup
{
    public PathRename()
    {
        InitializeComponent();
    }

    private void Cancel_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    private void Confirm_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(this.NameEditor.Text)) return;
        Close(this.NameEditor.Text);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        var view = this.Content.Handler.PlatformView as LayoutViewGroup;
        view.ClipToOutline = true;
        view.OutlineProvider = new RadiusOutlineProvider(10);
    }

    private void Popup_Opened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        this.Size = new Size(this.Content.Width, this.Content.Height);
        this.Content.Scale = 0;
        this.Content.ScaleTo(1, 250);
    }
}
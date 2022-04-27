using astator.Core.UI.Base;
using astator.Views;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Platform;

namespace astator.Popups;

public partial class FileOptions : Popup
{
    public FileOptions(CustomCard anchor)
    {
        InitializeComponent();
        this.Anchor = anchor;
        var fileName = anchor.Tag as string;

        if (fileName.EndsWith(".csproj"))
        {
            var runProject = new CustomCard { new Label { Text = "运行项目" } };
            var buildApk = new CustomCard { new Label { Text = "打包apk" } };
            var compileDll = new CustomCard { new Label { Text = "编译dll" } };

            runProject.Clicked += ItemClicked;
            buildApk.Clicked += ItemClicked;
            compileDll.Clicked += ItemClicked;

            this.Items.Add(runProject);
            this.Items.Add(buildApk);
            this.Items.Add(compileDll);
        }
        else if (fileName.EndsWith(".cs"))
        {
            var runScript = new CustomCard { new Label { Text = "运行脚本" } };
            runScript.Clicked += ItemClicked;
            this.Items.Add(runScript);
        }
        var rename = new CustomCard { new Label { Text = "重命名" } };
        var remove = new CustomCard { new Label { Text = "删除" } };
        var share = new CustomCard { new Label { Text = "分享" } };
        var open = new CustomCard { new Label { Text = "其他应用打开" } };

        rename.Clicked += ItemClicked;
        remove.Clicked += ItemClicked;
        share.Clicked += ItemClicked;
        open.Clicked += ItemClicked;

        this.Items.Add(rename);
        this.Items.Add(remove);
        this.Items.Add(share);
        this.Items.Add(open);
    }



    private void ItemClicked(object sender, EventArgs e)
    {
        var label = sender as CustomCard;
        Close((label.Children[0] as Label).Text);
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
        this.Content.ScaleTo(1, 100);
    }
}
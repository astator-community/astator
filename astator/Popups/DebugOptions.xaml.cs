using astator.Core.UI.Base;
using CommunityToolkit.Maui.Views;
using Java.Net;
using Microsoft.Maui.Platform;

namespace astator.Popups;

public partial class DebugOptions : Popup
{
	public DebugOptions()
	{
		InitializeComponent();
	}

    private void ClientMode_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            this.Address.IsReadOnly = false;
            this.Address.Text = Core.Script.Preferences.Get("latestServerIp", string.Empty, "astator");
            this.HintMsg.Text = "填入ide的ip地址连接到VSCode, 模拟器请填入10.0.2.2";
        }
        else
        {
            this.Address.IsReadOnly = true;
            this.Address.Text = GetLocalHostAddress();
            this.HintMsg.Text = "在ide扩展中输入以下ip地址连接到astator";
        }
    }

    private static string GetLocalHostAddress()
    {
        var ie = NetworkInterface.NetworkInterfaces;
        while (ie.HasMoreElements)
        {
            var intf = ie.NextElement() as NetworkInterface;
            var enumIpAddr = intf.InetAddresses;
            while (enumIpAddr.HasMoreElements)
            {
                var inetAddress = enumIpAddr.NextElement() as InetAddress;
                if (!inetAddress.IsLoopbackAddress && inetAddress.HostAddress.StartsWith("192.168."))
                {
                    return inetAddress.HostAddress.ToString();
                }
            }
        }
        return string.Empty;
    }

    private void Cancel_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    private void Connect_Clicked(object sender, EventArgs e)
    {
        if (this.ClientMode.IsChecked)
        {
            Core.Script.Preferences.Set("latestServerIp", this.Address.Text, "astator");
        }
        Close((this.ServiceMode.IsChecked ? 0 : 1, this.Address.Text));
    }

    private void Address_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(this.Address.Text))
            this.Connect.IsEnabled = false;
        else
            this.Connect.IsEnabled = true;
    }

    private void Popup_Opened(object sender, CommunityToolkit.Maui.Core.PopupOpenedEventArgs e)
    {
        this.Size = new Size(this.Content.Width, this.Content.Height);
        this.Content.Scale = 0;
        this.Content.ScaleTo(1, 250);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        var view = this.Content.Handler.PlatformView as LayoutViewGroup;
        view.ClipToOutline = true;
        view.OutlineProvider = new RadiusOutlineProvider(10);
    }
}
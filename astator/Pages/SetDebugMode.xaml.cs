using Android.Content;
using astator.Core.Script;
using astator.Modules;
using Java.Net;

namespace astator.Pages;

public partial class SetDebugMode : Grid
{
    public Action DismissCallback { get; set; }

    public SetDebugMode()
    {
        InitializeComponent();
        this.Address.Text = GetLocalHostAddress();
    }

    private void ClientMode_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (e.Value)
        {
            this.Address.IsReadOnly = false;
            this.Address.Text = Core.Script.Preferences.Get("latestServerIp", string.Empty, "astator");
            this.HintMsg.Text = "填入VSCode所在电脑的ip地址连接到VSCode, 模拟器请填入10.0.2.2";
        }
        else
        {
            this.Address.IsReadOnly = true;
            this.Address.Text = GetLocalHostAddress();
            this.HintMsg.Text = "在VSCode插件中输入以下ip地址连接到astator";
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
        this.DismissCallback?.Invoke();
    }

    private void Connect_Clicked(object sender, EventArgs e)
    {
        if (this.ClientMode.IsChecked)
        {
            Core.Script.Preferences.Set("latestServerIp", this.Address.Text, "astator");
        }

        var intent = new Intent(Globals.AppContext, typeof(DebugService));
        intent.PutExtra("mode", this.ServiceMode.IsChecked ? 0 : 1);
        intent.PutExtra("ip", this.Address.Text);
        if (OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            Globals.AppContext.StartForegroundService(intent);
        }
        else
        {
            Globals.AppContext.StartService(intent);
        }
        this.DismissCallback?.Invoke();
    }

    private void Address_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(this.Address.Text))
            this.Connect.IsEnabled = false;
        else
            this.Connect.IsEnabled = true;
    }
}
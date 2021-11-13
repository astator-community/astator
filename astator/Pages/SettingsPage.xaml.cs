using astator.Script;
using Java.Net;
using Microsoft.Maui.Controls;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace astator.Pages
{
    public partial class SettingsPage : ContentPage
    {
        private TcpListener tcpListener;
        public SettingsPage()
        {
            InitializeComponent();
            this.NavBar.ActiveTab = "settings";
        }
        void Connect_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                var RegexStr = "^192\\.168\\.(\\d{1}|[1-9]\\d|1\\d{2}|2[0-4]\\d|25\\d)\\.(\\d{1}|[1-9]\\d|1\\d{2}|2[0-4]\\d|25\\d)$";
                var networkInterfaces = NetworkInterface.NetworkInterfaces;
                while (networkInterfaces.HasMoreElements)
                {
                    var networkInterface = networkInterfaces.NextElement() as NetworkInterface;
                    var inetAddresses = networkInterface.InetAddresses;
                    while (inetAddresses.HasMoreElements)
                    {
                        var address = inetAddresses.NextElement() as InetAddress;
                        var hostAddress = address.HostAddress;
                        var matcher = Regex.Match(hostAddress, RegexStr);
                        if (matcher.Success)
                        {
                            Console.WriteLine(hostAddress);
                            this.tcpListener = new TcpListener(IPAddress.Parse(hostAddress), 1024);
                            this.tcpListener.Start();
                            this.tcpListener.BeginAcceptTcpClient(new AsyncCallback(ConnectCallback), this.tcpListener);
                        }

                    }
                }
            }
            else
            {
                this.tcpListener?.Stop();

            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            var listener = (TcpListener)ar.AsyncState;

            if (listener.Server == null || !listener.Server.IsBound)
            {
                return;
            }

            var client = listener.EndAcceptTcpClient(ar);

            Task.Run(async () =>
            {
                var stream = client.GetStream();
                while (true)
                {
                    Thread.Sleep(50);
                    var data = await Stick.ReadPackAsync(stream);
                    Console.WriteLine(data.Key);

                }

            });

            listener.BeginAcceptTcpClient(new AsyncCallback(ConnectCallback), listener);
        }
    }
}
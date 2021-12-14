using Java.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astator.Core
{
    public class Utils
    {
        public static string GetgetLocalIPAddress()
        {
            var ie = NetworkInterface.NetworkInterfaces;
            while (ie.HasMoreElements)
            {
                var intf = ie.NextElement() as NetworkInterface;
                var enumIpAddr = intf.InetAddresses;
                while (enumIpAddr.HasMoreElements)
                {
                    var inetAddress = enumIpAddr.NextElement() as InetAddress;
                    if (!inetAddress.IsLoopbackAddress && inetAddress is Inet4Address && inetAddress.HostAddress.StartsWith("192.168"))
                    {
                        return inetAddress.HostAddress.ToString();
                    }
                }
            }
            ie = NetworkInterface.NetworkInterfaces;
            while (ie.HasMoreElements)
            {
                var intf = ie.NextElement() as NetworkInterface;
                var enumIpAddr = intf.InetAddresses;
                while (enumIpAddr.HasMoreElements)
                {
                    var inetAddress = enumIpAddr.NextElement() as InetAddress;
                    if (!inetAddress.IsLoopbackAddress && inetAddress is Inet4Address && inetAddress.HostAddress.ToString() != "127.0.0.1")
                    {
                        return inetAddress.HostAddress.ToString();
                    }
                }
            }
            return null;
        }


    }
}

using Java.Net;

namespace astator.Core
{
    public class Utils
    {
        public static string GetLocalHostAddress()
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

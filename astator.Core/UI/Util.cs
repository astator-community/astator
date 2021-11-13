using Android.Views;
using astator.Core.Exceptions;
using astator.Core.Script;
using Java.Net;
using System;
using System.Collections.Generic;

namespace astator.Core.UI
{
    public enum RequestFlags
    {
        media_projection = 1000,
        floaty_window = 1001,
        ExternalStorage = 1002
    }
    public static class DefaultValue
    {
        public static int TextSize { get; } = 6;
        public static string TextColor { get; } = "#4a4a4d";
        public static string BackgroundColor { get; } = "#ffffff";
    }
    public static class Util
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
        public static Type GetType(string value)
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.GetType(value) is not null)
                {
                    return asm.GetType(value);
                }
            }
            return null;
        }
        public static T TypeParse<T>(object value)
        {
            var str = value.ToString().Trim().ToLower();
            var properties = typeof(T).GetProperties();
            foreach (var p in properties)
            {
                if (p.Name.ToString().ToLower().Equals(str))
                {
                    return (T)p.GetValue(null);
                }
            }
            throw new AttributeNotExistException(str);
        }
        public static T EnumParse<T>(object value)
        {
            var list = new List<int>();
            if (value is string strs)
            {
                var array = strs.Trim().ToLower().Split("|");
                foreach (var item in Enum.GetNames(typeof(T)))
                {
                    foreach (var str in array)
                    {
                        if (item.ToLower().Equals(str))
                        {
                            list.Add((int)Enum.Parse(typeof(T), item));
                        }
                    };
                }
                if (list.Count != 0)
                {
                    var result = 0;
                    foreach (var v in list)
                    {
                        result |= v;
                    }
                    return (T)(object)result;
                }
                else
                {
                    throw new AttributeNotExistException(strs);
                }
            }
            throw new AttributeNotExistException(value.ToString() ?? string.Empty);
        }
        public static int DpParse(object value)
        {
            return (int)(Devices.Dp * float.Parse(value.ToString().Trim()));
        }

        internal static void OnListener(this View view, string key, object listener)
        {
            switch (key)
            {
                case "click":
                    {
                        if (listener is OnClickListener temp)
                            view.SetOnClickListener(temp);
                        break;
                    }

                case "longClick":
                    {
                        if (listener is OnLongClickListener temp)
                            view.SetOnLongClickListener(temp);
                        break;
                    }
                case "touch":
                    {
                        if (listener is OnTouchListener temp)
                            view.SetOnTouchListener(temp);
                        break;
                    }

                default: throw new ArgumentException(key + ": 未定义属性!");
            }
        }
        internal static void LayoutOnListener(this View view, string key, object listener)
        {
            switch (key)
            {
                case "scrollChange":
                    {
                        if (listener is OnScrollChangeListener temp)
                            view.SetOnScrollChangeListener(temp);
                        break;
                    }
                default:
                    view.OnListener(key, listener);
                    break;
            }
        }

    }
}

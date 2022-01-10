using Android.Views;
using astator.Core.Exceptions;
using System;
using System.Collections.Generic;
using static astator.Core.Globals;

namespace astator.Core.UI
{
    /// <summary>
    /// view默认值
    /// </summary>
    public static class DefaultValue
    {
        public static int TextSize { get; } = 8;
        public static string TextColor { get; } = "#4a4a4d";
        public static string BackgroundColor { get; } = "#ffffff";
    }
    public static class Util
    {
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

        public static void OnListener(this View view, string key, object listener)
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
        public static void LayoutOnListener(this View view, string key, object listener)
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

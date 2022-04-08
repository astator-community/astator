using System;
using astator.Core.Exceptions;
using MauiPreferences = Microsoft.Maui.Essentials.Preferences;

namespace astator.Core.Script;


public class Preferences
{
    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="defaultValue">默认值, 当key不存在时返回</param>
    /// <param name="sharedName">共享名称</param>
    /// <returns></returns>
    /// <exception cref="TypeNotSupportedException"></exception>
    public static T Get<T>(string key, T defaultValue, string sharedName = null)
    {
        if (sharedName is null)
        {
            return defaultValue switch
            {
                string s => (T)(object)MauiPreferences.Get(key, s),
                bool b => (T)(object)MauiPreferences.Get(key, b),
                int i => (T)(object)MauiPreferences.Get(key, i),
                double d => (T)(object)MauiPreferences.Get(key, d),
                float f => (T)(object)MauiPreferences.Get(key, f),
                long l => (T)(object)MauiPreferences.Get(key, l),
                DateTime dt => (T)(object)MauiPreferences.Get(key, dt),
                _ => throw new TypeNotSupportedException(defaultValue.GetType().Name)
            };
        }
        else
        {
            return defaultValue switch
            {
                string s => (T)(object)MauiPreferences.Get(key, s, sharedName),
                bool b => (T)(object)MauiPreferences.Get(key, b, sharedName),
                int i => (T)(object)MauiPreferences.Get(key, i, sharedName),
                double d => (T)(object)MauiPreferences.Get(key, d, sharedName),
                float f => (T)(object)MauiPreferences.Get(key, f, sharedName),
                long l => (T)(object)MauiPreferences.Get(key, l, sharedName),
                DateTime dt => (T)(object)MauiPreferences.Get(key, dt, sharedName),
                _ => throw new TypeNotSupportedException(defaultValue.GetType().Name)
            };
        }
    }

    /// <summary>
    /// 设置数据
    /// </summary>
    /// <param name="sharedName">共享名称</param>
    /// <exception cref="TypeNotSupportedException"></exception>
    public static void Set(string key, object value, string sharedName = null)
    {
        if (sharedName is null)
        {
            switch (value)
            {
                case string s:
                    {
                        MauiPreferences.Set(key, s);
                        break;
                    }
                case bool b:
                    {
                        MauiPreferences.Set(key, b);
                        break;
                    }
                case int i:
                    {
                        MauiPreferences.Set(key, i);
                        break;
                    }
                case double d:
                    {
                        MauiPreferences.Set(key, d);
                        break;
                    }
                case float f:
                    {
                        MauiPreferences.Set(key, f);
                        break;
                    }
                case long l:
                    {
                        MauiPreferences.Set(key, l);
                        break;
                    }
                case DateTime dt:
                    {
                        MauiPreferences.Set(key, dt);
                        break;
                    }
                default:
                    {
                        throw new TypeNotSupportedException(value.GetType().Name);
                    }
            };
        }
        else
        {
            switch (value)
            {
                case string s:
                    {
                        MauiPreferences.Set(key, s, sharedName);
                        break;
                    }
                case bool b:
                    {
                        MauiPreferences.Set(key, b, sharedName);
                        break;
                    }
                case int i:
                    {
                        MauiPreferences.Set(key, i, sharedName);
                        break;
                    }
                case double d:
                    {
                        MauiPreferences.Set(key, d, sharedName);
                        break;
                    }
                case float f:
                    {
                        MauiPreferences.Set(key, f, sharedName);
                        break;
                    }
                case long l:
                    {
                        MauiPreferences.Set(key, l, sharedName);
                        break;
                    }
                case DateTime dt:
                    {
                        MauiPreferences.Set(key, dt, sharedName);
                        break;
                    }
                default:
                    {
                        throw new TypeNotSupportedException(value.GetType().Name);
                    }
            };
        }
    }

    /// <summary>
    /// 判断key是否存在
    /// </summary>
    /// <param name="sharedName">共享名称</param>
    /// <returns></returns>
    public static bool ContainsKey(string key, string sharedName = null)
    {
        return sharedName is null ? MauiPreferences.ContainsKey(key) : MauiPreferences.ContainsKey(key, sharedName);
    }

    /// <summary>
    /// 移除一个key
    /// </summary>
    /// <param name="sharedName">共享名称</param>
    public static void Remove(string key, string sharedName = null)
    {
        if (sharedName is null)
            MauiPreferences.Remove(key);
        else
            MauiPreferences.Remove(key, sharedName);
    }

    /// <summary>
    /// 清除所有数据
    /// </summary>
    /// <param name="sharedName">共享名称</param>
    public static void Clear(string sharedName = null)
    {
        if (sharedName is null)
            MauiPreferences.Clear();
        else
            MauiPreferences.Clear(sharedName);
    }


    private readonly string sharedName = string.Empty;

    /// <summary>
    /// 创建指定共享名称的对象
    /// </summary>
    /// <param name="sharedName">共享名称</param>
    public Preferences(string sharedName)
    {
        this.sharedName = sharedName;
    }

    /// <summary>
    /// 在当前共享名称获取数据
    /// </summary>
    /// <param name="defaultValue">默认值, 当key不存在时返回</param>
    /// <returns></returns>
    /// <exception cref="TypeNotSupportedException"></exception>
    public T Get<T>(string key, T defaultValue)
    {
        return defaultValue switch
        {
            string s => (T)(object)MauiPreferences.Get(key, s, this.sharedName),
            bool b => (T)(object)MauiPreferences.Get(key, b, this.sharedName),
            int i => (T)(object)MauiPreferences.Get(key, i, this.sharedName),
            double d => (T)(object)MauiPreferences.Get(key, d, this.sharedName),
            float f => (T)(object)MauiPreferences.Get(key, f, this.sharedName),
            long l => (T)(object)MauiPreferences.Get(key, l, this.sharedName),
            DateTime dt => (T)(object)MauiPreferences.Get(key, dt, this.sharedName),
            _ => throw new TypeNotSupportedException(defaultValue.GetType().Name)
        };
    }

    /// <summary>
    /// 在当前共享名称设置数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <exception cref="TypeNotSupportedException"></exception>
    public void Set(string key, object value)
    {
        switch (value)
        {
            case string s:
                {
                    MauiPreferences.Set(key, s, this.sharedName);
                    break;
                }
            case bool b:
                {
                    MauiPreferences.Set(key, b, this.sharedName);
                    break;
                }
            case int i:
                {
                    MauiPreferences.Set(key, i, this.sharedName);
                    break;
                }
            case double d:
                {
                    MauiPreferences.Set(key, d, this.sharedName);
                    break;
                }
            case float f:
                {
                    MauiPreferences.Set(key, f, this.sharedName);
                    break;
                }
            case long l:
                {
                    MauiPreferences.Set(key, l, this.sharedName);
                    break;
                }
            case DateTime dt:
                {
                    MauiPreferences.Set(key, dt, this.sharedName);
                    break;
                }
            default:
                {
                    throw new TypeNotSupportedException(value.GetType().Name);
                }
        };
    }

    /// <summary>
    /// 在当前共享名称判断key是否存在
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool ContainsKey(string key)
    {
        return MauiPreferences.ContainsKey(key, this.sharedName);
    }

    /// <summary>
    /// 在当前共享名称移除一个key
    /// </summary>
    /// <param name="key"></param>
    public void Remove(string key)
    {
        MauiPreferences.Remove(key, this.sharedName);
    }

    /// <summary>
    /// 在当前共享名称清除所有数据
    /// </summary>
    public void Clear()
    {
        MauiPreferences.Clear(this.sharedName);
    }
}

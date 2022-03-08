using System.Collections.Generic;

namespace astator.Core.UI.Base;

/// <summary>
/// view属性参数
///  <para>例子:</para>
/// <para>var args = new ViewArgs();</para>
/// <para>args["id"] = "test";</para>
/// </summary>
public class ViewArgs
{
    private readonly Dictionary<string, object> attrs = new();
    public object this[string key]
    {
        set
        {
            if (value is not null)
            {
                this.attrs.Add(key, value);
            }
        }
        get
        {
            if (this.attrs.ContainsKey(key))
            {
                return this.attrs[key];
            }
            return null;
        }
    }

    internal void Remove(params string[] key)
    {
        foreach (var k in key)
        {
            if (this.attrs.ContainsKey(k))
            {
                this.attrs.Remove(k);
            }
        }
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        foreach (var arg in this.attrs)
        {
            yield return arg;
        }
    }
}

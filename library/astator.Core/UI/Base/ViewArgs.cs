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
    private readonly Dictionary<string, object> args = new();
    public object this[string key]
    {
        set
        {
            if (value is not null)
            {
                this.args.Add(key, value);
            }
        }
        get
        {
            if (this.args.ContainsKey(key))
            {
                return this.args[key];
            }
            return null;
        }
    }

    internal void Remove(params string[] key)
    {
        foreach (var k in key)
        {
            if (this.args.ContainsKey(k))
            {
                this.args.Remove(k);
            }
        }
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        foreach (var arg in this.args)
        {
            yield return arg;
        }
    }
}

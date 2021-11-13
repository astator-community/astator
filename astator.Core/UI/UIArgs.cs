using System.Collections.Generic;

namespace astator.Core.UI
{
    public class UIArgs
    {
        private Dictionary<string, object> args = new();
        public object this[string key]
        {
            set
            {
                if (value is not null)
                    this.args[key] = value;
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

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (var arg in this.args)
            {
                yield return arg;
            }
        }
    }
}

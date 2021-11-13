using Android.Content;
using Android.Views;
using astator.Core.UI.Layout;
using astator.Core.UI.Widget;
using System;
using System.Collections.Generic;
namespace astator.Core.UI.Floaty
{
    public class FloatyManager : IManager
    {
        public List<FloatWindow> Floatys { get; set; } = new();

        private readonly Context context;

        private readonly Dictionary<string, Dictionary<string, object>> staticListeners = new();

        private string directory = string.Empty;

        private Dictionary<string, object> childs = new();
        public object this[string key]
        {
            set
            {
                if (value is not null)
                    this.childs[key] = value;
            }
            get
            {
                if (this.childs.ContainsKey(key))
                {
                    return this.childs[key];
                }
                return null;
            }
        }

        public FloatyManager(Context context, string directory)
        {
            this.context = context;
            this.directory = directory;
        }
        public FloatWindow Show(View view, int x = 0, int y = 0)
        {
            FloatWindow floaty = new(view, x, y);
            this.Floatys.Add(floaty);
            return floaty;
        }
        public void Hide(FloatWindow floaty)
        {
            floaty.Hide();
            this.Floatys.Remove(floaty);
        }
        public void HideAll()
        {
            foreach (var floaty in this.Floatys)
            {
                floaty.Hide();
            }
            this.Floatys.Clear();
        }
        public ScriptScrollView CreateScrollView(UIArgs args = null)
        {
            var result = new ScriptScrollView(this.context, args);
            if (this.staticListeners.ContainsKey("scroll"))
            {
                foreach (var listener in this.staticListeners["scroll"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }
        public ScriptWebView CreateWebView(UIArgs args = null)
        {
            var result = new ScriptWebView(this.context, args);
            if (this.staticListeners.ContainsKey("web"))
            {
                foreach (var listener in this.staticListeners["web"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }
        public ScriptSwitch CreateSwitch(UIArgs args = null)
        {
            var result = new ScriptSwitch(this.context, args);
            if (this.staticListeners.ContainsKey("switch"))
            {
                foreach (var listener in this.staticListeners["switch"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }
        public ScriptCheckBox CreateCheckBox(UIArgs args = null)
        {
            var result = new ScriptCheckBox(this.context, args);
            if (this.staticListeners.ContainsKey("check"))
            {
                foreach (var listener in this.staticListeners["check"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }
        public ScriptImageView CreateImageView(UIArgs args = null)
        {
            var result = new ScriptImageView(this.context, this.directory, args);
            if (this.staticListeners.ContainsKey("img"))
            {
                foreach (var listener in this.staticListeners["img"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }
        public ScriptButton CreateButton(UIArgs args = null)
        {
            var result = new ScriptButton(this.context, args);
            if (this.staticListeners.ContainsKey("btn"))
            {
                foreach (var listener in this.staticListeners["btn"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }
        public ScriptLinearLayout CreateLinearLayout(UIArgs args = null)
        {
            var result = new ScriptLinearLayout(this.context, args);
            if (this.staticListeners.ContainsKey("linear"))
            {
                foreach (var listener in this.staticListeners["linear"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }
        public ScriptFrameLayout CreateFrameLayout(UIArgs args = null)
        {
            var result = new ScriptFrameLayout(this.context, args);
            if (this.staticListeners.ContainsKey("frame"))
            {
                foreach (var listener in this.staticListeners["frame"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }
        public ScriptEditText CreateEditText(UIArgs args = null)
        {
            var result = new ScriptEditText(this.context, args);
            if (this.staticListeners.ContainsKey("edit"))
            {
                foreach (var listener in this.staticListeners["edit"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }
        public ScriptTextView CreateTextView(UIArgs args = null)
        {
            var result = new ScriptTextView(this.context, args);
            if (this.staticListeners.ContainsKey("text"))
            {
                foreach (var listener in this.staticListeners["text"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }
        public ScriptSpinner CreateSpinner(UIArgs args = null)
        {
            var result = new ScriptSpinner(this.context, args);
            if (this.staticListeners.ContainsKey("text"))
            {
                foreach (var listener in this.staticListeners["text"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptViewPager CreateViewPager(UIArgs args = null)
        {
            throw new NotImplementedException();
        }

        public ScriptRadioGroup CreateRadioGroup(UIArgs args = null)
        {
            throw new NotImplementedException();
        }

        public ScriptRadioButton CreateRadioButton(UIArgs args = null)
        {
            throw new NotImplementedException();
        }

        public ScriptCardView CreateCardView(UIArgs args = null)
        {
            throw new NotImplementedException();
        }

        public void On(string type, string key, object listener)
        {
            if (!this.staticListeners.ContainsKey(type))
            {
                this.staticListeners.Add(type, new Dictionary<string, object>());
            }
            if (!this.staticListeners[type].ContainsKey(key))
            {
                this.staticListeners[type].Add(key, listener);
            }
        }


    }
}

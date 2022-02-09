using Android.Content;
using Android.Views;
using astator.Core.UI.Base;
using astator.Core.UI.Controls;
using astator.Core.UI.Layouts;
using System;
using System.Collections.Generic;

namespace astator.Core.UI.Floaty
{

    /// <summary>
    /// 悬浮窗管理类
    /// </summary>
    public class FloatyManager : IManager
    {
        public List<FloatyWindow> Floatys { get; set; } = new();

        private readonly Context context;

        private readonly Dictionary<string, Dictionary<string, object>> staticListeners = new();

        private readonly string directory = string.Empty;

        private readonly Dictionary<string, IView> childs = new();

        /// <summary>
        /// 控件索引器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public IView this[string key]
        {
            set
            {
                if (value is not null)
                {
                    this.childs[key] = value;
                }
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

        /// <summary>
        /// 添加悬浮窗
        /// </summary>
        /// <param name="view"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public FloatyWindow Show(View view, int x = 0, int y = 0)
        {
            FloatyWindow floaty = new(view, x, y);
            this.Floatys.Add(floaty);
            return floaty;
        }

        /// <summary>
        /// 移除悬浮窗
        /// </summary>
        /// <param name="floaty"></param>
        public void Remove(FloatyWindow floaty)
        {
            floaty.Remove();
            this.Floatys.Remove(floaty);
        }

        /// <summary>
        /// 移除所有悬浮窗
        /// </summary>
        public void RemoveAll()
        {
            foreach (var floaty in this.Floatys)
            {
                floaty.Remove();
            }
            this.Floatys.Clear();
        }

        public ScriptScrollView CreateScrollView(ViewArgs args = null)
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

        public ScriptWebView CreateWebView(ViewArgs args = null)
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

        public ScriptSwitch CreateSwitch(ViewArgs args = null)
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

        public ScriptCheckBox CreateCheckBox(ViewArgs args = null)
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

        public ScriptImageView CreateImageView(ViewArgs args = null)
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

        public ScriptButton CreateButton(ViewArgs args = null)
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

        public ScriptLinearLayout CreateLinearLayout(ViewArgs args = null)
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

        public ScriptFrameLayout CreateFrameLayout(ViewArgs args = null)
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

        public ScriptEditText CreateEditText(ViewArgs args = null)
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

        public ScriptTextView CreateTextView(ViewArgs args = null)
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

        public ScriptSpinner CreateSpinner(ViewArgs args = null)
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

        public ScriptViewPager CreateViewPager(ViewArgs args = null)
        {
            throw new NotImplementedException();
        }

        public ScriptRadioGroup CreateRadioGroup(ViewArgs args = null)
        {
            throw new NotImplementedException();
        }

        public ScriptRadioButton CreateRadioButton(ViewArgs args = null)
        {
            throw new NotImplementedException();
        }

        public ScriptCardView CreateCardView(ViewArgs args = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 添加全局listener, 仅在创建view之前添加有效
        /// </summary>
        /// <param name="type">view类型</param>
        /// <param name="key">listener类型</param>
        /// <param name="listener"></param>
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

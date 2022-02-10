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

        private readonly Dictionary<string, Dictionary<string, object>> globalListeners = new();

        private readonly string workDir = string.Empty;

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
            this.workDir = directory;
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
            if (this.globalListeners.ContainsKey("scroll"))
            {
                foreach (var listener in this.globalListeners["scroll"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptWebView CreateWebView(ViewArgs args = null)
        {
            var result = new ScriptWebView(this.context, args);
            if (this.globalListeners.ContainsKey("web"))
            {
                foreach (var listener in this.globalListeners["web"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptSwitch CreateSwitch(ViewArgs args = null)
        {
            var result = new ScriptSwitch(this.context, args);
            if (this.globalListeners.ContainsKey("switch"))
            {
                foreach (var listener in this.globalListeners["switch"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptCheckBox CreateCheckBox(ViewArgs args = null)
        {
            var result = new ScriptCheckBox(this.context, args);
            if (this.globalListeners.ContainsKey("check"))
            {
                foreach (var listener in this.globalListeners["check"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptImageView CreateImageView(ViewArgs args = null)
        {
            var result = new ScriptImageView(this.context, this.workDir, args);
            if (this.globalListeners.ContainsKey("img"))
            {
                foreach (var listener in this.globalListeners["img"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptButton CreateButton(ViewArgs args = null)
        {
            var result = new ScriptButton(this.context, args);
            if (this.globalListeners.ContainsKey("btn"))
            {
                foreach (var listener in this.globalListeners["btn"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptLinearLayout CreateLinearLayout(ViewArgs args = null)
        {
            var result = new ScriptLinearLayout(this.context, args);
            if (this.globalListeners.ContainsKey("linear"))
            {
                foreach (var listener in this.globalListeners["linear"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptFrameLayout CreateFrameLayout(ViewArgs args = null)
        {
            var result = new ScriptFrameLayout(this.context, args);
            if (this.globalListeners.ContainsKey("frame"))
            {
                foreach (var listener in this.globalListeners["frame"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptEditText CreateEditText(ViewArgs args = null)
        {
            var result = new ScriptEditText(this.context, args);
            if (this.globalListeners.ContainsKey("edit"))
            {
                foreach (var listener in this.globalListeners["edit"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptTextView CreateTextView(ViewArgs args = null)
        {
            var result = new ScriptTextView(this.context, args);
            if (this.globalListeners.ContainsKey("text"))
            {
                foreach (var listener in this.globalListeners["text"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptSpinner CreateSpinner(ViewArgs args = null)
        {
            var result = new ScriptSpinner(this.context, args);
            if (this.globalListeners.ContainsKey("text"))
            {
                foreach (var listener in this.globalListeners["text"])
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

        public ScriptImageButton CreateImageButton(ViewArgs args = null)
        {
            var result = new ScriptImageButton(this.context, this.workDir, args);
            if (this.globalListeners.ContainsKey("img"))
            {
                foreach (var listener in this.globalListeners["img"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }
        public ScriptTabbedPage CreateTabbedPage(ViewArgs args = null)
        {
            var result = new ScriptTabbedPage(this.context, this.workDir, args);
            if (this.globalListeners.ContainsKey("tabPage"))
            {
                foreach (var listener in this.globalListeners["tabPage"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptTabbedView CreateTabbedView(ViewArgs args = null)
        {
            var result = new ScriptTabbedView(this.context, args);
            if (this.globalListeners.ContainsKey("tabView"))
            {
                foreach (var listener in this.globalListeners["tabView"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// 添加全局listener, 仅在创建view之前添加有效
        /// </summary>
        /// <param name="type">view类型</param>
        /// <param name="key">listener类型</param>
        /// <param name="listener"></param>
        public void On(string type, string key, object listener)
        {
            if (!this.globalListeners.ContainsKey(type))
            {
                this.globalListeners.Add(type, new Dictionary<string, object>());
            }
            if (!this.globalListeners[type].ContainsKey(key))
            {
                this.globalListeners[type].Add(key, listener);
            }
        }


    }
}

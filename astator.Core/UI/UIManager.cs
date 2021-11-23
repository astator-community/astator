using Android.Graphics;
using Android.Views;
using astator.Core.Exceptions;
using astator.Core.UI.Layout;
using astator.Core.UI.Widget;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using Activity = Android.App.Activity;
using View = Android.Views.View;

namespace astator.Core.UI
{
    public class UiManager : IManager
    {
        public static int CreateCount = 0;

        private readonly Activity activity;

        private readonly Dictionary<string, Dictionary<string, object>> globalListeners = new();

        private readonly string directory;


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

        public UiManager(Activity activity, string directory)
        {
            CreateCount = 0;
            this.activity = activity;
            this.directory = directory;
        }

        public View ParseXml(string xml)
        {
            return UiXml.Parse(this, xml);
        }

        public void Show(View view)
        {
            this.activity.SetContentView(view);
        }

        public View Create(string type, UiArgs args)
        {
            View view;
            switch (type)
            {
                case "frame":
                    view = CreateFrameLayout(args); break;
                case "linear":
                    view = CreateLinearLayout(args); break;
                case "scroll":
                    view = CreateScrollView(args); break;
                case "btn":
                    view = CreateButton(args); break;
                case "check":
                    view = CreateCheckBox(args); break;
                case "edit":
                    view = CreateEditText(args); break;
                case "text":
                    view = CreateTextView(args); break;
                case "switch":
                    view = CreateSwitch(args); break;
                case "web":
                    view = CreateWebView(args); break;
                case "img":
                    view = CreateImageView(args); break;
                case "pager":
                    view = CreateViewPager(args); break;
                case "spinner":
                    view = CreateSpinner(args); break;
                case "card":
                    view = CreateCardView(args); break;
                case "radioGroup":
                    view = CreateRadioGroup(args); break;
                case "radio":
                    view = CreateRadioButton(args); break;
                default:
                    throw new AttributeNotExistException(type);
            }
            return view;
        }

        public ScriptScrollView CreateScrollView(UiArgs args = null)
        {
            var result = new ScriptScrollView(this.activity, args);
            if (this.globalListeners.ContainsKey("scroll"))
            {
                foreach (var listener in this.globalListeners["scroll"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptWebView CreateWebView(UiArgs args = null)
        {
            var result = new ScriptWebView(this.activity, args);
            if (this.globalListeners.ContainsKey("web"))
            {
                foreach (var listener in this.globalListeners["web"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptSwitch CreateSwitch(UiArgs args = null)
        {
            var result = new ScriptSwitch(this.activity, args);
            if (this.globalListeners.ContainsKey("switch"))
            {
                foreach (var listener in this.globalListeners["switch"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptCheckBox CreateCheckBox(UiArgs args = null)
        {
            var result = new ScriptCheckBox(this.activity, args);
            if (this.globalListeners.ContainsKey("check"))
            {
                foreach (var listener in this.globalListeners["check"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptImageView CreateImageView(UiArgs args = null)
        {
            var result = new ScriptImageView(this.activity, this.directory, args);
            if (this.globalListeners.ContainsKey("img"))
            {
                foreach (var listener in this.globalListeners["img"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptButton CreateButton(UiArgs args = null)
        {
            var result = new ScriptButton(this.activity, args);
            if (this.globalListeners.ContainsKey("btn"))
            {
                foreach (var listener in this.globalListeners["btn"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptLinearLayout CreateLinearLayout(UiArgs args = null)
        {
            var result = new ScriptLinearLayout(this.activity, args);
            if (this.globalListeners.ContainsKey("linear"))
            {
                foreach (var listener in this.globalListeners["linear"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptFrameLayout CreateFrameLayout(UiArgs args = null)
        {
            var result = new ScriptFrameLayout(this.activity, args);
            if (this.globalListeners.ContainsKey("frame"))
            {
                foreach (var listener in this.globalListeners["frame"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptEditText CreateEditText(UiArgs args = null)
        {
            var result = new ScriptEditText(this.activity, args);
            if (this.globalListeners.ContainsKey("edit"))
            {
                foreach (var listener in this.globalListeners["edit"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptTextView CreateTextView(UiArgs args = null)
        {
            var result = new ScriptTextView(this.activity, args);
            if (this.globalListeners.ContainsKey("text"))
            {
                foreach (var listener in this.globalListeners["text"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptSpinner CreateSpinner(UiArgs args = null)
        {
            var result = new ScriptSpinner(this.activity, args);
            if (this.globalListeners.ContainsKey("spinner"))
            {
                foreach (var listener in this.globalListeners["spinner"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptViewPager CreateViewPager(UiArgs args = null)
        {
            var result = new ScriptViewPager(this.activity, args);
            if (this.globalListeners.ContainsKey("viewPager"))
            {
                foreach (var listener in this.globalListeners["viewPager"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptRadioGroup CreateRadioGroup(UiArgs args = null)
        {
            var result = new ScriptRadioGroup(this.activity, args);
            if (this.globalListeners.ContainsKey("radioGroup"))
            {
                foreach (var listener in this.globalListeners["radioGroup"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptRadioButton CreateRadioButton(UiArgs args = null)
        {
            var result = new ScriptRadioButton(this.activity, args);
            if (this.globalListeners.ContainsKey("radio"))
            {
                foreach (var listener in this.globalListeners["radio"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

        public ScriptCardView CreateCardView(UiArgs args = null)
        {
            var result = new ScriptCardView(this.activity, args);
            if (this.globalListeners.ContainsKey("card"))
            {
                foreach (var listener in this.globalListeners["card"])
                {
                    result.On(listener.Key, listener.Value);
                }
            }
            return result;
        }

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

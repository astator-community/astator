using Android.App;
using Android.Graphics;
using Android.Views;
using astator.Core.Exceptions;
using astator.Core.Script;
using astator.Core.UI.Base;
using astator.Core.UI.Controls;
using astator.Core.UI.Layouts;
using System;
using System.Collections.Generic;
using Activity = Android.App.Activity;
using View = Android.Views.View;

namespace astator.Core.UI;

public class UiManager : IManager
{
    internal static int CreateCount { get; set; } = 0;

    private readonly Activity context;

    /// <summary>
    /// 全局lisistener
    /// </summary>
    private readonly Dictionary<string, Dictionary<string, object>> globalListeners = new();

    private readonly string workDir;

    private readonly Dictionary<string, IView> childs = new();

    /// <summary>
    /// 控件索引器
    /// </summary>
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


    public UiManager(Activity activity, string dir)
    {
        CreateCount = 0;
        this.context = activity;
        this.workDir = dir;
    }

    public void Alert(string text, string title = "")
    {
        Globals.InvokeOnMainThreadAsync(() =>
        {
            var alert = new AlertDialog
              .Builder(this.context)
              .SetTitle(title)
              .SetMessage(text)
              .SetPositiveButton("确认", (s, e) => { })
              .Create();

            if (Android.Provider.Settings.CanDrawOverlays(this.context))
            {
                if (OperatingSystem.IsAndroidVersionAtLeast(26))
                {
                    alert.Window.SetType(WindowManagerTypes.ApplicationOverlay);
                }
                else
                {
                    alert.Window.SetType(WindowManagerTypes.Phone);
                }
            }
            alert.Show();
        });
    }

    /// <summary>
    /// 设置状态栏颜色
    /// </summary>
    /// <param name="color"></param>
    public void SetStatusBarColor(string color)
    {
        this.context.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
        this.context.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
        this.context.Window.SetStatusBarColor(Color.ParseColor(color));
    }

    /// <summary>
    /// 设置状态栏字体颜色为白色
    /// </summary>
    public void SetLightStatusBar()
    {
        this.context.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
    }

    /// <summary>
    /// 解析xml
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public View ParseXml(string xml)
    {
        return UiXml.Parse(this, xml);
    }

    /// <summary>
    /// 展示布局
    /// </summary>
    /// <param name="layout"></param>
    public void Show(View layout)
    {
        this.context.SetContentView(layout);
    }

    /// <summary>
    /// 创建控件
    /// </summary>
    /// <param name="type">控件类型</param>
    /// <param name="args">控件属性</param>
    /// <returns></returns>
    /// <exception cref="AttributeNotExistException"></exception>
    public View Create(string type, ViewArgs args)
    {
        View view = type switch
        {
            "frame" => CreateFrameLayout(args),
            "linear" => CreateLinearLayout(args),
            "scroll" => CreateScrollView(args),
            "btn" => CreateButton(args),
            "check" => CreateCheckBox(args),
            "edit" => CreateEditText(args),
            "text" => CreateTextView(args),
            "switch" => CreateSwitch(args),
            "web" => CreateWebView(args),
            "img" => CreateImageView(args),
            "imgBtn" => CreateImageButton(args),
            "pager" => CreateViewPager(args),
            "spinner" => CreateSpinner(args),
            "card" => CreateCardView(args),
            "radioGroup" => CreateRadioGroup(args),
            "radio" => CreateRadioButton(args),
            "tabPage" => CreateTabbedPage(args),
            "tabView" => CreateTabbedView(args),
            _ => throw new AttributeNotExistException(type),
        };
        return view;
    }

    public ScriptScrollView CreateScrollView(ViewArgs args = null)
    {
        var result = new ScriptScrollView(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("scroll"))
        {
            foreach (var listener in this.globalListeners["scroll"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptWebView CreateWebView(ViewArgs args = null)
    {
        var result = new ScriptWebView(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("web"))
        {
            foreach (var listener in this.globalListeners["web"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptSwitch CreateSwitch(ViewArgs args = null)
    {
        var result = new ScriptSwitch(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("switch"))
        {
            foreach (var listener in this.globalListeners["switch"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptCheckBox CreateCheckBox(ViewArgs args = null)
    {
        var result = new ScriptCheckBox(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("check"))
        {
            foreach (var listener in this.globalListeners["check"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptImageView CreateImageView(ViewArgs args = null)
    {
        var result = new ScriptImageView(this.context, this.workDir, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("img"))
        {
            foreach (var listener in this.globalListeners["img"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptButton CreateButton(ViewArgs args = null)
    {
        var result = new ScriptButton(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("btn"))
        {
            foreach (var listener in this.globalListeners["btn"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptLinearLayout CreateLinearLayout(ViewArgs args = null)
    {
        var result = new ScriptLinearLayout(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("linear"))
        {
            foreach (var listener in this.globalListeners["linear"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptFrameLayout CreateFrameLayout(ViewArgs args = null)
    {
        var result = new ScriptFrameLayout(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("frame"))
        {
            foreach (var listener in this.globalListeners["frame"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptEditText CreateEditText(ViewArgs args = null)
    {
        var result = new ScriptEditText(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("edit"))
        {
            foreach (var listener in this.globalListeners["edit"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptTextView CreateTextView(ViewArgs args = null)
    {
        var result = new ScriptTextView(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("text"))
        {
            foreach (var listener in this.globalListeners["text"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptSpinner CreateSpinner(ViewArgs args = null)
    {
        var result = new ScriptSpinner(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("spinner"))
        {
            foreach (var listener in this.globalListeners["spinner"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptViewPager CreateViewPager(ViewArgs args = null)
    {
        var result = new ScriptViewPager(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("viewPager"))
        {
            foreach (var listener in this.globalListeners["viewPager"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptRadioGroup CreateRadioGroup(ViewArgs args = null)
    {
        var result = new ScriptRadioGroup(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("radioGroup"))
        {
            foreach (var listener in this.globalListeners["radioGroup"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptRadioButton CreateRadioButton(ViewArgs args = null)
    {
        var result = new ScriptRadioButton(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("radio"))
        {
            foreach (var listener in this.globalListeners["radio"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptCardView CreateCardView(ViewArgs args = null)
    {
        var result = new ScriptCardView(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("card"))
        {
            foreach (var listener in this.globalListeners["card"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptImageButton CreateImageButton(ViewArgs args = null)
    {
        var result = new ScriptImageButton(this.context, this.workDir, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("img"))
        {
            foreach (var listener in this.globalListeners["img"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptTabbedPage CreateTabbedPage(ViewArgs args = null)
    {
        var result = new ScriptTabbedPage(this.context, this.workDir, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("tabPage"))
        {
            foreach (var listener in this.globalListeners["tabPage"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
        return result;
    }

    public ScriptTabbedView CreateTabbedView(ViewArgs args = null)
    {
        var result = new ScriptTabbedView(this.context, args);
        if (args?["id"] is string id) this[id] = result;

        if (this.globalListeners.ContainsKey("tabView"))
        {
            foreach (var listener in this.globalListeners["tabView"])
            {
                result.On(listener.Key, listener.Value);
            }
        }
        result.OnCreatedListener?.OnCreated(result);
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

    public void OnResumeListener(Action callback)
    {
        (this.context as TemplateActivity).OnResumeCallback = callback;
    }

    public void OnKeyDownListener(Func<Keycode, KeyEvent, bool> callback)
    {
        (this.context as TemplateActivity).OnKeyDownCallback = callback;
    }
}

using Android.Content;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using astator.Core;
using astator.Core.UI;
using astator.Core.UI.Widget;
using Java.Interop;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Widget.AdapterView;
using static astator.Core.UI.Widget.ScriptSpinner;

namespace astator.Views;

internal class CustomSpinner : View
{

    public static readonly BindableProperty ItemsProperty = BindableProperty.Create(nameof(Items), typeof(string), typeof(CustomSpinner));
    public string Items
    {
        get => GetValue(ItemsProperty) as string;
        set => SetValue(ItemsProperty, value);
    }

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(int), typeof(CustomSpinner), 0);
    public int SelectedItem
    {
        get => (int)GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public static readonly new BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(CustomSpinner), (Color)Application.Current.Resources["PrimaryColor"]);
    public new Color BackgroundColor
    {
        get => GetValue(BackgroundColorProperty) as Color;
        set => SetValue(BackgroundColorProperty, value);
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CustomSpinner), (Color)Application.Current.Resources["SecondaryColor"]);
    public Color TextColor
    {
        get => GetValue(TextColorProperty) as Color;
        set => SetValue(TextColorProperty, value);
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(int), typeof(CustomSpinner), 8);
    public int FontSize
    {
        get => (int)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty SelectionChangedProperty = BindableProperty.Create(nameof(SelectionChanged), typeof(EventHandler), typeof(CustomSpinner));

    public event EventHandler<SelectedItemChangedEventArgs> SelectionChanged;

    public Action<int> SelectionChangedCallback;
    public CustomSpinner() : base()
    {
        SelectionChangedCallback = (position) =>
        {
            SelectionChanged?.Invoke(this, new SelectedItemChangedEventArgs(null, position));
        };
    }
}


internal class CustomSpinnerHandler : ViewHandler<CustomSpinner, ScriptSpinner>
{
    public static PropertyMapper<CustomSpinner, CustomSpinnerHandler> SpinnerMapper = new(ViewMapper)
    {
        [nameof(CustomSpinner.BackgroundColor)] = MapBackgroundColor,
        [nameof(CustomSpinner.TextColor)] = MapTextColor,
        [nameof(CustomSpinner.FontSize)] = MapTextSize,
        [nameof(CustomSpinner.Items)] = MapItems,
        [nameof(CustomSpinner.SelectedItem)] = MapSelectedItem,
    };

    public CustomSpinnerHandler() : base(SpinnerMapper)
    {

    }

    public CustomSpinnerHandler(PropertyMapper mapper) : base(mapper)
    {
    }

    protected override ScriptSpinner CreateNativeView()
    {
        var nativeView = new ScriptSpinner(Context, null)
        {
            OnItemSelectedListener = new OnItemSelectedListener((AdapterView, v, position, id) =>
            {
                VirtualView.SelectedItem = position;
                VirtualView.SelectionChangedCallback?.Invoke(position);
            })
        };
        return nativeView;

    }

    static void MapItems(CustomSpinnerHandler handler, CustomSpinner view)
    {
        if (view.Items is null)
        {
            return;
        }

        var nativeView = handler?.NativeView;
        var items = view.Items.Replace(" ", "").Split(",").ToList();

        nativeView.Adapter = new SpinnerAdapter<string>(handler.Context, Android.Resource.Layout.SelectDialogItem, items)
        {
            TextColor = view.TextColor.ToNative(),
            BackgroundColor = view.BackgroundColor.ToNative(),
            TextSize = view.FontSize,
        };
    }

    static void MapSelectedItem(CustomSpinnerHandler handler, CustomSpinner view)
    {
        var nativeView = handler?.NativeView;
        nativeView.SetSelection(view.SelectedItem);
    }

    static void MapBackgroundColor(CustomSpinnerHandler handler, CustomSpinner view)
    {
        if (view.BackgroundColor is null)
        {
            return;
        }

        var nativeView = handler?.NativeView;
        nativeView.SetAttr("bg", view.BackgroundColor.ToHex());
    }

    static void MapTextColor(CustomSpinnerHandler handler, CustomSpinner view)
    {
        if (view.TextColor is null)
        {
            return;
        }

        var nativeView = handler?.NativeView;
        nativeView.SetAttr("textColor", view.TextColor.ToHex());
    }

    static void MapTextSize(CustomSpinnerHandler handler, CustomSpinner view)
    {
        var nativeView = handler?.NativeView;
        nativeView.SetAttr("textSize", view.FontSize.ToString());
    }
}

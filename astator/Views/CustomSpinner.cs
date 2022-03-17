using astator.Core.UI.Base;
using astator.Core.UI.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using static astator.Core.UI.Controls.ScriptSpinner;

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

    public static new readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(nameof(BackgroundColor), typeof(Color), typeof(CustomSpinner), (Color)Application.Current.Resources["PrimaryColor"]);
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
        this.SelectionChangedCallback = (position) =>
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

    protected override ScriptSpinner CreatePlatformView()
    {
        var nativeView = new ScriptSpinner(this.Context, null)
        {
            OnItemSelectedListener = new OnItemSelectedListener((AdapterView, v, position, id) =>
            {
                this.VirtualView.SelectedItem = position;
                this.VirtualView.SelectionChangedCallback?.Invoke(position);
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

        var nativeView = handler?.PlatformView;
        var items = view.Items.Replace(" ", "").Split(",").ToList();

        nativeView.Adapter = new SpinnerAdapter<string>(handler.Context, Android.Resource.Layout.SelectDialogItem, items)
        {
            TextColor = view.TextColor.ToPlatform(),
            BackgroundColor = view.BackgroundColor.ToPlatform(),
            TextSize = view.FontSize,
        };
    }

    static void MapSelectedItem(CustomSpinnerHandler handler, CustomSpinner view)
    {
        var nativeView = handler?.PlatformView;
        nativeView.SetSelection(view.SelectedItem);
    }

    static void MapBackgroundColor(CustomSpinnerHandler handler, CustomSpinner view)
    {
        if (view.BackgroundColor is null)
        {
            return;
        }

        var nativeView = handler?.PlatformView;
        nativeView.SetAttr("bg", view.BackgroundColor.ToHex());
    }

    static void MapTextColor(CustomSpinnerHandler handler, CustomSpinner view)
    {
        if (view.TextColor is null)
        {
            return;
        }

        var nativeView = handler?.PlatformView;
        nativeView.SetAttr("textColor", view.TextColor.ToHex());
    }

    static void MapTextSize(CustomSpinnerHandler handler, CustomSpinner view)
    {
        var nativeView = handler?.PlatformView;
        nativeView.SetAttr("textSize", view.FontSize.ToString());
    }
}

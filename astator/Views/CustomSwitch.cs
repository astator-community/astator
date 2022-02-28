using Microsoft.Maui.Handlers;

namespace astator.Views;

internal class CustomSwitch : Switch
{

    public CustomSwitch() : base()
    {

    }
}

internal class CustomSwitchHandler : SwitchHandler
{

    public CustomSwitchHandler() : base(SwitchMapper)
    {
        SwitchMapper.Add(nameof(Switch.IsToggled), MapIsToggled);
    }

    public CustomSwitchHandler(IPropertyMapper mapper = null) : base(mapper ?? SwitchMapper)
    {
        SwitchMapper.Add(nameof(Switch.IsToggled), MapIsToggled);
    }


    public static void MapIsToggled(SwitchHandler handler, ISwitch view)
    {
        handler.NativeView.Checked = (view as Switch).IsToggled;
    }
}

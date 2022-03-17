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

    public CustomSwitchHandler() : base(Mapper)
    {
        Mapper.Add(nameof(Switch.IsToggled), MapIsToggled);
    }

    public CustomSwitchHandler(IPropertyMapper mapper = null) : base(mapper ?? Mapper)
    {
        Mapper.Add(nameof(Switch.IsToggled), MapIsToggled);
    }


    public static void MapIsToggled(ISwitchHandler handler, ISwitch view)
    {
        handler.PlatformView.Checked = (view as Switch).IsToggled;
    }
}

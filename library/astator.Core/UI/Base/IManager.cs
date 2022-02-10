using astator.Core.UI.Controls;
using astator.Core.UI.Layouts;

namespace astator.Core.UI.Base;
internal interface IManager
{
    public IView this[string key] { get; set; }
    public ScriptScrollView CreateScrollView(ViewArgs args);
    public ScriptWebView CreateWebView(ViewArgs args);
    public ScriptSwitch CreateSwitch(ViewArgs args);
    public ScriptCheckBox CreateCheckBox(ViewArgs args);
    public ScriptImageView CreateImageView(ViewArgs args);
    public ScriptButton CreateButton(ViewArgs args);
    public ScriptLinearLayout CreateLinearLayout(ViewArgs args);
    public ScriptFrameLayout CreateFrameLayout(ViewArgs args);
    public ScriptEditText CreateEditText(ViewArgs args);
    public ScriptTextView CreateTextView(ViewArgs args);
    public ScriptSpinner CreateSpinner(ViewArgs args);
    public ScriptViewPager CreateViewPager(ViewArgs args);
    public ScriptRadioGroup CreateRadioGroup(ViewArgs args);
    public ScriptRadioButton CreateRadioButton(ViewArgs args);
    public ScriptCardView CreateCardView(ViewArgs args);
    public ScriptImageButton CreateImageButton(ViewArgs args);
    public ScriptTabbedPage CreateTabbedPage(ViewArgs args);
    public ScriptTabbedView CreateTabbedView(ViewArgs args);
}

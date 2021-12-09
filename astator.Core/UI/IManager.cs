using astator.Core.UI.Layout;
using astator.Core.UI.Widget;

namespace astator.Core.UI
{
    internal interface IManager
    {
        public IScriptView this[string key] { get; set; }
        public ScriptScrollView CreateScrollView(UiArgs args);
        public ScriptWebView CreateWebView(UiArgs args);
        public ScriptSwitch CreateSwitch(UiArgs args);
        public ScriptCheckBox CreateCheckBox(UiArgs args);
        public ScriptImageView CreateImageView(UiArgs args);
        public ScriptButton CreateButton(UiArgs args);
        public ScriptLinearLayout CreateLinearLayout(UiArgs args);
        public ScriptFrameLayout CreateFrameLayout(UiArgs args);
        public ScriptEditText CreateEditText(UiArgs args);
        public ScriptTextView CreateTextView(UiArgs args);
        public ScriptSpinner CreateSpinner(UiArgs args);
        public ScriptViewPager CreateViewPager(UiArgs args);
        public ScriptRadioGroup CreateRadioGroup(UiArgs args);
        public ScriptRadioButton CreateRadioButton(UiArgs args);
        public ScriptCardView CreateCardView(UiArgs args);
    }
}

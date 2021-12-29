using astator.Core.UI.Layout;
using astator.Core.UI.Widget;

namespace astator.Core.UI
{
    internal interface IManager
    {
        public IScriptView this[string key] { get; set; }
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
    }
}

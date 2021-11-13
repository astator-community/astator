using astator.Core.UI.Layout;
using astator.Core.UI.Widget;

namespace astator.Core.UI
{
    internal interface IManager
    {
        public object this[string key] { get; set; }
        public ScriptScrollView CreateScrollView(UIArgs args);
        public ScriptWebView CreateWebView(UIArgs args);
        public ScriptSwitch CreateSwitch(UIArgs args);
        public ScriptCheckBox CreateCheckBox(UIArgs args);
        public ScriptImageView CreateImageView(UIArgs args);
        public ScriptButton CreateButton(UIArgs args);
        public ScriptLinearLayout CreateLinearLayout(UIArgs args);
        public ScriptFrameLayout CreateFrameLayout(UIArgs args);
        public ScriptEditText CreateEditText(UIArgs args);
        public ScriptTextView CreateTextView(UIArgs args);
        public ScriptSpinner CreateSpinner(UIArgs args);
        public ScriptViewPager CreateViewPager(UIArgs args);
        public ScriptRadioGroup CreateRadioGroup(UIArgs args);
        public ScriptRadioButton CreateRadioButton(UIArgs args);
        public ScriptCardView CreateCardView(UIArgs args);
    }
}

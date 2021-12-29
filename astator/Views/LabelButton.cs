namespace astator.Views
{
    internal class LabelButton : Button
    {
        public static readonly BindableProperty TagBindableProperty = BindableProperty.Create(nameof(Tag), typeof(object), typeof(LabelButton));
        public object Tag
        {
            get => GetValue(TagBindableProperty);
            set => SetValue(TagBindableProperty, value);
        }
    }
}

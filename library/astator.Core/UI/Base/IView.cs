namespace astator.Core.UI.Base;

public interface IView
{
    public string CustomId { get; set; }
    public OnAttachedListener OnAttachedListener { get; set; }
    public void On(string key, object listener);
}

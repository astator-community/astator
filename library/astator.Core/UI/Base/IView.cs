namespace astator.Core.UI.Base;

public interface IView
{
    public string CustomId { get; set; }
    public OnCreatedListener OnCreatedListener { get; set; }
    public object GetAttr(string key);
    public void SetAttr(string key, object value);
    public void On(string key, object listener);
}

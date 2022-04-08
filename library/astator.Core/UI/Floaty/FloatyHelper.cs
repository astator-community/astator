using System.Collections.Generic;
using Android.Content;

namespace astator.Core.UI.Floaty;

public class FloatyHelper
{
    private readonly List<FloatyManager> managers = new();

    private readonly Context context;

    private readonly string workDir = string.Empty;

    public FloatyHelper(Context context, string dir)
    {
        this.context = context;
        this.workDir = dir;
    }

    public FloatyManager CreateFloatyManager()
    {
        var manager = new FloatyManager(this.context, this.workDir);
        this.managers.Add(manager);
        return manager;
    }

    public void RemoveAll()
    {
        foreach (var manager in this.managers)
        {
            manager.Remove();
        }
    }
}


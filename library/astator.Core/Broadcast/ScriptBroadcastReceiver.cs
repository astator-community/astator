using Android.Content;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace astator.Core.Broadcast;

public class ScriptBroadcastReceiver : BroadcastReceiver
{
    private static ScriptBroadcastReceiver instance;
    public static ScriptBroadcastReceiver Instance
    {
        get
        {
            if (instance is null)
            {
                instance = new ScriptBroadcastReceiver();
            }
            return instance;
        }
    }

    private readonly ConcurrentDictionary<string, List<Action>> listener = new();

    public static void AddListener(string action, Action callback)
    {
        var listener = Instance.listener;

        if (!listener.ContainsKey(action))
        {
            listener.TryAdd(action, new());
        }
        listener.TryGetValue(action, out var callbacks);
        callbacks?.Add(callback);
    }

    public static void RemoveListener(string action, Action callback)
    {
        var listener = Instance.listener;
        listener.TryGetValue(action, out var callbacks);
        callbacks?.Add(callback);
    }


    public override void OnReceive(Context context, Intent intent)
    {
        var action = intent.Action;

        if (this.listener.ContainsKey(action))
        {
            this.listener.TryGetValue(action, out var callbacks);

            if (callbacks is not null)
            {
                foreach (var callback in callbacks)
                {
                    try
                    {
                        callback.Invoke();
                    }
                    catch (Exception ex)
                    {
                        ScriptLogger.Error(ex);
                    }
                }
            }
        }
    }
}

using Android.Content;
using astator.Core.Script;
using System;
using System.Collections.Concurrent;

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

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Action>> listener = new();

    public static void AddListener(string action, string key, Action callback)
    {
        var listener = Instance.listener;

        if (!listener.ContainsKey(action))
        {
            listener.TryAdd(action, new());
        }

        listener.TryGetValue(action, out var callbacks);
        if (!callbacks.ContainsKey(key))
        {
            callbacks?.TryAdd(key, callback);
        }
    }

    public static void RemoveListener(string action, string key)
    {
        var listener = Instance.listener;
        listener.TryGetValue(action, out var callbacks);
        callbacks?.TryRemove(key, out _);
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
                        callback.Value.Invoke();
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

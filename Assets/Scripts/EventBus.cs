using System;
using UnityEngine;

public static class EventBus
{
    public class GameStateChangedEventArgs : EventArgs
    {
        public bool state;
        public GameStateChangedEventArgs(bool state)
        {
            this.state = state;
        }
    }
    public static event EventHandler<GameStateChangedEventArgs> GameStateChanged;
    public static event EventHandler<int> Example;
    public static void Invoke(object sender, object arg)
    {
        switch (arg)
        {
            case GameStateChangedEventArgs a:
                GameStateChanged?.Invoke(sender, a);
                Debug.Log($"Event GameStateChanged invoked");
                break;

            case int a:
                Example?.Invoke(sender, a);
                break;

            default:
                Debug.LogError($"No event registered for {arg.GetType().Name}");
                break;
        }
    }
}

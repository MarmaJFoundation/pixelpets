using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class FocusHandler : MonoBehaviour
{
    /*public OfflineController offlineController;

    [DllImport("__Internal")]
    private static extern void registerVisibilityChangeEvent();
    private DateTime stopTime;
    private
    void Start()
    {
#if !UNITY_EDITOR
        registerVisibilityChangeEvent();
#endif
    }

    void OnVisibilityChange(string visibilityState)
    {
        if (visibilityState == "visible")
        {
            float intervalTime = (float)(DateTime.Now - stopTime).TotalSeconds;
            offlineController.UpdateTimers(intervalTime);
        }
        else
        {
            stopTime = DateTime.Now;
        }

        //Console.WriteLine("[" + DateTime.Now + "] the game switched to " + (visibilityState == "visible" ? "foreground" : "background"));
    }*/
}
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugGameMode : MonoBehaviour
{
    public bool LogClicks = true;
    public bool LogReleases = true;
    public bool LogDrags = true;
    public bool LogHovers = true;
    
    void Start()
    {
        Application.runInBackground = true;
        GreenHeatEventManager.OnGreenHeatClick += ClickEvent;
        GreenHeatEventManager.OnGreenHeatRelease += ReleaseEvent;
        GreenHeatEventManager.OnGreenHeatDrag += DragEvent;
        GreenHeatEventManager.OnGreenHeatHover += HoverEvent;
    }
    
    Queue<GreenHeatMessage> messageQueue = new Queue<GreenHeatMessage>();

    void ClickEvent(GreenHeatMessage message)
    {
        Debug.Log(message.ToJsonString());
        messageQueue.Enqueue(message);
    }
    void ReleaseEvent(GreenHeatMessage message)
    {
        Debug.Log(message.ToJsonString());
        messageQueue.Enqueue(message);
    }
    void DragEvent(GreenHeatMessage message)
    {
        Debug.Log(message.ToJsonString());
        messageQueue.Enqueue(message);
    }    
    void HoverEvent(GreenHeatMessage message)
    {
        Debug.Log(message.ToJsonString());
        messageQueue.Enqueue(message);
    }

    void OnGUI()
    {
        StringBuilder sb = new StringBuilder();
        foreach(var message in messageQueue)
        {
            sb.Append("time: ");
            sb.Append(message.time);
            sb.Append(" pos: ");
            sb.Append(message.x);
            sb.Append(" ");
            sb.Append(message.y);
            sb.Append(" type: ");
            sb.Append(message.type);
            sb.Append(" user:");
            sb.Append(message.id);
            GUILayout.Label(sb.ToString());
            sb.Clear();
        }
    }
    
    void OnDestroy()
    {
        GreenHeatEventManager.OnGreenHeatClick -= ClickEvent;
        GreenHeatEventManager.OnGreenHeatRelease -= ReleaseEvent;
        GreenHeatEventManager.OnGreenHeatDrag -= DragEvent;
        GreenHeatEventManager.OnGreenHeatHover -= HoverEvent;
    }
}

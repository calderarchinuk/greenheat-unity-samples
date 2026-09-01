using System;
using System.Collections.Generic;
using UnityEngine;

public class GreenHeatPointerSystem : MonoBehaviour
{
    public GameObject clickPrefab;
    public GameObject pointerPrefab;
    public float pointerSmoothing = 0.1f;
    public float pointerTimeout = 10;
    public bool RunInBackground = true;
    
    //user pointers
    Dictionary<string, UserPointerData> pointers = new Dictionary<string, UserPointerData>();
    
    void OnEnable()
    {
        if (RunInBackground)
            Application.runInBackground = true;
        GreenHeatEventManager.OnGreenHeatClick += ClickEvent;
        GreenHeatEventManager.OnGreenHeatHover += HoverEvent;
    }
    
    void HoverEvent(GreenHeatMessage message)
    {
        //create pointers
        if (pointers.ContainsKey(message.id) == false)
        {
            var go = Instantiate(pointerPrefab);
            pointers.Add(message.id, new UserPointerData(message,go));
            go.GetComponent<MeshRenderer>().material.color = pointers[message.id].color;
            //TODO text name over pointer
            //TODO turn id into a viewer name. does this require the twitch api?
            //TODO snap pointer to initial position
        }
        else
        {
            //update existing pointers
            pointers[message.id].latestMessage = message;
        }
    }
    
    void Update()
    {
        foreach(var pointerValue in pointers.Values)
        {
            //lerp pointer positions
            if (pointerValue.active)
            {
                Vector3 viewportPos = pointerValue.latestMessage.ViewportPosition();
                Vector3 worldPosition = Camera.main.ViewportToWorldPoint(viewportPos);
                worldPosition.z = 0;
                
                pointerValue.pointer.transform.position =
                    Vector3.Lerp(
                        pointerValue.pointer.transform.position,
                        worldPosition,
                        pointerSmoothing);
                        
            }
            
            //invalidate/time out pointers
            long expiryTime = pointerValue.latestMessage.time + (long)(pointerTimeout*1000f);
            if (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() > expiryTime)
            {
                if (pointerValue.active == true)
                {
                    pointerValue.active = false;
                    pointerValue.pointer.SetActive(false);
                }
            }
            else
            {
                if (pointerValue.active != true)
                {
                    pointerValue.active = true;
                    pointerValue.pointer.SetActive(true);
                }
            }
        }
    }

    void OnGUI()
    {
        GUILayout.Label(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + " unix time ms");
        foreach( var pointervalue in pointers.Values)
        {
            GUILayout.Label(pointervalue.latestMessage.time + "  " +
            pointervalue.latestMessage.id + "  " +
            (pointervalue.active?"active":"paused") +
            " [" + pointervalue.latestMessage.x.ToString("0.00") + ","+pointervalue.latestMessage.y.ToString("0.00")+"]");
        }
    }

    void ClickEvent(GreenHeatMessage message)
    {
        Vector3 viewportPos = message.ViewportPosition();
        Vector3 worldPosition = Camera.main.ViewportToWorldPoint(viewportPos);
        worldPosition.z = 0;
        var go = Instantiate(clickPrefab,worldPosition,Quaternion.identity);
        
        var debugComponent = go.GetComponent<GreenHeatMessageComponent>();
        if (debugComponent)
        {
            debugComponent.messageDetails = message;
        }
    }
    
    void OnDisable()
    {
        GreenHeatEventManager.OnGreenHeatClick -= ClickEvent;
        GreenHeatEventManager.OnGreenHeatHover -= HoverEvent;
    }
}

using System.Collections.Generic;
using System.Collections;
using UnityEngine;

using Random = UnityEngine.Random;

//spawns bubbles across the screen
//users need to click on them to make the bubbles disappear

public class BubblePopGameMode : MonoBehaviour
{
    public bool RunInBackground = true;
    public float MaxSpawnTime = 5;
    public float MinSpawnTime = 1;
    public int MaxBubbleCount = 30;
    public GameObject BubblePrefab;
    
    public Dictionary<string,int> Scoreboard = new Dictionary<string, int>();
    public bool IsPlaying = false;
    
    void OnEnable()
    {
        IsPlaying = true;
        if (RunInBackground)
            Application.runInBackground = true;
        GreenHeatEventManager.OnGreenHeatClick += ClickEvent;
        StartCoroutine(SpawnBubbleLoop());
    }
    
    List<GameObject> bubbleInstances = new List<GameObject>();
    
    public Vector3 RandomViewportPosition()
    {
        return new Vector3(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),0);
    }
    
    IEnumerator SpawnBubbleLoop()
    {
        while (IsPlaying)
        {
            //wait a bit
            yield return new WaitForSeconds(Random.Range(MinSpawnTime,MaxSpawnTime));
            
            //if there aren't enough bubbles, spawn more
            if (bubbleInstances.Count < MaxBubbleCount)
            {
                //spawn at random position
                Camera c = Camera.main;
                var p = new Plane(c.transform.forward * -1 ,Vector3.zero);
                Ray ray = c.ViewportPointToRay(RandomViewportPosition());
                float distance;
                
                if (p.Raycast( ray,out distance))
                {
                    Vector3 spawnPoint = ray.GetPoint(distance);
                    var go = GameObject.Instantiate(BubblePrefab,spawnPoint,Quaternion.identity);
                    bubbleInstances.Add(go);
                }
            }
        }
    }

    void OnGUI()
    {
        //TODO sort the scoreboard
        foreach(var player in Scoreboard)
        {
            GUILayout.Label(player.Key + " score " + player.Value);
        }
    }

    void ClickEvent(GreenHeatMessage message)
    {
        //raycast
        Camera c = Camera.main;
        Ray ray = c.ViewportPointToRay(RandomViewportPosition());
        
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,100f))
        {
            var hitGo = hit.collider.gameObject;
            if (bubbleInstances.Contains(hitGo))
            {
                bubbleInstances.Remove(hitGo);
                hitGo.SendMessage("Pop");
                
                //user scoreboard
                if (!Scoreboard.ContainsKey(message.id))
                {
                    Scoreboard.Add(message.id,0);
                }
                Scoreboard[message.id] += 1;
            }
        }
    }
    
    void OnDisable()
    {
        IsPlaying = false;
        GreenHeatEventManager.OnGreenHeatClick -= ClickEvent;
    }
}

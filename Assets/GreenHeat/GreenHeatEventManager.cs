using UnityEngine;
using WebSocketSharp;
using System.Collections.Concurrent;
using System.Text;

//this is the script to recieve viewer's input
//use GreenHeatEventManager.OnGreenHeatClick += YourClickEvent; to use these inputs in some script

public class GreenHeatEventManager : MonoBehaviour
{
    public delegate void GreenHeatClick(GreenHeatMessage message);
    public static event GreenHeatClick OnGreenHeatClick;
    public delegate void GreenHeatRelease(GreenHeatMessage message);
    public static event GreenHeatRelease OnGreenHeatRelease;
    public delegate void GreenHeatHover(GreenHeatMessage message);
    public static event GreenHeatHover OnGreenHeatHover;
    public delegate void GreenHeatDrag(GreenHeatMessage message);
    public static event GreenHeatDrag OnGreenHeatDrag;

    private bool isConnecting = false;
    private WebSocket ws;

    public string url = "wss://heat.prod.kr/your_stream_name";
    
    private static GreenHeatEventManager _instance;

    public static GreenHeatEventManager Instance { get { return _instance; } }


    private void Awake() //making it persist across scenes
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        Connect();
    }

    private void Connect()
    {
        if (isConnecting) return;
        isConnecting = true;
        ws = new WebSocket(url);
        Debug.Log("URL: " + url +"\nConnection Origin: "+ws.Origin + "\nPort: "+ws.Url.Port);

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("Connection open!");
            isConnecting = false;
        };

        ws.OnError += (sender, e) =>
        {
            Debug.Log("Error! " + e.Exception);

            isConnecting = false;
        };
        
        ws.OnMessage += (sender, e) =>
        {
            var message = GreenHeatMessage.CreateFromJson(e.Data);
            messages.Enqueue(message);
        };
        ws.Connect();
    }
    
    ConcurrentQueue<GreenHeatMessage> messages = new ConcurrentQueue<GreenHeatMessage>();

    private void Update()
    {
        if (ws?.ReadyState != WebSocketState.Open && ws?.ReadyState != WebSocketState.Connecting) //in update loop because otherwise it doesn't reconnect if it disconnects when tabbed out
        {
            Debug.Log("Connection closed! Attempting reconnection.");
            isConnecting = false;
            Connect();
        }
        
        GreenHeatMessage message;
        while (messages.TryDequeue(out message))
        {
            switch (message.type)
            {
                case "click":
                    OnGreenHeatClick?.Invoke(message);
                    break;
                case "release":
                    OnGreenHeatRelease?.Invoke(message);
                    break;
                case "hover":
                    OnGreenHeatHover?.Invoke(message);
                    break;
                case "drag":
                    OnGreenHeatDrag?.Invoke(message);
                    break;
            }   
        }
    }

    private void OnApplicationQuit()
    {
        Debug.Log("Application quit, disconnecting from greenheat.");
        ws.Close();
    }
}

[System.Serializable]
public class GreenHeatMessage
{
    public string id;
    public float x;
    public float y;
    public string type; //can be "click", "release", "drag", or "hover"
    public string button; //can be "left", "right", "middle"
    public bool shift;
    public bool ctrl;
    public bool alt;
    public long time; // timestamp in ms
    public long latency; //latency between viewer and streamer
    
    /// <summary>
    /// return normalized screen position (0,0 is top left, 1,1 is bottom right)
    /// </summary>
    /// <returns></returns>
    public Vector3 ViewportPosition()
    {
        return new Vector3(x, 1-y, 0);
    }
    
    /// <summary>
    /// returns pixel on the screen (0,0 is top left, pixelWidth, pixelHeight is bottom right)
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public Vector3 ScreenSpacePosition()
    {
        return new Vector3(x* Screen.width, (1-y)*Screen.height, 0);
    }
    
    public static GreenHeatMessage CreateFromJson(string jsonString)
    {
        return JsonUtility.FromJson<GreenHeatMessage>(jsonString);
    }

    public string ToJsonString()
    {
        return JsonUtility.ToJson(this);
    }
}
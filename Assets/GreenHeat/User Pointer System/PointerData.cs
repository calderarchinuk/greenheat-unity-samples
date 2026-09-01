using UnityEngine;


//holds data and gameobject instances for viewer cursors
public class UserPointerData
{
    public GreenHeatMessage latestMessage;
    public GameObject pointer;
    public Color color;
    public bool active;
    
    public UserPointerData(GreenHeatMessage message, GameObject pointerObject)
    {
        latestMessage = message;
        color = ColorUtility.RandomColor();
        pointer = pointerObject;
        active = true;
    }
}

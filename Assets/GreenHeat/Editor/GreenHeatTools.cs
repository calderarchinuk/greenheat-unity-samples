using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GreenHeatEventManager))]
public class GreenHeatTools : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open URL"))
        {
            GreenHeatEventManager gh = (GreenHeatEventManager)target;
            string testUrl = gh.url.Replace("wss://","https://");
            Application.OpenURL(testUrl);
        }
        base.OnInspectorGUI();
    }
}

using UnityEngine;

//spawns cubes with random colors, for testing palettes

public class ColorTester : MonoBehaviour
{   
    void SpawnGameObject(Vector3 position, Color color)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = position;
        var mr = go.GetComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mr.material.color = color;
    }
    
    void Start()
    {
        for(int x = 0;x<10;x++)
        {
            for(int y = 0;y<10;y++)
            {
                SpawnGameObject(new Vector3(x,y,0),ColorUtility.RandomColor());
            }   
        }
    }
}

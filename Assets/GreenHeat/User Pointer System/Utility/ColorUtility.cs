using UnityEngine;

public static class ColorUtility
{
    public static Color RandomColor()
    {
        //bright colours
        return Color.HSVToRGB(Random.Range(0f,1f),Random.Range(0.8f,1f),1);
        
        //pastels
        //return Color.HSVToRGB(Random.Range(0f,1f),Random.Range(0.1f,0.4f),Random.Range(0.3f,0.9f));
        
        //fully random
        //return new Color(UnityEngine.Random.Range(0f,1f),UnityEngine.Random.Range(0f,1f),UnityEngine.Random.Range(0f,1f));
    }
}
using System.Collections;
using UnityEngine;

public class BubbleEffects : MonoBehaviour
{
    public ParticleSystem particles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    //called with SendMessage from BubblePopGameMode
    public void Pop()
    {
        Destroy(gameObject,3);
    }
}

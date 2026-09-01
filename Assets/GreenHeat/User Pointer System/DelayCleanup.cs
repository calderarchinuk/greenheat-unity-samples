using UnityEngine;

public class DelayCleanup : MonoBehaviour
{
    public float CleanupTime = 3;
    
    public enum CleanupAction
    {
        Disable,
        Destroy
    }
    public CleanupAction Action = CleanupAction.Disable;
    
    async void Start()
    {
        await Awaitable.WaitForSecondsAsync(CleanupTime);
        
        switch (Action)
        {
            case CleanupAction.Disable: gameObject.SetActive(false); break;
            case CleanupAction.Destroy: Destroy(gameObject); break;
        }
    }
}

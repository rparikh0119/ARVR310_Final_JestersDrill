using UnityEngine;

public class ClownDeathDetector : MonoBehaviour
{
    [HideInInspector]
    public Zone1CarouselController zoneController;
    
    void OnDestroy()
    {
        // When clown is destroyed (by weapon), notify zone controller
        if (zoneController != null)
        {
            zoneController.OnClownDefeated();
        }
    }
}
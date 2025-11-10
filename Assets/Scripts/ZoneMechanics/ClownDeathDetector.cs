using UnityEngine;

public class ClownDeathDetector : MonoBehaviour
{
    [HideInInspector]
    public Zone1CarouselController zoneController; // For Zone 1
    
    [HideInInspector]
    public Zone2FerrisWheelController zone2Controller; // ✅ For Zone 2
    
    void OnDestroy()
    {
        // Notify appropriate zone controller
        if (zoneController != null)
        {
            zoneController.OnClownDefeated();
        }
        
        if (zone2Controller != null)
        {
            zone2Controller.OnClownDefeated();
        }
    }
}


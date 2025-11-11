using UnityEngine;

/// <summary>
/// Emergency fix - runs in Awake to fix camera BEFORE anything else can break it
/// Minimal code to avoid blocking
/// </summary>
public class EmergencyCameraFix : MonoBehaviour
{
    void Awake()
    {
        // Set execution order to run FIRST (you may need to set this in Project Settings > Script Execution Order)
        // Just fix the camera viewport immediately - no expensive operations
        FixCameraViewportNow();
    }
    
    void FixCameraViewportNow()
    {
        // Find camera - use Camera.main first (fastest)
        Camera cam = Camera.main;
        
        // If no main camera, try to find XR Origin camera (but don't block if it doesn't exist yet)
        if (cam == null)
        {
            GameObject xr = GameObject.Find("XR Origin");
            if (xr != null)
            {
                cam = xr.GetComponentInChildren<Camera>();
            }
        }
        
        if (cam != null)
        {
            // FORCE viewport to full screen - this is the fix
            cam.rect = new Rect(0, 0, 1, 1);
            cam.enabled = true;
        }
    }
    
    void Start()
    {
        // Fix again in Start in case camera wasn't ready in Awake
        FixCameraViewportNow();
    }
}


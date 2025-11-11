using UnityEngine;

/// <summary>
/// Detects if something is modifying the camera viewport (like a night vision effect)
/// </summary>
public class CameraViewportDetector : MonoBehaviour
{
    private Camera mainCamera;
    private Rect originalViewportRect;
    private bool isMonitoring = false;
    
    void Start()
    {
        // Find the VR camera
        GameObject xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin != null)
        {
            mainCamera = xrOrigin.GetComponentInChildren<Camera>();
        }
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (mainCamera != null)
        {
            originalViewportRect = mainCamera.rect;
            isMonitoring = true;
            Debug.Log($"CameraViewportDetector: Monitoring camera '{mainCamera.name}'. Original viewport: {originalViewportRect}");
        }
        else
        {
            Debug.LogError("CameraViewportDetector: No camera found!");
        }
    }
    
    void Update()
    {
        if (!isMonitoring || mainCamera == null) return;
        
        // Check if viewport has been modified
        if (mainCamera.rect != originalViewportRect)
        {
            Debug.LogWarning($"CameraViewportDetector: Camera viewport was modified! Current: {mainCamera.rect}, Original: {originalViewportRect}");
            Debug.LogWarning("This might be caused by a night vision effect or similar script!");
            
            // Try to find what's modifying it
            CheckForViewportModifiers();
        }
        
        // Check if camera is disabled
        if (!mainCamera.enabled)
        {
            Debug.LogWarning("CameraViewportDetector: Camera is disabled!");
        }
    }
    
    void CheckForViewportModifiers()
    {
        // Search for scripts that might modify viewport
        MonoBehaviour[] allScripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in allScripts)
        {
            if (script == null) continue;
            
            string scriptName = script.GetType().Name.ToLower();
            if (scriptName.Contains("night") || 
                scriptName.Contains("vision") || 
                scriptName.Contains("mask") ||
                scriptName.Contains("overlay") ||
                scriptName.Contains("viewport") ||
                scriptName.Contains("circle"))
            {
                Debug.LogError($"CameraViewportDetector: Found suspicious script: {script.GetType().Name} on {script.gameObject.name}");
            }
        }
    }
    
    void OnDestroy()
    {
        // Restore original viewport if it was modified
        if (mainCamera != null && mainCamera.rect != originalViewportRect)
        {
            Debug.Log("CameraViewportDetector: Restoring original viewport");
            mainCamera.rect = originalViewportRect;
        }
    }
}


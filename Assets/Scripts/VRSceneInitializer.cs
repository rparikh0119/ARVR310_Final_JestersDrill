using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// Ensures VR is properly initialized when the carnival scene loads
/// </summary>
public class VRSceneInitializer : MonoBehaviour
{
    void Start()
    {
        // Wait a frame to ensure everything is loaded
        StartCoroutine(InitializeVR());
    }
    
    System.Collections.IEnumerator InitializeVR()
    {
        // Wait for end of frame to ensure scene is fully loaded
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        
        // Check if XR is active
        if (!XRSettings.enabled)
        {
            Debug.LogWarning("VRSceneInitializer: XR is not enabled! Check Project Settings > XR Plug-in Management");
        }
        else
        {
            Debug.Log($"VRSceneInitializer: XR is enabled. Device: {XRSettings.loadedDeviceName}");
        }
        
        // Find and verify XR Origin exists
        GameObject xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin == null)
        {
            Debug.LogError("VRSceneInitializer: XR Origin not found in scene! VR will not work!");
        }
        else
        {
            Debug.Log("VRSceneInitializer: XR Origin found");
            
            // Ensure XR Origin is active
            if (!xrOrigin.activeInHierarchy)
            {
                Debug.LogWarning("VRSceneInitializer: XR Origin was inactive! Activating now...");
                xrOrigin.SetActive(true);
            }
            
            // Find camera under XR Origin
            Camera vrCamera = xrOrigin.GetComponentInChildren<Camera>();
            if (vrCamera == null)
            {
                Debug.LogError("VRSceneInitializer: No Camera found under XR Origin!");
            }
            else
            {
                if (!vrCamera.enabled)
                {
                    Debug.LogWarning("VRSceneInitializer: VR Camera was disabled! Enabling now...");
                    vrCamera.enabled = true;
                }
                Debug.Log($"VRSceneInitializer: VR Camera found and enabled: {vrCamera.name}");
            }
        }
        
        Debug.Log("VRSceneInitializer: VR initialization check complete");
    }
}


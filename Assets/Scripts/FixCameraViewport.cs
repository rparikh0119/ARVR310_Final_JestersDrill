using UnityEngine;
using System.Collections;

/// <summary>
/// Fixes camera viewport issues and disables any circular/night vision effects
/// </summary>
public class FixCameraViewport : MonoBehaviour
{
    void Start()
    {
        // Use coroutine to avoid blocking scene load
        StartCoroutine(FixCameraDelayed());
    }
    
    System.Collections.IEnumerator FixCameraDelayed()
    {
        // Wait for scene to fully load
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        
        // Now fix the camera
        FixCamera();
        
        // Also check periodically in case something tries to modify it
        InvokeRepeating(nameof(FixCamera), 0.5f, 0.5f);
    }
    
    void FixCamera()
    {
        // Find VR camera - cache it to avoid repeated searches
        Camera vrCamera = null;
        
        // Try to find XR Origin camera
        GameObject xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin != null)
        {
            vrCamera = xrOrigin.GetComponentInChildren<Camera>();
        }
        
        if (vrCamera == null)
        {
            vrCamera = Camera.main;
        }
        
        if (vrCamera != null)
        {
            // Reset viewport to full screen (no circular mask) - FORCE IT
            vrCamera.rect = new Rect(0, 0, 1, 1);
            
            // Ensure camera is enabled
            vrCamera.enabled = true;
        }
        
        // Disable masks in separate coroutine to avoid blocking
        StartCoroutine(DisableMasksCoroutine());
    }
    
    System.Collections.IEnumerator DisableMasksCoroutine()
    {
        yield return null; // Wait a frame to avoid blocking
        
        // Find all Canvas objects
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            // Check for RectMask2D or Mask components that might create circular viewport
            UnityEngine.UI.RectMask2D[] masks = canvas.GetComponentsInChildren<UnityEngine.UI.RectMask2D>();
            foreach (var mask in masks)
            {
                // If it's creating a circular effect, disable it
                if (mask.gameObject.name.ToLower().Contains("night") || 
                    mask.gameObject.name.ToLower().Contains("vision") ||
                    mask.gameObject.name.ToLower().Contains("circle") ||
                    mask.gameObject.name.ToLower().Contains("mask"))
                {
                    Debug.LogWarning($"FixCameraViewport: Disabling suspicious mask: {mask.gameObject.name}");
                    mask.gameObject.SetActive(false);
                }
            }
            
            // Check for Image components with circular sprites - DISABLE ALL LARGE OVERLAYS
            UnityEngine.UI.Image[] images = canvas.GetComponentsInChildren<UnityEngine.UI.Image>();
            foreach (var image in images)
            {
                RectTransform rect = image.GetComponent<RectTransform>();
                if (rect != null)
                {
                    // If it's a large overlay covering the screen, disable it
                    if (rect.sizeDelta.x > 1000 || rect.sizeDelta.y > 1000 || 
                        rect.anchorMin == Vector2.zero && rect.anchorMax == Vector2.one)
                    {
                        string name = image.gameObject.name.ToLower();
                        if (name.Contains("night") || name.Contains("vision") || 
                            name.Contains("circle") || name.Contains("overlay") ||
                            name.Contains("mask") || name.Contains("viewport"))
                        {
                            Debug.LogWarning($"FixCameraViewport: Disabling large overlay image: {image.gameObject.name}");
                            image.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
    
    void OnDestroy()
    {
        CancelInvoke();
    }
}


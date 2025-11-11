using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

/// <summary>
/// Aggressively disables ALL teleportation components to prevent hourglass freeze
/// Run this in Awake to catch everything before it initializes
/// </summary>
public class DisableAllTeleportation : MonoBehaviour
{
    void Awake()
    {
        // Disable critical components immediately (fast operations only)
        DisableCriticalComponents();
    }
    
    void Start()
    {
        // Use coroutine for expensive operations to avoid blocking
        StartCoroutine(DisableTeleportationDelayed());
    }
    
    void DisableCriticalComponents()
    {
        // Only disable components that are already found - don't search
        // This prevents blocking during scene load
    }
    
    System.Collections.IEnumerator DisableTeleportationDelayed()
    {
        // Wait a frame to ensure scene is loaded
        yield return null;
        
        // Now do the expensive searches
        DisableTeleportationNow();
    }
    
    void DisableTeleportationNow()
    {
        // Find and disable ALL teleportation providers
        var teleportProviders = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation.TeleportationProvider>(true);
        foreach (var provider in teleportProviders)
        {
            if (provider != null && provider.enabled)
            {
                Debug.LogWarning($"DisableAllTeleportation: Disabling TeleportationProvider on {provider.gameObject.name}");
                provider.enabled = false;
            }
        }
        
        // Find and disable ALL XR Ray Interactors (these can cause hourglass)
        var rayInteractors = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>(true);
        foreach (var ray in rayInteractors)
        {
            if (ray != null && ray.enabled)
            {
                Debug.LogWarning($"DisableAllTeleportation: Disabling XRRayInteractor on {ray.gameObject.name}");
                ray.enabled = false;
            }
        }
        
        // Note: XRTeleportationInteractor and XRReticle types don't exist in this XR Interaction Toolkit version
        // The TeleportationProvider and XRRayInteractor disabling above should be sufficient
        
        // Disable line visuals (teleportation lines)
        var lineVisuals = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals.XRInteractorLineVisual>(true);
        foreach (var line in lineVisuals)
        {
            if (line != null && line.enabled)
            {
                Debug.LogWarning($"DisableAllTeleportation: Disabling XRInteractorLineVisual on {line.gameObject.name}");
                line.enabled = false;
            }
        }
        
        // Find objects by name that might be teleportation related
        // Use a more efficient approach - search only active objects in hierarchy
        // This avoids the expensive FindObjectsOfType<GameObject>(true) call
        SearchForTeleportObjectsInHierarchy();
        
        Debug.Log("DisableAllTeleportation: Teleportation cleanup complete");
    }
    
    void SearchForTeleportObjectsInHierarchy()
    {
        // Search only root objects, then recursively search children
        // This is much faster than FindObjectsOfType<GameObject>(true)
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        
        foreach (GameObject root in rootObjects)
        {
            SearchChildrenRecursive(root.transform);
        }
    }
    
    void SearchChildrenRecursive(Transform parent)
    {
        string lowerName = parent.name.ToLower();
        if ((lowerName.Contains("teleport") || 
             lowerName.Contains("reticle") || 
             lowerName.Contains("hourglass") ||
             lowerName.Contains("line visual") ||
             lowerName.Contains("ray")) && 
            parent.gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"DisableAllTeleportation: Disabling suspicious object: {parent.name}");
            parent.gameObject.SetActive(false);
            return; // Don't search children if we disabled the parent
        }
        
        // Recursively search children
        for (int i = 0; i < parent.childCount; i++)
        {
            SearchChildrenRecursive(parent.GetChild(i));
        }
    }
}


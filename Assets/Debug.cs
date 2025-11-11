using UnityEngine;
using UnityEngine.XR.Management;
using System.Collections;
using System.Collections.Generic;

public class DebugEverything : MonoBehaviour
{
    void Start()
    {
        // Use coroutine to avoid blocking scene load
        StartCoroutine(DebugDelayed());
    }
    
    System.Collections.IEnumerator DebugDelayed()
    {
        // Wait for scene to fully load
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.5f);
        
        Debug.Log("==========================================");
        Debug.Log("=== COMPREHENSIVE DEBUG LOG START ===");
        Debug.Log("==========================================");
        
        // Check XR Status (fast, no delay needed)
        CheckXRStatus();
        
        yield return null; // Yield between expensive operations
        
        // Find all suspicious objects (EXPENSIVE - delayed)
        FindSuspiciousObjects();
        
        yield return null;
        
        // Check all cameras
        CheckCameras();
        
        yield return null;
        
        // Check all canvases
        CheckCanvases();
        
        yield return null;
        
        // Check all interactors
        CheckInteractors();
        
        yield return null;
        
        // Check for fade/vignette (EXPENSIVE - delayed)
        CheckFadeComponents();
        
        Debug.Log("==========================================");
        Debug.Log("=== COMPREHENSIVE DEBUG LOG END ===");
        Debug.Log("==========================================");
    }
    
    void CheckXRStatus()
    {
        Debug.Log("--- XR STATUS ---");
        
        if (XRGeneralSettings.Instance == null)
        {
            Debug.LogError("XRGeneralSettings.Instance is NULL!");
            return;
        }
        
        Debug.Log($"XR Manager Active: {XRGeneralSettings.Instance.Manager != null}");
        
        if (XRGeneralSettings.Instance.Manager != null)
        {
            Debug.Log($"Active Loader: {XRGeneralSettings.Instance.Manager.activeLoader}");
            Debug.Log($"Is Initialized: {XRGeneralSettings.Instance.Manager.isInitializationComplete}");
        }
    }
    
    void FindSuspiciousObjects()
    {
        Debug.Log("--- SUSPICIOUS OBJECTS ---");
        
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>(true);
        List<string> suspiciousNames = new List<string> { "teleport", "reticle", "hourglass", "cursor", "fade", "vignette", "loading" };
        
        foreach (GameObject obj in allObjects)
        {
            string lowerName = obj.name.ToLower();
            foreach (string suspicious in suspiciousNames)
            {
                if (lowerName.Contains(suspicious))
                {
                    Debug.Log($"FOUND: {GetPath(obj.transform)} | Active: {obj.activeInHierarchy} | Layer: {LayerMask.LayerToName(obj.layer)}");
                    
                    // List all components
                    Component[] components = obj.GetComponents<Component>();
                    foreach (Component comp in components)
                    {
                        if (comp != null)
                        {
                            Debug.Log($"  Component: {comp.GetType().Name} | Enabled: {(comp is Behaviour ? ((Behaviour)comp).enabled.ToString() : "N/A")}");
                        }
                    }
                    break;
                }
            }
        }
    }
    
    void CheckCameras()
    {
        Debug.Log("--- ALL CAMERAS ---");
        
        Camera[] cameras = GameObject.FindObjectsOfType<Camera>(true);
        Debug.Log($"Total Cameras Found: {cameras.Length}");
        
        foreach (Camera cam in cameras)
        {
            Debug.Log($"Camera: {GetPath(cam.transform)}");
            Debug.Log($"  Enabled: {cam.enabled} | Active: {cam.gameObject.activeInHierarchy}");
            Debug.Log($"  Clear Flags: {cam.clearFlags} | Background: {cam.backgroundColor}");
            Debug.Log($"  Culling Mask: {cam.cullingMask}");
            Debug.Log($"  Depth: {cam.depth}");
            
            // Check for post-processing
            Component[] camComponents = cam.GetComponents<Component>();
            foreach (Component comp in camComponents)
            {
                if (comp != null)
                {
                    Debug.Log($"  Component: {comp.GetType().Name}");
                }
            }
        }
    }
    
    void CheckCanvases()
    {
        Debug.Log("--- ALL CANVASES ---");
        
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>(true);
        Debug.Log($"Total Canvases Found: {canvases.Length}");
        
        foreach (Canvas canvas in canvases)
        {
            Debug.Log($"Canvas: {GetPath(canvas.transform)} | Active: {canvas.gameObject.activeInHierarchy}");
            Debug.Log($"  Render Mode: {canvas.renderMode} | Sort Order: {canvas.sortingOrder}");
        }
    }
    
    void CheckInteractors()
    {
        Debug.Log("--- ALL INTERACTORS ---");
        
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor[] interactors = GameObject.FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor>(true);
        Debug.Log($"Total Interactors Found: {interactors.Length}");
        
        foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInteractor interactor in interactors)
        {
            Debug.Log($"Interactor: {interactor.GetType().Name} on {GetPath(interactor.transform)}");
            Debug.Log($"  Enabled: {interactor.enabled} | Active: {interactor.gameObject.activeInHierarchy}");
        }
    }
    
    void CheckFadeComponents()
    {
        Debug.Log("--- FADE/VIGNETTE COMPONENTS ---");
        
        MonoBehaviour[] allScripts = GameObject.FindObjectsOfType<MonoBehaviour>(true);
        
        foreach (MonoBehaviour script in allScripts)
        {
            string typeName = script.GetType().Name.ToLower();
            if (typeName.Contains("fade") || typeName.Contains("vignette") || typeName.Contains("tunnel"))
            {
                Debug.Log($"FOUND FADE/VIGNETTE: {script.GetType().Name} on {GetPath(script.transform)}");
                Debug.Log($"  Enabled: {script.enabled} | Active: {script.gameObject.activeInHierarchy}");
            }
        }
    }
    
    string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
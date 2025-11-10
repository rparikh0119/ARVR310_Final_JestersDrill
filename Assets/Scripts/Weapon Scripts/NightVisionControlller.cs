using UnityEngine;

public class NightVisionController : MonoBehaviour
{
    [Header("Night Vision Settings")]
    public Material nightVisionMaterial;
    public bool isNightVisionActive = false;
    
    [Header("Audio")]
    public AudioClip toggleOnSound;
    public AudioClip toggleOffSound;
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f; // 2D sound
        isNightVisionActive = false;
    }
    
    public void ToggleNightVision()
    {
        isNightVisionActive = !isNightVisionActive;
        
        if (isNightVisionActive)
        {
            if (toggleOnSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(toggleOnSound);
            }
            Debug.Log("Night Vision: ON");
        }
        else
        {
            if (toggleOffSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(toggleOffSound);
            }
            Debug.Log("Night Vision: OFF");
        }
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (isNightVisionActive && nightVisionMaterial != null)
        {
            // Apply night vision shader
            Graphics.Blit(source, destination, nightVisionMaterial);
        }
        else
        {
            // No effect - pass through
            Graphics.Blit(source, destination);
        }
    }
}
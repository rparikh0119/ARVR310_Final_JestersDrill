using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem; // ADD THIS

public class NightVisionEffect : MonoBehaviour
{
    [Header("Night Vision Settings")]
    public Volume postProcessVolume;
    public float transitionSpeed = 2f;
    
    [Header("Input (Temporary - will use VR button later)")]
    public InputActionReference toggleAction; // We'll set this up
    
    private ColorAdjustments colorAdjustments;
    private Vignette vignette;
    private bool isNightVisionActive = false;
    
    // Night vision values
    private Color nightVisionTint = new Color(0.2f, 1f, 0.2f, 0f);
    private float nightVisionSaturation = -100f;
    private float nightVisionContrast = 30f;
    
    // Normal values
    private Color normalTint = new Color(0f, 0f, 0f, 0f);
    private float normalSaturation = 0f;
    private float normalContrast = 0f;
    
    void Start()
    {
        Debug.Log("NightVisionEffect script started!");
        
        if (postProcessVolume != null)
        {
            Debug.Log("Post process volume found!");
            postProcessVolume.profile.TryGet(out colorAdjustments);
            postProcessVolume.profile.TryGet(out vignette);
            
            if (colorAdjustments == null)
            {
                colorAdjustments = postProcessVolume.profile.Add<ColorAdjustments>();
            }
            if (vignette == null)
            {
                vignette = postProcessVolume.profile.Add<Vignette>();
            }
            
            SetNormalVision();
        }
        
        // Enable the input action if assigned
        if (toggleAction != null)
        {
            toggleAction.action.Enable();
            toggleAction.action.performed += OnToggle;
        }
    }
    
    void OnDestroy()
    {
        // Clean up input
        if (toggleAction != null)
        {
            toggleAction.action.performed -= OnToggle;
        }
    }
    
    void OnToggle(InputAction.CallbackContext context)
    {
        ToggleNightVision();
    }
    
    void Update()
    {
        // Smooth transition
        if (isNightVisionActive)
        {
            ApplyNightVision();
        }
        else
        {
            ApplyNormalVision();
        }
    }
    
    public void ToggleNightVision()
    {
        isNightVisionActive = !isNightVisionActive;
        Debug.Log("Night Vision: " + (isNightVisionActive ? "ON" : "OFF"));
    }
    
    void ApplyNightVision()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = Color.Lerp(
                colorAdjustments.colorFilter.value, 
                nightVisionTint, 
                Time.deltaTime * transitionSpeed
            );
            
            colorAdjustments.saturation.value = Mathf.Lerp(
                colorAdjustments.saturation.value,
                nightVisionSaturation,
                Time.deltaTime * transitionSpeed
            );
            
            colorAdjustments.contrast.value = Mathf.Lerp(
                colorAdjustments.contrast.value,
                nightVisionContrast,
                Time.deltaTime * transitionSpeed
            );
        }
        
        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(
                vignette.intensity.value,
                0.4f,
                Time.deltaTime * transitionSpeed
            );
        }
    }
    
    void ApplyNormalVision()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = Color.Lerp(
                colorAdjustments.colorFilter.value,
                normalTint,
                Time.deltaTime * transitionSpeed
            );
            
            colorAdjustments.saturation.value = Mathf.Lerp(
                colorAdjustments.saturation.value,
                normalSaturation,
                Time.deltaTime * transitionSpeed
            );
            
            colorAdjustments.contrast.value = Mathf.Lerp(
                colorAdjustments.contrast.value,
                normalContrast,
                Time.deltaTime * transitionSpeed
            );
        }
        
        if (vignette != null)
        {
            vignette.intensity.value = Mathf.Lerp(
                vignette.intensity.value,
                0f,
                Time.deltaTime * transitionSpeed
            );
        }
    }
    
    void SetNormalVision()
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.colorFilter.value = normalTint;
            colorAdjustments.saturation.value = normalSaturation;
            colorAdjustments.contrast.value = normalContrast;
        }
        if (vignette != null)
        {
            vignette.intensity.value = 0f;
        }
    }
}
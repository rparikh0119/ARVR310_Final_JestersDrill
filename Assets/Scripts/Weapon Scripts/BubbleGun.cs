using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class BubbleGun : MonoBehaviour
{
    [Header("Gun Settings")]
    public GameObject bubbleBulletPrefab;
    public Transform muzzlePoint;
    public float fireRate = 0.3f;
    private float nextFireTime = 0f;
    
    [Header("Visual Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem shootEffect;
    public Light muzzleLight;
    public float lightDuration = 0.1f;
    
    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip emptyClickSound;
    private AudioSource audioSource;
    
    [Header("Haptics")]
    public float hapticIntensity = 0.5f;
    public float hapticDuration = 0.1f;
    
    [Header("Input")]
    public InputActionProperty activateAction;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor;
    private bool wasTriggerPressed = false;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.spatialBlend = 1f;
        
        if (muzzleLight != null)
            muzzleLight.enabled = false;
    }
    
    void OnEnable()
    {
        // Only initialize if we're in play mode (not during scene load)
        if (!Application.isPlaying) return;
        
        // Find the controller when weapon is equipped
        StartCoroutine(FindControllerDelayed());
        
        // Enable the activate action (safely)
        try
        {
            if (activateAction.action != null)
            {
                activateAction.action.Enable();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"BubbleGun: Could not enable input action: {e.Message}");
        }
    }
    
    void OnDisable()
    {
        // Disable the activate action
        if (activateAction.action != null)
        {
            activateAction.action.Disable();
        }
    }
    
    System.Collections.IEnumerator FindControllerDelayed()
    {
        // Small delay to ensure parent hierarchy is set up
        yield return new WaitForSeconds(0.1f);
        
        // Look for controller in parent hierarchy
        controllerInteractor = GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor>();
        
        if (controllerInteractor == null)
        {
            Debug.LogWarning("BubbleGun: Could not find XRBaseControllerInteractor in parent!");
        }
        else
        {
            Debug.Log("BubbleGun: Found controller, ready to shoot!");
        }
    }
    
    void Update()
    {
        // Check if trigger is pressed using Input Action
        if (activateAction.action != null)
        {
            float triggerValue = activateAction.action.ReadValue<float>();
            bool isTriggerPressed = triggerValue > 0.1f; // Threshold for trigger press
            
            // Detect when trigger was just pressed (transition from not pressed to pressed)
            if (isTriggerPressed && !wasTriggerPressed)
            {
                Shoot();
            }
            
            wasTriggerPressed = isTriggerPressed;
        }
    }
    
    public void Shoot()
    {
        // Check fire rate cooldown
        if (Time.time < nextFireTime)
        {
            // Gun is still cooling down - play empty click
            if (emptyClickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(emptyClickSound);
            }
            return;
        }
        
        // FIRE THE GUN
        if (bubbleBulletPrefab != null && muzzlePoint != null)
        {
            // Spawn bullet
            GameObject bullet = Instantiate(bubbleBulletPrefab, 
                                           muzzlePoint.position, 
                                           muzzlePoint.rotation);
            
            Debug.Log("BubbleGun: Fired!");
        }
        else
        {
            Debug.LogError("BubbleGun: Missing bubbleBulletPrefab or muzzlePoint!");
        }
        
        // Play shoot sound
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
        
        // Visual effects
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        
        if (shootEffect != null)
        {
            shootEffect.Play();
        }
        
        // Muzzle flash light
        if (muzzleLight != null)
        {
            StartCoroutine(FlashLight());
        }
        
        // Haptic feedback
        if (controllerInteractor != null)
        {
            controllerInteractor.SendHapticImpulse(hapticIntensity, hapticDuration);
        }
        
        // Set next fire time
        nextFireTime = Time.time + fireRate;
    }
    
    System.Collections.IEnumerator FlashLight()
    {
        if (muzzleLight != null)
        {
            muzzleLight.enabled = true;
            yield return new WaitForSeconds(lightDuration);
            muzzleLight.enabled = false;
        }
    }
}
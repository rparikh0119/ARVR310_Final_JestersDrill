using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class PrizeDartLauncher : MonoBehaviour
{
    [Header("Dart Settings")]
    public GameObject dartPrefab;
    public Transform muzzlePoint;
    public float dartSpeed = 30f;
    public float fireRate = 0.4f; // Fast fire rate
    private float nextFireTime = 0f;
    
    [Header("Visual Effects")]
    public ParticleSystem muzzleFlash;
    public Light muzzleLight;
    public float lightDuration = 0.1f;
    
    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip emptyClickSound;
    private AudioSource audioSource;
    
    [Header("Haptics")]
    public float hapticIntensity = 0.3f;
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
            Debug.LogWarning($"PrizeDartLauncher: Could not enable input action: {e.Message}");
        }
    }
    
    void OnDisable()
    {
        if (activateAction.action != null)
        {
            activateAction.action.Disable();
        }
    }
    
    System.Collections.IEnumerator FindControllerDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        
        controllerInteractor = GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor>();
        
        if (controllerInteractor == null)
        {
            Debug.LogWarning("PrizeDartLauncher: Could not find XRBaseControllerInteractor in parent!");
        }
        else
        {
            Debug.Log("PrizeDartLauncher: Found controller, ready to shoot darts!");
        }
    }
    
    void Update()
    {
        if (activateAction.action != null)
        {
            float triggerValue = activateAction.action.ReadValue<float>();
            bool isTriggerPressed = triggerValue > 0.1f;
            
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
            if (emptyClickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(emptyClickSound);
            }
            return;
        }
        
        // FIRE THE DART
        if (dartPrefab != null && muzzlePoint != null)
        {
            GameObject dart = Instantiate(dartPrefab, muzzlePoint.position, muzzlePoint.rotation);
            
            // Add velocity to dart
            Rigidbody rb = dart.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = muzzlePoint.forward * dartSpeed;
            }
            
            Debug.Log("PrizeDartLauncher: Fired dart!");
        }
        else
        {
            Debug.LogError("PrizeDartLauncher: Missing dartPrefab or muzzlePoint!");
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
        
        // Muzzle light
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
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class PrizeDartLauncher : MonoBehaviour
{
    [Header("Dart Settings")]
    public GameObject dartPrefab;
    public Transform muzzlePoint;
    public float dartSpeed = 30f;
    public float fireRate = 0.4f;
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
        Debug.Log($"[PrizeDartLauncher] Start() called on {gameObject.name}");
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 1f;
        
        if (muzzleLight != null)
        {
            muzzleLight.enabled = false;
        }
        
        StartCoroutine(FindControllerDelayed());
    }
    
    void OnEnable()
    {
        // Only enable if we're in play mode and action is valid
        if (!Application.isPlaying) return;
        
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
        Debug.Log($"[PrizeDartLauncher] Shoot() - Time: {Time.time}");
        
        if (Time.time < nextFireTime)
        {
            if (emptyClickSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(emptyClickSound);
            }
            return;
        }
        
        if (dartPrefab != null && muzzlePoint != null)
        {
            GameObject dart = Instantiate(dartPrefab, muzzlePoint.position, muzzlePoint.rotation);
            
            Rigidbody rb = dart.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = muzzlePoint.forward * dartSpeed;
            }
            
            Debug.Log("[PrizeDartLauncher] ✅ Dart fired!");
        }
        else
        {
            Debug.LogError("[PrizeDartLauncher] ❌ Missing dartPrefab or muzzlePoint!");
        }
        
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
        
        if (muzzleFlash != null) muzzleFlash.Play();
        
        if (muzzleLight != null)
        {
            StartCoroutine(FlashLight());
        }
        
        if (controllerInteractor != null)
        {
            controllerInteractor.SendHapticImpulse(hapticIntensity, hapticDuration);
        }
        
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
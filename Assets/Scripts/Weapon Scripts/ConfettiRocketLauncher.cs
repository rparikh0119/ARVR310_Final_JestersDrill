using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ConfettiRocketLauncher : MonoBehaviour
{
    [Header("Rocket Settings")]
    public GameObject rocketPrefab;
    public Transform muzzlePoint;
    public float rocketSpeed = 25f;
    public float fireRate = 1.2f; // Slower fire rate for powerful weapon
    private float nextFireTime = 0f;
    
    [Header("Visual Effects")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem smokeTrail; // Smoke when firing
    public Light muzzleLight;
    public float lightDuration = 0.2f;
    
    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip emptyClickSound;
    private AudioSource audioSource;
    
    [Header("Haptics")]
    public float hapticIntensity = 0.8f; // Strong kick
    public float hapticDuration = 0.2f;
    
    [Header("Input")]
    public InputActionProperty activateAction;
    
    private UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor;
    private bool wasTriggerPressed = false;
    
    void Start()
    {
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
            Debug.LogWarning($"ConfettiRocketLauncher: Could not enable input action: {e.Message}");
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
            Debug.LogWarning("ConfettiRocketLauncher: Could not find XRBaseControllerInteractor in parent!");
        }
        else
        {
            Debug.Log("ConfettiRocketLauncher: Found controller, ready to launch!");
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
        
        // FIRE THE ROCKET
        if (rocketPrefab != null && muzzlePoint != null)
        {
            GameObject rocket = Instantiate(rocketPrefab, muzzlePoint.position, muzzlePoint.rotation);
            
            // Add velocity to rocket
            Rigidbody rb = rocket.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = muzzlePoint.forward * rocketSpeed;
            }
            
            Debug.Log("ConfettiRocketLauncher: Fired rocket!");
        }
        else
        {
            Debug.LogError("ConfettiRocketLauncher: Missing rocketPrefab or muzzlePoint!");
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
        
        if (smokeTrail != null)
        {
            smokeTrail.Play();
        }
        
        // Muzzle light
        if (muzzleLight != null)
        {
            StartCoroutine(FlashLight());
        }
        
        // Strong haptic feedback
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

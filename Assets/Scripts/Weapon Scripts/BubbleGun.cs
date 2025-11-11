using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class BubbleGun : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bubbleBulletPrefab;
    public Transform muzzlePoint;
    public float fireRate = 0.3f;
    
    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    public Light muzzleLight;
    public float lightDuration = 0.05f;
    
    [Header("Audio")]
    public AudioClip shootSound;
    private AudioSource audioSource;
    
    [Header("Haptics")]
    public float hapticIntensity = 0.5f;
    public float hapticDuration = 0.1f;
    
    [Header("Input")]
    public InputActionProperty activateAction;
    
    private float nextFireTime;
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
            Debug.LogWarning($"BubbleGun: Could not enable input action: {e.Message}");
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
        if (Time.time < nextFireTime) return;
        
        // SPAWN BULLET
        if (bubbleBulletPrefab != null && muzzlePoint != null)
        {
            GameObject bullet = Instantiate(bubbleBulletPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Debug.Log("✅ BUBBLE BULLET SPAWNED!");
        }
        else
        {
            if (bubbleBulletPrefab == null) Debug.LogError("❌ BUBBLE BULLET PREFAB NOT ASSIGNED!");
            if (muzzlePoint == null) Debug.LogError("❌ MUZZLE POINT NOT ASSIGNED!");
        }
        
        // Effects
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
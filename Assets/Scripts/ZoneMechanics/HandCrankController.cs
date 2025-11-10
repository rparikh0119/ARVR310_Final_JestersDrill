using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class HandCrankController : MonoBehaviour
{
    [Header("Crank Settings")]
    public float rotationsNeeded = 5f; // Full turns needed
    public float rotationThreshold = 30f; // Degrees per update to count
    
    [Header("Audio")]
    public AudioClip crankSound;
    public AudioClip completeSound;
    
    [Header("Unity Events")]
    public UnityEvent onCrankComplete;
    
    private AudioSource audioSource;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private float totalRotation = 0f;
    private float lastYRotation = 0f;
    private bool isGrabbed = false;
    private bool isComplete = false;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }
        
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
        
        lastYRotation = transform.localEulerAngles.y;
    }
    
    void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        lastYRotation = transform.localEulerAngles.y;
    }
    
    void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }
    
    void Update()
    {
        if (!isGrabbed || isComplete) return;
        
        float currentY = transform.localEulerAngles.y;
        float deltaRotation = Mathf.DeltaAngle(lastYRotation, currentY);
        
        // Only count positive rotation (clockwise)
        if (Mathf.Abs(deltaRotation) > rotationThreshold)
        {
            totalRotation += Mathf.Abs(deltaRotation);
            
            // Play crank sound
            if (audioSource != null && crankSound != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(crankSound, 0.5f);
            }
            
            Debug.Log($"Crank Progress: {totalRotation}/{rotationsNeeded * 360f} degrees");
        }
        
        lastYRotation = currentY;
        
        // Check if complete
        if (totalRotation >= (rotationsNeeded * 360f))
        {
            CompleteCrank();
        }
    }
    
    void CompleteCrank()
    {
        if (isComplete) return;
        isComplete = true;
        
        Debug.Log("HandCrank: Complete!");
        
        // Play complete sound
        if (audioSource != null && completeSound != null)
        {
            audioSource.PlayOneShot(completeSound);
        }
        
        // Invoke event
        onCrankComplete?.Invoke();
        
        // Disable further interaction
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }
    }
}
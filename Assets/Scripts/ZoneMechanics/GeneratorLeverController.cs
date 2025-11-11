using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using System.Linq;

[System.Serializable]
public class LeverPushEvent : UnityEvent<int> { }

public class GeneratorLeverController : MonoBehaviour
{
    [Header("Push Settings")]
    public int requiredPushes = 3;
    public float maxPushDistance = 0.2f;
    public float pushThreshold = 0.15f;
    
    [Header("Audio")]
    public AudioClip pushSound;
    public AudioClip finalPushSound;
    
    [Header("Unity Events")]
    public LeverPushEvent onLeverPushed;
    
    private AudioSource audioSource;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Vector3 startLocalPosition;
    private int currentPushCount = 0;
    private bool wasPushedDown = false;
    private bool isGrabbed = false;
    private Vector3 grabStartPos;
    private Vector3 leverStartPos;
    private Transform currentInteractorTransform;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }
        
        startLocalPosition = transform.localPosition;
        
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
        else
        {
            Debug.LogError("GeneratorLeverController: No XRGrabInteractable found!");
        }
    }
    
    void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        
        // Store the interactor transform safely
        if (args.interactorObject != null && args.interactorObject.transform != null)
        {
            currentInteractorTransform = args.interactorObject.transform;
            grabStartPos = currentInteractorTransform.position;
            leverStartPos = transform.position;
            Debug.Log("Lever grabbed! Ready to push.");
        }
        else
        {
            Debug.LogError("Interactor transform is null!");
            isGrabbed = false;
        }
    }
    
    void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
        wasPushedDown = false;
        currentInteractorTransform = null;
        Debug.Log("Lever released!");
        
        // Spring back if not final push
        if (currentPushCount < requiredPushes)
        {
            transform.localPosition = startLocalPosition;
        }
    }
    
    void LateUpdate()
    {
        // Safety checks
        if (!isGrabbed || currentInteractorTransform == null || grabInteractable == null)
        {
            return;
        }
        
        if (!grabInteractable.isSelected)
        {
            return;
        }
        
        try
        {
            // Get current hand position
            Vector3 currentHandPos = currentInteractorTransform.position;
            
            // Calculate how much hand moved down
            float handMovementY = currentHandPos.y - grabStartPos.y;
            
            // Apply to lever
            Vector3 newWorldPos = leverStartPos + new Vector3(0, handMovementY, 0);
            
            // Convert to local position
            Vector3 newLocalPos;
            if (transform.parent != null)
            {
                newLocalPos = transform.parent.InverseTransformPoint(newWorldPos);
            }
            else
            {
                newLocalPos = newWorldPos;
            }
            
            // Constrain to Y axis only and clamp
            newLocalPos.x = startLocalPosition.x;
            newLocalPos.z = startLocalPosition.z;
            newLocalPos.y = Mathf.Clamp(newLocalPos.y, 
                                        startLocalPosition.y - maxPushDistance, 
                                        startLocalPosition.y);
            
            transform.localPosition = newLocalPos;
            
            // Check if pushed far enough
            float distancePushed = startLocalPosition.y - newLocalPos.y;
            if (distancePushed >= pushThreshold && !wasPushedDown)
            {
                wasPushedDown = true;
                RegisterPush();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in LateUpdate: {e.Message}");
            isGrabbed = false;
        }
    }
    
    void RegisterPush()
    {
        currentPushCount++;
        Debug.Log($"🎰 Lever pushed: {currentPushCount}/{requiredPushes}");
        
        if (audioSource != null)
        {
            if (currentPushCount >= requiredPushes && finalPushSound != null)
                audioSource.PlayOneShot(finalPushSound);
            else if (pushSound != null)
                audioSource.PlayOneShot(pushSound);
        }
        
        onLeverPushed?.Invoke(currentPushCount);
        
        if (currentPushCount >= requiredPushes)
        {
            Debug.Log("💥 FINAL PUSH - Generator will explode!");
            
            // Lock in down position
            Vector3 lockedPos = startLocalPosition;
            lockedPos.y -= maxPushDistance;
            transform.localPosition = lockedPos;
            
            if (grabInteractable != null)
            {
                grabInteractable.enabled = false;
            }
        }
    }
    
    void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class LeverPushEvent : UnityEvent<int> { }

public class GeneratorLeverController : MonoBehaviour
{
    [Header("Push Settings")]
    public int requiredPushes = 3;
    public float pushAnimationDistance = 0.2f;
    public float pushAnimationSpeed = 5f;
    
    [Header("Audio")]
    public AudioClip pushSound;
    public AudioClip finalPushSound;
    
    [Header("Unity Events")]
    public LeverPushEvent onLeverPushed;
    
    private AudioSource audioSource;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    private Vector3 originalPosition;
    private int currentPushCount = 0;
    private bool isAnimating = false;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }
        
        originalPosition = transform.localPosition;
        
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        if (interactable == null)
        {
            interactable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
        }
        
        interactable.selectEntered.AddListener(OnPushed);
    }
    
    void OnPushed(SelectEnterEventArgs args)
    {
        if (isAnimating) return;
        
        currentPushCount++;
        Debug.Log($"Lever pushed: {currentPushCount}/{requiredPushes}");
        
        if (audioSource != null)
        {
            if (currentPushCount >= requiredPushes && finalPushSound != null)
                audioSource.PlayOneShot(finalPushSound);
            else if (pushSound != null)
                audioSource.PlayOneShot(pushSound);
        }
        
        StartCoroutine(PushDownAnimation());
        
        onLeverPushed?.Invoke(currentPushCount);
    }
    
    System.Collections.IEnumerator PushDownAnimation()
    {
        isAnimating = true;
        
        Vector3 targetPos = originalPosition + Vector3.down * pushAnimationDistance;
        
        float t = 0;
        while (t < 1)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPos, t);
            t += Time.deltaTime * pushAnimationSpeed;
            yield return null;
        }
        
        if (currentPushCount >= requiredPushes)
        {
            transform.localPosition = targetPos;
            interactable.enabled = false;
        }
        else
        {
            yield return new WaitForSeconds(0.3f);
            
            t = 0;
            while (t < 1)
            {
                transform.localPosition = Vector3.Lerp(targetPos, originalPosition, t);
                t += Time.deltaTime * pushAnimationSpeed;
                yield return null;
            }
            
            transform.localPosition = originalPosition;
        }
        
        isAnimating = false;
    }
}

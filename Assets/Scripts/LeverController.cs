using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class LeverController : MonoBehaviour
{
    [Header("Animation Settings")]
    public float spinSpeed = 360f; // Degrees per second
    public float pushDownDistance = 0.3f;
    public float pushDownSpeed = 1f;
    
    [Header("Audio")]
    public AudioClip socketSound; // When lever enters socket
    public AudioClip spinSound; // During spin
    public AudioClip clickSound; // When pushed down
    
    [Header("Unity Events - Configure in Inspector!")]
    public UnityEvent onLeverSocketed; // When lever first enters socket
    public UnityEvent onLeverPushedDown; // When animation completes
    
    private AudioSource audioSource;
    private Vector3 socketedPosition;
    private bool isAnimating = false;
    private bool hasCompletedAnimation = false;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }
    }
    
    // Called by XR Socket Interactor via Unity Event
    public void OnSocketed()
    {
        if (hasCompletedAnimation) return;
        
        Debug.Log("Lever: Socketed into control box!");
        
        // Play socket sound
        if (socketSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(socketSound);
        }
        
        // Store position
        socketedPosition = transform.localPosition;
        
        // Fire Unity Event
        onLeverSocketed.Invoke();
        
        // Start animation
        StartCoroutine(LeverAnimationSequence());
    }
    
    System.Collections.IEnumerator LeverAnimationSequence()
    {
        isAnimating = true;
        
        // PHASE 1: Spin 360 degrees
        float spinProgress = 0f;
        
        // Play spin sound
        if (spinSound != null && audioSource != null)
        {
            audioSource.clip = spinSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        while (spinProgress < 360f)
        {
            float spinThisFrame = spinSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, spinThisFrame, Space.Self);
            spinProgress += spinThisFrame;
            yield return null;
        }
        
        // Stop spin sound
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        
        // PHASE 2: Push down
        Vector3 startPos = transform.localPosition;
        Vector3 endPos = startPos + Vector3.down * pushDownDistance;
        float pushProgress = 0f;
        
        // Play click sound
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        
        while (pushProgress < 1f)
        {
            pushProgress += Time.deltaTime * pushDownSpeed;
            transform.localPosition = Vector3.Lerp(startPos, endPos, pushProgress);
            yield return null;
        }
        
        // Animation complete!
        hasCompletedAnimation = true;
        isAnimating = false;
        
        Debug.Log("Lever: Animation complete! Shutting down carousel...");
        
        // Fire Unity Event for shutdown
        onLeverPushedDown.Invoke();
    }
}
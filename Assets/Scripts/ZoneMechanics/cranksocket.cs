using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleCrankSocket : MonoBehaviour
{
    [Header("Spin Settings")]
    public float spinSpeed = 360f; // Degrees per second
    public int rotationsNeeded = 5;
    
    [Header("Audio")]
    public AudioClip socketSound;
    public AudioClip spinningSound;
    public AudioClip completeSound;
    
    [Header("Unity Events")]
    public UnityEvent onCrankComplete;
    
    private AudioSource audioSource;
    private Transform socketedCrank;
    private bool isSpinning = false;
    private float totalRotation = 0f;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }
        
        GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>().selectEntered.AddListener(OnSocketed);
    }
    
    void OnSocketed(SelectEnterEventArgs args)
    {
        socketedCrank = args.interactableObject.transform;
        
        Debug.Log("🔧 Crank socketed! Auto-spinning...");
        
        // Socket sound
        if (socketSound != null)
        {
            audioSource.PlayOneShot(socketSound);
        }
        
        // Start spinning
        isSpinning = true;
        
        // Spinning sound loop
        if (spinningSound != null)
        {
            audioSource.clip = spinningSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    
    void Update()
    {
        if (!isSpinning || socketedCrank == null) return;
        
        // Spin the socketed crank
        float rotationThisFrame = spinSpeed * Time.deltaTime;
        socketedCrank.Rotate(Vector3.right, rotationThisFrame, Space.Self);
        
        totalRotation += rotationThisFrame;
        
        // Check if done
        if (totalRotation >= (rotationsNeeded * 360f))
        {
            isSpinning = false;
            
            Debug.Log("🎯 Crank complete! Stopping ride!");
            
            // Stop spinning sound
            audioSource.Stop();
            
            // Complete sound
            if (completeSound != null)
            {
                audioSource.PlayOneShot(completeSound);
            }
            
            // Fire event
            onCrankComplete?.Invoke();
        }
    }
}
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class GearShutoffController : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip socketSound; // When gear enters socket
    public AudioClip clickSound; // When gear locks in
    public AudioClip shutoffSound; // Mechanical shutdown
    
    [Header("Unity Events - Configure in Inspector!")]
    public UnityEvent onGearSocketed; // When gear first enters socket
    public UnityEvent onShutoffComplete; // When shutdown completes
    
    private AudioSource audioSource;
    private bool hasBeenSocketed = false;
    
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
        if (hasBeenSocketed) return;
        
        hasBeenSocketed = true;
        Debug.Log("Gear: Socketed! Initiating shutdown...");
        
        // Play socket sound
        if (socketSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(socketSound);
        }
        
        // Fire Unity Event
        onGearSocketed.Invoke();
        
        // Start shutdown sequence
        StartCoroutine(ShutoffSequence());
    }
    
    System.Collections.IEnumerator ShutoffSequence()
    {
        // Small delay for realism
        yield return new WaitForSeconds(0.5f);
        
        // Play click/lock sound
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
        
        yield return new WaitForSeconds(0.3f);
        
        // Play shutoff sound
        if (shutoffSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shutoffSound);
        }
        
        // Shutdown complete!
        Debug.Log("Gear: Shutdown complete!");
        onShutoffComplete.Invoke();
    }
}
using UnityEngine;

public class RadioManager : MonoBehaviour
{
    [Header("Briggs Audio Clips")]
    public AudioClip[] briggsClips; // Drag 5 clips here
    
    [Header("Audio Source")]
    public AudioSource audioSource;
    
    [Header("Settings")]
    public float cooldown = 5f;
    
    private int currentClipIndex = 0;
    private float lastPressTime = -999f; // Allow first press immediately
    
    void Start()
    {
        // Get or add audio source
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.spatialBlend = 0f; // 2D sound (plays through headset)
        audioSource.playOnAwake = false;
    }
    
    // PUBLIC - Called by UI button
    public void PlayNextBriggsMessage()
    {
        // Check cooldown
        if (Time.time < lastPressTime + cooldown)
        {
            Debug.Log($"Radio on cooldown. Wait {(lastPressTime + cooldown - Time.time):F1} more seconds.");
            return;
        }
        
        // Check if we have clips
        if (briggsClips == null || briggsClips.Length == 0)
        {
            Debug.LogError("RadioManager: No audio clips assigned!");
            return;
        }
        
        // Play current clip
        audioSource.PlayOneShot(briggsClips[currentClipIndex]);
        Debug.Log($"✅ Playing Briggs message {currentClipIndex + 1} of {briggsClips.Length}");
        
        // Update for next press
        lastPressTime = Time.time;
        currentClipIndex++;
        
        // Loop back to first clip after all are played
        if (currentClipIndex >= briggsClips.Length)
        {
            currentClipIndex = 0;
            Debug.Log("🔁 Radio sequence completed. Restarting from message 1.");
        }
    }
}

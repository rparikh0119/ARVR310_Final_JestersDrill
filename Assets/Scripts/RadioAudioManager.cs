using UnityEngine;
using UnityEngine.Events;

public class RadioBriefingManager : MonoBehaviour
{
    [Header("Audio Clips - In Order")]
    public AudioClip[] briefingClips; // Drag Zone 1, Zone 2, Zone 3, etc. in order
    
    [Header("Settings")]
    public float cooldownTime = 30f; // 30 second delay
    
    [Header("Events")]
    public UnityEvent onBriefingComplete; // For unlocking zones later
    
    private AudioSource audioSource;
    private int currentBriefingIndex = 0;
    private float lastPlayTime = -999f;
    private bool canPlay = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayNextBriefing()
    {
        // Check cooldown
        if (!canPlay)
        {
            Debug.Log("Radio on cooldown. Wait before pressing again.");
            return;
        }

        // Check if we have more briefings to play
        if (currentBriefingIndex >= briefingClips.Length)
        {
            Debug.Log("All briefings complete.");
            return;
        }

        // Play current briefing
        audioSource.clip = briefingClips[currentBriefingIndex];
        audioSource.Play();
        
        Debug.Log($"Playing briefing {currentBriefingIndex + 1} of {briefingClips.Length}");
        
        // Start cooldown
        canPlay = false;
        Invoke("ResetCooldown", cooldownTime);
        
        // Move to next briefing for next time
        currentBriefingIndex++;
        
        // Fire event (for unlocking zones if needed)
        onBriefingComplete.Invoke();
    }

    void ResetCooldown()
    {
        canPlay = true;
        Debug.Log("Radio ready for next briefing.");
    }

    // Optional: Check if ready (for UI feedback)
    public bool IsReady()
    {
        return canPlay;
    }
}
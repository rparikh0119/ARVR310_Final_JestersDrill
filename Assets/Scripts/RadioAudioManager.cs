using UnityEngine;
using UnityEngine.Events;

public class RadioBriefingManager : MonoBehaviour
{
    [Header("Audio Clips - In Order")]
    public AudioClip[] briefingClips; // Zone 1, Zone 2, Zone 3, Zone 4, Zone 5
    
    [Header("Settings")]
    public float cooldownTime = 30f; // 30 second delay between briefings
    
    [Header("Events")]
    public UnityEvent onBriefingComplete; // Optional extra events
    
    private AudioSource audioSource;
    private int currentBriefingIndex = 0;
    private bool canPlay = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            Debug.LogError("RadioBriefingManager: No AudioSource found! Add one to this GameObject.");
        }
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
            Debug.Log("All briefings complete. All zones unlocked!");
            return;
        }

        // Play current briefing
        audioSource.clip = briefingClips[currentBriefingIndex];
        audioSource.Play();
        
        // Calculate which zone this briefing unlocks (Briefing 0 = Zone 1)
        int zoneToUnlock = currentBriefingIndex + 1;
        
        Debug.Log($"Playing briefing {currentBriefingIndex + 1} of {briefingClips.Length} - Unlocking Zone {zoneToUnlock}");
        
        // UNLOCK THE CORRESPONDING ZONE
        if (ZoneBarrierManager.Instance != null)
        {
            ZoneBarrierManager.Instance.UnlockZone(zoneToUnlock);
        }
        else
        {
            Debug.LogWarning("ZoneBarrierManager not found! Make sure it exists in the scene.");
        }
        
        // Start cooldown
        canPlay = false;
        Invoke("ResetCooldown", cooldownTime);
        
        // Move to next briefing for next time
        currentBriefingIndex++;
        
        // Fire optional event (for any extra functionality)
        onBriefingComplete.Invoke();
    }

    void ResetCooldown()
    {
        canPlay = true;
        Debug.Log("Radio ready for next briefing.");
    }

    // Check if radio is ready (for UI feedback or animations)
    public bool IsReady()
    {
        return canPlay;
    }
    
    // Check which zone is next
    public int GetNextZoneNumber()
    {
        return currentBriefingIndex + 1;
    }
}
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Zone4_SwingRideController : MonoBehaviour
{
    [Header("Ride Controller")]
    public SwingRideController swingRideController;
    
    [Header("Audio")]
    public AudioSource rideAudioSource;
    public AudioClip rideStopSound;
    public AudioClip clownSpawnSound;
    
    [Header("Enemy Spawning")]
    public GameObject animatedClownPrefab;
    public Transform[] spawnPoints;
    public float spawnDelay = 2f;
    
    [Header("Unity Events")]
    public UnityEvent onRideShutdown;
    
    private bool isShutdown = false;
    
    public void OnCrankComplete()
    {
        if (isShutdown) return;
        isShutdown = true;
        
        Debug.Log("Zone4: Crank complete - shutting down ride!");
        
        // Stop the ride
        if (swingRideController != null)
        {
            swingRideController.StopRide();
        }
        
        // Play stop sound
        if (rideAudioSource != null && rideStopSound != null)
        {
            rideAudioSource.PlayOneShot(rideStopSound);
        }
        
        // Spawn enemies
        StartCoroutine(SpawnEnemiesAfterDelay());
        
        // Invoke event
        onRideShutdown?.Invoke();
    }
    
    IEnumerator SpawnEnemiesAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        
        Debug.Log("Zone4: Spawning enemies");
        
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null && animatedClownPrefab != null)
            {
                GameObject clown = Instantiate(animatedClownPrefab, spawnPoint.position, spawnPoint.rotation);
                
                if (clownSpawnSound != null)
                {
                    AudioSource.PlayClipAtPoint(clownSpawnSound, spawnPoint.position);
                }
                
                Debug.Log($"Zone4: Spawned clown at {spawnPoint.name}");
            }
        }
    }
}
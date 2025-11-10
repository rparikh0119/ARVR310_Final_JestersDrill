using UnityEngine;
using UnityEngine.Events;

public class Zone2FerrisWheelController : MonoBehaviour
{
    [Header("References")]
    public FerrisWheelController ferrisWheelController;
    public Light[] ferrisWheelLights; // Array of all lights on the wheel
    
    [Header("Enemy Spawning")]
    public GameObject animatedClownPrefab;
    public Transform[] groundSpawnPoints;
    public float spawnDelay = 1f;
    public int clownsToSpawn = 3;
    
    [Header("Audio")]
    public AudioClip wheelStopSound;
    public AudioClip lightsOffSound;
    public AudioClip clownSpawnSound;
    
    [Header("Unity Events")]
    public UnityEvent onWheelShutdown; // When wheel stops
    public UnityEvent onAllClownsDefeated; // When zone complete
    
    private AudioSource audioSource;
    private int activeClowns = 0;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
        }
    }
    
    // Called by GearShutoffController via Unity Event
    public void ShutdownFerrisWheel()
    {
        Debug.Log("Zone2: Shutting down Ferris Wheel...");
        
        // Stop wheel rotation
        if (ferrisWheelController != null)
        {
            ferrisWheelController.StopFerrisWheel();
            
            if (wheelStopSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(wheelStopSound);
            }
        }
        
        // Turn off all lights
        StartCoroutine(FadeOutLights());
        
        // Fire event
        onWheelShutdown.Invoke();
        
        // Spawn animated clowns on ground
        StartCoroutine(SpawnAnimatedClowns());
    }
    
    System.Collections.IEnumerator FadeOutLights()
    {
        if (ferrisWheelLights.Length == 0) yield break;
        
        // Get starting intensities
        float[] startIntensities = new float[ferrisWheelLights.Length];
        for (int i = 0; i < ferrisWheelLights.Length; i++)
        {
            if (ferrisWheelLights[i] != null)
            {
                startIntensities[i] = ferrisWheelLights[i].intensity;
            }
        }
        
        // Fade out over 2 seconds
        float elapsed = 0f;
        float fadeDuration = 2f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            
            for (int i = 0; i < ferrisWheelLights.Length; i++)
            {
                if (ferrisWheelLights[i] != null)
                {
                    ferrisWheelLights[i].intensity = Mathf.Lerp(startIntensities[i], 0f, t);
                }
            }
            
            yield return null;
        }
        
        // Turn off all lights
        foreach (Light light in ferrisWheelLights)
        {
            if (light != null)
            {
                light.enabled = false;
            }
        }
        
        // Play lights off sound
        if (lightsOffSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(lightsOffSound);
        }
    }
    
    System.Collections.IEnumerator SpawnAnimatedClowns()
    {
        yield return new WaitForSeconds(spawnDelay);
        
        // Spawn clowns on ground
        for (int i = 0; i < Mathf.Min(clownsToSpawn, groundSpawnPoints.Length); i++)
        {
            if (animatedClownPrefab != null && groundSpawnPoints[i] != null)
            {
                GameObject clown = Instantiate(animatedClownPrefab, 
                                              groundSpawnPoints[i].position, 
                                              groundSpawnPoints[i].rotation);
                
                // Track this clown
                activeClowns++;
                
                // Add death detection
                ClownDeathDetector deathDetector = clown.GetComponent<ClownDeathDetector>();
                if (deathDetector == null)
                {
                    deathDetector = clown.AddComponent<ClownDeathDetector>();
                }
                deathDetector.zone2Controller = this; // ✅ Zone 2 reference
                
                // Play spawn sound
                if (clownSpawnSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(clownSpawnSound);
                }
                
                Debug.Log($"Zone2: Spawned animated clown {i + 1} at {groundSpawnPoints[i].name}");
                
                // Small delay between spawns
                if (i < groundSpawnPoints.Length - 1)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }
    }
    
    // Called by ClownDeathDetector when a clown dies
    public void OnClownDefeated()
    {
        activeClowns--;
        Debug.Log($"Zone2: Clown defeated! Remaining: {activeClowns}");
        
        if (activeClowns <= 0)
        {
            Debug.Log("Zone2: All clowns defeated! Zone complete!");
            onAllClownsDefeated.Invoke();
        }
    }
}
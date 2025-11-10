using UnityEngine;
using UnityEngine.Events;

public class Zone1CarouselController : MonoBehaviour
{
    [Header("References")]
    public CarouselController carouselController;
    public Light carouselLight;
    public CarouselHorse[] horses; // All horses that bob
    
    [Header("Enemy Spawning")]
    public GameObject animatedClownPrefab;
    public Transform[] spawnPoints;
    public float spawnDelay = 1f;
    
    [Header("Audio")]
    public AudioClip carouselStopSound;
    public AudioClip lightOffSound;
    public AudioClip clownSpawnSound;
    
    [Header("Unity Events")]
    public UnityEvent onCarouselShutdown; // When carousel stops
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
    
    // Called by LeverController via Unity Event
    public void ShutdownCarousel()
    {
        Debug.Log("Zone1: Shutting down carousel...");
        
        // Stop carousel rotation
        if (carouselController != null)
        {
            carouselController.StopCarousel();
            
            if (carouselStopSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(carouselStopSound);
            }
        }
        
        // Turn off light with fade
        if (carouselLight != null)
        {
            StartCoroutine(FadeOutLight());
        }
        
        // Stop horse bobbing
        foreach (CarouselHorse horse in horses)
        {
            if (horse != null)
            {
                horse.enabled = false; // Disable bobbing script
            }
        }
        
        // Fire event
        onCarouselShutdown.Invoke();
        
        // Spawn animated clowns
        StartCoroutine(SpawnAnimatedClowns());
    }
    
    System.Collections.IEnumerator FadeOutLight()
    {
        if (carouselLight == null) yield break;
        
        float startIntensity = carouselLight.intensity;
        float elapsed = 0f;
        float fadeDuration = 2f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / fadeDuration;
            carouselLight.intensity = Mathf.Lerp(startIntensity, 0f, t);
            yield return null;
        }
        
        carouselLight.enabled = false;
        
        // Play light off sound
        if (lightOffSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(lightOffSound);
        }
    }
    
    System.Collections.IEnumerator SpawnAnimatedClowns()
    {
        yield return new WaitForSeconds(spawnDelay);
        
        // Spawn 2 animated clowns
        for (int i = 0; i < Mathf.Min(2, spawnPoints.Length); i++)
        {
            if (animatedClownPrefab != null && spawnPoints[i] != null)
            {
                GameObject clown = Instantiate(animatedClownPrefab, 
                                              spawnPoints[i].position, 
                                              spawnPoints[i].rotation);
                
                // Track this clown
                activeClowns++;
                
                // Add death detection
                ClownDeathDetector deathDetector = clown.GetComponent<ClownDeathDetector>();
                if (deathDetector == null)
                {
                    deathDetector = clown.AddComponent<ClownDeathDetector>();
                }
                deathDetector.zoneController = this;
                
                // Play spawn sound
                if (clownSpawnSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(clownSpawnSound);
                }
                
                Debug.Log($"Zone1: Spawned animated clown {i + 1} at {spawnPoints[i].name}");
                
                // Small delay between spawns
                if (i < spawnPoints.Length - 1)
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
        Debug.Log($"Zone1: Clown defeated! Remaining: {activeClowns}");
        
        if (activeClowns <= 0)
        {
            // All clowns defeated!
            Debug.Log("Zone1: All clowns defeated! Zone complete!");
            onAllClownsDefeated.Invoke();
        }
    }
}
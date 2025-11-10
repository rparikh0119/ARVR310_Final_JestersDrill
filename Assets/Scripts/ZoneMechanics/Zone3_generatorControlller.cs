using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Zone3_GeneratorController : MonoBehaviour
{
    [Header("Ride Controllers")]
    public BumperCarController bumperCarController;
    public TeaCupController teaCupController;
    
    [Header("Generator Visual Effects")]
    public GameObject generator;
    public ParticleSystem sparkEffect;
    public Light generatorLight;
    public float rumbleIntensity = 0.1f;
    public float rumbleDuration = 0.5f;
    
    [Header("Audio")]
    public AudioSource generatorAudioSource;
    public AudioClip generatorHumLoop;
    public AudioClip rumbleSound;
    public AudioClip sparkSound;
    public AudioClip explosionSound;
    public AudioClip ridesStopSound;
    
    [Header("Enemy Spawning")]
    public GameObject animatedClownPrefab;
    public Transform[] spawnPoints;
    public float spawnDelay = 2f;
    public AudioClip clownSpawnSound;
    
    [Header("Unity Events")]
    public UnityEvent onGeneratorDestroyed;
    
    private bool isDestroyed = false;
    private Vector3 generatorOriginalPos;
    
    void Start()
    {
        if (generator != null)
        {
            generatorOriginalPos = generator.transform.localPosition;
        }
        
        if (generatorAudioSource != null && generatorHumLoop != null)
        {
            generatorAudioSource.clip = generatorHumLoop;
            generatorAudioSource.loop = true;
            generatorAudioSource.Play();
        }
        
        if (sparkEffect != null)
        {
            sparkEffect.Stop();
        }
    }
    
    public void OnLeverPush(int pushCount)
    {
        if (isDestroyed) return;
        
        Debug.Log($"Zone3: Lever pushed {pushCount} times");
        
        StartCoroutine(RumbleGenerator());
        
        if (sparkEffect != null)
        {
            sparkEffect.Play();
        }
        
        if (generatorAudioSource != null)
        {
            if (rumbleSound != null)
                generatorAudioSource.PlayOneShot(rumbleSound);
            if (sparkSound != null)
                generatorAudioSource.PlayOneShot(sparkSound, 0.5f);
        }
        
        if (pushCount >= 3)
        {
            DestroyGenerator();
        }
    }
    
    IEnumerator RumbleGenerator()
    {
        float elapsed = 0f;
        
        while (elapsed < rumbleDuration)
        {
            if (generator != null)
            {
                Vector3 randomOffset = Random.insideUnitSphere * rumbleIntensity;
                generator.transform.localPosition = generatorOriginalPos + randomOffset;
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (generator != null)
        {
            generator.transform.localPosition = generatorOriginalPos;
        }
    }
    
    void DestroyGenerator()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        
        Debug.Log("Zone3: Generator destroyed!");
        
        if (generatorAudioSource != null)
        {
            generatorAudioSource.Stop();
            if (explosionSound != null)
                generatorAudioSource.PlayOneShot(explosionSound);
        }
        
        if (sparkEffect != null)
        {
            var emission = sparkEffect.emission;
            emission.rateOverTime = 100;
            sparkEffect.Play();
        }
        
        if (generatorLight != null)
        {
            StartCoroutine(FadeLight());
        }
        
        StopRides();
        StartCoroutine(SpawnEnemiesAfterDelay());
        onGeneratorDestroyed?.Invoke();
    }
    
    void StopRides()
    {
        Debug.Log("Zone3: Stopping all rides");
        
        if (bumperCarController != null)
        {
            bumperCarController.StopRide();
        }
        
        if (teaCupController != null)
        {
            teaCupController.StopRide();
        }
        
        if (generatorAudioSource != null && ridesStopSound != null)
        {
            generatorAudioSource.PlayOneShot(ridesStopSound);
        }
    }
    
    IEnumerator FadeLight()
    {
        if (generatorLight == null) yield break;
        
        float startIntensity = generatorLight.intensity;
        float elapsed = 0f;
        float fadeDuration = 1f;
        
        while (elapsed < fadeDuration)
        {
            generatorLight.intensity = Mathf.Lerp(startIntensity, 0f, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        generatorLight.intensity = 0f;
        generatorLight.enabled = false;
    }
    
    IEnumerator SpawnEnemiesAfterDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        
        Debug.Log("Zone3: Spawning enemies");
        
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint != null && animatedClownPrefab != null)
            {
                GameObject clown = Instantiate(animatedClownPrefab, spawnPoint.position, spawnPoint.rotation);
                
                if (clownSpawnSound != null)
                {
                    AudioSource.PlayClipAtPoint(clownSpawnSound, spawnPoint.position);
                }
                
                Debug.Log($"Zone3: Spawned clown at {spawnPoint.name}");
            }
        }
    }
}
using UnityEngine;
using System.Collections;

public class ZoneBarrier : MonoBehaviour
{
    [Header("Zone Settings")]
    public int zoneNumber = 1; // Set this for each zone (1-5)
    
    [Header("Dissolve Settings")]
    public float dissolveSpeed = 1f;
    public float dissolveDelay = 0.5f;
    
    [Header("Spawn Settings")]
    public Transform[] enemySpawnPoints;
    public GameObject enemyPrefab;
    public float spawnInitialDelay = 10f;
    public float spawnInterval = 10f;
    public int enemiesToSpawn = 3;
    
    [Header("Audio")]
    public AudioClip barrierDownSound;
    public AudioClip blockedSound; // When player tries before unlocked
    
    private Material barrierMaterial;
    private bool isDissolving = false;
    private bool hasBeenEntered = false;
    private float dissolveProgress = 0f;

    void Start()
    {
        // Use coroutine to avoid blocking - material access can be expensive
        StartCoroutine(InitializeBarrierDelayed());
    }
    
    System.Collections.IEnumerator InitializeBarrierDelayed()
    {
        // Wait for scene to load
        yield return null;
        
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            barrierMaterial = renderer.material; // Instance
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if player entered
        if (hasBeenEntered) return; // Already processed
        
        if (other.CompareTag("Player") || other.CompareTag("MainCamera"))
        {
            // Check if this zone is unlocked
            if (ZoneBarrierManager.Instance != null && 
                ZoneBarrierManager.Instance.IsZoneUnlocked(zoneNumber))
            {
                // Zone is unlocked, allow entry
                hasBeenEntered = true;
                StartDissolve();
            }
            else
            {
                // Zone is locked, play blocked sound
                if (blockedSound != null)
                {
                    AudioSource.PlayClipAtPoint(blockedSound, transform.position);
                }
                Debug.Log($"Zone {zoneNumber} is locked! Listen to Briggs' briefing first.");
            }
        }
    }

    public void StartDissolve()
    {
        if (!isDissolving)
        {
            StartCoroutine(DissolveSequence());
        }
    }

    IEnumerator DissolveSequence()
    {
        isDissolving = true;
        
        yield return new WaitForSeconds(dissolveDelay);
        
        // Play sound
        if (barrierDownSound != null)
        {
            AudioSource.PlayClipAtPoint(barrierDownSound, transform.position);
        }
        
        // Dissolve effect (fade out shader opacity)
        while (dissolveProgress < 1f)
        {
            dissolveProgress += Time.deltaTime * dissolveSpeed;
            
            // Adjust shader properties (property names depend on your shader)
            if (barrierMaterial != null)
            {
                // Try common property names
                if (barrierMaterial.HasProperty("_Opacity"))
                    barrierMaterial.SetFloat("_Opacity", 1f - dissolveProgress);
                
                if (barrierMaterial.HasProperty("_Alpha"))
                    barrierMaterial.SetFloat("_Alpha", 1f - dissolveProgress);
                
                // Or adjust color alpha
                if (barrierMaterial.HasProperty("_Color"))
                {
                    Color col = barrierMaterial.GetColor("_Color");
                    col.a = 1f - dissolveProgress;
                    barrierMaterial.SetColor("_Color", col);
                }
            }
            
            yield return null;
        }
        
        // Start spawning enemies
        StartCoroutine(SpawnEnemiesSequence());
        
        // Disable barrier
        gameObject.SetActive(false);
    }

    IEnumerator SpawnEnemiesSequence()
    {
        yield return new WaitForSeconds(spawnInitialDelay);
        
        int spawned = 0;
        foreach (Transform spawnPoint in enemySpawnPoints)
        {
            if (spawned >= enemiesToSpawn) break;
            
            if (enemyPrefab != null && spawnPoint != null)
            {
                Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
                Debug.Log($"Zone {zoneNumber}: Spawned enemy {spawned + 1} of {enemiesToSpawn}");
                spawned++;
                
                if (spawned < enemiesToSpawn)
                {
                    yield return new WaitForSeconds(spawnInterval);
                }
            }
        }
    }
}
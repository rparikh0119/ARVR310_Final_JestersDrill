using UnityEngine;
using System.Collections;

public class ConfettiRocketProjectile : MonoBehaviour
{
    [Header("Rocket Settings")]
    public float explosionDelay = 2f;
    public float launchForce = 15f;
    public LayerMask hitLayers; // SET TO "Enemy" AND "Default"
    
    [Header("Visual Effects")]
    public ParticleSystem rocketTrail;
    public GameObject explosionEffect;
    
    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip explosionSound;
    public AudioClip rocketFlySound;
    private AudioSource audioSource;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private bool hasHit = false;
    private GameObject attachedEnemy;
    private Rigidbody rb;
    private Collider col;
    
    void Start()
    {
        // Get and verify components
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        
        // Setup audio
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.loop = true;
        
        if (rocketFlySound != null)
        {
            audioSource.clip = rocketFlySound;
            audioSource.Play();
        }
        
        // Debug startup info
        if (showDebugLogs)
        {
            Debug.Log($"🚀 CONFETTI ROCKET SPAWNED:");
            Debug.Log($"  - Position: {transform.position}");
            Debug.Log($"  - Velocity: {(rb != null ? rb.linearVelocity.magnitude.ToString() : "No Rigidbody!")}");
            Debug.Log($"  - Collider Enabled: {(col != null ? col.enabled.ToString() : "No Collider!")}");
            Debug.Log($"  - Is Trigger: {(col != null ? col.isTrigger.ToString() : "N/A")}");
            Debug.Log($"  - Hit Layers Mask: {hitLayers.value}");
            Debug.Log($"  - Collision Detection: {(rb != null ? rb.collisionDetectionMode.ToString() : "N/A")}");
            
            // Check which layers are included
            if ((hitLayers.value & (1 << LayerMask.NameToLayer("Enemy"))) != 0)
                Debug.Log($"  - ✅ Enemy layer included");
            else
                Debug.Log($"  - ❌ Enemy layer NOT included!");
            
            if ((hitLayers.value & (1 << LayerMask.NameToLayer("Default"))) != 0)
                Debug.Log($"  - ✅ Default layer included");
            else
                Debug.Log($"  - ⚠️ Default layer NOT included");
        }
        
        // Auto-destroy after lifetime
        Destroy(gameObject, 5f);
    }
    
    // TRIGGER-BASED COLLISION (for trigger colliders)
    void OnTriggerEnter(Collider other)
    {
        if (showDebugLogs)
        {
            Debug.Log($"═══ TRIGGER ENTER DETECTED ═══");
            Debug.Log($"  Hit Object: {other.gameObject.name}");
            Debug.Log($"  Hit Layer: {LayerMask.LayerToName(other.gameObject.layer)} (#{other.gameObject.layer})");
            Debug.Log($"  Is Trigger: {other.isTrigger}");
            Debug.Log($"  Has Rigidbody: {other.attachedRigidbody != null}");
        }
        
        ProcessHit(other);
    }
    
    // SOLID COLLISION (backup method for non-trigger colliders)
    void OnCollisionEnter(Collision collision)
    {
        if (showDebugLogs)
        {
            Debug.Log($"═══ COLLISION ENTER DETECTED ═══");
            Debug.Log($"  Hit Object: {collision.gameObject.name}");
            Debug.Log($"  Hit Layer: {LayerMask.LayerToName(collision.gameObject.layer)} (#{collision.gameObject.layer})");
            Debug.Log($"  Impact Force: {collision.impulse.magnitude}");
            Debug.Log($"  Contact Points: {collision.contactCount}");
        }
        
        ProcessHit(collision.collider);
    }
    
    void ProcessHit(Collider other)
    {
        if (hasHit) 
        {
            if (showDebugLogs)
                Debug.Log("  ⏭️ Already processed a hit - ignoring");
            return;
        }
        
        // Check if the hit object's layer is in our hit layers mask
        bool layerMatch = ((1 << other.gameObject.layer) & hitLayers) != 0;
        
        if (showDebugLogs)
        {
            Debug.Log($"  Layer Match Check: {layerMatch}");
            Debug.Log($"  Calculation: (1 << {other.gameObject.layer}) & {hitLayers.value} = {(1 << other.gameObject.layer) & hitLayers.value}");
        }
        
        if (layerMatch)
        {
            if (showDebugLogs)
                Debug.Log("  ✅ LAYER MATCH - PROCESSING HIT");
            
            hasHit = true;
            
            // Stop rocket sound
            if (audioSource != null)
            {
                audioSource.Stop();
            }
            
            // Play hit sound
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }
            
            // Check if we hit an enemy or something else
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (showDebugLogs)
                    Debug.Log("  🎯 ENEMY LAYER DETECTED - LAUNCHING!");
                
                AttachToEnemy(other.gameObject);
            }
            else
            {
                if (showDebugLogs)
                    Debug.Log("  💥 NON-ENEMY HIT - EXPLODING IMMEDIATELY!");
                
                Explode(null);
            }
        }
        else
        {
            if (showDebugLogs)
                Debug.Log($"  ❌ LAYER MISMATCH - Object is on '{LayerMask.LayerToName(other.gameObject.layer)}' layer");
        }
    }
    
    void AttachToEnemy(GameObject enemy)
    {
        attachedEnemy = enemy;
        
        if (showDebugLogs)
            Debug.Log($"🎯 ATTACHING TO ENEMY: {enemy.name}");
        
        // Stop rocket physics
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            
            if (showDebugLogs)
                Debug.Log("  - Rocket physics stopped");
        }
        
        // Attach rocket to enemy
        transform.SetParent(enemy.transform);
        
        if (showDebugLogs)
            Debug.Log("  - Rocket parented to enemy");
        
        // LAUNCH THE ENEMY
        Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
        if (enemyRb != null)
        {
            // Make enemy physical
            enemyRb.isKinematic = false;
            enemyRb.useGravity = true;
            
            // Calculate launch direction (up and forward)
            Vector3 launchDirection = (Vector3.up * 1.5f + transform.forward).normalized;
            
            // Apply launch force
            enemyRb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
            enemyRb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
            
            if (showDebugLogs)
            {
                Debug.Log($"  🚀 ENEMY LAUNCHED!");
                Debug.Log($"     Direction: {launchDirection}");
                Debug.Log($"     Force: {launchForce}");
                Debug.Log($"     Velocity: {enemyRb.linearVelocity.magnitude}");
            }
        }
        else
        {
            if (showDebugLogs)
                Debug.LogWarning($"  ⚠️ Enemy '{enemy.name}' has no Rigidbody - cannot launch!");
        }
        
        // Start explosion countdown
        StartCoroutine(ExplodeAfterDelay());
    }
    
    IEnumerator ExplodeAfterDelay()
    {
        if (showDebugLogs)
            Debug.Log($"  ⏱️ Starting {explosionDelay}s countdown to explosion...");
        
        yield return new WaitForSeconds(explosionDelay);
        
        if (showDebugLogs)
            Debug.Log("  💥 EXPLODING NOW!");
        
        Explode(attachedEnemy);
    }
    
    void Explode(GameObject enemy)
    {
        if (showDebugLogs)
            Debug.Log($"💥 CONFETTI EXPLOSION at {transform.position}");
        
        // Play explosion sound
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }
        
        // Spawn explosion effect
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
            
            if (showDebugLogs)
                Debug.Log("  - Explosion VFX spawned");
        }
        
        // Destroy enemy
        if (enemy != null)
        {
            if (showDebugLogs)
                Debug.Log($"  - Destroying enemy: {enemy.name}");
            
            Destroy(enemy);
        }
        
        // Destroy rocket
        Destroy(gameObject);
    }
}

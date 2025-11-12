using UnityEngine;
using System.Collections;

/// <summary>
/// SIMPLIFIED Prize dart - freezes animation and destroys enemy
/// NO grey effect to avoid Unity freeze
/// </summary>
public class PrizeDart : MonoBehaviour
{
    [Header("Dart Settings")]
    [Tooltip("Speed of the dart")]
    public float speed = 25f;
    
    [Tooltip("How long dart exists before auto-destroying")]
    public float lifetime = 5f;
    
    [Tooltip("How long enemy stays pinned before disappearing")]
    public float pinDuration = 3f;
    
    [Tooltip("Only objects on these layers can be hit")]
    public LayerMask hitLayers; // SET TO "Enemy" in Inspector
    
    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip pinSound;
    
    private Rigidbody rb;
    private bool hasHit = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = transform.forward * speed;
            rb.useGravity = false;
        }
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
        
        Debug.Log($"PrizeDart: Spawned, flying at {speed}m/s");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return; // Only hit once
        
        // Check if object is on Enemy layer
        if (((1 << other.gameObject.layer) & hitLayers) == 0)
        {
            Debug.Log($"PrizeDart: Hit {other.gameObject.name} but not on Enemy layer, ignoring");
            return; // Not on Enemy layer, ignore it
        }
        
        hasHit = true;
        
        Debug.Log($"PrizeDart: HIT ENEMY: {other.gameObject.name}!");
        
        // Play hit sound
        if (hitSound != null)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }
        
        // Pin the enemy
        PinEnemy(other.gameObject);
    }
    
    /// <summary>
    /// Pins the enemy, freezes animation, destroys after delay
    /// </summary>
    void PinEnemy(GameObject enemy)
    {
        Debug.Log($"PrizeDart: Pinning {enemy.name}...");
        
        // Play pin sound
        if (pinSound != null)
        {
            AudioSource.PlayClipAtPoint(pinSound, transform.position);
        }
        
        // Stick dart to enemy
        transform.SetParent(enemy.transform);
        
        // Stop dart physics
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }
        
        // Freeze enemy movement
        Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
        if (enemyRb != null)
        {
            enemyRb.isKinematic = true;
            enemyRb.linearVelocity = Vector3.zero;
            enemyRb.angularVelocity = Vector3.zero;
            Debug.Log($"PrizeDart: Froze {enemy.name} physics");
        }
        
        // Stop enemy animation
        Animator enemyAnimator = enemy.GetComponent<Animator>();
        if (enemyAnimator != null)
        {
            enemyAnimator.enabled = false; // Freeze animation
            Debug.Log($"PrizeDart: Froze {enemy.name} animation");
        }
        
        // SKIP GREY EFFECT FOR NOW (was causing freeze)
        Debug.Log($"PrizeDart: Skipping grey effect to prevent freeze");
        
        // Destroy enemy after delay
        StartCoroutine(DestroyEnemyAfterDelay(enemy));
        
        Debug.Log($"PrizeDart: {enemy.name} will be destroyed in {pinDuration} seconds");
    }
    
    /// <summary>
    /// Waits for pin duration, then destroys enemy and dart
    /// </summary>
    IEnumerator DestroyEnemyAfterDelay(GameObject enemy)
    {
        Debug.Log($"PrizeDart: Starting {pinDuration}s countdown for {enemy.name}");
        
        yield return new WaitForSeconds(pinDuration);
        
        if (enemy != null)
        {
            Debug.Log($"PrizeDart: Time's up! Destroying {enemy.name}");
            Destroy(enemy);
        }
        
        // Destroy the dart too
        Destroy(gameObject);
        
        Debug.Log($"PrizeDart: Cleanup complete");
    }
}
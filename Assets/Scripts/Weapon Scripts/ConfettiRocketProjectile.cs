using UnityEngine;

public class ConfettiRocketProjectile : MonoBehaviour
{
    [Header("Rocket Settings")]
    public float explosionDelay = 2f; // Time before explosion
    public float launchForce = 15f; // How hard enemy is launched
    public LayerMask hitLayers;
    
    [Header("Visual Effects")]
    public ParticleSystem rocketTrail; // Trail while flying
    public GameObject explosionEffect; // Big confetti explosion
    
    [Header("Audio")]
    public AudioClip hitSound; // When rocket hits
    public AudioClip explosionSound; // When rocket explodes
    public AudioClip rocketFlySound; // Looping while flying
    private AudioSource audioSource;
    
    private bool hasHit = false;
    private GameObject attachedEnemy;
    
    void Start()
    {
        // Add audio source for flying sound
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.loop = true;
        
        if (rocketFlySound != null)
        {
            audioSource.clip = rocketFlySound;
            audioSource.Play();
        }
        
        // Destroy rocket after 5 seconds if it doesn't hit anything
        Destroy(gameObject, 5f);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        
        // Check if we hit something on the correct layers
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            hasHit = true;
            
            // Stop flying sound
            if (audioSource != null)
            {
                audioSource.Stop();
            }
            
            // Play hit sound
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }
            
            // Check if this is an enemy
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                AttachToEnemy(other.gameObject);
            }
            else
            {
                // Hit a wall or ground - explode immediately
                Explode(null);
            }
        }
    }
    
    void AttachToEnemy(GameObject enemy)
    {
        attachedEnemy = enemy;
        
        // Stop rocket physics
        Rigidbody rocketRb = GetComponent<Rigidbody>();
        if (rocketRb != null)
        {
            rocketRb.isKinematic = true;
            rocketRb.linearVelocity = Vector3.zero;
        }
        
        // Attach rocket to enemy
        transform.SetParent(enemy.transform);
        
        // LAUNCH THE ENEMY
        Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
        if (enemyRb != null)
        {
            enemyRb.isKinematic = false; // Make sure physics works
            enemyRb.useGravity = true;
            
            // Launch upward and in the direction rocket was traveling
            Vector3 launchDirection = (Vector3.up * 1.5f + transform.forward).normalized;
            enemyRb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
            
            // Add random spin
            enemyRb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }
        
        Debug.Log($"Confetti Rocket: Hit and launched {enemy.name}!");
        
        // Start explosion countdown
        StartCoroutine(ExplodeAfterDelay());
    }
    
    System.Collections.IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode(attachedEnemy);
    }
    
    void Explode(GameObject enemy)
    {
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
        }
        
        // Destroy enemy
        if (enemy != null)
        {
            Destroy(enemy);
        }
        
        // Destroy rocket
        Destroy(gameObject);
    }
}

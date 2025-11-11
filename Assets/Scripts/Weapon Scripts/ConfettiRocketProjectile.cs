using UnityEngine;

public class ConfettiRocketProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 25f;
    public float lifetime = 5f;
    
    [Header("Rocket Settings")]
    public float explosionDelay = 0.5f; // Time before explosion after hit
    public float launchForce = 15f; // How high enemy flies
    public LayerMask enemyLayer; // Set to "Enemy" layer
    
    [Header("Visual Effects")]
    public ParticleSystem rocketTrail;
    public GameObject explosionEffect;
    
    private Vector3 velocity;
    private bool hasHit = false;
    private GameObject attachedEnemy;
    
    void Start()
    {
        // Calculate velocity from forward direction
        velocity = transform.forward * speed;
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
        
        Debug.Log($"Rocket spawned! Moving forward at speed: {speed}");
    }
    
    void Update()
    {
        if (!hasHit)
        {
            // Move forward
            transform.position += velocity * Time.deltaTime;
            
            // Rotate for spin effect
            transform.Rotate(Vector3.forward, 360f * Time.deltaTime);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        
        // Check if hit Enemy layer
        if (((1 << other.gameObject.layer) & enemyLayer) == 0)
        {
            // Not an enemy - ignore
            return;
        }
        
        hasHit = true;
        
        Debug.Log($"Rocket hit enemy: {other.gameObject.name}");
        
        // Attach to enemy and launch them
        AttachToEnemy(other.gameObject);
    }
    
    void AttachToEnemy(GameObject enemy)
    {
        attachedEnemy = enemy;
        
        // Stop rocket movement
        velocity = Vector3.zero;
        
        // Attach rocket to enemy
        transform.SetParent(enemy.transform);
        
        // Launch enemy upward with spin
        Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
        if (enemyRb != null)
        {
            enemyRb.isKinematic = false;
            enemyRb.useGravity = true;
            
            // Launch UP and slightly forward
            Vector3 launchDirection = (Vector3.up * 1.5f + transform.forward).normalized;
            enemyRb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
            
            // Add spin
            enemyRb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
        }
        
        // Explode after delay
        StartCoroutine(ExplodeAfterDelay());
    }
    
    System.Collections.IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }
    
    void Explode()
    {
        Debug.Log("💥 CONFETTI EXPLOSION!");
        
        // Spawn explosion effect
        if (explosionEffect != null)
        {
            GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
        }
        
        // Kill the enemy
        if (attachedEnemy != null)
        {
            Destroy(attachedEnemy);
        }
        
        // Destroy rocket
        Destroy(gameObject);
    }
}
using UnityEngine;

public class PrizeDart : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 30f;
    public float lifetime = 5f;
    
    [Header("Dart Settings")]
    public LayerMask enemyLayer; // Set to "Enemy" layer
    
    private Vector3 velocity;
    private bool hasHit = false;
    
    void Start()
    {
        // Calculate velocity from forward direction
        velocity = transform.forward * speed;
        
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);
        
        Debug.Log($"Dart spawned! Moving forward at speed: {speed}");
    }
    
    void Update()
    {
        if (!hasHit)
        {
            // Move forward
            transform.position += velocity * Time.deltaTime;
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
        
        Debug.Log($"Dart hit enemy: {other.gameObject.name}");
        
        // Kill the enemy
        Destroy(other.gameObject);
        
        // Destroy dart
        Destroy(gameObject);
    }
}
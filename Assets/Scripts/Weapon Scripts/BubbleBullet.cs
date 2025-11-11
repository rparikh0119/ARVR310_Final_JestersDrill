using UnityEngine;

public class BubbleBullet : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 20f;
    public float lifetime = 5f;
    
    [Header("Bubble Creation")]
    public GameObject bubblePrefab;
    public LayerMask bubbleableLayer; // ONLY THIS LAYER GETS BUBBLED
    
    private Vector3 velocity;
    
    void Start()
    {
        velocity = transform.forward * speed;
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        transform.position += velocity * Time.deltaTime;
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if object is on Bubbleable layer
        if (((1 << other.gameObject.layer) & bubbleableLayer) == 0)
        {
            // Not on bubbleable layer - ignore it
            return;
        }
        
        // It's bubbleable! Create bubble
        if (bubblePrefab != null)
        {
            GameObject bubble = Instantiate(bubblePrefab, other.transform.position, Quaternion.identity);
            
            Bubble bubbleScript = bubble.GetComponent<Bubble>();
            if (bubbleScript != null)
            {
                bubbleScript.TrapObject(other.gameObject);
            }
        }
        
        // Destroy bullet
        Destroy(gameObject);
    }
}
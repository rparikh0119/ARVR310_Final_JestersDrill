using UnityEngine;

public class BubbleBullet : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 15f;
    public float lifetime = 5f;
    public AnimationCurve speedCurve = AnimationCurve.Linear(0, 1, 1, 0.8f); // Slows down over time
    
    [Header("Wobble Animation")]
    public float wobbleSpeed = 5f;
    public float wobbleAmount = 0.05f;
    
    [Header("Bubble Creation")]
    public GameObject bubblePrefab;
    public LayerMask bubbleableLayers;
    
    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip hitSound;
    private AudioSource audioSource;
    
    [Header("Visual Effects")]
    public ParticleSystem trailEffect;
    public GameObject hitEffect;
    
    private float timer;
    private Vector3 originalScale;
    
    void Start()
    {
        timer = 0f;
        originalScale = transform.localScale;
        audioSource = GetComponent<AudioSource>();
        
        // Play shoot sound
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        // Check lifetime
        if (timer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }
        
        // Move forward with speed curve (slows down slightly over time for more dynamic look)
        float currentSpeed = speed * speedCurve.Evaluate(timer / lifetime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;
        
        // Add wobble animation for dynamic look
        float wobbleX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
        float wobbleY = Mathf.Cos(Time.time * wobbleSpeed * 1.3f) * wobbleAmount;
        transform.localScale = originalScale + new Vector3(wobbleX, wobbleY, wobbleX);
        
        // Slight rotation for spin effect
        transform.Rotate(Vector3.forward, 200f * Time.deltaTime);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if we hit something that can be bubbled
        if (((1 << other.gameObject.layer) & bubbleableLayers) != 0)
        {
            // Play hit sound
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }
            
            // Spawn hit effect
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }
            
            // Create bubble around the object
            CreateBubble(other.gameObject);
            
            // Destroy this bullet
            Destroy(gameObject);
        }
    }
    
    void CreateBubble(GameObject target)
    {
        // Spawn bubble at target's position
        GameObject bubble = Instantiate(bubblePrefab, target.transform.position, Quaternion.identity);
        
        // Scale bubble based on target size
        Bounds targetBounds = GetObjectBounds(target);
        float bubbleSize = Mathf.Max(targetBounds.size.x, targetBounds.size.y, targetBounds.size.z) * 1.5f;
        bubble.transform.localScale = Vector3.one * bubbleSize;
        
        // Get the Bubble script and tell it what to bubble
        Bubble bubbleScript = bubble.GetComponent<Bubble>();
        if (bubbleScript != null)
        {
            bubbleScript.BubbleObject(target);
        }
    }
    
    Bounds GetObjectBounds(GameObject obj)
    {
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        if (renderer != null)
            return renderer.bounds;
        
        Collider collider = obj.GetComponent<Collider>();
        if (collider != null)
            return collider.bounds;
        
        return new Bounds(obj.transform.position, Vector3.one);
    }
}
using UnityEngine;
using System.Collections;

public class Bubble : MonoBehaviour
{
    [Header("Bubble Physics")]
    public float bubbleDuration = 5f;
    public float floatSpeed = 2f;
    public float driftSpeed = 0.5f;
    public AnimationCurve floatCurve = AnimationCurve.EaseInOut(0, 1, 1, 0.5f);
    
    [Header("Bubble Animation")]
    public float wobbleSpeed = 2f;
    public float wobbleAmount = 0.1f;
    public float spinSpeed = 30f;
    public float expandDuration = 0.5f; // How long the bubble takes to expand
    
    [Header("Pop Settings")]
    public LayerMask whatPopsMe;
    public GameObject popEffect;
    
    [Header("Audio")]
    public AudioClip bubbleCreateSound;
    public AudioClip bubbleFloatSound;
    public AudioClip bubblePopSound;
    private AudioSource audioSource;
    
    private GameObject bubbledObject;
    private Rigidbody originalRb;
    private Collider originalCollider;
    
    private Vector3 baseScale;
    private Vector3 targetScale;
    private float timer;
    private Vector3 driftDirection;
    private bool isExpanding = true;
    
    void Start()
    {
        // Store the target scale (what we'll grow TO)
        targetScale = transform.localScale;
        
        // Start at zero scale (invisible)
        transform.localScale = Vector3.zero;
        baseScale = targetScale;
        
        timer = 0f;
        
        // Random drift direction
        driftDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        
        audioSource.spatialBlend = 1f;
    }
    
    void Update()
    {
        // Don't do normal update behavior while expanding
        if (!isExpanding)
        {
            timer += Time.deltaTime;
            
            // Float upward with curve
            float currentFloatSpeed = floatSpeed * floatCurve.Evaluate(timer / bubbleDuration);
            transform.position += Vector3.up * currentFloatSpeed * Time.deltaTime;
            
            // Add sideways drift
            transform.position += driftDirection * driftSpeed * Time.deltaTime;
            
            // Wobble animation
            float wobbleX = Mathf.Sin(Time.time * wobbleSpeed) * wobbleAmount;
            float wobbleY = Mathf.Cos(Time.time * wobbleSpeed * 1.2f) * wobbleAmount;
            Vector3 wobbleScale = new Vector3(wobbleX, -wobbleX * 0.5f, wobbleY);
            transform.localScale = baseScale + wobbleScale;
            
            // Gentle rotation
            transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        }
    }
    
    public void BubbleObject(GameObject target)
    {
        bubbledObject = target;
        
        // Store references
        originalRb = target.GetComponent<Rigidbody>();
        originalCollider = target.GetComponent<Collider>();
        
        // Disable physics
        if (originalRb != null)
        {
            originalRb.isKinematic = true;
            originalRb.useGravity = false;
            originalRb.linearVelocity = Vector3.zero;
        }
        
        // Disable collider
        if (originalCollider != null)
        {
            originalCollider.enabled = false;
        }
        
        // Make object a child of bubble
        target.transform.SetParent(transform);
        target.transform.localPosition = Vector3.zero;
        
        // START THE EXPANDING ANIMATION
        StartCoroutine(ExpandBubble());
        
        // Start the pop timer
        StartCoroutine(PopAfterDelay());
    }
    
    IEnumerator ExpandBubble()
    {
        isExpanding = true;
        
        // Play creation sound at start of expansion
        if (bubbleCreateSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(bubbleCreateSound);
        }
        
        float elapsed = 0f;
        
        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / expandDuration;
            
            // Ease-out cubic curve for satisfying bubble growth
            float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
            
            // Scale from zero to target size
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, easedProgress);
            
            yield return null;
        }
        
        // Ensure we reach exactly the target scale
        transform.localScale = targetScale;
        baseScale = targetScale;
        isExpanding = false;
        
        // Start floating sound loop AFTER expansion completes
        if (bubbleFloatSound != null && audioSource != null)
        {
            audioSource.clip = bubbleFloatSound;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
    
    IEnumerator PopAfterDelay()
    {
        yield return new WaitForSeconds(bubbleDuration);
        PopBubble();
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if we hit something that pops bubbles
        if (((1 << other.gameObject.layer) & whatPopsMe) != 0)
        {
            PopBubble();
        }
    }
    
    void PopBubble()
    {
        // Play pop sound
        if (bubblePopSound != null)
        {
            AudioSource.PlayClipAtPoint(bubblePopSound, transform.position);
        }
        
        // Spawn pop effect
        if (popEffect != null)
        {
            Instantiate(popEffect, transform.position, Quaternion.identity);
        }
        
        // Destroy the animatronic (eliminating the target)
        if (bubbledObject != null)
        {
            Destroy(bubbledObject);
        }
        
        // Destroy the bubble
        Destroy(gameObject);
    }
}
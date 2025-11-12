using UnityEngine;
using System.Collections;

/// <summary>
/// Bubble that wraps around objects, floats them upward, and pops after a duration
/// Follows Professor Ahmadi's methodology: Inspector-based configuration, Unity Events
/// </summary>
public class Bubble : MonoBehaviour
{
    [Header("Bubble Physics")]
    [Tooltip("How long until the bubble pops and destroys the object")]
    public float bubbleDuration = 5f;
    
    [Tooltip("(Legacy/Unused) Kept for compatibility")]
    public float floatForce = 15f;
    
    [Tooltip("Constant upward speed for floating (m/s) - Higher = faster float")]
    public float floatSpeed = 3f;
    
    [Tooltip("Speed of bobbing animation while floating")]
    public float bobSpeed = 2f;
    
    [Tooltip("Amount of vertical bob displacement")]
    public float bobAmount = 0.3f;
    
    [Header("Visual Animation")]
    [Tooltip("Duration of the pop shrink animation")]
    public float popDuration = 0.5f;
    
    [Header("Audio")]
    [Tooltip("Sound when bubble is created")]
    public AudioClip bubbleCreateSound;
    
    [Tooltip("Looping sound while bubble floats")]
    public AudioClip bubbleFloatSound;
    
    [Tooltip("Sound when bubble pops")]
    public AudioClip bubblePopSound;
    
    private AudioSource audioSource;
    
    [Header("Pop Effects")]
    [Tooltip("Particle effect when bubble pops")]
    public GameObject popEffect;
    
    // Private variables
    private GameObject bubbledObject;
    private Rigidbody bubbledRb;
    private Vector3 startPos;
    private bool isPopping = false;
    
    void Start()
    {
        startPos = transform.position;
        
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 1f; // 3D sound
        
        // Play creation sound
        if (bubbleCreateSound != null)
        {
            audioSource.PlayOneShot(bubbleCreateSound);
        }
        
        // Play looping float sound
        if (bubbleFloatSound != null)
        {
            audioSource.clip = bubbleFloatSound;
            audioSource.loop = true;
            audioSource.Play();
        }
        
        // Start the pop timer
        StartCoroutine(PopAfterDelay());
    }
    
    void Update()
    {
        if (isPopping) return;
        
        // Move the BUBBLE upward (clown is parented to it, so moves with it)
        // Use simple transform movement instead of physics
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        
        // Add bobbing animation (gentle side-to-side and up-down wobble)
        float bobX = Mathf.Sin(Time.time * bobSpeed) * bobAmount * 0.3f;
        float bobY = Mathf.Sin(Time.time * bobSpeed * 1.3f) * bobAmount * 0.5f;
        float bobZ = Mathf.Cos(Time.time * bobSpeed * 0.8f) * bobAmount * 0.3f;
        
        Vector3 wobble = new Vector3(bobX, bobY, bobZ);
        transform.position += wobble * Time.deltaTime;
        
        // Gentle rotation of bubble for visual interest
        transform.Rotate(Vector3.up, 20f * Time.deltaTime);
    }
    
    /// <summary>
    /// Captures a GameObject in the bubble and applies floating physics
    /// Called by BubbleBullet when it hits a target
    /// </summary>
    public void TrapObject(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("Bubble: Cannot bubble null target!");
            return;
        }
        
        bubbledObject = target;
        
        Debug.Log($"Bubble: Capturing {target.name}...");
        
        // Get or add Rigidbody
        bubbledRb = target.GetComponent<Rigidbody>();
        if (bubbledRb == null)
        {
            bubbledRb = target.AddComponent<Rigidbody>();
            Debug.Log($"Bubble: Added Rigidbody to {target.name}");
        }
        
        // DISABLE physics on target - bubble will control movement instead
        bubbledRb.isKinematic = true;   // Disable physics simulation
        bubbledRb.useGravity = false;   // Disable gravity completely
        bubbledRb.linearVelocity = Vector3.zero;  // Reset velocity
        bubbledRb.angularVelocity = Vector3.zero; // Reset rotation
        
        Debug.Log($"Bubble: Rigidbody configured - Kinematic: {bubbledRb.isKinematic}, Gravity: {bubbledRb.useGravity}, Mass: {bubbledRb.mass}");
        
        // PARENT TARGET TO BUBBLE (bubble is parent, target is child)
        // This way when bubble moves, target moves with it!
        target.transform.SetParent(transform);
        target.transform.localPosition = Vector3.zero; // Center inside bubble
        
        // Add slight random rotation for visual interest
        target.transform.localRotation = Quaternion.Euler(
            Random.Range(-15f, 15f),
            Random.Range(0f, 360f),
            Random.Range(-15f, 15f)
        );
        
        Debug.Log($"Bubble: {target.name} is now trapped inside bubble and will float together!");
    }
    
    /// <summary>
    /// Waits for bubble duration, then pops the bubble
    /// </summary>
    IEnumerator PopAfterDelay()
    {
        yield return new WaitForSeconds(bubbleDuration);
        PopBubble();
    }
    
    /// <summary>
    /// Pops the bubble with animation and destroys the captured object
    /// </summary>
    void PopBubble()
    {
        if (isPopping) return; // Prevent double-pop
        isPopping = true;
        
        Debug.Log($"Bubble: Popping! Destroying {(bubbledObject != null ? bubbledObject.name : "target")}");
        
        // Play pop sound
        if (bubblePopSound != null)
        {
            AudioSource.PlayClipAtPoint(bubblePopSound, transform.position);
        }
        
        // Spawn pop particle effect
        if (popEffect != null)
        {
            GameObject effect = Instantiate(popEffect, transform.position, Quaternion.identity);
            Destroy(effect, 3f); // Clean up after 3 seconds
        }
        
        // Start shrink animation
        StartCoroutine(ShrinkAndDestroy());
    }
    
    /// <summary>
    /// Animates the bubble shrinking before destroying everything
    /// </summary>
    IEnumerator ShrinkAndDestroy()
    {
        Vector3 startScale = transform.localScale;
        float elapsed = 0f;
        
        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popDuration;
            
            // Shrink the bubble
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            
            yield return null;
        }
        
        // Unparent target before destroying (prevents issues)
        if (bubbledObject != null)
        {
            bubbledObject.transform.SetParent(null);
            Debug.Log($"Bubble: Destroying {bubbledObject.name}");
            Destroy(bubbledObject);
        }
        
        // Destroy the bubble itself
        Destroy(gameObject);
    }
}
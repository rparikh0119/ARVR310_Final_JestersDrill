using UnityEngine;

public class BubblegumStretch : MonoBehaviour
{
    [Header("Bubblegum Physics")]
    public float stretchSpeed = 3f;
    public float stretchAmount = 0.15f;
    public float wobbleSpeed = 2f;
    
    private Vector3 originalScale;
    private float randomOffset;
    
    void Start()
    {
        originalScale = transform.localScale;
        randomOffset = Random.Range(0f, 100f); // Each bubble wobbles differently
    }
    
    void Update()
    {
        // Main stretch oscillation (like gum expanding/contracting)
        float stretch = Mathf.Sin((Time.time + randomOffset) * stretchSpeed) * stretchAmount;
        
        // Wobble on different axes (like gum isn't perfectly round)
        float wobbleX = Mathf.Sin((Time.time + randomOffset) * wobbleSpeed) * stretchAmount * 0.5f;
        float wobbleZ = Mathf.Cos((Time.time + randomOffset) * wobbleSpeed) * stretchAmount * 0.5f;
        
        // Apply stretchy bubblegum effect
        Vector3 newScale = originalScale;
        newScale.x += stretch + wobbleX;
        newScale.y -= stretch * 0.7f; // Squish vertically when stretching horizontally
        newScale.z += stretch + wobbleZ;
        
        transform.localScale = newScale;
    }
}
using UnityEngine;

public class CarouselHorse : MonoBehaviour
{
    [Header("Bob Settings")]
    public float bobHeight = 0.4f;
    public float bobSpeed = 0.3f;
    
    private Vector3 originalPosition;
    private float timer;
    private bool isEnabled = true;

    void Start()
    {
        originalPosition = transform.localPosition;
        timer = Random.Range(0f, 6.28f); // Random start
    }

    void Update()
    {
        if (!isEnabled) return; // Stop bobbing when disabled
        
        // Simple sine wave up/down
        timer += Time.deltaTime * bobSpeed;
        float yOffset = Mathf.Sin(timer) * bobHeight;
        transform.localPosition = originalPosition + Vector3.up * yOffset;
    }
    
    // Called to stop bobbing
    public void StopBobbing()
    {
        isEnabled = false;
        transform.localPosition = originalPosition; // Return to start pos
    }
}

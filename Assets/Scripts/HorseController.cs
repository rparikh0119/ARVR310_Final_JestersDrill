using UnityEngine;

public class CarouselHorse : MonoBehaviour
{
    public float bobHeight = 0.4f;
    public float bobSpeed = 0.3f;
    
    private Vector3 originalPosition;
    private float timer;

    void Start()
    {
        originalPosition = transform.localPosition;
        timer = Random.Range(0f, 6.28f); // Random start
    }

    void Update()
    {
        // Simple sine wave up/down
        timer += Time.deltaTime * bobSpeed;
        float yOffset = Mathf.Sin(timer) * bobHeight;
        transform.localPosition = originalPosition + Vector3.up * yOffset;
    }
}
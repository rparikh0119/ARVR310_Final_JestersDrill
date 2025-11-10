using UnityEngine;

public class CarouselController : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = -12f;
    public float crankSpeedMultiplier = 1.25f;
    public bool isRunning = true;

    private Transform platform;
    private Transform[] cranks;

    void Start()
    {
        // Auto-find the rotating platform
        platform = transform.Find("SM_Prop_Merry_Go_Round_01_Rotate_01");
        
        if (platform == null)
        {
            Debug.LogError("Couldn't find Rotate_01! Make sure it's a child of this GameObject.");
        }

        // Auto-find all cranks (searches all children)
        cranks = GetComponentsInChildren<Transform>();
        
        Debug.Log($"Carousel initialized with {cranks.Length} transforms");
    }

    void Update()
    {
        if (!isRunning) return;

        // Rotate platform
        if (platform != null)
        {
            platform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        // Rotate cranks (only objects with "Crank" in name)
        foreach (Transform t in cranks)
        {
            if (t.name.Contains("Crank"))
            {
                t.Rotate(Vector3.forward * (rotationSpeed * crankSpeedMultiplier) * Time.deltaTime * 10);
            }
        }
    }

    public void StopCarousel()
    {
        isRunning = false;
    }

    public void StartCarousel()
    {
        isRunning = true;
    }
}
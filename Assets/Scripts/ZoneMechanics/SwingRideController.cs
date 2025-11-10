using UnityEngine;

public class SwingRideController : MonoBehaviour
{
    [Header("Ride Settings")]
    public float rotationSpeed = 20f;
    public bool isRunning = true;
    
    [Header("Swing Chairs")]
    public Transform[] swingChairs;
    public float swingAngle = 30f;
    public float swingSpeed = 1f;
    
    private float[] swingOffsets;
    
    void Start()
    {
        // Random offsets for natural swing motion
        swingOffsets = new float[swingChairs.Length];
        for (int i = 0; i < swingChairs.Length; i++)
        {
            swingOffsets[i] = Random.Range(0f, 360f);
        }
    }
    
    void Update()
    {
        if (!isRunning) return;
        
        // Rotate main ride
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
        // Swing each chair
        for (int i = 0; i < swingChairs.Length; i++)
        {
            if (swingChairs[i] != null)
            {
                float swing = Mathf.Sin((Time.time * swingSpeed) + swingOffsets[i]) * swingAngle;
                swingChairs[i].localRotation = Quaternion.Euler(0, 0, swing);
            }
        }
    }
    
    public void StopRide()
    {
        isRunning = false;
        Debug.Log("SwingRide: Stopped");
        
        // Reset chairs to resting position
        foreach (Transform chair in swingChairs)
        {
            if (chair != null)
            {
                chair.localRotation = Quaternion.identity;
            }
        }
    }
    
    public void StartRide()
    {
        isRunning = true;
        Debug.Log("SwingRide: Started");
    }
}
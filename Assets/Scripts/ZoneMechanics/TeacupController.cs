using UnityEngine;

public class TeaCupController : MonoBehaviour
{
    [Header("Ride Components")]
    public GameObject platform;
    public GameObject teaPot;
    public Transform[] teaCups;

    [Header("Rotation Settings")]
    [Range(-60, 60)]
    public float rideSpeed = 15.0f;
    public bool isRunning = true; // âœ… ADDED - Control on/off

    void Update()
    {
        if (!isRunning) return; // âœ… ADDED - Stop all rotation if not running
        
        // Main platform rotation speed
        if (platform != null)
        {
            platform.transform.Rotate(Vector3.up * rideSpeed * Time.deltaTime);
        }

        // Centre ornament (teapot) rotation speed 
        if (teaPot != null)
        {
            teaPot.transform.Rotate(Vector3.down * (rideSpeed * 0.5f) * Time.deltaTime);
        }

        // Tea cup rotations in relation to set ride speed
        foreach (Transform teacup in teaCups)
        {
            if (teacup != null)
            {
                teacup.Rotate(Vector3.up * (rideSpeed * 1.5f) * Time.deltaTime);
            }
        }
    }
    
    // âœ… ADDED METHODS for modular control
    public void StopRide()
    {
        isRunning = false;
        Debug.Log("TeaCupController: Ride stopped");
    }
    
    public void StartRide()
    {
        isRunning = true;
        Debug.Log("TeaCupController: Ride started");
    }
}
using UnityEngine;

public class BumperCarController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float rotationSpeed = 30f;
    public bool isRunning = true;
    
    [Header("Individual Car Settings")]
    public Transform[] bumperCars; // Drag individual car transforms here
    
    private float[] carRotationOffsets;
    
    void Start()
    {
        // Initialize random rotation offsets for variety
        if (bumperCars != null && bumperCars.Length > 0)
        {
            carRotationOffsets = new float[bumperCars.Length];
            for (int i = 0; i < bumperCars.Length; i++)
            {
                carRotationOffsets[i] = Random.Range(0f, 360f);
            }
        }
    }
    
    void Update()
    {
        if (!isRunning) return;
        
        // Rotate each car around its own Y-axis
        for (int i = 0; i < bumperCars.Length; i++)
        {
            if (bumperCars[i] != null)
            {
                // Slight variation in speed per car
                float speedVariation = 1f + Mathf.Sin(Time.time + carRotationOffsets[i]) * 0.3f;
                bumperCars[i].Rotate(Vector3.up * rotationSpeed * speedVariation * Time.deltaTime);
            }
        }
    }
    
    public void StopRide()
    {
        isRunning = false;
        Debug.Log("BumperCarController: Ride stopped");
    }
    
    public void StartRide()
    {
        isRunning = true;
        Debug.Log("BumperCarController: Ride started");
    }
}

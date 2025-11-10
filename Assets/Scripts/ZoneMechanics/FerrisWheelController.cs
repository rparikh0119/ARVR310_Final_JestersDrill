using UnityEngine;

public class FerrisWheelController : MonoBehaviour
{
    [Header("Ride Controls")]
    public float rotationSpeed = 15.0f;
    public float rockingSpeed = 0.2f;
    public float rockingAmplitude = 18f;
    public bool isRunning = true; // ✅ ADD THIS

    public Transform[] chairs;
    public Transform[] wheelsForward;
    public Transform[] wheelsReverse;

    private float timeCounter = 0.0f;

    private void Update()
    {
        if (!isRunning) return; // ✅ ADD THIS - stops all movement when false
        
        // Rotate the Ferris wheel
        transform.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);

        // Rotate top wheels forwards with rotation speed multiplier
        foreach (Transform wheelFwd in wheelsForward)
        {
            if (wheelFwd != null)
            {
                wheelFwd.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime * 10);
            }
        }

        // Rotate bottom wheels backwards with rotation speed multiplier
        foreach (Transform wheelRev in wheelsReverse)
        {
            if (wheelRev != null)
            {
                wheelRev.Rotate(Vector3.back * rotationSpeed * Time.deltaTime * 10);
            }
        }

        // Chair rocking motion
        foreach (Transform chair in chairs)
        {
            if (chair != null)
            {
                timeCounter += rockingSpeed * Time.deltaTime;
                float rockingOffset = Mathf.Sin(timeCounter) * rockingAmplitude * (rotationSpeed / 10);
                chair.localRotation = Quaternion.Euler(0, 0, rockingOffset);
            }
        }
    }

    // ✅ ADD THESE METHODS
    public void StopFerrisWheel()
    {
        isRunning = false;
        Debug.Log("Ferris Wheel: Stopped");
    }

    public void StartFerrisWheel()
    {
        isRunning = true;
        Debug.Log("Ferris Wheel: Started");
    }
}
using UnityEngine;

public class SimpleDoorOpen : MonoBehaviour
{
    public float openSpeed = 2f;
    public float openAngle = 90f;
    private bool shouldOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Start()
    {
        closedRotation = transform.rotation;
        // Opens by rotating on Y axis
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);
    }

    void Update()
    {
        if (shouldOpen)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, openRotation, Time.deltaTime * openSpeed);
        }
    }

    public void OpenDoor()
    {
        shouldOpen = true;
    }
}
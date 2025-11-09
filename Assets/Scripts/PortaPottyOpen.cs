using UnityEngine;
using UnityEngine.Events;

public class DoorTriggerOpen : MonoBehaviour
{
    public UnityEvent onPlayerApproach;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name.Contains("XR Origin") || other.name.Contains("Camera"))
        {
            onPlayerApproach.Invoke();
            // Disable so door only opens once
            GetComponent<Collider>().enabled = false;
        }
    }
}
using UnityEngine;

public class PlayAudioOnce : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name.Contains("XR Origin") || other.name.Contains("Camera"))
        {
            audioSource.Play();
            // Disable so it only plays once
            this.enabled = false;
        }
    }
}
using UnityEngine;

public class SergeantAudioTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip welcomeAudio;
    public AudioSource audioSource;
    
    private bool hasTriggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            
            if (audioSource && welcomeAudio)
            {
                audioSource.PlayOneShot(welcomeAudio);
            }
        }
    }
}
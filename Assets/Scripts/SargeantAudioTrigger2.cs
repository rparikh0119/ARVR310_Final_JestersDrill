using UnityEngine;

public class SergeantAudioTrigger2 : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip beginAudio;
    public AudioSource audioSource;
    
    private bool hasTriggered = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            
            if (audioSource && beginAudio)
            {
                audioSource.PlayOneShot(beginAudio);
            }
        }
    }
}
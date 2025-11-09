using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform doorPanel;
    public float openHeight = 4f;
    public float openSpeed = 2f;
    public AudioClip doorOpenSound;
    
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool isOpening = false;
    private bool hasOpened = false;
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        closedPosition = doorPanel.position;
        openPosition = closedPosition + Vector3.up * openHeight;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasOpened)
        {
            isOpening = true;
            hasOpened = true;
            
            if (audioSource && doorOpenSound)
            {
                audioSource.PlayOneShot(doorOpenSound);
            }
        }
    }
    
    void Update()
    {
        if (isOpening)
        {
            doorPanel.position = Vector3.Lerp(
                doorPanel.position, 
                openPosition, 
                Time.deltaTime * openSpeed
            );
            
            // Stop when close enough
            if (Vector3.Distance(doorPanel.position, openPosition) < 0.1f)
            {
                isOpening = false;
            }
        }
    }
}


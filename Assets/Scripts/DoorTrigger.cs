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
        if (doorPanel != null)
        {
            closedPosition = doorPanel.position;
            openPosition = closedPosition + Vector3.up * openHeight;
        }
        else
        {
            Debug.LogError("DoorTrigger: Door Panel not assigned!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check multiple conditions to detect VR player
        if (!hasOpened)
        {
            // Option 1: Check for Player tag
            if (other.CompareTag("Player"))
            {
                OpenDoor();
                return;
            }
            
            // Option 2: Check if collider belongs to XR Origin hierarchy
            if (other.transform.root.name.Contains("XR Origin"))
            {
                OpenDoor();
                return;
            }
            
            // Option 3: Check for CharacterController (XR Origin has this)
            if (other.GetComponent<CharacterController>() != null)
            {
                OpenDoor();
                return;
            }
            
            // Debug: Log what entered the trigger
            Debug.Log($"Trigger entered by: {other.gameObject.name} (Tag: {other.tag})");
        }
    }
    
    void OpenDoor()
    {
        isOpening = true;
        hasOpened = true;
        
        if (audioSource && doorOpenSound)
        {
            audioSource.PlayOneShot(doorOpenSound);
        }
        
        Debug.Log("Door opening!");
    }
    
    void Update()
    {
        if (isOpening && doorPanel != null)
        {
            doorPanel.position = Vector3.Lerp(
                doorPanel.position, 
                openPosition, 
                Time.deltaTime * openSpeed
            );
            
            // Stop when close enough
            if (Vector3.Distance(doorPanel.position, openPosition) < 0.1f)
            {
                doorPanel.position = openPosition;
                isOpening = false;
                Debug.Log("Door fully open!");
            }
        }
    }
}
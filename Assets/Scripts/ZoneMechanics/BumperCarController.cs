using UnityEngine;

public class BumperCarController : MonoBehaviour
{
    [Header("Bumper Cars")]
    public Transform[] bumperCars;
    
    [Header("Movement Settings")]
    public float rotationSpeed = 30f;
    public float movementRadius = 2f;
    public float movementSpeed = 1f;
    public bool isRunning = true;
    
    [Header("Audio")] // ✅ UPDATED
    public AudioClip rideAmbientSound;
    public float audioVolume = 0.5f;
    
    private AudioSource audioSource;
    private Vector3[] originalPositions;
    private float[] timeOffsets;
    
    void Start()
    {
        // ✅ Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.spatialBlend = 1f; // Full 3D
        audioSource.loop = true;
        audioSource.volume = audioVolume;
        audioSource.playOnAwake = false;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 50f;
        
        // Store original positions
        originalPositions = new Vector3[bumperCars.Length];
        timeOffsets = new float[bumperCars.Length];
        
        for (int i = 0; i < bumperCars.Length; i++)
        {
            if (bumperCars[i] != null)
            {
                originalPositions[i] = bumperCars[i].localPosition;
                timeOffsets[i] = Random.Range(0f, 360f);
            }
        }
        
        // Start audio if ride is running
        if (isRunning && rideAmbientSound != null)
        {
            audioSource.clip = rideAmbientSound;
            audioSource.Play();
        }
    }
    
    void Update()
    {
        if (!isRunning) return;
        
        for (int i = 0; i < bumperCars.Length; i++)
        {
            if (bumperCars[i] != null)
            {
                // Rotate car
                bumperCars[i].Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
                
                // Move in circular pattern
                float angle = (Time.time * movementSpeed + timeOffsets[i]) * Mathf.Deg2Rad;
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * movementRadius,
                    0,
                    Mathf.Sin(angle) * movementRadius
                );
                
                bumperCars[i].localPosition = originalPositions[i] + offset;
            }
        }
    }
    
    public void StopRide()
    {
        isRunning = false;
        
        // ✅ Fade out audio
        if (audioSource != null && audioSource.isPlaying)
        {
            StartCoroutine(FadeOutAudio(1.5f));
        }
        
        Debug.Log("BumperCarController: Ride stopped");
    }
    
    public void StartRide()
    {
        isRunning = true;
        
        // ✅ Start audio
        if (audioSource != null && rideAmbientSound != null && !audioSource.isPlaying)
        {
            audioSource.volume = audioVolume;
            audioSource.Play();
        }
        
        Debug.Log("BumperCarController: Ride started");
    }
    
    // ✅ Smooth audio fade out
    System.Collections.IEnumerator FadeOutAudio(float duration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        audioSource.Stop();
        audioSource.volume = audioVolume; // Reset for next time
    }
}
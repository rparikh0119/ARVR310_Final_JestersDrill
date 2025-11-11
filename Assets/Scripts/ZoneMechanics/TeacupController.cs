using UnityEngine;

public class TeaCupController : MonoBehaviour
{
    [Header("Ride Components")]
    public GameObject platform;
    public GameObject teaPot;
    public Transform[] teaCups;

    [Header("Rotation Settings")]
    [Range(-60, 60)]
    public float rideSpeed = 15.0f;
    public bool isRunning = true;

    [Header("Audio")] // ✅ NEW
    public AudioClip rideAmbientSound;
    public float audioVolume = 0.5f;
    
    private AudioSource audioSource;

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
        
        // Main platform rotation speed
        if (platform != null)
        {
            platform.transform.Rotate(Vector3.up * rideSpeed * Time.deltaTime);
        }

        // Centre ornament (teapot) rotation speed 
        if (teaPot != null)
        {
            teaPot.transform.Rotate(Vector3.down * (rideSpeed * 0.5f) * Time.deltaTime);
        }

        // Tea cup rotations in relation to set ride speed
        foreach (Transform teacup in teaCups)
        {
            if (teacup != null)
            {
                teacup.Rotate(Vector3.up * (rideSpeed * 1.5f) * Time.deltaTime);
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
        
        Debug.Log("TeaCupController: Ride stopped");
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
        
        Debug.Log("TeaCupController: Ride started");
    }
    
    // ✅ Smooth audio fade out
    private System.Collections.IEnumerator FadeOutAudio(float fadeDuration)
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
            yield return null;
        }
        
        audioSource.Stop();
        audioSource.volume = audioVolume; // Reset for next time
    }
}
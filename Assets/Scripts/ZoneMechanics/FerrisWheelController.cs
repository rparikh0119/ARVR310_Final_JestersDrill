using UnityEngine;

public class FerrisWheelController : MonoBehaviour
{
    [Header("Ride Controls")]
    public Transform wheelRotatePart;
    public float rotationSpeed = 15.0f;
    public float rockingSpeed = 0.2f;
    public float rockingAmplitude = 18f;
    public bool isRunning = true;

    public Transform[] chairs;
    public Transform[] wheelsForward;
    public Transform[] wheelsReverse;

    [Header("Audio")] // ✅ NEW SECTION
    public AudioClip rideAmbientSound; // Looping mechanical/carnival sound
    public float audioVolume = 0.5f;
    
    private AudioSource audioSource;
    private float timeCounter = 0.0f;

    private void Start()
    {
        // ✅ Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure audio for 3D spatial sound
        audioSource.spatialBlend = 1f; // Full 3D
        audioSource.loop = true; // Loop continuously
        audioSource.volume = audioVolume;
        audioSource.playOnAwake = false;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 50f; // Adjust based on your scene scale
        
        // Start audio if ride is running
        if (isRunning && rideAmbientSound != null)
        {
            audioSource.clip = rideAmbientSound;
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (!isRunning) return;
        
        // Rotate only the wheel part, not the entire transform
        if (wheelRotatePart != null)
        {
            wheelRotatePart.Rotate(Vector3.back * rotationSpeed * Time.deltaTime);
        }

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

    public void StopFerrisWheel()
    {
        isRunning = false;
        
        // ✅ Fade out audio smoothly
        if (audioSource != null && audioSource.isPlaying)
        {
            StartCoroutine(FadeOutAudio(1.5f)); // Fade over 1.5 seconds
        }
        
        Debug.Log("🛑 Ferris Wheel: Stopped");
    }

    public void StartFerrisWheel()
    {
        isRunning = true;
        
        // ✅ Start audio
        if (audioSource != null && rideAmbientSound != null && !audioSource.isPlaying)
        {
            audioSource.volume = audioVolume;
            audioSource.Play();
        }
        
        Debug.Log("✅ Ferris Wheel: Started");
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
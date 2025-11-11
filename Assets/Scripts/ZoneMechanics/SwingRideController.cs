using UnityEngine;

public class SwingRideController : MonoBehaviour
{
    [Header("Ride Settings")]
    public float rotationSpeed = 20f;
    public bool isRunning = true;
    
    [Header("Swing Chairs")]
    public Transform[] swingChairs;
    public float swingAngle = 30f;
    public float swingSpeed = 1f;
    
    [Header("Audio")] // ✅ NEW
    public AudioClip rideAmbientSound;
    public float audioVolume = 0.5f;
    
    private AudioSource audioSource;
    private float[] swingOffsets;
    
    void Start()
    {
        // ✅ Setup AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.spatialBlend = 1f;
        audioSource.loop = true;
        audioSource.volume = audioVolume;
        audioSource.playOnAwake = false;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.maxDistance = 50f;
        
        // Start audio if running
        if (isRunning && rideAmbientSound != null)
        {
            audioSource.clip = rideAmbientSound;
            audioSource.Play();
        }
        
        // Random offsets for natural swing motion
        swingOffsets = new float[swingChairs.Length];
        for (int i = 0; i < swingChairs.Length; i++)
        {
            swingOffsets[i] = Random.Range(0f, 360f);
        }
    }
    
    void Update()
    {
        if (!isRunning) return;
        
        // Rotate main ride
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
        // Swing each chair
        for (int i = 0; i < swingChairs.Length; i++)
        {
            if (swingChairs[i] != null)
            {
                float swing = Mathf.Sin((Time.time * swingSpeed) + swingOffsets[i]) * swingAngle;
                swingChairs[i].localRotation = Quaternion.Euler(0, 0, swing);
            }
        }
    }
    
    public void StopRide()
{
    isRunning = false;
    
    Debug.Log("🛑 SwingRide: Stopping ride and fading audio");
    
    // Fade out audio
    if (audioSource != null && audioSource.isPlaying)
    {
        StartCoroutine(FadeOutAudio(2f));
    }
    
    // Gradually stop chairs
    StartCoroutine(StopChairsGradually());
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
        
        Debug.Log("SwingRide: Started");
    }
    
    // ✅ Smooth audio fade
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
        audioSource.volume = audioVolume;
    }
    
    // ✅ Gradually bring chairs to rest
    System.Collections.IEnumerator StopChairsGradually()
    {
        float duration = 2f;
        float elapsed = 0f;
        
        Quaternion[] startRotations = new Quaternion[swingChairs.Length];
        for (int i = 0; i < swingChairs.Length; i++)
        {
            if (swingChairs[i] != null)
            {
                startRotations[i] = swingChairs[i].localRotation;
            }
        }
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            for (int i = 0; i < swingChairs.Length; i++)
            {
                if (swingChairs[i] != null)
                {
                    swingChairs[i].localRotation = Quaternion.Slerp(startRotations[i], Quaternion.identity, t);
                }
            }
            
            yield return null;
        }
        
        // Ensure fully reset
        foreach (Transform chair in swingChairs)
        {
            if (chair != null)
            {
                chair.localRotation = Quaternion.identity;
            }
        }
    }
}
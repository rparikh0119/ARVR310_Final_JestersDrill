using UnityEngine;
using System.Collections;

public class CarouselController : MonoBehaviour
{
    [Header("References")]
    public Transform platform;
    
    [Header("Settings")]
    public float rotationSpeed = -12f;
    public float crankSpeedMultiplier = 1.25f;
    public bool isRunning = true;

    [Header("Audio")]
    public AudioClip carouselMusic; // Merry-go-round music
    private AudioSource audioSource;

    private Transform[] cranks;

    void Start()
    {
        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 1f; // 3D sound
        audioSource.loop = true; // Loop the music
        audioSource.playOnAwake = false;
        
        // Play music if carousel is running
        if (isRunning && carouselMusic != null)
        {
            audioSource.clip = carouselMusic;
            audioSource.Play();
            Debug.Log("🎵 Carousel music started!");
        }
        
        // Find platform
        if (platform == null)
        {
            platform = transform.Find("SM_Prop_Merry_Go_Round_01_Rotate_01");
        }
        
        if (platform == null)
        {
            Debug.LogError("❌ Couldn't find Rotate_01!");
        }
        else
        {
            Debug.Log("✅ Platform found! Carousel should be spinning.");
        }

        StartCoroutine(FindCranksDelayed());
    }
    
    System.Collections.IEnumerator FindCranksDelayed()
    {
        yield return null;
        
        Transform[] allTransforms = GetComponentsInChildren<Transform>();
        System.Collections.Generic.List<Transform> crankList = new System.Collections.Generic.List<Transform>();
        
        foreach (Transform t in allTransforms)
        {
            if (t.name.Contains("Crank"))
            {
                crankList.Add(t);
            }
        }
        
        cranks = crankList.ToArray();
        Debug.Log($"✅ Carousel initialized with {cranks.Length} cranks");
    }

    void Update()
    {
        if (!isRunning) return;

        // Rotate platform
        if (platform != null)
        {
            platform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        // Rotate cranks
        if (cranks != null && cranks.Length > 0)
        {
            foreach (Transform t in cranks)
            {
                if (t != null && t.name.Contains("Crank"))
                {
                    t.Rotate(Vector3.forward * (rotationSpeed * crankSpeedMultiplier) * Time.deltaTime * 10);
                }
            }
        }
    }

    public void StopCarousel()
    {
        isRunning = false;
        
        // Stop music with fade out
        if (audioSource != null && audioSource.isPlaying)
        {
            StartCoroutine(FadeOutMusic());
        }
        
        Debug.Log("🛑 Carousel stopped!");
    }

    public void StartCarousel()
    {
        isRunning = true;
        
        // Start music
        if (audioSource != null && carouselMusic != null && !audioSource.isPlaying)
        {
            audioSource.clip = carouselMusic;
            audioSource.Play();
            Debug.Log("🎵 Carousel music started!");
        }
        
        Debug.Log("▶️ Carousel started!");
    }
    
    System.Collections.IEnumerator FadeOutMusic()
    {
        float startVolume = audioSource.volume;
        float fadeTime = 2f; // 2 second fade
        float elapsed = 0f;
        
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeTime);
            yield return null;
        }
        
        audioSource.Stop();
        audioSource.volume = startVolume; // Reset volume for next play
        Debug.Log("🔇 Carousel music stopped!");
    }
}


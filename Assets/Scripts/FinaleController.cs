using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CircusTentFinale : MonoBehaviour
{
    [Header("Spotlights")]
    public Light spotlight1;
    public Light spotlight2;
    public Light spotlight3;
    public float spotlightTurnOnDelay = 0.8f;
    public float spotlightIntensity = 10f;
    
    [Header("Ringleader")]
    public Transform ringleaderPosition;
    public AudioClip ringleaderDialogue;
    
    [Header("Bomb Sequence")]
    public AudioClip tickingBombSound;
    public AudioClip explosionSound;
    public float tickingDuration = 5f;
    public float explosionDelay = 0.5f;
    
    [Header("Fade to Black")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeSpeed = 1f;
    
    [Header("Debug")]
    public bool showDebugLogs = true;
    
    private AudioSource audioSource;
    private bool hasTriggered = false;
    
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        
        if (spotlight1 != null) spotlight1.intensity = 0f;
        if (spotlight2 != null) spotlight2.intensity = 0f;
        if (spotlight3 != null) spotlight3.intensity = 0f;
        
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
        }
        
        if (showDebugLogs)
            Debug.Log("🎪 Circus Tent Finale initialized - waiting for player...");
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            
            if (showDebugLogs)
                Debug.Log("🎭 PLAYER ENTERED TENT - STARTING FINALE!");
            
            StartCoroutine(FinaleSequence());
        }
    }
    
    IEnumerator FinaleSequence()
    {
        if (showDebugLogs)
            Debug.Log("🎬 ACT 1: Spotlight Reveal");
        
        yield return new WaitForSeconds(0.5f);
        
        if (spotlight1 != null)
        {
            StartCoroutine(FadeInSpotlight(spotlight1));
            if (showDebugLogs) Debug.Log("💡 Spotlight 1 ON");
        }
        
        yield return new WaitForSeconds(spotlightTurnOnDelay);
        
        if (spotlight2 != null)
        {
            StartCoroutine(FadeInSpotlight(spotlight2));
            if (showDebugLogs) Debug.Log("💡 Spotlight 2 ON");
        }
        
        yield return new WaitForSeconds(spotlightTurnOnDelay);
        
        if (spotlight3 != null)
        {
            StartCoroutine(FadeInSpotlight(spotlight3));
            if (showDebugLogs) Debug.Log("💡 Spotlight 3 ON");
        }
        
        yield return new WaitForSeconds(1f);
        
        if (showDebugLogs)
            Debug.Log("🎬 ACT 2: Ringleader Monologue");
        
        if (ringleaderDialogue != null)
        {
            audioSource.PlayOneShot(ringleaderDialogue);
            float dialogueDuration = ringleaderDialogue.length;
            
            if (showDebugLogs)
                Debug.Log($"🎙️ Playing ringleader dialogue ({dialogueDuration}s)");
            
            yield return new WaitForSeconds(dialogueDuration);
        }
        else
        {
            if (showDebugLogs)
                Debug.LogWarning("⚠️ No dialogue assigned! Waiting 5 seconds...");
            
            yield return new WaitForSeconds(5f);
        }
        
        if (showDebugLogs)
            Debug.Log("🎬 ACT 3: Lights Out");
        
        StartCoroutine(FadeOutAllSpotlights());
        
        yield return new WaitForSeconds(1f);
        
        if (showDebugLogs)
            Debug.Log("💣 ACT 4: The Bomb!");
        
        if (tickingBombSound != null)
        {
            audioSource.loop = true;
            audioSource.clip = tickingBombSound;
            audioSource.Play();
            
            if (showDebugLogs)
                Debug.Log($"⏱️ Ticking for {tickingDuration} seconds...");
            
            yield return new WaitForSeconds(tickingDuration);
            
            audioSource.Stop();
            audioSource.loop = false;
        }
        else
        {
            if (showDebugLogs)
                Debug.LogWarning("⚠️ No ticking sound assigned!");
        }
        
        if (showDebugLogs)
            Debug.Log("😰 Brief silence...");
        
        yield return new WaitForSeconds(explosionDelay);
        
        if (showDebugLogs)
            Debug.Log("💥 BOOM!!!");
        
        if (explosionSound != null)
        {
            audioSource.PlayOneShot(explosionSound);
            
            if (showDebugLogs)
                Debug.Log("💥 Playing explosion sound");
            
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            if (showDebugLogs)
                Debug.LogWarning("⚠️ No explosion sound assigned!");
        }
        
        if (showDebugLogs)
            Debug.Log("🎬 ACT 5: Fade to Black");
        
        yield return StartCoroutine(FadeToBlack());
        
        if (showDebugLogs)
            Debug.Log("🎭 FINALE COMPLETE - THE END");
    }
    
    IEnumerator FadeInSpotlight(Light spotlight)
    {
        float currentIntensity = 0f;
        
        while (currentIntensity < spotlightIntensity)
        {
            currentIntensity += spotlightIntensity * Time.deltaTime * 3f;
            spotlight.intensity = currentIntensity;
            yield return null;
        }
        
        spotlight.intensity = spotlightIntensity;
    }
    
    IEnumerator FadeOutAllSpotlights()
    {
        float t = 0f;
        float duration = 1.5f;
        
        float intensity1 = spotlight1 != null ? spotlight1.intensity : 0f;
        float intensity2 = spotlight2 != null ? spotlight2.intensity : 0f;
        float intensity3 = spotlight3 != null ? spotlight3.intensity : 0f;
        
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            
            if (spotlight1 != null) spotlight1.intensity = Mathf.Lerp(intensity1, 0f, progress);
            if (spotlight2 != null) spotlight2.intensity = Mathf.Lerp(intensity2, 0f, progress);
            if (spotlight3 != null) spotlight3.intensity = Mathf.Lerp(intensity3, 0f, progress);
            
            yield return null;
        }
        
        if (spotlight1 != null) spotlight1.intensity = 0f;
        if (spotlight2 != null) spotlight2.intensity = 0f;
        if (spotlight3 != null) spotlight3.intensity = 0f;
    }
    
    IEnumerator FadeToBlack()
    {
        if (fadeCanvasGroup != null)
        {
            float t = 0f;
            
            while (t < 1f)
            {
                t += Time.deltaTime * fadeSpeed;
                fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
            
            fadeCanvasGroup.alpha = 1f;
            
            if (showDebugLogs)
                Debug.Log("💀 Screen faded to black - THE END");
        }
        
        yield return new WaitForSeconds(2f);
    }
}
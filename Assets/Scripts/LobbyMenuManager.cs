using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LobbyMenuManager : MonoBehaviour
{
    [Header("Full-Size Panels")]
    public GameObject panel_Page1;
    public GameObject panel_Page2;
    public GameObject panel_Page3;
    
    [Header("Small Equipment Panels (6 cards)")]
    public GameObject[] equipmentPanels = new GameObject[6];
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip panelTransitionSound;
    public AudioClip equipmentCardsSound;
    public AudioClip startButtonSound;
    
    [Header("Scene Transition")]
    public string carnivalSceneName = "CarnivalGrounds_Main";
    
    void Start()
    {
        // Show only Page 1 on start
        ShowPage1();
    }
    
    // === PAGE NAVIGATION ===
    
    public void ShowPage1()
    {
        panel_Page1.SetActive(true);
        panel_Page2.SetActive(false);
        panel_Page3.SetActive(false);
        HideAllEquipmentPanels();
        
        PlaySound(panelTransitionSound);
    }
    
    public void ShowPage2()
    {
        panel_Page1.SetActive(false);
        panel_Page2.SetActive(true);
        panel_Page3.SetActive(false);
        HideAllEquipmentPanels();
        
        PlaySound(panelTransitionSound);
    }
    
    public void ShowPage3()
    {
        panel_Page1.SetActive(false);
        panel_Page2.SetActive(false);
        panel_Page3.SetActive(true);
        HideAllEquipmentPanels(); // Keep equipment hidden until NEXT clicked on Page 3
        
        PlaySound(panelTransitionSound);
    }
    
    // === EQUIPMENT PANELS ===
    
    public void ShowEquipmentPanels()
    {
        // Hide all full-size panels
        panel_Page1.SetActive(false);
        panel_Page2.SetActive(false);
        panel_Page3.SetActive(false);
        
        // Show equipment cards with cascade effect
        StartCoroutine(ShowEquipmentPanelsWithDelay());
        
        PlaySound(equipmentCardsSound);
    }
    
    void HideAllEquipmentPanels()
    {
        foreach (GameObject panel in equipmentPanels)
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }
    }
    
    IEnumerator ShowEquipmentPanelsWithDelay()
    {
        foreach (GameObject panel in equipmentPanels)
        {
            if (panel != null)
            {
                panel.SetActive(true);
                yield return new WaitForSeconds(0.1f); // 0.1 second delay between each card
            }
        }
    }
    
    // === START MISSION ===
    
    public void StartMission()
    {
        PlaySound(startButtonSound);
        
        // Use coroutine for async loading
        StartCoroutine(LoadCarnivalSceneAsync());
    }
    
    IEnumerator LoadCarnivalSceneAsync()
    {
        // Wait for button sound to finish
        yield return new WaitForSeconds(0.5f);
        
        // Use ASYNC loading to prevent freeze
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(carnivalSceneName);
        
        // Optional: Prevent scene from activating immediately
        asyncLoad.allowSceneActivation = false;
        
        // Wait until scene is 90% loaded
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }
        
        // Small delay before activation
        yield return new WaitForSeconds(0.1f);
        
        // Activate the scene
        asyncLoad.allowSceneActivation = true;
        
        // Wait until fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        Debug.Log("Carnival scene loaded successfully!");
    }
    
    // === AUDIO ===
    
    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
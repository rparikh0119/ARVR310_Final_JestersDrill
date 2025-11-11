using UnityEngine;
using System.Collections;

public class WristUIManager : MonoBehaviour
{
    [Header("Panel References")]
    public GameObject smallMenuPanel;
    public GameObject mainMenuPanel;
    public GameObject weaponsPanel;
    public GameObject equipmentPanel;
    
    [Header("Weapon Manager")]
    public WeaponManager weaponManager;
    
    [Header("Radio Audio")]
    public AudioSource radioAudioSource;
    public AudioClip sergeantBriggsWeaponInstructions;
    
    [Header("UI Sound")]
    public AudioSource uiAudioSource;
    public AudioClip buttonClickSound;
    
    void Start()
    {
        // Use coroutine to avoid blocking scene load
        StartCoroutine(InitializeUIDelayed());
    }
    
    System.Collections.IEnumerator InitializeUIDelayed()
    {
        // Wait for scene to fully load
        yield return new WaitForEndOfFrame();
        
        // Start with only small menu visible
        ShowSmallMenu();
    }
    
    // ===== PANEL NAVIGATION =====
    
    public void ShowSmallMenu()
    {
        smallMenuPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        weaponsPanel.SetActive(false);
        equipmentPanel.SetActive(false);
        
        Debug.Log("UI: Showing Small Menu");
    }
    
    public void ShowMainMenu()
    {
        smallMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        weaponsPanel.SetActive(false);
        equipmentPanel.SetActive(false);
        
        PlayButtonSound();
        Debug.Log("UI: Showing Main Menu");
    }
    
    public void ShowWeaponsPanel()
    {
        smallMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        weaponsPanel.SetActive(true);
        equipmentPanel.SetActive(false);
        
        PlayButtonSound();
        Debug.Log("UI: Showing Weapons Panel");
    }
    
    public void ShowEquipmentPanel()
    {
        smallMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        weaponsPanel.SetActive(false);
        equipmentPanel.SetActive(true);
        
        PlayButtonSound();
        Debug.Log("UI: Showing Equipment Panel");
    }
    
    // ===== WEAPON SELECTION =====
    
    public void EquipBubbleGun()
    {
        if (weaponManager != null)
        {
            weaponManager.EquipWeapon(0);
            PlayButtonSound();
            Debug.Log("Equipped: Bubble Gun");
        }
        else
        {
            Debug.LogError("WeaponManager reference missing!");
        }
    }
    
    public void EquipDartLauncher()
    {
        if (weaponManager != null)
        {
            weaponManager.EquipWeapon(1);
            PlayButtonSound();
            Debug.Log("Equipped: Prize Dart Launcher");
        }
        else
        {
            Debug.LogError("WeaponManager reference missing!");
        }
    }
    
    public void EquipConfettiLauncher()
    {
        if (weaponManager != null)
        {
            weaponManager.EquipWeapon(2);
            PlayButtonSound();
            Debug.Log("Equipped: Confetti Rocket Launcher");
        }
        else
        {
            Debug.LogError("WeaponManager reference missing!");
        }
    }
    
    // ===== EQUIPMENT TOGGLES =====
    
    public void PlayRadioInstructions()
    {
        if (radioAudioSource != null && sergeantBriggsWeaponInstructions != null)
        {
            radioAudioSource.PlayOneShot(sergeantBriggsWeaponInstructions);
            Debug.Log("Playing Sergeant Briggs radio instructions");
        }
        else
        {
            Debug.LogError("Radio AudioSource or AudioClip missing!");
        }
    }
    
    // ===== AUDIO HELPER =====
    
    private void PlayButtonSound()
    {
        if (uiAudioSource != null && buttonClickSound != null)
        {
            uiAudioSource.PlayOneShot(buttonClickSound);
        }
    }
}
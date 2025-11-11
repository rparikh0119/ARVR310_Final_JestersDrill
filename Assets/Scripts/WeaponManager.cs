using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Prefabs")]
    public GameObject[] weaponPrefabs; // Array of all your weapons
    
    [Header("Weapon Attachment")]
    public Transform weaponAttachPoint; // Right hand controller
    
    [Header("Current Weapon")]
    public int currentWeaponIndex = 0;
    private GameObject currentWeaponInstance;
    
    [Header("Auto-Equip Settings")]
    public bool autoEquipOnStart = false; // Temporarily disabled to debug scene loading
    public float autoEquipDelay = 0.5f;
    
    void Start()
    {
        // Only auto-equip if enabled and we have valid references
        if (autoEquipOnStart && weaponPrefabs.Length > 0 && weaponAttachPoint != null)
        {
            // Use Invoke to delay without blocking
            Invoke(nameof(AutoEquipFirstWeapon), autoEquipDelay);
        }
        else if (autoEquipOnStart)
        {
            if (weaponPrefabs.Length == 0)
            {
                Debug.LogWarning("WeaponManager: No weapons assigned in weaponPrefabs array! Auto-equip disabled.");
            }
            if (weaponAttachPoint == null)
            {
                Debug.LogWarning("WeaponManager: weaponAttachPoint is not assigned! Auto-equip disabled.");
            }
        }
    }
    
    void AutoEquipFirstWeapon()
    {
        if (weaponPrefabs.Length > 0 && weaponAttachPoint != null)
        {
            EquipWeapon(0);
        }
    }
    
    // PUBLIC - Called by UI buttons
    public void EquipWeapon(int weaponIndex)
    {
        // Safety checks
        if (weaponAttachPoint == null)
        {
            Debug.LogError("WeaponManager: Cannot equip weapon - weaponAttachPoint is null!");
            return;
        }
        
        if (weaponIndex < 0 || weaponIndex >= weaponPrefabs.Length)
        {
            Debug.LogError($"WeaponManager: Invalid weapon index {weaponIndex}!");
            return;
        }
        
        if (weaponPrefabs[weaponIndex] == null)
        {
            Debug.LogError($"WeaponManager: Weapon prefab at index {weaponIndex} is null!");
            return;
        }
        
        // Destroy current weapon if one exists
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }
        
        // Spawn new weapon as child of right hand
        currentWeaponInstance = Instantiate(weaponPrefabs[weaponIndex], weaponAttachPoint);
        
        // Reset position and rotation to match hand exactly
        currentWeaponInstance.transform.localPosition = Vector3.zero;
        currentWeaponInstance.transform.localRotation = Quaternion.identity;
        
        // Update index
        currentWeaponIndex = weaponIndex;
        
        Debug.Log($"Equipped weapon: {weaponPrefabs[weaponIndex].name}");
    }
}
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Prefabs")]
    public GameObject[] weaponPrefabs; // Array of all your weapons
    
    [Header("Weapon Attachment")]
    public Transform weaponAttachPoint; // Right hand controller
    
    [Header("Current Weapon")]
    public int currentWeaponIndex = 0;
    private GameObject currentWeaponInstance;
    
    void Start()
    {
        // Auto-equip first weapon at start
        if (weaponPrefabs.Length > 0)
        {
            EquipWeapon(0);
        }
        else
        {
            Debug.LogError("WeaponManager: No weapons assigned in weaponPrefabs array!");
        }
    }
    
    // PUBLIC - Called by UI buttons
    public void EquipWeapon(int weaponIndex)
    {
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
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Prefabs")]
    public GameObject[] weaponPrefabs; // Array of all your weapons
    
    [Header("Weapon Attachment")]
    public Transform weaponAttachPoint; // Right hand controller
    
    [Header("Current Weapon")]
    public int currentWeaponIndex = 0;
    private GameObject currentWeaponInstance;
    
    [Header("Input")]
    public InputActionProperty weaponSwitchInput; // Left thumbstick
    
    private float switchCooldown = 0.5f; // Prevents accidental double-switching
    private float lastSwitchTime = 0f;
    
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
    
    void Update()
    {
        // Only allow switching if cooldown has passed
        if (Time.time < lastSwitchTime + switchCooldown)
            return;
        
        // Read left thumbstick input
        Vector2 switchInput = weaponSwitchInput.action.ReadValue<Vector2>();
        
        if (switchInput.y > 0.7f) // Thumbstick pushed UP
        {
            SwitchToNextWeapon();
            lastSwitchTime = Time.time;
        }
        else if (switchInput.y < -0.7f) // Thumbstick pushed DOWN
        {
            SwitchToPreviousWeapon();
            lastSwitchTime = Time.time;
        }
    }
    
    public void EquipWeapon(int weaponIndex)  // ✅ Add "public" keyword
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
    
    void SwitchToNextWeapon()
    {
        currentWeaponIndex++;
        
        // Loop back to first weapon if we go past the end
        if (currentWeaponIndex >= weaponPrefabs.Length)
        {
            currentWeaponIndex = 0;
        }
        
        EquipWeapon(currentWeaponIndex);
    }
    
    void SwitchToPreviousWeapon()
    {
        currentWeaponIndex--;
        
        // Loop to last weapon if we go before the first
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weaponPrefabs.Length - 1;
        }
        
        EquipWeapon(currentWeaponIndex);
    }
}

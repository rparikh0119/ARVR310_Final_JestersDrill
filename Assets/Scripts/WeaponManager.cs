using UnityEngine;
using System.Collections;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Prefabs")]
    public GameObject[] weaponPrefabs;
    
    [Header("Weapon Attachment")]
    public Transform weaponAttachPoint;
    
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
            Debug.LogError("WeaponManager: No weapons assigned!");
        }
    }
    
    // PUBLIC - Called by UI buttons
    public void EquipWeapon(int weaponIndex)
    {
        // Validation
        if (weaponIndex < 0 || weaponIndex >= weaponPrefabs.Length)
        {
            Debug.LogError($"Invalid weapon index: {weaponIndex}");
            return;
        }
        
        // If trying to equip the same weapon, ignore
        if (currentWeaponIndex == weaponIndex && currentWeaponInstance != null)
        {
            Debug.Log("Same weapon already equipped, ignoring");
            return;
        }
        
        // Use coroutine to prevent freezing during instantiation
        StartCoroutine(EquipWeaponAsync(weaponIndex));
    }
    
    private IEnumerator EquipWeaponAsync(int weaponIndex)
    {
        // Destroy current weapon
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
            currentWeaponInstance = null;
            // Yield to allow frame update after destroy
            yield return null;
        }
        
        // Spawn new weapon as child of right hand
        // This can be slow for complex prefabs, so we yield after
        currentWeaponInstance = Instantiate(weaponPrefabs[weaponIndex], weaponAttachPoint);
        
        // Yield to allow frame update after instantiation
        yield return null;

        // Reset position only - KEEP prefab's rotation
        if (currentWeaponInstance != null)
        {
            currentWeaponInstance.transform.localPosition = Vector3.zero;
            // ✅ REMOVED the localRotation line - let prefab keep its Y=180 rotation!
        }

        currentWeaponIndex = weaponIndex;
        
        Debug.Log($"✅ Equipped weapon: {weaponPrefabs[weaponIndex].name}");
    }
}
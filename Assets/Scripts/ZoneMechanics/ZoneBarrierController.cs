using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneBarrierManager : MonoBehaviour
{
    public static ZoneBarrierManager Instance;

    [Header("Zone Barriers")]
    public GameObject[] zoneBarriers;
    
    [Header("Zone Status")]
    public bool[] zoneUnlocked = new bool[5];
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Zone 1 unlocked by default, others locked
        zoneUnlocked[0] = true;
        for (int i = 1; i < zoneUnlocked.Length; i++)
        {
            zoneUnlocked[i] = false;
        }
        
        StartCoroutine(SetupBarriersDelayed());
    }
    
    System.Collections.IEnumerator SetupBarriersDelayed()
    {
        yield return new WaitForEndOfFrame();
        yield return null;
        
        // Setup barriers based on unlock status
        for (int i = 0; i < zoneBarriers.Length; i++)
        {
            if (zoneBarriers[i] != null)
            {
                // Zone 1 active, others hidden
                zoneBarriers[i].SetActive(zoneUnlocked[i]);
                yield return null;
            }
        }
        
        Debug.Log("✅ Zone barriers initialized. Zone 1 is active!");
    }

    public void UnlockZone(int zoneNumber)
    {
        int index = zoneNumber - 1;
        
        if (index >= 0 && index < zoneUnlocked.Length)
        {
            zoneUnlocked[index] = true;
            
            if (zoneBarriers[index] != null)
            {
                zoneBarriers[index].SetActive(true);
            }
            
            Debug.Log($"Zone {zoneNumber} unlocked! Barrier now active.");
        }
    }

    public bool IsZoneUnlocked(int zoneNumber)
    {
        int index = zoneNumber - 1;
        if (index >= 0 && index < zoneUnlocked.Length)
        {
            return zoneUnlocked[index];
        }
        return false;
    }

    public void DissolveZoneBarrier(int zoneNumber)
    {
        int index = zoneNumber - 1;
        if (index >= 0 && index < zoneBarriers.Length)
        {
            if (zoneBarriers[index] != null)
            {
                ZoneBarrier barrier = zoneBarriers[index].GetComponent<ZoneBarrier>();
                if (barrier != null)
                {
                    barrier.StartDissolve();
                }
            }
        }
    }
}
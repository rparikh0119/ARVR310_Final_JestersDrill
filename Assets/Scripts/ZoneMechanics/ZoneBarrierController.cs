using UnityEngine;
using System.Collections.Generic;

public class ZoneBarrierManager : MonoBehaviour
{
    public static ZoneBarrierManager Instance; // Singleton

    [Header("Zone Barriers")]
    public GameObject[] zoneBarriers; // Assign all 5 barrier domes
    
    [Header("Zone Status")]
    public bool[] zoneUnlocked = new bool[5]; // Tracks which zones are unlocked
    
    void Awake()
    {
        // Singleton setup
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
        // All zones locked initially
        for (int i = 0; i < zoneUnlocked.Length; i++)
        {
            zoneUnlocked[i] = false;
        }
        
        // All barriers invisible initially
        foreach (GameObject barrier in zoneBarriers)
        {
            if (barrier != null)
                barrier.SetActive(false);
        }
    }

    // Called by RadioBriefingManager when briefing completes
    public void UnlockZone(int zoneNumber)
    {
        // Zone numbers are 1-5, array is 0-4
        int index = zoneNumber - 1;
        
        if (index >= 0 && index < zoneUnlocked.Length)
        {
            zoneUnlocked[index] = true;
            
            // Enable the barrier (make it visible and interactable)
            if (zoneBarriers[index] != null)
            {
                zoneBarriers[index].SetActive(true);
            }
            
            Debug.Log($"Zone {zoneNumber} unlocked! Barrier now active.");
        }
    }

    // Check if a specific zone is unlocked
    public bool IsZoneUnlocked(int zoneNumber)
    {
        int index = zoneNumber - 1;
        if (index >= 0 && index < zoneUnlocked.Length)
        {
            return zoneUnlocked[index];
        }
        return false;
    }

    // Dissolve a zone barrier (called when player enters)
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
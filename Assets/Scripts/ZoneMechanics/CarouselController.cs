using UnityEngine;
using System.Collections;

public class CarouselController : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = -12f;
    public float crankSpeedMultiplier = 1.25f;
    public bool isRunning = true;

    private Transform platform;
    private Transform[] cranks;

    void Start()
    {
        // Auto-find the rotating platform
        platform = transform.Find("SM_Prop_Merry_Go_Round_01_Rotate_01");
        
        if (platform == null)
        {
            Debug.LogError("Couldn't find Rotate_01! Make sure it's a child of this GameObject.");
        }

        // Auto-find all cranks - use coroutine to avoid blocking
        StartCoroutine(FindCranksDelayed());
    }
    
    System.Collections.IEnumerator FindCranksDelayed()
    {
        // Wait a frame to avoid blocking scene load
        yield return null;
        
        // Find only Transform components (not all components)
        Transform[] allTransforms = GetComponentsInChildren<Transform>();
        System.Collections.Generic.List<Transform> crankList = new System.Collections.Generic.List<Transform>();
        
        // Filter to only cranks
        foreach (Transform t in allTransforms)
        {
            if (t.name.Contains("Crank"))
            {
                crankList.Add(t);
            }
        }
        
        cranks = crankList.ToArray();
        Debug.Log($"Carousel initialized with {cranks.Length} cranks");
    }

    void Update()
    {
        if (!isRunning) return;

        // Rotate platform
        if (platform != null)
        {
            platform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }

        // Rotate cranks (only objects with "Crank" in name)
        foreach (Transform t in cranks)
        {
            if (t.name.Contains("Crank"))
            {
                t.Rotate(Vector3.forward * (rotationSpeed * crankSpeedMultiplier) * Time.deltaTime * 10);
            }
        }
    }

    public void StopCarousel()
    {
        isRunning = false;
    }

    public void StartCarousel()
    {
        isRunning = true;
    }
}
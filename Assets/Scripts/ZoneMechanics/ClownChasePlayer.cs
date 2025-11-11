using UnityEngine;
using System.Collections;

public class ClownChasePlayer : MonoBehaviour
{
    [Header("Chase Settings")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    public float chaseRange = 20f;
    public float stopDistance = 2f;
    
    private Transform player;
    private Animator animator;
    private CharacterController controller;

    void Start()
    {
        // Find player - use coroutine to avoid blocking scene load
        StartCoroutine(FindPlayerDelayed());
        
        animator = GetComponent<Animator>();
        
        // Add character controller if missing
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = new Vector3(0, 1, 0);
        }
    }
    
    System.Collections.IEnumerator FindPlayerDelayed()
    {
        // Wait for scene to fully load
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);
        
        // Try to find player
        GameObject xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin != null)
        {
            player = xrOrigin.transform;
        }
        else
        {
            // Try alternative: find by tag
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.position);
        
        // Only chase if in range
        if (distance < chaseRange && distance > stopDistance)
        {
            // Move toward player
            Vector3 direction = (player.position - transform.position).normalized;
            controller.Move(direction * moveSpeed * Time.deltaTime);
            
            // Rotate to face player
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        // Apply gravity
        controller.Move(Vector3.down * 9.8f * Time.deltaTime);
    }
}
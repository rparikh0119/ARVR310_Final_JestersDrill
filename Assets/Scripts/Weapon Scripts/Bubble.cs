using UnityEngine;

public class Bubble : MonoBehaviour
{
    [Header("Bubble Settings")]
    public float floatSpeed = 2f;
    public float lifetime = 5f;
    
    private GameObject trappedObject;
    
    void Start()
    {
        transform.localScale = Vector3.one * 2f;
        Destroy(gameObject, lifetime);
    }
    
    void Update()
    {
        // Float upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
        
        // Move trapped object with bubble
        if (trappedObject != null)
        {
            trappedObject.transform.position = transform.position;
        }
    }
    
    public void TrapObject(GameObject obj)
    {
        trappedObject = obj;
        
        // Disable physics
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        
        // Parent to bubble
        obj.transform.SetParent(transform);
    }
    
    void OnDestroy()
    {
        // Destroy trapped object when bubble pops
        if (trappedObject != null)
        {
            Destroy(trappedObject);
        }
    }
}
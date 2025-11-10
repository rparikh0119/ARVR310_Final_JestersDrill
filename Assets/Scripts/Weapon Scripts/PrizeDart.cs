using UnityEngine;

public class PrizeDart : MonoBehaviour
{
    [Header("Dart Settings")]
    public float pinDuration = 3f;
    public LayerMask hitLayers;
    
    [Header("Visual")]
    public GameObject pinEffect;
    public TrailRenderer trail;
    
    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip pinSound;
    
    private bool hasHit = false;
    
    void Start()
    {
        Destroy(gameObject, 5f);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        
        if (((1 << other.gameObject.layer) & hitLayers) != 0)
        {
            hasHit = true;
            
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position);
            }
            
            // Check if this is an enemy (has Enemy layer)
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                PinEnemy(other.gameObject);
            }
            else
            {
                StickToSurface(other);
            }
        }
    }
    
    void PinEnemy(GameObject enemyObject)
    {
        if (pinSound != null)
        {
            AudioSource.PlayClipAtPoint(pinSound, transform.position);
        }
        
        if (pinEffect != null)
        {
            GameObject effect = Instantiate(pinEffect, transform.position, Quaternion.identity);
            effect.transform.SetParent(enemyObject.transform);
            Destroy(effect, pinDuration);
        }
        
        transform.SetParent(enemyObject.transform);
        
        Rigidbody rb = enemyObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }
        
        Renderer[] renderers = enemyObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                mat.color = Color.Lerp(mat.color, Color.grey, 0.5f);
            }
        }
        
        StartCoroutine(EliminateAfterDelay(enemyObject));
        
        Debug.Log($"Prize Dart: Pinned {enemyObject.name}!");
    }
    
    System.Collections.IEnumerator EliminateAfterDelay(GameObject enemyObject)
    {
        yield return new WaitForSeconds(pinDuration);
        
        if (enemyObject != null)
        {
            Destroy(enemyObject);
        }
        
        Destroy(gameObject);
    }
    
    void StickToSurface(Collider surface)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }
        
        if (trail != null)
        {
            trail.emitting = false;
        }
        
        transform.SetParent(surface.transform);
    }
}
using UnityEngine;

public class Destructible : MonoBehaviour
{
    private int hitCount = 0;        // Track how many times the object is hit
    public int maxHits = 3;          // Destroy after this many hits
    public GameObject destructionEffect; // Optional: Particle effect on destruction

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collider is a Projectile
        if (collision.gameObject.GetComponent<Projectile>() != null)
        {
            hitCount++;  // Increment hit counter

            if (hitCount >= maxHits)
            {
                DestroySlab();
            }
        }
    }

    void DestroySlab()
    {
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);  // Destroy the slab or wall
    }
}

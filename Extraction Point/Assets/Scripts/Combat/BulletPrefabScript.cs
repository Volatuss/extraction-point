using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPrefabScript : MonoBehaviour
{
    
    [SerializeField] private float projectileLifetime; //time in seconds for projectile to exist.
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, projectileLifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Obstacle")){
            Destroy(gameObject);
        }else if(collision.CompareTag("Enemy") || collision.CompareTag("Player")){
            BloodParticleSystemHandler.Instance.SpawnGore(transform.position, rb.velocity.normalized);
            Destroy(gameObject);
        }
    }   

    

}

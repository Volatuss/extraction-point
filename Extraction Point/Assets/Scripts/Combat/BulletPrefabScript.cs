using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPrefabScript : MonoBehaviour
{
    
    [SerializeField] public float projectileLifetime, damageMin, damageMax, speed; //time in seconds for projectile to exist.
    Rigidbody2D rb;
    AudioSource hitSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, projectileLifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Obstacle")){
            Destroy(gameObject);
        }else if(collision.CompareTag("Enemy")){
            if(PlayerPrefs.GetInt("showBlood", 1) == 1){
                BloodParticleSystemHandler.Instance.SpawnGore(transform.position, rb.velocity.normalized);
            }
            collision.GetComponent<EnemyAI>().currenthealth -= Random.Range(damageMin, damageMax);
            Destroy(gameObject);
        }else if(collision.CompareTag("HitBox")){
            if(PlayerPrefs.GetInt("showBlood", 1) == 1){
                BloodParticleSystemHandler.Instance.SpawnGore(transform.position, rb.velocity.normalized);
            }
            hitSource = collision.gameObject.GetComponent<AudioSource>();
            hitSource.pitch = Random.Range(-.1f, .2f) + 1f;
            hitSource.Play();
            PlayerHealth.DoDamage(Random.Range(damageMin, damageMax));
            Destroy(gameObject);
        }
    }   

    

}

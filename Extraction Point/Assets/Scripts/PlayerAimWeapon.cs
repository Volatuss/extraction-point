using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimWeapon : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float bulletForce = 20f;
    [SerializeField] private GameObject aimTransform;
    [SerializeField] ParticleSystem barrelSmoke; //also add muzzle flash and casingEjection
    [SerializeField] private Transform aimGunEndPointTransform;
    [SerializeField] private Transform shellEjectPosition;
    [SerializeField] private MeshParticleSystem meshParticleSystem;
    Vector3 shellMoveDir;

    private bool canShoot = true;
    private Animator aimAnimator;

    private void Awake() {
        //aimAnimator = aimTransform.GetComponent<Animator>();
    }

    private void Update() {
       HandleAiming();

       if(Input.GetMouseButtonDown(0) && canShoot){
        HandleShooting();
       }
    }


    private void HandleAiming()
    {
        float angle = GetAngleOfAim();

        //flips aimTransform at the left/right breakpoints on screen
        Vector3 aimLocalScale = Vector3.one;
        if (angle > 90 || angle < -90)
        {
            aimLocalScale.y = -1f;
        }
        else
        {
            aimLocalScale.y = +1f;
        }
        aimTransform.transform.localScale = aimLocalScale;

    }

    private float GetAngleOfAim()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        aimTransform.transform.eulerAngles = new Vector3(0, 0, angle);
        return angle;
    }

    public void HandleShooting(){

        //aimAnimator.SetTrigger("shoot"); where we call the anim for muzzle flash 
        
        GameObject bullet = Instantiate(bulletPrefab, aimGunEndPointTransform.position, aimGunEndPointTransform.rotation );
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(aimGunEndPointTransform.right * bulletForce, ForceMode2D.Impulse);
        barrelSmoke.Play();     
        //float rotation = -90f;
        Vector3 quadPosition = shellEjectPosition.position;
        Vector3 quadSize =  new Vector3(.1f,.1f);
        
        Vector3 aimDirection = (aimTransform.transform.position - shellEjectPosition.position).normalized;
     
        Vector3 shellDir = ApplyRotationToVector(aimDirection, UnityEngine.Random.Range(-100f, -80f));
        

        float angle = GetAngleOfAim();

        if (angle > 90 || angle < -90)
        {
            ShellParticleSystemHandler.Instance.SpawnShell(quadPosition, -1f * shellDir);
        }
        else
        {
            ShellParticleSystemHandler.Instance.SpawnShell(quadPosition, shellDir);
        }
        

        
        
    }

    public static Vector3 ApplyRotationToVector(Vector3 vec, float angle) {
        return Quaternion.Euler(0, 0, angle) * vec;
    }


    
}

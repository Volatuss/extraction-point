using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random=UnityEngine.Random;

public class PlayerWeaponHandler : MonoBehaviour
{
    
    [SerializeField] TextMeshProUGUI ammoCounter;
    [SerializeField] GameObject equippedWeapon;
    [SerializeField] float bulletForce = 20f, RPM; //later on this will fetch the rpm from a given weapon scriptable object
    [SerializeField] private GameObject bulletPrefab, aimTransform;
    [SerializeField] ParticleSystem barrelSmoke; //also add muzzle flash and casingEjection
    [SerializeField] private Transform aimGunEndPointTransform, shellEjectPosition;
    [SerializeField] private int magSize = 30; //later this will fetch mag size from scriptable obj
    private int remainingAmmo = 0, consecShot, currSlotEquipped = 0; //currslot - 1 = primary, 2 = secondary
    private float lastFired = 0.0f;
    private Vector3 shellMoveDir;
    public bool canShoot = true, isFiring = false, isAiming = false, weaponEquipped = true;
    private Animator aimAnimator;
    private SpriteRenderer weapSR;
    

    private void Awake() {
        //aimAnimator = aimTransform.GetComponent<Animator>();
        consecShot = 0;
        UpdateAmmoCounter();
        aimTransform.SetActive(false);
        weaponEquipped = false;
        ammoCounter.gameObject.SetActive(false);
        weapSR = equippedWeapon.GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if(PlayerHealth.isPlayerDead){return; }

        if(Input.GetKeyDown(KeyCode.Alpha1) && EquipmentData.primaryData != null){
            
            if(currSlotEquipped == 1){
                UnequipWeapon();
            }else {
                EquipPrimary();
            }
        }else if(Input.GetKeyDown(KeyCode.Alpha2) && EquipmentData.secondaryData != null){
            if(currSlotEquipped == 2){
                UnequipWeapon();
            }else {
                EquipSecondary();
            }
        }
        
        if(!weaponEquipped){return; }
        HandleAiming();
        if(Input.GetKeyDown(KeyCode.R) && remainingAmmo < magSize){
            canShoot = false;
            StartCoroutine(ReloadRoutine());
        }

        if(Input.GetMouseButtonDown(0) && canShoot && (Time.time - lastFired) >= 60f/RPM){     
            isFiring = true;              
            StartCoroutine(FireAutomatic());
        }
       
        if(Input.GetMouseButtonUp(0)){
            isFiring = false;
            consecShot = 0;
            //Debug.Log("Released " + consecShot);
        }

        if(Input.GetMouseButtonDown(1)){ //aiming
            isAiming = !isAiming;
        }

        if(Input.GetMouseButtonUp(1)){ //stop aiming
            isAiming = !isAiming;
        }

    }

    IEnumerator FireAutomatic(){
        while(isFiring && consecShot < magSize && remainingAmmo >= 1){
            remainingAmmo--;
            UpdateAmmoCounter();
            lastFired = Time.time;
            HandleShooting(consecShot);
            consecShot++;
            //Debug.Log("Held Down " + consecShot);
            yield return new WaitForSeconds(60f/RPM);
            yield return null;
        }
    }

    IEnumerator ReloadRoutine(){
        yield return new WaitForSeconds(2f);
        canShoot = true;
        remainingAmmo = magSize;
        UpdateAmmoCounter();
    }

    private void EquipPrimary(){
        //handling the technical side of the swap
        aimTransform.SetActive(true);
        currSlotEquipped = 1;
        
        RPM = EquipmentData.GetRPM(1);
        magSize = EquipmentData.GetMagSize(1);
        weapSR.sprite = EquipmentData.GetEquipSprite(1);
        ammoCounter.gameObject.SetActive(true);
        UpdateTransforms(1);
        remainingAmmo = magSize;
        UpdateAmmoCounter();

        weaponEquipped = true;
    }

    
    private void EquipSecondary(){
        //handling the technical side of the swap
        aimTransform.SetActive(true);
        currSlotEquipped = 2;
        RPM = EquipmentData.GetRPM(2);
        magSize = EquipmentData.GetMagSize(2);
        weapSR.sprite = EquipmentData.GetEquipSprite(2);
        ammoCounter.gameObject.SetActive(true);
        UpdateTransforms(2);
        remainingAmmo = magSize;
        UpdateAmmoCounter();



        weaponEquipped = true;
    }

    private void UpdateTransforms(int slot)
    {
        barrelSmoke.transform.localPosition = EquipmentData.GetBarrelPos(slot);
        aimGunEndPointTransform.localPosition = EquipmentData.GetBarrelPos(slot);
        shellEjectPosition.localPosition = EquipmentData.GetEjectPos(slot);
    }

    private void UnequipWeapon(){
        //g
        aimTransform.SetActive(false);
        currSlotEquipped = 0;
        remainingAmmo = 0;
        RPM = 0;
        magSize = 0;
        ammoCounter.gameObject.SetActive(false);



        weaponEquipped = false;
    }

    private void UpdateAmmoCounter(){
        if(remainingAmmo != 0){
            ammoCounter.SetText(remainingAmmo + "/" + magSize);
        }else{
            ammoCounter.SetText("--/" + magSize);
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

    public void HandleShooting(int consecutiveShot){

        //aimAnimator.SetTrigger("shoot"); where we call the anim for muzzle flash 
        
        
        GameObject bullet = Instantiate(bulletPrefab, aimGunEndPointTransform.position, aimGunEndPointTransform.rotation );
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        
        if(isAiming){
            rb.AddForce(ApplyRotationToVector(
                aimGunEndPointTransform.right, 
                Random.Range(-consecShot/4, consecShot/4)) * bulletForce, 
                ForceMode2D.Impulse
                );
        }else{
            rb.AddForce(ApplyRotationToVector(
                aimGunEndPointTransform.right,
                Random.Range(-consecShot, consecShot)) * bulletForce, 
                ForceMode2D.Impulse
                );
        }
        barrelSmoke.Play();     
        StartCoroutine(ScreenShake(.1f, .1f));
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

    IEnumerator ScreenShake(float duration, float magnitude){ //screen shake for firing
        Vector3 lastCameraMovement = Vector3.zero;
        float elapsed = 0.0f;
        while(elapsed < duration){
            Vector3 randomMovement = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * magnitude;
            Camera.main.transform.position = Camera.main.transform.position - lastCameraMovement + randomMovement;
            lastCameraMovement = randomMovement;
            elapsed += Time.deltaTime;
            yield return null;
        }
        

    }
    
}
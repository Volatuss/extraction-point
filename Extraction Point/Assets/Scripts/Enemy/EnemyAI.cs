using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
   //enemies should retreat if they are too far from their ally or if they are shot
    [SerializeField] private float moveSpeed, patrolRange, alertedRange, patrolFOV, alertedFOV, engageRange, fireRate;
    [SerializeField] private Transform castPoint, firePoint, aimTransform;
    [SerializeField] private GameObject bulletPrefab, body;
    [SerializeField] private WeaponScriptable weapon;
    private Vector3 patrolPosition, originPosition, target, lookingDirection, lastSeenPos;
    public float currentRange, currentAngle, currentFOV, patrolDistance = 3, bulletForce, currenthealth = 100f;
    public GameObject player;
    private List<float> FOVAngles = new List<float>();
    private List<RaycastHit2D> HitList = new List<RaycastHit2D>();
    private bool isPatrolling = true, isAlerted = false, isDead = false, isActive = false, areaChecked = false, isMoving = false;
    private int seenCount = 0;

    private void Start() {
        bulletForce = 10; //need to get from the equipmentData probbaly
        fireRate = weapon.rpm; //need to get from the equipmentData probbaly
        originPosition = transform.position;
        player = MapGenerator.playerTransform.gameObject;
        patrolPosition.x = Mathf.Clamp(patrolPosition.x, -patrolDistance, patrolDistance);
        patrolPosition.y = Mathf.Clamp(patrolPosition.y, -patrolDistance, patrolDistance);
        currentRange = patrolRange;
        currentFOV = patrolFOV;
        if(player == null){
            Debug.Log("NULL PLAYER");
        }else{
            StartCoroutine(CheckForPlayer());
            StartCoroutine(EnemyPatrol());
        }
    }


    private void LateUpdate() 
    {
        if(currenthealth < 0){
            isDead = true;
            GameObject deadBody = Instantiate(body, transform.position, Quaternion.Euler(0, 0, -90));
            Destroy(gameObject);
        }
        if(isDead || !isActive || player == null){ return; }
        HandleAiming();
        DisplayFOV(currentAngle);
        if((target.x != transform.position.x) && (target.y != transform.position.y)){
            transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            isMoving = true;
        }else{
            isMoving = false;
        }
    }

    private bool CheckIfValidPosition(){
       
        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, target, patrolDistance);
        if(raycastHit2D.collider == null){
            //noHit, if no hit then target pos is good, assign the target looking poisition
            lookingDirection = (target - transform.position).normalized;
            currentAngle = FieldOfView.GetAngleFromVectorFloat(lookingDirection);
            return true;
        }else{
            //hit, need to make the enemy only walk to near the collision
            
            target = transform.position;
            return false;
        }
    }

    private void DisplayFOV(float centerAngle)
    {
        
        FOVAngles.Clear();
        int raycount = 8;
        float angleIncrement = currentFOV / raycount;
        float startAngle = currentAngle - currentFOV / 2; 
        for(int i = 0; i < raycount; i++)
        {
            FOVAngles.Add(startAngle + angleIncrement*i);
        }
        
        //testing purpooses
        // foreach(float angle in FOVAngles){
        //     Debug.DrawRay(transform.position, FieldOfView.GetVectorFromAngle(angle) * currentRange, Color.green, .5f);
        // }
    }

    private void DrawRaycasts(float distance){
        HitList.Clear();
        foreach(float angle in FOVAngles){
            Vector3 dir = FieldOfView.GetVectorFromAngle(angle);
            Vector2 endPos = castPoint.position + dir * distance;
            HitList.Add(Physics2D.Linecast(castPoint.position, endPos));
        }
    }

    private bool CanSeePlayer( float distance )
    {  
        
        DrawRaycasts(distance);
        foreach(RaycastHit2D hit in HitList){
            if(hit.collider != null && !hit.collider.gameObject.CompareTag("Obstacle") )//Hit
            {
                if(hit.collider.gameObject.CompareTag("HitBox")) //Hit players hitbox, begin aggro
                {
                    lastSeenPos = hit.transform.position;
                    return true;
                } 
            }
        }
        return false;
    }
    
    //error is either here
    IEnumerator CheckForPlayer()
    {
        while(true)
        {
            
            if(isDead || player.transform == null){ break;}
            
            if(Vector3.Distance(player.transform.position, transform.position) > 45){
                //do nothing, essentially deactivate the AI until player is in the range
                isActive = false;
            }else{
                DisplayFOV(currentAngle);
                isActive = true;
                if(CanSeePlayer(currentRange))
                {
                    if(seenCount <= 5)
                    {
                        if(isAlerted){
                            seenCount = 6;
                        }else{
                            seenCount++;
                        }
                    }else if (!isAlerted){
                        
                        isAlerted = true;
                        isPatrolling = false;
                        StartCoroutine(AggroTimer());
                        StartCoroutine(EnemyAlerted());
                    }
                }    
            }
            yield return new WaitForSeconds(.5f);
        }
        yield return null;
    }


    IEnumerator AggroTimer(){
        while(true){
            
            if(seenCount == 0)
            { //enemy should detransition back to patrolling and return to patrol position
                isPatrolling = true;
                isAlerted = false;
                StartCoroutine(EnemyPatrol());
                break;
            }else{
                if(CanSeePlayer(currentRange))
                {

                }else{ //if los is broken, run to th last seen position, look around, and then return if nothing is found

                    if(seenCount > 1){
                        yield return new WaitForSeconds(UnityEngine.Random.Range(0.0f, 3f));
                        seenCount = 1;
                    }else{ //seen count == 1, we will go to the area and then 
                        StartCoroutine(CheckAreaOut(lastSeenPos));
                        while(!areaChecked){
                            yield return null;
                        }
                        
                    }
                } 
            }
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    IEnumerator CheckAreaOut(Vector3 pos){
       
        target = pos;
        while(isMoving == true){
            yield return null;
        } //once stopped moving, need to look around a bit
        // StartCoroutine(LookAround());
        while(true){
            if(isDead){ break; }
            for(int i = 0; i< 4; i++){
                currentAngle = FieldOfView.GetAngleFromVectorFloat(lookingDirection) + UnityEngine.Random.Range(-100, 100);
                if(CanSeePlayer(currentRange)){
                    seenCount = 6;
                    break;
                }else{

                }
                yield return new WaitForSeconds(1.5f);
            }
            if(seenCount > 1) //player was seen again, need to stop the de aggro process
            {
                areaChecked = !areaChecked;
                break;
            }else{
                target = originPosition;
                while(isMoving){
                    yield return null;
                }
                seenCount = 0;
                areaChecked = !areaChecked;
                break;
            }
        }   
    }

    IEnumerator EnemyPatrol() //or here
    {
        currentRange = patrolRange;
        currentFOV = patrolFOV;
        while(true)
        {
            
            if(!isPatrolling || isDead)
            {
                break;
            }

            if(isActive)
            {
                if(seenCount > 0){
                    yield return new WaitForSeconds(.25f);
                }else{
                    target = new Vector2(
                        originPosition.x + UnityEngine.Random.Range(-3, 3), 
                        originPosition.y + UnityEngine.Random.Range(-3, 3)
                    );
                    if(CheckIfValidPosition()){
                        yield return new WaitForSeconds(5f); 
                    }else{
                        yield return new WaitForSeconds(1f);
                    }
                }
            }else{
                yield return new WaitForSeconds(1f);
            }
        }
        yield return null;
    }

    IEnumerator EnemyAlerted()
    {
        
        currentRange = alertedRange;
        currentFOV = alertedFOV;
        StartCoroutine(HandleShooting());
        while (true)
        {
           
            if(!isAlerted || isDead)
            {
                break;
            }
           
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }

    IEnumerator HandleShooting(){
        while(isActive && isAlerted){
       
            if(CanSeePlayer(currentRange)){
                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation );
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                rb.AddForce(PlayerWeaponHandler.ApplyRotationToVector(
                    firePoint.right,
                    1 + UnityEngine.Random.Range(-10, 10)) * bullet.GetComponent<BulletPrefabScript>().speed, 
                    ForceMode2D.Impulse
                );
            }else{

            }
            yield return new WaitForSeconds(60/fireRate);
        }
        yield return null;
    }

    //need function to make enemy shoot properly

    private void HandleAiming()
    {
        
        //flips aimTransform at the left/right breakpoints on screen
        if(isActive && isAlerted){
            lookingDirection = (player.transform.position - transform.position).normalized;
            currentAngle = FieldOfView.GetAngleFromVectorFloat(lookingDirection);
        }
        Vector3 aimLocalScale = Vector3.one;
        aimTransform.eulerAngles = new Vector3(0, 0, currentAngle);
        if (currentAngle > 90 || currentAngle < -90)
        {
            aimLocalScale.y = -1f;
        }
        else
        {
            aimLocalScale.y = +1f;
        }
        aimTransform.transform.localScale = aimLocalScale;
    }

    
}

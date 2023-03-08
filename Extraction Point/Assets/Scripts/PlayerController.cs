using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject lootToolTip;
    [SerializeField] GameObject InventoryInterface;
    [SerializeField] private FieldOfView fieldOfView;
    private bool aimDownSights = false;
    public static bool isObjectLootable = false;
    

    void Update()
    {
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDir = (targetPosition - transform.position).normalized;
        fieldOfView.SetAimDirection(aimDir);
        fieldOfView.SetOrigin(transform.position);


        if(Input.GetKeyDown(KeyCode.F) && isObjectLootable){
            InventoryInterface.SetActive(true);
            InventoryController.isInvOpen = true;
        }    

        if(Input.GetMouseButtonDown(1)){
            aimDownSights = !aimDownSights;
            if(aimDownSights){
                fieldOfView.SetFoV(45f);
            }
            
        }else if((Input.GetMouseButtonUp(1))){
            aimDownSights = !aimDownSights;
            if(!aimDownSights){
                
                fieldOfView.SetFoV(120f);
            
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Container")){
            lootToolTip.SetActive(true);
            isObjectLootable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Container")){
            lootToolTip.SetActive(false);
            isObjectLootable = false;
        }
    }
}

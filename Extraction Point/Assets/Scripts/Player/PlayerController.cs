using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject lootToolTip, npcToolTip, InventoryInterface, questInterface;
    
    [SerializeField] private FieldOfView fieldOfView;

    private List<GameObject> openToolTips = new List<GameObject>();
    
    public static bool isObjectLootable = false, isNPCLabel = false, isQuestOpen = false;

    private int currentInteractionPrompts = 0;
    

    void Update()
    {

        /*
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 aimDir = (targetPosition - transform.position).normalized;
        fieldOfView.SetAimDirection(aimDir);*/
        fieldOfView.SetOrigin(transform.position);


        if(Input.GetKeyDown(KeyCode.F) && isObjectLootable){
            InventoryInterface.SetActive(true);
            InventoryController.isInvOpen = true;
        }else if(Input.GetKeyDown(KeyCode.F) && isNPCLabel){
            
        }    

        /*
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
        }*/
        if(Input.GetKeyDown(KeyCode.C)){
            questInterface.SetActive(!isQuestOpen);
            Cursor.visible = !isQuestOpen;
            isQuestOpen = !isQuestOpen;
        }

        
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Container")){
            lootToolTip.SetActive(true);
            isObjectLootable = true;
        }else if(other.CompareTag("FriendlyNPC")){
            npcToolTip.SetActive(true);
            isNPCLabel = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.CompareTag("Container")){
            lootToolTip.SetActive(false);
            isObjectLootable = false;
        }else if(other.CompareTag("FriendlyNPC")){
            npcToolTip.SetActive(false);
            isNPCLabel = false;
        }
        
    }

    private void CreateNewLootTip(){
        
    }

    private void CreateNewNPCTip(){

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject lootToolTip, npcToolTip, InventoryInterface, questInterface;
    
    [SerializeField] private FieldOfView fieldOfView;

    private List<GameObject> openToolTips = new List<GameObject>();
    
    public static bool isObjectLootable = false, isNPCLabel = false, isQuestOpen = false, isPod = false;
    public static GameObject container;

    void Update()
    {
        fieldOfView.SetOrigin(transform.position);
        if(GameHandler.isUIOpen){ return; }
        
    }

    
    private void CreateNewLootTip(){
        
    }

    private void CreateNewNPCTip(){

    }
}

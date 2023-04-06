using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootableController : MonoBehaviour
{
    public string containerName { get; set; }
    public InventoryController inventoryController { get; set; }
    public Dictionary<Vector2, ItemData> itemsInContainer { get; set; }
    public bool isOpen = false;
    public InteractToolTips tooltipController;
    
    

    private void Update() {
        if(!inventoryController.inventoryInterface.activeSelf && isOpen){ 
            StartCoroutine(CloseContainer());
        }
    }

    public void OpenContainer(){
        if(!isOpen){
            inventoryController.OpenContainerID = gameObject.GetComponentInChildren<Interactable>().index;
            inventoryController.currentController = this;
            isOpen = true;
            inventoryController.LoadLootFromObject(itemsInContainer, containerName);
        }
    }

    IEnumerator CloseContainer(){
        inventoryController.OpenContainerID = 0;
        
        isOpen = false;
        if(itemsInContainer.Count == 0){
            tooltipController = GameObject.Find("InteractOptions").GetComponent<InteractToolTips>();
            StartCoroutine(tooltipController.RemoveAction(gameObject.GetComponentInChildren<Interactable>().index));
            yield return null;
            Destroy(gameObject);
        }
        yield return null;
    }

    

}

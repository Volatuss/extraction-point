using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemGrid))] //due to needing the grid
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //assigns selected grid when hovering over
    InventoryController inventoryController;
    ItemGrid itemGrid;
    private void Awake() {
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController; //inefficient, fix this eventually
        itemGrid = GetComponent<ItemGrid>();
    }

    public void OnPointerEnter(PointerEventData eventData){
        inventoryController.SelectedItemGrid = itemGrid;
        Debug.Log("Pointer Enter");
        Debug.Log(itemGrid.inventoryType);
    }
    public void OnPointerExit(PointerEventData eventData){
        inventoryController.SelectedItemGrid = null;
        Debug.Log("Pointer Exit");
        
    }

}

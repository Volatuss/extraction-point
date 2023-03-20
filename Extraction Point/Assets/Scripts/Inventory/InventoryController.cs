using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryController : MonoBehaviour
{

    [HideInInspector] 
    private ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid{ 
        get=> selectedItemGrid;
        set {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    } 
    [SerializeField] ItemGrid InventoryGrid;  
    [SerializeField] ItemGrid LootGrid;  
    [SerializeField] GameObject inventoryInterface;
    InventoryItem selectedItem;
    InventoryItem overlapItem;
    InventoryItem itemToHighlight;
    RectTransform rectTransform;
    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;
    [SerializeField] TextMeshProUGUI totalWeightText;
    [SerializeField] TextMeshProUGUI lootLabel;
    [SerializeField] GameObject rightClickContext;
    public float totalWeight = 0.0f; //wip
    InventoryItem[] itemsToInsert;
    public static bool isInvOpen = false;

    InventoryHighlight inventoryHighlight;
    Vector2 oldPositon;

    private void Awake() {
        inventoryHighlight = GetComponent<InventoryHighlight>();
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I)){
            if(isInvOpen){
                
            }
            inventoryInterface.SetActive(!isInvOpen);
            isInvOpen = !isInvOpen;
        }   
        if(isInvOpen == false){return; }
        ItemIconDrag();
        if(Input.GetKeyDown(KeyCode.Q)){
            if(selectedItem == null){
                CreateRandomItem();
            }
        }

        if(Input.GetKeyDown(KeyCode.W) && isInvOpen){
            InsertRandomItem();
        }
        
        if(Input.GetKeyDown(KeyCode.R)){
            RotateItem();
            //Debug.Log(selectedItemGrid.inventoryType);
        }

        if (selectedItemGrid == null) { 
            inventoryHighlight.Show(false);
            return; 
        } //only execute actions when grid is selected, ie. the mouse is over a grid

        HandleHighlight();
 
        if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftControl) )
        {
            QuickTransferItem();     
        }else if (Input.GetMouseButtonDown(0)){
            LeftMouseButtonPress();
        }
    }

    

    public void OpenInventoryInterface(){
        inventoryInterface.SetActive(!isInvOpen);
        isInvOpen = !isInvOpen;
    }

    private void QuickTransferItem() //working for now, if controlClick on item
    {
        if(selectedItemGrid.CompareTag("LootInv")){ //from loot to inventory
            Debug.Log("ToInv");
            toInventory();
        }else if(selectedItemGrid.CompareTag("Inventory")){ //from inventory to loot
            Debug.Log("FromInv");
            fromInventory();
        }
    }

    private void toInventory() //to make containers, remove an item from the containers item list, this is also where I can generate the items??
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        InventoryItem itemToTransfer = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);;
        if(itemToTransfer == null){return;}
        Vector2Int? posOnGrid = InventoryGrid.FindSpaceForObject(itemToTransfer);
        if(posOnGrid == null){return;}
        LootGrid.CleanGridReference(itemToTransfer);
        InventoryGrid.PlaceItem(itemToTransfer, posOnGrid.Value.x, posOnGrid.Value.y);
    }
    private void fromInventory()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        InventoryItem itemToTransfer = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);;
        if(itemToTransfer == null){return;}
        Vector2Int? posOnGrid = LootGrid.FindSpaceForObject(itemToTransfer);
        if(posOnGrid == null){return;}
        InventoryGrid.CleanGridReference(itemToTransfer);
        LootGrid.PlaceItem(itemToTransfer, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void RotateItem()
    {
        if(selectedItem == null){return;}
        selectedItem.Rotate();
    }

    private void InsertRandomItem()
    {
        if(selectedItemGrid == null){return; }
        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);
        if(posOnGrid == null){return;}
        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        
    }

    private void HandleHighlight()
    {   
        if(!isInvOpen){return;}
        Vector2Int positionOnGrid = GetTileGridPosition();
        if(oldPositon == positionOnGrid){return;}
        oldPositon = positionOnGrid;
        
        if(selectedItem == null){
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
            if(itemToHighlight != null){
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                //inventoryHighlight.SetParent(selectedItemGrid);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }else{
                inventoryHighlight.Show(false);
            }
        }else{
            inventoryHighlight.Show(selectedItemGrid.BoundryCheck(
                positionOnGrid.x, 
                positionOnGrid.y, 
                selectedItem.WIDTH, 
                selectedItem.HEIGHT)
                );

            inventoryHighlight.SetSize(selectedItem);
            //inventoryHighlight.SetParent(selectedItemGrid);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }

    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();

        if (selectedItem == null)
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            PlaceItem(tileGridPosition);
        }
    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;
        if (selectedItem != null)
        {
            position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        }
        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
        if(complete){
            selectedItem = null;
            if(overlapItem != null){
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }
    }

    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }

    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;
        }
    }

    public void LoadLoot() //when a container is opened need to process the list of items from the container object and place the items in the container, we also need to clear this once we close the container
    {

    }
}
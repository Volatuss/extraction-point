using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class InventoryController : MonoBehaviour
{

    [HideInInspector] 
    private ItemGrid selectedItemGrid, GridContainingInteract = null;
    public ItemGrid SelectedItemGrid{ 
        get=> selectedItemGrid;
        set {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    } 
    [SerializeField] ItemGrid InventoryGrid, LootGrid, PrimaryGrid, SecondaryGrid;  
    [SerializeField] public GameObject inventoryInterface, itemPrefab, groundContainerPrefab, itemRightClick, equipableRightClick, EquipBut, unEquipBut, player;
    [SerializeField] List<ItemData> items;
    [SerializeField] Transform canvasTransform;
    [SerializeField] public TextMeshProUGUI totalWeightText, lootLabel;
    [SerializeField] GameHandler gameHandler;
    static List<ItemData> itemDataList;
    public int OpenContainerID;
    public LootableController currentController = null;

    InventoryItem selectedItem, overlapItem, itemToHighlight, itemToEquip, itemToUnequip;
    RectTransform rectTransform, primRect, secRect;
    
    public static Dictionary<Vector2, ItemData> InventoryItems = new Dictionary<Vector2, ItemData>(); //This needs to be saved 
    
    public float totalWeight = 0.0f; //wip
    InventoryItem[] itemsToInsert;
    public static bool isInvOpen = false, isEquipRightClickOpen = false, isItemRightClickOpen = false, primSlotFull = false, secondSlotFull = false;
    InventoryHighlight inventoryHighlight;
    Vector2 oldPositon;


    private void Awake() {
        inventoryHighlight = GetComponent<InventoryHighlight>();
        primRect = PrimaryGrid.GetComponent<RectTransform>(); //giving error here
        secRect = SecondaryGrid.GetComponent<RectTransform>();
        itemDataList = items;
        
    }

    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.I)){
            //Cursor.visible = !isInvOpen;
            //inventoryInterface.SetActive(!isInvOpen);
            //isInvOpen = !isInvOpen;
            selectedItemGrid = null;
        }   
        if(GameHandler.CurrentActiveUI != inventoryInterface){return; }
        ItemIconDrag();
        if(Input.GetKeyDown(KeyCode.Q)){
            if(selectedItem == null){
                CreateRandomItem();
            }
        }

        if(Input.GetKeyDown(KeyCode.W) && GameHandler.CurrentActiveUI == inventoryInterface){
            if(selectedItemGrid == null){ return; }
            InsertRandomItem();
        }
        
        if(Input.GetKeyDown(KeyCode.J) && GameHandler.CurrentActiveUI == inventoryInterface){
            Debug.Log("Display Contents");
            InventoryGrid.DisplayContents();
            foreach(var data in InventoryItems){
                Debug.Log(data);
            }
            
        }
        
        
        if(Input.GetKeyDown(KeyCode.R)){
            RotateItem();
            
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

        if(Input.GetMouseButtonDown(1)){
            
            OpenContextMenu();
        }

        if(isEquipRightClickOpen){

        }else if(isItemRightClickOpen){
            
        }
    }

    public ItemGrid GetInvGrid(){
        return InventoryGrid;
    }


    public void OpenContextMenu(){
        
        if(!GameHandler.CurrentActiveUI == inventoryInterface){ return; }
        Vector2Int positionOnGrid = GetTileGridPosition();
        InventoryItem itemRightClicked = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
        if(itemRightClicked == null){return; } //exit gate
        
        CloseOtherContexts();
        GridContainingInteract = selectedItemGrid;
        
        if(itemRightClicked.itemData.isArmor || itemRightClicked.itemData.isWeapon || itemRightClicked.itemData.isBackpack){ //open menu with drop, equip and transfer options
            itemToEquip = itemRightClicked;
            itemToUnequip = itemRightClicked;
            equipableRightClick.SetActive(!isEquipRightClickOpen);
            equipableRightClick.transform.position = new Vector3(Input.mousePosition.x + 175/2, Input.mousePosition.y - 60);

            if(GridContainingInteract == PrimaryGrid || GridContainingInteract == SecondaryGrid){
                
                unEquipBut.SetActive(true);
                EquipBut.SetActive(false);
            }else{
                
                unEquipBut.SetActive(false);
                EquipBut.SetActive(true);
                
            }
            

        }else{ //open normal context menu with transfer and Drop options possibly add third context menu with split for stackable items
            
            itemRightClick.SetActive(!isItemRightClickOpen);
            itemRightClick.transform.position = new Vector3(Input.mousePosition.x + 175/2, Input.mousePosition.y - 40);
        }

    }

    private void CloseOtherContexts()
    {
        GridContainingInteract = null;
        itemToEquip = null;
        equipableRightClick.SetActive(false);
        itemRightClick.SetActive(false);
    }

    public void EquipItem(){
        if(itemToEquip.rotated){
            itemToEquip.Rotate();
        }
        if(!primSlotFull){ //check if prim slot full
            primSlotFull = true;
            PrimaryGrid.gridSizeWidth = itemToEquip.itemData.width;
            PrimaryGrid.gridSizeHeight = itemToEquip.itemData.height;
            primRect.sizeDelta = new Vector2(itemToEquip.itemData.width * 48, itemToEquip.itemData.height * 48);
            EquipmentData.UpdatePrimary(itemToEquip.itemData);
            InventoryGrid.CleanGridReference(itemToEquip);
            PrimaryGrid.PlaceItem(itemToEquip, 0, 0);

            CloseOtherContexts();

        }else if(primSlotFull && !secondSlotFull){ //check if secondary full if prim is

            secondSlotFull = true;
            SecondaryGrid.gridSizeWidth = itemToEquip.itemData.width;
            SecondaryGrid.gridSizeHeight = itemToEquip.itemData.height;
            secRect.sizeDelta = new Vector2(itemToEquip.itemData.width * 48, itemToEquip.itemData.height * 48);
            EquipmentData.UpdateSecondary(itemToEquip.itemData);
            InventoryGrid.CleanGridReference(itemToEquip);
            SecondaryGrid.PlaceItem(itemToEquip, 0, 0);

            CloseOtherContexts();

        }else if(primSlotFull && secondSlotFull){ //if both slots full then replace primary

            InsertItem(PrimaryGrid.GetItem(0,0), InventoryGrid);
            PrimaryGrid.ClearWholeGrid();
            EquipmentData.UpdatePrimary(itemToEquip.itemData);
            PrimaryGrid.gridSizeWidth = itemToEquip.itemData.width;
            PrimaryGrid.gridSizeHeight = itemToEquip.itemData.height;
            primRect.sizeDelta = new Vector2(itemToEquip.itemData.width * 48, itemToEquip.itemData.height * 48);
            
            InventoryGrid.CleanGridReference(itemToEquip);
            PrimaryGrid.PlaceItem(itemToEquip, 0, 0);
            
            CloseOtherContexts();

        }
    }

    public void UnequipItem()
    {
        if(GridContainingInteract == PrimaryGrid){
            primSlotFull = false;
            EquipmentData.UpdatePrimary(null);
        }else{
            secondSlotFull = false;
            EquipmentData.UpdateSecondary(null);
        }
        InsertItem(itemToUnequip, InventoryGrid);
        GridContainingInteract.ClearWholeGrid();
        
        CloseOtherContexts();
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
        InventoryItem itemToTransfer = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);
        if(itemToTransfer == null){return;}
        Vector2Int? posOnGrid = InventoryGrid.FindSpaceForObject(itemToTransfer);
        if(posOnGrid == null){return;}
        LootGrid.CleanGridReference(itemToTransfer);
        InventoryGrid.PlaceItem(itemToTransfer, posOnGrid.Value.x, posOnGrid.Value.y);
        
    }
    
    private void fromInventory()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();
        InventoryItem itemToTransfer = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

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
        InsertItem(itemToInsert, selectedItemGrid);
    }

    private void InsertItem(InventoryItem itemToInsert, ItemGrid GridToInsert)
    {
        
        Vector2Int? posOnGrid = GridToInsert.FindSpaceForObject(itemToInsert);
        if(posOnGrid == null){return;}
        GridToInsert.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        
        
    }

    

    private void HandleHighlight()
    {   
        if(!GameHandler.CurrentActiveUI == inventoryInterface){return;}
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

    public void LoadSavedLoot(ItemGrid itemGrid, int itemCount, Dictionary<Vector2, int> toLoad)
    { //this currently functions as is
        foreach(var itemToLoad in toLoad)
        {
            InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
            selectedItem = inventoryItem;

            rectTransform = inventoryItem.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasTransform);
            rectTransform.SetAsLastSibling();

            inventoryItem.Set(items[itemToLoad.Value - 1]);
            inventoryItem.name = inventoryItem.Get(items[itemToLoad.Value - 1]).ToString() + " : " + items[itemToLoad.Value - 1].itemName;

            itemGrid.PlaceItem(selectedItem, ((int)itemToLoad.Key.x), ((int)itemToLoad.Key.y));           
        }
        GameHandler.loadedData = true;
        selectedItem = null;
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
        inventoryItem.name = inventoryItem.Get(items[selectedItemID]).ToString() + " : " + items[selectedItemID].itemName;
        
    }

    public void GiveRewards(List<ItemData> rewards){
        foreach(ItemData r_itemData in rewards){
            InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        
            selectedItem = inventoryItem;

            rectTransform = inventoryItem.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasTransform);
            rectTransform.SetAsLastSibling();
            
            inventoryItem.Set(r_itemData);
            inventoryItem.name = inventoryItem.Get(r_itemData).ToString() + " : " + r_itemData.itemName;

            InventoryItem itemToInsert = selectedItem;
            selectedItem = null;
            
            InsertItem(itemToInsert, InventoryGrid);
        }
    }

    private void LeftMouseButtonPress()
    {
        if (selectedItemGrid == null) { return; }
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
        if(selectedItemGrid == PrimaryGrid || selectedItemGrid == SecondaryGrid){return; }
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
        if(selectedItemGrid == PrimaryGrid || selectedItemGrid == SecondaryGrid){return; }
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

    public void InteractWithContainer(GameObject container) //when a container is opened need to process the list of items from the container object and place the items in the container, we also need to clear this once we close the container
    {
        Debug.Log(container);
    }

    public void OnCloseInventory() //upon being disabled, the objects in the ground grid should create a bag on the ground which contains the contents of the loot grid !!!!ONLY IF NO LOOTABLE CONTAINER IS OPEN!!!!
    {
        Debug.Log("Closing container " + OpenContainerID + " with " + LootGrid.itemsInGrid.Count + " items");
        
        if(LootGrid.itemsInGrid.Count > 0 && OpenContainerID == 0){ //if there is an item in the loot grid then make an object on the ground
            StartCoroutine(CreateGroundBag());
        }else if(currentController != null && currentController.itemsInContainer.Count > 0 && OpenContainerID != 0){
            StartCoroutine(NormalClose());
        }
        
        lootLabel.text = "Ground";
    }

    IEnumerator NormalClose(){ //need to compare original list to current list
        
        Dictionary<Vector2, ItemData> itemList = new Dictionary<Vector2, ItemData>();
        foreach(var item in LootGrid.itemsInGrid){
            Debug.Log(item.Value);
            itemList.Add(item.Key, item.Value);
        }
        
        
        currentController.itemsInContainer = itemList;
        
        yield return null;
        LootGrid.ClearLootGrid();
        currentController = null;
        
        yield return null;
    }

    IEnumerator CreateGroundBag(){
        Dictionary<Vector2, ItemData> itemList = new Dictionary<Vector2, ItemData>();
        GameObject groundBag = Instantiate(groundContainerPrefab, new Vector3(player.transform.position.x, player.transform.position.y, 0.0f), Quaternion.identity);
        LootableController bagCont = groundBag.GetComponent<LootableController>();
        bagCont.containerName = "Bag";
        bagCont.inventoryController = this;
        foreach(var item in LootGrid.itemsInGrid){
            itemList.Add(item.Key, item.Value);
        }
        groundBag.GetComponent<LootableController>().itemsInContainer = itemList;
        yield return null;
        LootGrid.ClearLootGrid();
        yield return null;
    }

    IEnumerator LoadTest(Dictionary<Vector2, ItemData> itemsToLoad, string name){
        gameHandler.TryOpenInterface(inventoryInterface);
        yield return null;
        lootLabel.SetText(name);
        foreach(var item in itemsToLoad){
            InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
            selectedItem = inventoryItem;

            rectTransform = inventoryItem.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasTransform);
            rectTransform.SetAsLastSibling();

            inventoryItem.Set(item.Value);
            inventoryItem.name = inventoryItem.Get(item.Value).ToString() + " : " + item.Value.itemName;

            LootGrid.PlaceItem(selectedItem, ((int)item.Key.x), ((int)item.Key.y));
        }
        selectedItem = null;
        yield return null;
    }

    public void LoadLootFromObject(Dictionary<Vector2, ItemData> itemsToLoad, string name){
        StartCoroutine(LoadTest(itemsToLoad, name));
        
    }
}
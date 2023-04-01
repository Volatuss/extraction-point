using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 48;
    public const float tileSizeHeight = 48;
    [SerializeField] public int gridSizeWidth = 10, gridSizeHeight = 14;
    [SerializeField] GameObject inventoryItemPrefab;
    RectTransform rectTransform;
    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();
    public String inventoryType = "Inventory";
    InventoryItem[,] inventoryItemSlot;
    public int itemCount = 0;

    private void Start() 
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
        
    }

    public void DisplayContents(){
        Debug.Log("Start of Display");
        
        String toDisplay = "@";
        for(int x = 0; x < gridSizeHeight; x++){
            for(int y = 0; y < gridSizeWidth; y++){
                //Debug.Log(x+", "+y);
                if(inventoryItemSlot[y,x] == null){
                    toDisplay = toDisplay + "0 ";
                }else{
                    toDisplay = toDisplay + inventoryItemSlot[y,x].itemData.itemID.ToString() + " ";
                }
            }
            toDisplay = toDisplay + "@";
            
        }
        
        toDisplay = toDisplay.Replace("@", "" + System.Environment.NewLine);
        Debug.Log(toDisplay);
        Debug.Log("End of Display");
    }


    public bool ContainsItem(ItemData itemData){
        
        for(int x = 0; x < gridSizeHeight; x++){
            for(int y = 0; y < gridSizeWidth; y++){
                
                    
                if(inventoryItemSlot[y,x].itemData == itemData){
                    return true;
                }
            }
        }
        return false;
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];
        

        if (toReturn == null) { return null; }

        CleanGridReference(toReturn);
        

        return toReturn;
    }

    public void CleanGridReference(InventoryItem item)
    {
        if(transform.name == "InvGrid"){
            InventoryController.InventoryItems.Remove(new Vector2(item.onGridPositionX, item.onGridPositionY));
        }
        for (int ix = 0; ix < item.WIDTH; ix++)
        { //removing whole item
            for (int iy = 0; iy < item.HEIGHT; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
            }
        }
    }

    internal InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlot[x,y];
    }

    public void Init(int width, int height)
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }
    
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        //get the mouse position
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;
        //calculate tile grid position
        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        return tileGridPosition;
    }

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert) //resource heavy, optimize this 
    {
        int height = gridSizeHeight - itemToInsert.HEIGHT + 1; // 1x2 item: width = 10, height = 13
        int width = gridSizeWidth - itemToInsert.WIDTH + 1;
        for(int y = 0; y<height; y++)
        {
            for(int x = 0; x<width; x++)
            {
                if(CheckAvailableSpace(x,y, itemToInsert.HEIGHT, itemToInsert.WIDTH) == true)
                {
                    return new Vector2Int(x,y);
                }
            }
        }
        return null;
    }

    internal void ClearWholeGrid()
    {
        for(int x = 0; x < gridSizeHeight-1; x++){
            for(int y = 0; y< gridSizeWidth-1; y++){
                inventoryItemSlot[x,y] = null;
            }
        }
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        

        if (BoundryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)
        {
            return false;
        }
        if (OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }
        PlaceItem(inventoryItem, posX, posY);
        


        return true;
    }

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();

        if(transform.name == "InvGrid"){
            InventoryController.InventoryItems.Add(new Vector2(posX, posY), inventoryItem.itemData);
        }
        //InventoryItem itemToWeigh = inventoryItem;

        rectTransform.SetParent(this.rectTransform);
        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = position;
        
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();

        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.WIDTH / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.HEIGHT / 2);
        return position;
    }

    public bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem){
        for(int x = 0; x < width; x++){
            for(int y = 0; y < height; y++){
                if((inventoryItemSlot[posX + x, posY + y]) != null){
                   if(overlapItem == null){
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }else{
                        if(overlapItem != inventoryItemSlot[posX + x, posY + y]){
                            return false;               
                        }   
                   }
                }
            }
        }
        return true;
    }

    public bool CheckAvailableSpace(int posX, int posY, int width, int height){ //for inserting item and checking where it can go
        for(int y = 0; y < width; y++){
            for(int x = 0; x < height; x++){
                if((inventoryItemSlot[posX + x, posY + y]) != null){
                    return false;
                }
            }
        }
        
        return true;
    }

    bool PositionCheck(int posX, int posY){ //checks if position is valid for item
        if(posX<0 || posY<0){
            return false;
        }
        if(posX>= gridSizeWidth || posY>=gridSizeHeight){
            return false;
        }

        return true;
    }

    public bool BoundryCheck(int posX, int posY, int width, int height){
        if(PositionCheck(posX, posY) == false){return false;}//check top left
        posX += width - 1;
        posY += height - 1;
        if(PositionCheck(posX, posY) == false){return false;}//check bot right

        return true;
    }

    public bool isLootGrid(){
        if(inventoryType.CompareTo("LootInv") == 0){
            return true;
        }
        return false;
    }
    
}


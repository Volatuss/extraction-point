using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public static float tileSizeWidth = 48 ;
    public static float tileSizeHeight = 48 ;
    [SerializeField] public int gridSizeWidth = 10, gridSizeHeight = 14;
    [SerializeField] GameObject inventoryItemPrefab;
    public List<InventoryItem> inventoryItemsInGrid = new List<InventoryItem>();
    public Dictionary<Vector2, ItemData> itemsInGrid = new Dictionary<Vector2, ItemData>();
    RectTransform rectTransform;
    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();
    public String inventoryType = "Inventory";
    InventoryItem[,] inventoryItemSlot;
    public int itemCount = 0;

    private void Awake() 
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
        
    }

    public void DisplayContents(){ //           ********** TEST FUNCTION, CAN BE REMOVED ***********
        String toDisplay = "@";
        for(int x = 0; x < gridSizeHeight; x++)
        {
            for(int y = 0; y < gridSizeWidth; y++)
            {
                //Debug.Log(x+", "+y);
                if(inventoryItemSlot[y,x] == null)
                {
                    toDisplay = toDisplay + "0 ";
                }else
                {
                    toDisplay = toDisplay + inventoryItemSlot[y,x].itemData.itemID.ToString() + " ";
                }
            }
            toDisplay = toDisplay + "@";
        }
        toDisplay = toDisplay.Replace("@", "" + System.Environment.NewLine);
        Debug.Log(toDisplay);
        Debug.Log("End of Display");
    }

    public Vector2Int? ContainsItem(ItemData itemData)
    {
        for(int x = 0; x < gridSizeHeight; x++)
        {
            for(int y = 0; y < gridSizeWidth; y++)
            {
                if(inventoryItemSlot[y,x].itemData == itemData)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];
        if (toReturn == null) { return null; } 
        CleanGridReference(toReturn);
        itemCount--;
        return toReturn;
    }

    public void CleanGridReference(InventoryItem item)
    {
        if(transform.name == "InvGrid")
        {
            InventoryController.InventoryItems.Remove(new Vector2(item.onGridPositionX, item.onGridPositionY));
        }
        itemsInGrid.Remove(new Vector2(item.onGridPositionX, item.onGridPositionY));
        inventoryItemsInGrid.Remove(item);
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
        tileGridPosition.x = (int)(positionOnTheGrid.x / (tileSizeWidth * (Screen.width / 1920f) ));
        tileGridPosition.y = (int)(positionOnTheGrid.y / (tileSizeHeight * (Screen.height / 1080f) ));

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
                if(CheckAvailableSpace(x,y, itemToInsert.HEIGHT, itemToInsert.WIDTH, itemToInsert) == true)
                {
                    if(inventoryItemSlot[x, y] != null){ //if there is a stackable item here then we need to return null still i think
                        Debug.Log("Stackable");
                        return new Vector2Int(-999, -999);
                    }else{
                        return new Vector2Int(x,y);
                    }
                    
                }
            }
        }
        return null;
    }

    internal void ClearWholeGrid()
    {
        for(int x = 0; x < gridSizeHeight-1; x++){
            for(int y = 0; y < gridSizeWidth-1; y++){
                inventoryItemSlot[y, x] = null;
            }
        }
        itemCount = 0;
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {

        if (BoundryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)
        {
            return false;
        }
        if (OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem, ref inventoryItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            
            if(overlapItem.itemData.itemID == inventoryItem.itemData.itemID  &&  inventoryItem.itemData.maxStackSize > 1)
            {
                if(inventoryItem.currentStackSize + overlapItem.currentStackSize <= inventoryItem.itemData.maxStackSize)
                {
                    inventoryItem.currentStackSize += overlapItem.currentStackSize;
                    inventoryItem.UpdateCounter();
                    AddToStack(overlapItem);
                }else
                {
                    if(inventoryItem.currentStackSize >= overlapItem.currentStackSize)
                    {
                        //dont change anything, simply add what you can to it
                        int remainderItems = inventoryItem.currentStackSize + overlapItem.currentStackSize - inventoryItem.itemData.maxStackSize;
                        inventoryItem.currentStackSize = inventoryItem.itemData.maxStackSize;
                        overlapItem.currentStackSize = remainderItems;
                        inventoryItem.UpdateCounter();
                        overlapItem.UpdateCounter();
                    }
                    CleanGridReference(overlapItem);
                }
            }else
            {
                //either not same item or item is not stackable
                CleanGridReference(overlapItem);
            }
        }  
        PlaceItem(inventoryItem, posX, posY);
        return true;
    }

    public void AddToStack(InventoryItem inventoryItem)
    {
        Destroy(inventoryItem.gameObject);
        
        CleanGridReference(inventoryItem);
        
    }

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();

        if(transform.name == "InvGrid")
        {
            InventoryController.InventoryItems.Add(new Vector2(posX, posY), inventoryItem.itemData);
        }
        itemsInGrid.Add(new Vector2(posX, posY), inventoryItem.itemData);
        inventoryItemsInGrid.Add(inventoryItem);
        //InventoryItem itemToWeigh = inventoryItem;
        rectTransform.SetParent(this.rectTransform);
        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }
        itemCount++;
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

    public bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem, ref InventoryItem itemToPlace){
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                if((inventoryItemSlot[posX + x, posY + y]) != null)
                {
                    if(overlapItem == null){
                        
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }else{
                        if(overlapItem != inventoryItemSlot[posX + x, posY + y])
                        {
                            
                            return false;          
                        }   
                   }
                }
            }
        }
        return true;
    }

    public bool CheckAvailableSpace(int posX, int posY, int width, int height, InventoryItem inventoryItem){ //for inserting item and checking where it can go
        for(int y = 0; y < width; y++)
        {
            for(int x = 0; x < height; x++)
            {
                if((inventoryItemSlot[posX + x, posY + y]) != null)
                {
                    if(inventoryItem.itemData == inventoryItemSlot[posX + x, posY + y].itemData && inventoryItem.itemData.maxStackSize > 1)
                    {
                        //items can be stacked
                        if(inventoryItemSlot[posX + x, posY + y].currentStackSize + inventoryItem.currentStackSize <= inventoryItemSlot[posX + x, posY + y].itemData.maxStackSize)
                        {
                            inventoryItemSlot[posX + x, posY + y].currentStackSize += inventoryItem.currentStackSize;
                            inventoryItemSlot[posX + x, posY + y].UpdateCounter();
                            // Dont insert the item because this can fit in a preexisting stack just fine, destroy the new item and return true to break out.
                            Destroy(inventoryItem.gameObject);
                            return true;
                        }else if(inventoryItemSlot[posX + x, posY + y].currentStackSize >= inventoryItem.currentStackSize)
                        { 
                            int remainderItems = inventoryItemSlot[posX + x, posY + y].currentStackSize + inventoryItem.currentStackSize - inventoryItemSlot[posX + x, posY + y].itemData.maxStackSize;
                            inventoryItemSlot[posX + x, posY + y].currentStackSize = inventoryItemSlot[posX + x, posY + y].itemData.maxStackSize;
                            inventoryItem.currentStackSize = remainderItems;
                            inventoryItemSlot[posX + x, posY + y].UpdateCounter();
                            inventoryItem.UpdateCounter();
                            return false;
                        }
                    }else
                    {
                        
                        return false;
                    }     
                }
            }
        }
        return true;
    }

    bool PositionCheck(int posX, int posY) //checks if position is valid for item
    { 
        if(posX<0 || posY<0)
        {
            return false;
        }
        if(posX>= gridSizeWidth || posY>=gridSizeHeight)
        {
            return false;
        }
        return true;
    }

    public bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if(PositionCheck(posX, posY) == false){return false;}//check top left
        posX += width - 1;
        posY += height - 1;
        if(PositionCheck(posX, posY) == false){return false;}//check bot right
        return true;
    }

    public bool isLootGrid(){
        if(inventoryType.CompareTo("LootInv") == 0)
        {
            return true;
        }
        return false;
    }

    public void ClearLootGrid(){
        ClearWholeGrid();
        foreach(InventoryItem item in inventoryItemsInGrid)
        {
            Destroy(item.gameObject);
        }
        inventoryItemsInGrid.Clear();
        itemsInGrid.Clear();
    }
    

}


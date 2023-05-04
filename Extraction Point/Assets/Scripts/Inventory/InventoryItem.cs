using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;
    public int onGridPositionX, onGridPositionY, currentStackSize = 1;
    public TextMeshProUGUI stackCounter;
    public bool rotated = false;
    public bool weight;

    public int HEIGHT{
        get{
            if(rotated == false){
                return itemData.height;
            }
            return itemData.width;
        }
    }
    public int WIDTH{
        get{
            if(rotated == false){
                return itemData.width;
            }
            return itemData.height;
        }
    }

    

    internal void Set(ItemData itemData){
        this.itemData = itemData;

        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = (itemData.width * (Screen.width / 1920f) )*  ItemGrid.tileSizeWidth;
        size.y = (itemData.height * (Screen.height / 1080f) )*  ItemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;

        if(currentStackSize > 1){

        }
        
    }

    internal void UpdateCounter(){
        if(currentStackSize.ToString() != stackCounter.text && currentStackSize > 1){
            stackCounter.text = currentStackSize.ToString();
        }else if(currentStackSize == 1){
            stackCounter.text = "";
        }
        itemData.currentStackSize = currentStackSize;
    }

    internal int Get(ItemData itemData){
        this.itemData = itemData;
        
        return itemData.itemID;
    }

    internal void Rotate(){
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0,0, rotated == true ? 90f : 0f);
    }
}

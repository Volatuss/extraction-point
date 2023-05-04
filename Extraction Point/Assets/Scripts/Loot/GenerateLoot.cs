using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateLoot : MonoBehaviour
{
    [SerializeField] public InventoryController inventoryController;
    [SerializeField] List<GameObject> ContainerPrefabList;
    [SerializeField] List<string> ContainerPrefabType;
    [SerializeField] GameObject itemPrefab;
    InventoryItem[,] itemsTest;
    
    public void TemporaryTestLoot() //temporary method to test generation of loot
    {

        StartCoroutine(CreateLootableContainer( new Vector3(1,1,0), ContainerPrefabType[0], ContainerPrefabList[0]));
        StartCoroutine(CreateLootableContainer( new Vector3(-1,1,0), ContainerPrefabType[1], ContainerPrefabList[1]));
        StartCoroutine(CreateLootableContainer( new Vector3(-1,-1,0), ContainerPrefabType[0], ContainerPrefabList[0]));
        StartCoroutine(CreateLootableContainer( new Vector3(1,-1,0), ContainerPrefabType[1], ContainerPrefabList[1]));

    }

    


    

    public IEnumerator CreateLootableContainer(Vector3 position, string containerType, GameObject containerPrefab, GameObject parent = null) //works for now
    {
        itemsTest = new InventoryItem[10,14];
        Dictionary<Vector2, ItemData> itemList = new Dictionary<Vector2, ItemData>();
        GameObject container = Instantiate(containerPrefab, position, Quaternion.identity);
        LootableController containerController = container.GetComponent<LootableController>();
        containerController.containerName = containerType;
        containerController.inventoryController = inventoryController;
        
        int ItemCount = Random.Range(1, containerController.maxItems);
        for(int i = 0; i < ItemCount; i++){
            InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
            ItemData tempData = containerController.lootPool[Random.Range(0, containerController.lootPool.Count)];
            inventoryItem.Set(tempData);
            itemList.Add(FindSpaceForObject(inventoryItem), tempData);
            Destroy(inventoryItem.gameObject);
        }
        container.GetComponent<LootableController>().itemsInContainer = itemList;

        if(parent != null)
        {
            container.transform.SetParent(parent.transform);
        }
        

        yield return null;
    }

    public Vector2 FindSpaceForObject(InventoryItem itemToInsert) 
    {
        int height = 14 - itemToInsert.HEIGHT + 1; // 1x2 item: width = 10, height = 13
        int width = 10 - itemToInsert.WIDTH + 1;
        for(int y = 0; y<height; y++)
        {
            for(int x = 0; x<width; x++)
            {
                if(CheckAvailableSpace(x,y, itemToInsert.HEIGHT, itemToInsert.WIDTH) == true)
                {
                   for (int ix = 0; ix < itemToInsert.WIDTH; ix++)
                    {
                        for (int iy = 0; iy < itemToInsert.HEIGHT; iy++)
                        {
                            itemsTest[x + ix, y + iy] = itemToInsert;
                        }
                    }
                    return new Vector2(x, y);
                }
            }
        }
        return Vector2.zero;
    }

     public bool CheckAvailableSpace(int posX, int posY, int width, int height){ 
        for(int y = 0; y < width; y++){
            for(int x = 0; x < height; x++){
                if((itemsTest[posX + x, posY + y]) != null){
                    return false;
                }
            }
        }
        
        return true;
    }

   
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerController : MonoBehaviour
{
    [SerializeField] List<ItemData> LootPool; //loot pool for the given container
    [SerializeField] public string ContainerType; //medical, weapon, ammo, etc
    [SerializeField] public LootableController lootableController;
    
    public List<ItemData> GeneratedLoot = new List<ItemData>();
    private int numberOfItems;

    public void GenerateLoot(){
        numberOfItems = Random.Range(1, 12);

        for(int i = 0; i < numberOfItems; i++){
            GeneratedLoot.Add(LootPool[Random.Range(0, LootPool.Count)]);
        }
    }
}

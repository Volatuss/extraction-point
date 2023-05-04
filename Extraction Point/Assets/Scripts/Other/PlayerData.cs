using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;

public class PlayerData
{
    public int TotalItemsInInventory;
    public ConvertToSerializable.InventoryContents ItemsInInventory;
    public ConvertToSerializable.TaskList CurrentTasks, CompletedTasks;
    public float[] CurrentPlayerPosition = new float[3];  
    public int  level, money; //not implemented yet
    public float health;
    

    

    public PlayerData ()
    {
        
        //GeneralInfo
        health = PlayerHealth.currentHealth;
        
        
        //inventory items, need to make count
        TotalItemsInInventory = InventoryController.InventoryItems.Count;
        ItemsInInventory  = ConvertToSerializable.i_ConvertTo(InventoryController.InventoryItems);
        InventoryController.InventoryItems.Clear();
        //need to do with equipment slots

        //need to do similar thing with stash
        
        //completed tasks and current tasks

        CurrentTasks = ConvertToSerializable.t_ConvertTo(TaskManager.CurrentTasks);
        CompletedTasks = ConvertToSerializable.t_ConvertTo(TaskManager.CompletedTasks);

    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public float[,] ItemsInInventory; //2d array, stores x, y, item id
    public float[] position = new float[3];    
    public int totalItemCount, level, money; //not implemented yet
    public float health;
    public List<string> currentTasks = new List<string>(), completedTasks = new List<string>();

    public PlayerData (){
        int i = 0;
        totalItemCount = InventoryController.InventoryItems.Count;
        ItemsInInventory = new float[totalItemCount, 3];
        health = PlayerHealth.currentHealth;

        foreach(var item in InventoryController.InventoryItems){
            ItemsInInventory[i, 0] = item.Key.x;
            ItemsInInventory[i, 1] = item.Key.y;
            ItemsInInventory[i, 2] = item.Value.itemID;
            i++;
        }
        
        
        foreach(TaskOBJ task in TaskManager.CurrentTasks){
            currentTasks.Add(task.name);
        }

        foreach(TaskOBJ task in TaskManager.CompletedTasks){
            completedTasks.Add(task.name);
        }
        
    }
}

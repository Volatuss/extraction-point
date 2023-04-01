using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class GameHandler : MonoBehaviour
{

    [SerializeField] InventoryController iController;
    [SerializeField] ItemGrid inventoryGrid;
    private IDataService DataService = new JsonDataService();
    
    private bool EncryptionEnabled = false;
    private long SaveTime;


    private void Update() {
        if(Input.GetKeyDown(KeyCode.S)){
            SerializeJson(); //Saves the data
        }

        if(Input.GetKeyDown(KeyCode.L)){
            DeserializeJson();
        }
    }
    public void ToggleEncryption(bool EncryptionEnabled){
        this.EncryptionEnabled = EncryptionEnabled;
    }

    public void SerializeJson(){
        long startTime = System.DateTime.Now.Ticks;
        PlayerData playerData = new PlayerData();
        if (DataService.SaveData("/player-data-save.json", playerData, EncryptionEnabled))
        {
            SaveTime = System.DateTime.Now.Ticks - startTime;
            Debug.Log($"Save time took: {(SaveTime / 10000f):N4}ms");
        }
        else
        {
            Debug.LogError("Could not save file!");
        }
    }


    public void DeserializeJson(){
        //Example of loading the data
        long startTime = System.DateTime.Now.Ticks;
        try
        {
            PlayerData data = DataService.LoadData<PlayerData>("/player-data-save.json", EncryptionEnabled);
            long LoadTime = System.DateTime.Now.Ticks - startTime;
            Debug.Log($"Loaded from file:\r\n" + JsonConvert.SerializeObject(data));
            Debug.Log($"Loaded Time: {(LoadTime / 10000f):N4}ms");
            LoadPlayerData(data);
        }
        catch(Exception e)
        {
            Debug.LogError($"Could not read from file! Show something on UI here!");
        }
    }

    public void LoadPlayerData(PlayerData data){ //need to take the values from the PlayerData Object and use them to change the playerdata
        iController.LoadSavedLoot(inventoryGrid, data.totalItemCount, data.ItemsInInventory);
    }

   
}

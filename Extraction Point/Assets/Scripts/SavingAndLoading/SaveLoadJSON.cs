using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadJSON : MonoBehaviour
{
    private IDataService DataService;
    private PlayerData playerData = new PlayerData();
    private bool EncryptionEnabled;
    private float SaveTime;

    public void ToggleEncryption(bool EncryptionEnabled){
        this.EncryptionEnabled = EncryptionEnabled;
    }

    public void SerializeJson(){
        float startTime = Time.time;
        if (DataService.SaveData("/player-data-save", playerData, EncryptionEnabled))
        {
            SaveTime = Time.time - startTime;
            Debug.Log("Save time took: " + SaveTime);
        }
        else
        {
            Debug.LogError("Could not save file!");
        }
    }
}

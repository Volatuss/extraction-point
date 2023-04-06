using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{

    [SerializeField] GameObject StaticObjects, EscapeMenu, InventoryUI, QuestUI;
    [SerializeField] InventoryController i_Controller;
    [SerializeField] TaskManager t_Manager;
    [SerializeField] ItemGrid i_Grid;
    public TaskOBJ[] CompleteTaskList;
    private IDataService DataService = new JsonDataService();
    private bool EncryptionEnabled = false;
    public static bool isPlayerInRaid = false, isGamePaused = false, loadedData = false, isUIOpen = false;
    public static GameObject CurrentActiveUI; //manages what ui is open at a given time, prevents multiple UIs from being open

    
    private void Awake() { //loads the data from file when opening the save file
        DeserializeJson();
    }

    //should probably do my inputs here for almost everything, infact, should use new Input system
    private void Update() 
    { 
        if(CurrentActiveUI == EscapeMenu){ //wip
            isGamePaused = true;
        }else{
            isGamePaused = false;
        }

        //Open Escape menu
        if(Input.GetKeyDown(KeyCode.Escape)){
           TryOpenInterface(EscapeMenu);
           Debug.Log(CurrentActiveUI);
        }

        //Open Inventory Menu
        if(Input.GetKeyDown(KeyCode.I)){
           TryOpenInterface(InventoryUI);
           Debug.Log(CurrentActiveUI);
        }

        //Open Quest Menu
        if(Input.GetKeyDown(KeyCode.C)){
            TryOpenInterface(QuestUI);
            Debug.Log(CurrentActiveUI);
        }

        //Interacting with objects
        if(Input.GetKeyDown(KeyCode.F) && PlayerController.isObjectLootable){ //need to rethink how to do lootable objects 
            TryOpenInterface(InventoryUI);
            
            
        }

        //Temporary KeyBinds for Testing purposes
        if(Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftControl)){ //saving for the moment
            Debug.Log("Saving sequence");
            SerializeJson(); //Saves the data
        }

        if(Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftControl)){ //loading for the moment
            DeserializeJson();
        }

        if(Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl)){ //reset the tasks
            TaskManager.ResetTasks();
        }

        
    }
    public void ToggleEncryption(bool EncryptionEnabled){
        this.EncryptionEnabled = EncryptionEnabled;
    }

    public void SerializeJson(){ //will serialize and save the data
        Debug.Log("Starting save routine");
        long startTime = System.DateTime.Now.Ticks, SaveTime;
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


    public void DeserializeJson(){ //will deserialize and load the data
        long startTime = System.DateTime.Now.Ticks;
        try
        {
            PlayerData data = DataService.LoadData<PlayerData>("/player-data-save.json", EncryptionEnabled);
            long LoadTime = System.DateTime.Now.Ticks - startTime;
            //Debug.Log($"Loaded from file:\r\n" + JsonConvert.SerializeObject(data));
            Debug.Log($"Loaded Data From File, Load took {(LoadTime / 10000f):N4}ms");
            StartCoroutine(LoadPlayerData(data));
        }
        catch(Exception e)
        {
            Debug.LogError($"Failed to read from file data to: {e.Message} {e.StackTrace}");
        }
    }

    IEnumerator LoadPlayerData(PlayerData data)
    { 
        //wait for .1second, then open the interface
        yield return new WaitForSeconds(.1f);
        InventoryUI.SetActive(true);
        QuestUI.SetActive(true);
        //loads inventory, this is modular and can work for any item grid, I just need to save other ones
        
        i_Controller.LoadSavedLoot(i_Grid, data.TotalItemsInInventory, ConvertToSerializable.i_ConvertFrom(data.ItemsInInventory));
        t_Manager.LoadTaskLists(ConvertToSerializable.t_ConvertFrom(CompleteTaskList, data.CurrentTasks), ConvertToSerializable.t_ConvertFrom(CompleteTaskList, data.CompletedTasks));

        //once data is loaded, set to true, wait .2seconds to then close menu to ensure data is loaded
        //loadedData = true;
        while(!loadedData){
            yield return new WaitForSeconds(.1f);
        }
        InventoryUI.SetActive(false);
        QuestUI.SetActive(false);
        yield return null;
    }

    //for escape menu button
    public void QuitToMainMenu()
    {  
        if(!isPlayerInRaid){
            //dont save the player information if they are in a raid
            SerializeJson();
        }
        SceneManager.LoadScene("MainMenuScene");
        Destroy(StaticObjects);
    }

    public void ResumeGame(){
        EscapeMenu.SetActive(false);
    }

    

    public void TryOpenInterface(GameObject toOpen){
        if(toOpen == CurrentActiveUI){ //if the current interface is open and the hotkey is input again
            toOpen.SetActive(false);
            isUIOpen = false;
            Cursor.visible = false;
            if(CurrentActiveUI == InventoryUI){
                i_Controller.OnCloseInventory();
            }
            CurrentActiveUI = null;
            
        }else if(CurrentActiveUI == null){ //if no current ui is open, open one
            toOpen.SetActive(true);
            isUIOpen = true;
            Cursor.visible = true;
            CurrentActiveUI = toOpen;
        }else if(CurrentActiveUI != EscapeMenu && toOpen == EscapeMenu){ //if a ui is open but not the escape menu and the escape was pressed, exit the current ui
            CurrentActiveUI.SetActive(false);
            isUIOpen = false;
            Cursor.visible = false;
            if(CurrentActiveUI == InventoryUI){
                i_Controller.OnCloseInventory();
            }
            CurrentActiveUI = null;
        }
    }

}

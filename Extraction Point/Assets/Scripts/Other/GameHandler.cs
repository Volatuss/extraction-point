using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class GameHandler : MonoBehaviour
{

    [SerializeField] GameObject StaticObjects, EscapeMenu, InventoryUI, QuestUI, SettingsMenu;
    [SerializeField] InventoryController i_Controller;
    [SerializeField] TaskManager t_Manager;
    [SerializeField] PlayerWeaponHandler weaponHandler;
    [SerializeField] ItemGrid i_Grid;
    public TaskOBJ[] CompleteTaskList;
    public InputActionAsset actions;
    // public KeyCode inventoryKey, questKey, reloadKey, interactKey;
    public ItemGrid saveGrid;
   
    private IDataService DataService = new JsonDataService();
    private bool EncryptionEnabled = false;
    public static bool isPlayerInRaid = false, isGamePaused = false, loadedData = false, isUIOpen = false, isSaving = false;
    public static GameObject CurrentActiveUI; //manages what ui is open at a given time, prevents multiple UIs from being open

    
    private void Awake() { //loads the data from file when opening the save file
        if(File.Exists(Application.persistentDataPath + PlayerPrefs.GetString("savePath", "/player-data-save-1.json"))){
            DeserializeJson();
        }else{
            //if no file then we will want to make 3 save files
            Debug.Log("No Save file, making one");
            using FileStream stream = File.Create((Application.persistentDataPath + PlayerPrefs.GetString("savePath", "/player-data-save-1.json")));
            stream.Close();
        }
        
        
        

      

        // actions.FindAction("Quests").performed += _ => this.OnQuests();
        // actions.FindAction("Inventory").performed += _ => this.OnInventory();
        // actions.FindAction("Escape").performed += _ => this.OnEscape();
        // actions.FindAction("Interact").performed += _ => this.OnInteract();
        // actions.FindAction("Reload").performed += _ => this.weaponHandler.Reload();
    }

    

    private void Update() 
    { 
        if(CurrentActiveUI == EscapeMenu){ //wip
            isGamePaused = true;
        }else{
            isGamePaused = false;
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
        
        if(Input.GetKeyDown(KeyCode.I)){
            OnInventory();
        }

        if(Input.GetKeyDown(KeyCode.J)){
            OnQuests();
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            OnEscape();
        }

        if(Input.GetKeyDown(KeyCode.R)){
            weaponHandler.Reload();
        }

        if(Input.GetKeyDown(KeyCode.F)){
            OnInteract();
        }

        
    }

    public void ToggleEncryption(bool EncryptionEnabled){
        this.EncryptionEnabled = EncryptionEnabled;
    }
    
    public void SerializeJson(){ //will serialize and save the data
        isSaving = true;
        long startTime = System.DateTime.Now.Ticks, SaveTime;
        PlayerData playerData = new PlayerData();
        if (DataService.SaveData(PlayerPrefs.GetString("savePath", "/player-data-save-1.json"), playerData, EncryptionEnabled))
        {
            SaveTime = System.DateTime.Now.Ticks - startTime;
            Debug.Log($"Save time took: {(SaveTime / 10000f):N4}ms");
        }
        else
        {
            Debug.LogError("Could not save file!");
        }
        isSaving = false;
    }

    public void DeserializeJson(){ //will deserialize and load the data
        long startTime = System.DateTime.Now.Ticks;
        try
        {
            PlayerData data = DataService.LoadData<PlayerData>(PlayerPrefs.GetString("savePath", "/player-data-save-1.json"), EncryptionEnabled);
            long LoadTime = System.DateTime.Now.Ticks - startTime;
            Debug.Log($"Loaded Data From File, Load took {(LoadTime / 10000f):N4}ms");
            if(data == null){
                    
            }else{
                StartCoroutine(LoadPlayerData(data));
            }
            
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
        //loads inventory, this is modular and can work for any item grid, I just need to save other ones.
        
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

    //Menu Functions
    public void QuitToMainMeny(){
        StartCoroutine(Quit());
    }
    IEnumerator Quit(){
        if(!isPlayerInRaid){
            //dont save the player information if they are in a raid
            SerializeJson();
        }
        while(isSaving){
            yield return null;
        }
        SceneManager.LoadScene("MainMenuScene");
        Destroy(StaticObjects);
        yield return null;
    }

    public void OpenSettings(){
        if(SettingsMenu != null){ 
            SettingsMenu.SetActive(true);
        }
    }

    public void CloseSettings(){
        if(SettingsMenu != null){ 
            SettingsMenu.SetActive(false);
        }
    }

    public void ResumeGame(){
        if(SettingsMenu != null){  
            if(SettingsMenu.activeSelf){
                CloseSettings();
            }

            TryOpenInterface(EscapeMenu);
        }
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

    //Controls

    public void OnInventory() //opens and closes inventory
    {
        if(SettingsMenu != null){ 
            TryOpenInterface(InventoryUI);
        }
    }

    public void OnEscape() //opens and closes escape menu
    {
        if(SettingsMenu != null){ 
            if(SettingsMenu.activeSelf)
            {
                CloseSettings();
            }
            TryOpenInterface(EscapeMenu);
        }
    }

    public void OnInteract() //interact hotkey
    {
        if(InventoryUI != null){ 
            if(PlayerController.isObjectLootable)
            {
                TryOpenInterface(InventoryUI);
            }
        }
    }

    public void OnQuests() //opens and closes quest menu
    {
        if(QuestUI != null){ 
            TryOpenInterface(QuestUI);
        }
    }

}

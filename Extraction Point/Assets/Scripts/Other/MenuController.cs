using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.Audio;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject SettingsWindow, SaveWindow;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private AudioSource buttonClick;
    

    public void OnStartPress(){
        buttonClick.Play();
        SceneManager.LoadScene("InitScene");
    }

    public void OnLoadPress(){
        buttonClick.Play();
        SaveWindow.SetActive(true);
    }

    public void OnSettingsPress(){
        buttonClick.Play();
        SettingsWindow.SetActive(true);
    }

    public void OnQuitPress(){
        buttonClick.Play();
        Application.Quit();
    }

    public void ResetAllBindings(){
        foreach(InputActionMap map in inputActions.actionMaps){
            map.RemoveAllBindingOverrides();
            PlayerPrefs.DeleteKey("rebinds");
        }
    }

    public void SelectSlotOne(){
        buttonClick.Play();
        PlayerPrefs.SetString("savePath", "/player-data-save-1.json");
        SaveWindow.SetActive(false);
        Debug.Log("Selected save 1");
    }
    
    public void SelectSlotTwo(){
        buttonClick.Play();
        PlayerPrefs.SetString("savePath", "/player-data-save-2.json");
        SaveWindow.SetActive(false);
        Debug.Log("Selected save 2");
    }
    
    public void SelectSlotThree(){
        buttonClick.Play();
        PlayerPrefs.SetString("savePath", "/player-data-save-3.json");
        SaveWindow.SetActive(false);
        Debug.Log("Selected save 3");
    }

    public void ResetSlotOne(){
        buttonClick.Play();
        string path = Application.persistentDataPath + "/player-data-save-1.json";
        File.Delete(path);
    }

    public void ResetSlotTwo(){
        buttonClick.Play();
        string path = Application.persistentDataPath + "/player-data-save-2.json";
        File.Delete(path);
    }

    public void ResetSlotThree(){
        buttonClick.Play();
        string path = Application.persistentDataPath + "/player-data-save-3.json";
        File.Delete(path);
    }
}
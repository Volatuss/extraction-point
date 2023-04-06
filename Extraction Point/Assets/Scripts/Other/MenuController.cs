using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] GameObject MenuWindow, SettingsWindow;
    
    private void Start() {
        
    }

    public void OnStartPress(){
        Debug.Log("Starting Game");
        SceneManager.LoadScene("InitScene");
    }

    public void OnLoadPress(){
        Debug.Log("Load Save");
    }

    public void OnSettingsPress(){
        Debug.Log("Open Settings");
        SettingsWindow.SetActive(true);
    }

    public void OnQuitPress(){
        Debug.Log("Quit Game");
        Application.Quit();
    }
}

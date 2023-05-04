using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] GameObject SettingsWindow;
    [SerializeField] Slider masSlider, sfxSlider, musSlider, ambSlider;
    [SerializeField] Toggle bloodTog, shellTog;
    [SerializeField] TMP_Dropdown resDropdown, winDropdown;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioSource buttonClick;
    private int currentResW, currentResH, prevResW, prevResH, selectedSave = 1;
    private float musicVol, ambVol, sfxVol, masterVol;
    FullScreenMode currentMode, previousMode;
    


    private void Start() {
        //fullscreen mode
        HandleWindowModeChange(PlayerPrefs.GetInt("screenMode", 0));
        winDropdown.value = PlayerPrefs.GetInt("screenMode", 0); 

        //resolution
        HandleResolutionChange(PlayerPrefs.GetInt("resolution", 1));
        resDropdown.value = PlayerPrefs.GetInt("resolution", 1); 

        //volume
        masSlider.value = PlayerPrefs.GetFloat("masVol", Mathf.Log10(.25f) * 20);
        musSlider.value =  PlayerPrefs.GetFloat("musVol", Mathf.Log10(.25f) * 20);
        sfxSlider.value = PlayerPrefs.GetFloat("sfxVol", Mathf.Log10(.25f) * 20);
        ambSlider.value = PlayerPrefs.GetFloat("ambVol", Mathf.Log10(.25f) * 20);

        OnUpdateAmbient();
        OnUpdateMaster();
        OnUpdateMusic();
        OnUpdateSFX();

        //save selection
        if(selectedSave == 1){
            PlayerPrefs.SetString("savePath", "/player-data-save-1.json");
        }else if(selectedSave == 2){
            PlayerPrefs.SetString("savePath", "/player-data-save-2.json");
        }else if (selectedSave == 3){
            PlayerPrefs.SetString("savePath", "/player-data-save-3.json");
        }else{
            PlayerPrefs.SetString("savePath", "/player-data-save-1.json");
        }

        //disable/enable particles
        bool onOff = (PlayerPrefs.GetInt("showBlood", 1) != 0);
        bloodTog.isOn = (onOff);
        onOff = (PlayerPrefs.GetInt("showShells", 1) != 0);
        shellTog.isOn = (onOff);

        SettingsCloseButton();
    }

    public void SettingsCloseButton(){ //check for changes, save changes if made
        buttonClick.Play();
        if(currentMode != previousMode){
            Screen.fullScreenMode = currentMode;
            previousMode = currentMode;
        }
        if(prevResW != currentResW){
            Screen.SetResolution(currentResW, currentResH, currentMode, 60);
            prevResH = currentResH;
            prevResW = currentResW;
        }
        PlayerPrefs.Save();
        SettingsWindow.SetActive(false);
    }

    public void ResetAllBindings(){
        foreach(InputActionMap map in inputActions.actionMaps){
            map.RemoveAllBindingOverrides();
            PlayerPrefs.DeleteKey("rebinds");
        }
    }

    public void HandleResolutionChange(int val){
        buttonClick.Play();
        prevResW = Screen.width;
        prevResH = Screen.height;         
        switch(val){
            case 0:     //2560x1440
                currentResW = 2560;
                currentResH = 1440;
                break;
            case 1:     //1920x1080
                currentResW = 1920;
                currentResH = 1080; 
                break;
            case 2:     //1600x900
                currentResW = 1600;
                currentResH = 900; 
                break;
            case 3:     //1280x720
                currentResW = 1280;
                currentResH = 720; 
                break;
            case 4:     //640x360
                currentResW = 640;
                currentResH = 360; 
                break;
            default:
                break;
        }
        PlayerPrefs.SetInt("resolution", val);
    }

    public void HandleWindowModeChange(int val)
    {
        buttonClick.Play();
        previousMode = currentMode;
        switch(val){
            case 0:     //Windowed
                currentMode = FullScreenMode.Windowed;
                break;
            case 1:     //Fullscreen
                currentMode = FullScreenMode.MaximizedWindow;
                break;
            case 2:     //Fullscreen
                currentMode = FullScreenMode.ExclusiveFullScreen;
                break;
            default:
                break;
        }
        PlayerPrefs.SetInt("screenMode", val);
    }

    public void BloodToggle(bool val)
    {
        buttonClick.Play();
        if(val){
            PlayerPrefs.SetInt("showBlood", 1);
        }else{
            PlayerPrefs.SetInt("showBlood", 0);
        } 
    }

    public void ShellToggle(bool val)
    {
        buttonClick.Play();
        if(val){
            PlayerPrefs.SetInt("showShells", 1);
        }else{
            PlayerPrefs.SetInt("showShells", 0);
        }
    }
    public void OnUpdateMaster()
    {
        if(masSlider.value == 0f){
            mixer.SetFloat("masterVol", -80f);
        }else{
            mixer.SetFloat("masterVol", Mathf.Log10(masSlider.value) * 20);   
        }
        PlayerPrefs.SetFloat("masVol",  masSlider.value);
    }

    public void OnUpdateMusic()
    {
        if(musSlider.value == 0f){
            mixer.SetFloat("musicVol", -80f);
        }else{
            mixer.SetFloat("musicVol", Mathf.Log10(musSlider.value) * 20);
        }
        PlayerPrefs.SetFloat("musVol", musSlider.value);
    }

    public void OnUpdateSFX()
    {   
        if(sfxSlider.value == 0f){
            mixer.SetFloat("sfxVol", -80f);
        }else{
            mixer.SetFloat("sfxVol", Mathf.Log10(sfxSlider.value) * 20);
        }
        PlayerPrefs.SetFloat("sfxVol", sfxSlider.value);
        
    }

    public void OnUpdateAmbient()
    {
        if(ambSlider.value == 0f){
            mixer.SetFloat("ambientVol", -80f);
        }else{
            mixer.SetFloat("ambientVol", Mathf.Log10(ambSlider.value) * 20);
        }
        PlayerPrefs.SetFloat("ambVol",  ambSlider.value);
    }
}

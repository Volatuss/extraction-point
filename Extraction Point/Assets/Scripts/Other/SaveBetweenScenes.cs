using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveBetweenScenes : MonoBehaviour
{   
    public static SaveBetweenScenes instance;
    void Start()
    {
        if(instance != null){
            Destroy(instance);
        }else{
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    
}

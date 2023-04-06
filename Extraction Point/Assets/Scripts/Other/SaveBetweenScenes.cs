using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveBetweenScenes : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    
}

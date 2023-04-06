using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubHandler : MonoBehaviour
{
    public void GoToLevel(){
        SceneManager.LoadScene("MapGenerationTesting");
        MapGenerator.playerTransform = GameObject.Find("Player").GetComponent<Transform>();
    }
}

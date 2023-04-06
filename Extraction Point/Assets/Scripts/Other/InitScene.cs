using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InitScene : MonoBehaviour
{

    [SerializeField] List<GameObject> ToInitialize = new List<GameObject>();
    [SerializeField] GameObject LoadingScreen;
    private void Awake() {
        StartCoroutine(InitializeUserInterfaces());
    }

    IEnumerator InitializeUserInterfaces(){
        LoadingScreen.SetActive(true);
        foreach(GameObject toInit in ToInitialize){
            toInit.SetActive(false);
        }

        yield return new WaitForSeconds(2f);
        //StartCoroutine(LoadingScreen.GetComponent<LoadingScreen>().FinishedLoading());
        SceneManager.LoadScene("HubShip");
        yield return null;
    }
}

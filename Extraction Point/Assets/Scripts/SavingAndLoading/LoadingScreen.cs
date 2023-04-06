using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    
    [SerializeField] GameObject ImageComponent;
    Image image;

    private void Awake() {
       StartCoroutine(FinishedLoading());
    }
    
    public IEnumerator FinishedLoading(){
        yield return new WaitForSeconds(4f);
        Destroy(gameObject);
    }
}

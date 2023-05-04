using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExtractionZone : MonoBehaviour
{
    private bool extracted = false, isInZone = false;
    [SerializeField] TextMeshProUGUI extractCounter;
    GameObject player;
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("HitBox")){
            isInZone = true;
            extractCounter.gameObject.SetActive(true);
            StartCoroutine(ExtractRoutine());
        }  
    }
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("HitBox") && !extracted){
            isInZone = false;
            extractCounter.gameObject.SetActive(false);
        }  
    }

    IEnumerator ExtractRoutine()
    {
        float timeRemaining = 5.00f;
        while(true){
            if(timeRemaining <= 0f){
                extracted = true;
                Debug.Log("Extracted!");

                extractCounter.gameObject.SetActive(false);

                GameObject.FindGameObjectWithTag("Player").gameObject.transform.position = new Vector3(0, 0, 0);
                SceneManager.LoadScene("HubShip");
                MapGenerator.ResetStatics();
                break;
            }else if(!isInZone){
                Debug.Log("Left Zone!");
                extractCounter.gameObject.SetActive(false);
                break;
            }
            extractCounter.text = timeRemaining.ToString("0.00") + "s";
            Debug.Log(timeRemaining + "s");
            timeRemaining-=0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        
        yield return null;
    }
}

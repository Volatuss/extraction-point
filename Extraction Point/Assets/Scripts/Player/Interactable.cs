using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private InteractToolTips tooltipController;
    public bool isInRange = false;
    public string actionDescription;
    public KeyCode interactKey = KeyCode.F; //default will be 'F'
    public UnityEvent interactAction;
    public int index;
    private System.Random indexer = new System.Random();
   
    private void Awake() {
        tooltipController = GameObject.Find("InteractOptions").GetComponent<InteractToolTips>();
        this.index = indexer.Next();
    }

    private void Update() {
        if(isInRange){
            if(Input.GetKeyDown(interactKey) && tooltipController.selectedActionIndex == index){
                interactAction.Invoke();
            }
        }
    }

    

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("HitBox")){
            AddToolTip();
            isInRange = true;

        }  
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("HitBox")){
            RemoveToolTip();
            isInRange = false;

        }  
    }

    private void AddToolTip(){ //need to create an entry to add to list above player
        StartCoroutine(tooltipController.NewAction(this.index, actionDescription));
        Debug.Log("Added tip: " + this.index);
    }

    private void RemoveToolTip(){
        StartCoroutine(tooltipController.RemoveAction(this.index));
        Debug.Log("Removed tip: " + this.index);
    }

}

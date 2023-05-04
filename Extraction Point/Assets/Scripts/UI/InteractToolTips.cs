using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InteractToolTips : MonoBehaviour
{
    [SerializeField] GameObject interactContent, actionHilighter, actionPrefab;
    GameObject selector;
    public static Dictionary<int, GameObject> currentActions = new Dictionary<int, GameObject>();
    public int selectedActionIndex;
    
    private void Awake() {
        selector = Instantiate(actionHilighter, gameObject.transform);
        selector.SetActive(false);
        selectedActionIndex = -1;
    }

    private void Update() {
        if(selectedActionIndex == -1 || currentActions.Count <= 1){ return; }

        if(Input.mouseScrollDelta != Vector2.zero){
            if(Input.mouseScrollDelta.y > 0f){
                ScrollUp();
            }else{
                ScrollDown();
            }
        }
        
    }

    public void SelectAction(int index, GameObject actionToSelect)
    {
        selectedActionIndex = index;
        selector.SetActive(true);
        selector.transform.SetParent(actionToSelect.transform);
        selector.transform.position = actionToSelect.transform.position;

    }

    public void DeselectAction()
    {
        if(currentActions.Count == 0){
            selector.transform.SetParent(gameObject.transform);
            selector.SetActive(false);
            selectedActionIndex = -1;
        }else{
            SelectAction(currentActions.ElementAt(0).Key, currentActions.ElementAt(0).Value);
        }

    }

    public void ScrollUp()
    {
        if(selectedActionIndex == currentActions.ElementAt(0).Key)
        {
            
        }else{
            int next = currentActions.Keys.ToList().IndexOf(selectedActionIndex) - 1;
            SelectAction(currentActions.ElementAt(next).Key, currentActions.ElementAt(next).Value);
            
        }
        
    }

    public void ScrollDown()
    {
        
        if(selectedActionIndex == currentActions.ElementAt(currentActions.Count-1).Key)
        {

        }else{
            int next = currentActions.Keys.ToList().IndexOf(selectedActionIndex) + 1;
            SelectAction(currentActions.ElementAt(next).Key, currentActions.ElementAt(next).Value);
        }
    }

    public IEnumerator NewAction(int actionIndex, string action){
        GameObject newAction = Instantiate(actionPrefab, interactContent.transform);
        newAction.GetComponentInChildren<TextMeshProUGUI>().text = action;
        newAction.name = "Action - " + actionIndex.ToString();
        currentActions.Add(actionIndex, newAction);

        if(currentActions.Count == 1){ //ie. this is the only action, auto select it
            SelectAction(actionIndex, newAction);
            
        }
        
        yield return null;
    }

    public IEnumerator RemoveAction(int index){
        GameObject toRemove;
        currentActions.TryGetValue(index, out toRemove);
        currentActions.Remove(index);
        if(toRemove == null){

        }else if(selector.transform.parent == toRemove.transform){
            DeselectAction();
        }

        yield return null;

        Destroy(toRemove);

        yield return null;
    }
}

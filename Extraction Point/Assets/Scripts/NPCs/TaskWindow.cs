using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskWindow : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI taskName, taskDescription, xpReward, currencyReward;
   [SerializeField] private GameObject goalPrefab, ItemRewardPrefab;
   [SerializeField] private Transform goalContent, rewardContent; 
   [SerializeField] private Button SubmitButton;
   [SerializeField] TaskManager taskManager;
   [SerializeField] InventoryController inventoryController;
   

   public void Initialize(TaskOBJ task){
        taskName.text = task.Information.Name;
        taskDescription.text = task.Information.TaskDescription;        

        foreach(var goal in task.Goals)
        {
            GameObject goalObj = Instantiate(goalPrefab, goalContent);
            goalObj.transform.Find("GoalDescription").GetComponent<TextMeshProUGUI>().text = goal.GetDescription();

            GameObject countObj = goalObj.transform.Find("Count").gameObject;
            GameObject checkObj = goalObj.transform.Find("Checkmark").gameObject;
                     
            goal.CheckForCompletion();           
            
            if(goal.Completed){
                countObj.SetActive(false);
                checkObj.SetActive(true);
                SubmitButton.gameObject.SetActive(true);
            }else{
                countObj.GetComponent<TextMeshProUGUI>().text = goal.CurrentAmmount + "/" + goal.RequiredAmount;
            }            
        }

        
        //need to display the rewards
        xpReward.text = task.Reward.XP.ToString() + " Experience";
        currencyReward.text = task.Reward.Currency.ToString() + " Money";
        
        foreach(ItemData itemData in task.Reward.Items)
        {
            GameObject rewardObj = Instantiate(ItemRewardPrefab, rewardContent);
            rewardObj.transform.GetComponent<TextMeshProUGUI>().text = itemData.itemName;
        }

        SubmitButton.onClick.AddListener(delegate //handling submission of quests
        {
            SubmitButton.gameObject.SetActive(false);
            //Give the rewards to the player
            inventoryController.GiveRewards(task.Reward.Items);
            
            //Remove current task
            CloseWindow();
            TaskManager.CurrentTasks.Remove(task);
            TaskManager.CompletedTasks.Add(task);

            foreach(TaskOBJ next in task.Information.NextTasks){
                if(!TaskManager.CurrentTasks.Contains(next)){ //ensure only one iteration of the quest is added
                    Debug.Log(next);
                    TaskManager.CurrentTasks.Add(next);
                }
            }
            //Refresh tasksHolder following this to refresh available quests
            taskManager.Refresh();

        });

    }

    public void CloseWindow(){
        for(int i = 0; i < goalContent.childCount; i++){
            Destroy(goalContent.GetChild(i).gameObject);
        }
        for(int i = 2; i< rewardContent.childCount; i++){
            Destroy(rewardContent.GetChild(i).gameObject);
        }
        gameObject.SetActive(false);
    }

    
}

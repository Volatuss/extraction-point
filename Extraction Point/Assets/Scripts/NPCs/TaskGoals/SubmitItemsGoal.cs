using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitItemsGoal : TaskOBJ.TaskGoal
{
    public string GoalDescription;
    public string type;
    public List<ItemData> ItemsToSubmit;
    public bool invContainsItems = true;

    public override void CheckForCompletion(){
        invContainsItems = true;

        foreach(ItemData item in ItemsToSubmit){
            Debug.Log(InventoryController.InventoryItems.ContainsValue(item));
            if(!InventoryController.InventoryItems.ContainsValue(item)){
                invContainsItems = false;
            }
        }
        
        if(invContainsItems){
            Debug.Log("Completed?");
            Completed = true;
        }else{
            Completed = false;
        }
        

    }

    public override string GetDescription()
    {
        return GoalDescription;
    }
    public override string GetGoalType()
    {
        return type;
    }
        
}

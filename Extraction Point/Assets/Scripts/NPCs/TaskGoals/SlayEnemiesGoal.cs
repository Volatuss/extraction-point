using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlayEnemiesGoal : TaskOBJ.TaskGoal
{
    public string GoalDescription;
    public string type;
    public GameObject EnemyToSlay;

    public override void CheckForCompletion(){ //to be returned to once enemies are implemented
        if(CurrentAmmount >= RequiredAmount){
            Completed = true;
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

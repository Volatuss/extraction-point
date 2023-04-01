using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGoal : TaskOBJ.TaskGoal
{
    public string GoalDescription;
    public string type;

    public override void CheckForCompletion(){
        Completed = true;
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

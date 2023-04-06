using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConvertToSerializable
{
    //For InventoryItemList
    public class ItemAsString
    {
        public string name { get; set; }
        public int itemId { get; set; }
        public float xCoord { get; set; }
        public float yCoord { get; set; }
    }

    public class InventoryContents
    {
        public List<ItemAsString> contents { get; set; }
    }


    public static InventoryContents i_ConvertTo(Dictionary<Vector2, ItemData> toConvert)
    {
        
        List<ItemAsString> iList = new List<ItemAsString>();

        foreach(var item in toConvert){
            iList.Add(new ItemAsString() {name = item.Value.name, itemId = item.Value.itemID, xCoord = item.Key.x, yCoord = item.Key.y} );
        }

        InventoryContents toReturn = new InventoryContents() {contents = iList };
        
        
        return toReturn;

    }

    public static Dictionary<Vector2, int> i_ConvertFrom(InventoryContents contents)
    {
        List<ItemAsString> toConvert = contents.contents;

        Dictionary<Vector2, int> toReturn = new Dictionary<Vector2, int>();

        foreach(ItemAsString item in toConvert)
        {
            toReturn.Add(new Vector2(item.xCoord, item.yCoord), item.itemId);
        }

        return toReturn;
    }

    //For Tasks
    
    public class Task
    {
        public string name;
        public int id;
    }

    public class TaskList
    {
        public List<Task> taskList;
    }

    public static TaskList t_ConvertTo(List<TaskOBJ> toConvert)
    {

        List<Task> tList = new List<Task>();        

        foreach(TaskOBJ task in toConvert){
            tList.Add(new Task() {name = task.Information.Name, id = task.Information.TaskID} );
        }

        TaskList toReturn = new TaskList() {taskList = tList};

        return toReturn;
    }

    public static List<TaskOBJ> t_ConvertFrom(TaskOBJ[] taskArray, TaskList toConvert)
    {
        List<TaskOBJ> toReturn = new List<TaskOBJ>();

        foreach(Task task in toConvert.taskList){
            toReturn.Add(taskArray[task.id]);
        }

        return toReturn;
    }



}

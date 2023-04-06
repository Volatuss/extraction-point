using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private GameObject taskButtonPrefab, taskHolder;
    [SerializeField] private Transform tasksContent;
    [SerializeField] private TaskOBJ firstTask;
    private TaskOBJ listedTask;
    private static TaskOBJ starterTask;
    public static List<TaskOBJ> CurrentTasks = new List<TaskOBJ>(), CompletedTasks = new List<TaskOBJ>();

    private void Awake() {
        starterTask = firstTask;
    }

    private void OnEnable() {
        //if(!CurrentTasks.Contains(firstTask) && !CompletedTasks.Contains(firstTask)){CurrentTasks.Add(firstTask); }
        
        
        listedTask = null;
        foreach(var task in CurrentTasks){
            task.Initialize();
            task.TaskCompleted.AddListener(OnTaskCompleted);
            
            GameObject taskObj = Instantiate(taskButtonPrefab, tasksContent);
            
            taskObj.transform.Find("TaskName").GetComponent<TextMeshProUGUI>().text = task.Information.Name;
            taskObj.GetComponent<Button>().onClick.AddListener(delegate
            {
                if(listedTask != task){ //open new task window
                    taskHolder.GetComponent<TaskWindow>().CloseWindow();
                    listedTask = task;
                    taskHolder.SetActive(true);
                    taskHolder.GetComponent<TaskWindow>().Initialize(task);
                    
                    

                }else{ //close the old task window
                    listedTask = null;
                    taskHolder.GetComponent<TaskWindow>().CloseWindow();
                }
            
            });
        }
    }

    private void OnDisable() {
        taskHolder.GetComponent<TaskWindow>().CloseWindow();
        for(int i = 0; i < tasksContent.childCount; i++){
            Destroy(tasksContent.GetChild(i).gameObject);
        }
    }

    public void Refresh(){
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }

    /*for her build task goal:

    public void Build(string buildingName){
        EventManager.Instance.TaskEvent(new BuildingGameEvent(buildingName));
    }

    */

    private void OnTaskCompleted(TaskOBJ task){
        tasksContent.GetChild(CurrentTasks.IndexOf(task)).Find("Checkmark").gameObject.SetActive(true);
    }

    internal void LoadTaskLists(List<TaskOBJ> cur_tasks, List<TaskOBJ> comp_tasks)
    {
        CurrentTasks = cur_tasks;
        CompletedTasks = comp_tasks;
    }

    public static void ResetTasks(){
        CurrentTasks.Clear();
        CompletedTasks.Clear();
        CurrentTasks.Add(starterTask);
    }
}

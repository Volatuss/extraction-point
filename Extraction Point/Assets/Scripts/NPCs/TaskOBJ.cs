using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class TaskOBJ : ScriptableObject {

    [System.Serializable]
    public struct Info
    {
        public string Name;
        public string TaskDescription;
        public List<TaskOBJ> NextTasks;
        
    }

    [Header("Reward")] public Info Information; 

    [System.Serializable]
    public struct Stat
    {
        public int Currency;
        public int XP;
        public List<ItemData> Items;

    }

    [Header("Reward")] public Stat Reward = new Stat{Currency = 10, XP = 10};

    public bool Completed { get; protected set; }
    public TaskCompletedEvent TaskCompleted;

    public abstract class TaskGoal : ScriptableObject
    {
        protected string Description;
        protected string GoalType;
        public int CurrentAmmount { get; protected set;}
        public int RequiredAmount = 1;

        public bool Completed { get; protected set; }
        [HideInInspector] public UnityEvent GoalCompleted;

        public virtual string GetDescription()
        {
            return Description;
        }
        public virtual string GetGoalType()
        {
            return GoalType;
        }

        public virtual void Initialize(){
            Completed = false;
            GoalCompleted = new UnityEvent();
        }

        protected void Evaluate(){
            if(CurrentAmmount >= RequiredAmount){
                Complete();
            }
        }

        private void Complete(){
            Completed = true;
            GoalCompleted.Invoke();
            GoalCompleted.RemoveAllListeners();
        }

        public void Skip(){
            //charge player currency, wont need
        }

        public virtual void CheckForCompletion(){
            return;
        }
    }

    public List<TaskGoal> Goals;

    public void Initialize()
    {
        Completed = false;
        TaskCompleted = new TaskCompletedEvent();

        foreach(var goal in Goals){
            goal.Initialize();
            goal.GoalCompleted.AddListener(delegate { CheckGoals(); });
        }

        

    }
    private void CheckGoals(){
        Completed = Goals.TrueForAll(g => g.Completed);
        if(Completed){
            //give reward
            TaskCompleted.Invoke(this);
            TaskCompleted.RemoveAllListeners();
        }
    }

    
}

public class TaskCompletedEvent : UnityEvent<TaskOBJ> { }


#if UNITY_EDITOR

[CustomEditor(typeof(TaskOBJ))]
public class QuestEditor : Editor
{
    SerializedProperty m_TaskStatProperty;
    SerializedProperty m_TaskInfoProperty;

    List<string> m_TaskGoalType;
    SerializedProperty m_TaskGoalListProperty;

    [MenuItem("Assets/TaskOBJ", priority = 0)]

    public static void CreateTask()
    {
        var newTask = CreateInstance<TaskOBJ>();

        ProjectWindowUtil.CreateAsset(newTask, "task.asset");
    }

    void OnEnable() 
    {
        m_TaskInfoProperty = serializedObject.FindProperty(nameof(TaskOBJ.Information));
        m_TaskStatProperty = serializedObject.FindProperty(nameof(TaskOBJ.Reward));

        m_TaskGoalListProperty = serializedObject.FindProperty(nameof(TaskOBJ.Goals));

        var lookup = typeof(TaskOBJ.TaskGoal);
        m_TaskGoalType = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(lookup))
            .Select(type => type.Name)
            .ToList();
    }

    public override void OnInspectorGUI()
    {
        var child = m_TaskInfoProperty.Copy();
        var depth = child.depth;
        child.NextVisible(true);

        EditorGUILayout.LabelField("Task Info", EditorStyles.boldLabel); //display task info
        while(child.depth > depth)
        {
            EditorGUILayout.PropertyField(child, true);
            child.NextVisible(false);
        }

        child = m_TaskStatProperty.Copy();
        depth = child.depth;
        child.NextVisible(true);

        EditorGUILayout.LabelField("Task Reward", EditorStyles.boldLabel); //display reward info
        while(child.depth > depth)
        {
            EditorGUILayout.PropertyField(child, true);
            child.NextVisible(false);
        }

        int choice = EditorGUILayout.Popup("Add new Task Goal", -1, m_TaskGoalType.ToArray());
        if (choice != -1)
        {
            var newInstance = ScriptableObject.CreateInstance(m_TaskGoalType[choice]);

            AssetDatabase.AddObjectToAsset(newInstance, target);

            m_TaskGoalListProperty.InsertArrayElementAtIndex(m_TaskGoalListProperty.arraySize);
            m_TaskGoalListProperty.GetArrayElementAtIndex(m_TaskGoalListProperty.arraySize - 1)
                .objectReferenceValue = newInstance;
        }

        Editor ed = null;
        int toDelete = -1;

        for(int i = 0; i < m_TaskGoalListProperty.arraySize; ++i){
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            var item = m_TaskGoalListProperty.GetArrayElementAtIndex(i);
            SerializedObject obj = new SerializedObject(item.objectReferenceValue);

            Editor.CreateCachedEditor(item.objectReferenceValue, null, ref ed);

            ed.OnInspectorGUI();
            EditorGUILayout.EndVertical();

            if(GUILayout.Button("-", GUILayout.Width(32))){
                toDelete = i;
            }
            EditorGUILayout.EndHorizontal();
        }  

        if(toDelete != -1)
        {
            var item = m_TaskGoalListProperty.GetArrayElementAtIndex(toDelete).objectReferenceValue;
            DestroyImmediate(item, true);

            //need to do twice, once to nullify, once to remove it
            m_TaskGoalListProperty.DeleteArrayElementAtIndex(toDelete);
            m_TaskGoalListProperty.DeleteArrayElementAtIndex(toDelete);
        }
    
        serializedObject.ApplyModifiedProperties();
    }
}

#endif
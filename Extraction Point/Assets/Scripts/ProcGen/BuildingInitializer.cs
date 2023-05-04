using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInitializer : MonoBehaviour
{
    [SerializeField] List<Transform> LootContainerPositions, EnemySpawnPositions;
    [SerializeField] List<string> ContainerNames;
    [SerializeField] List<GameObject> Containers, Enemies;
    [SerializeField] GameObject parent;
    public GenerateLoot lootGenerator;

    private void Awake() {
        lootGenerator = FindObjectOfType<GenerateLoot>();
        int index;
        foreach(Transform pos in LootContainerPositions)
        {
            index = Random.Range(0, ContainerNames.Count);
            StartCoroutine(lootGenerator.CreateLootableContainer(pos.position, ContainerNames[index], Containers[index], gameObject));          
        }
    }
}

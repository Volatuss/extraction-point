using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random=UnityEngine.Random;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] MapGenerator mapGen;
    [SerializeField] AudioSource ambientSound;
    [SerializeField] Tilemap groundMap, overlayMap, obstaceleTile;
    [SerializeField] RuleTile waterTile, grassTile;
    [SerializeField] List<GameObject> treeList, rockList;
    [SerializeField] GameObject smallBuildingPrefab, medBuildingPrefab, extractPrefab, loadingScreen;
    
    private int[,] map;
    private int mapWidth, mapHeight;
    public static bool beginBuilding = false, finishedBuilding = false;


    private void Awake() {
        StartCoroutine(WaitTime()); 
    }

    IEnumerator WaitTime(){
        while(!beginBuilding){
            yield return new WaitForSeconds(1f);
            Debug.Log(beginBuilding);
            yield return null;
        }
        MapGenerator.CurrentStage = "BuildingMap";
        Debug.Log(beginBuilding);
        map = mapGen.map;
        mapWidth = mapGen.mapWidth;
        mapHeight = mapGen.mapHeight;
        PlaceGroundTiles();
           
    }

    private void PlaceGroundTiles()
    {
        MapGenerator.CurrentStage = "Placing Ground Tiles";
        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                if(map[x, y] != 5 && map[x, y] != 4){
                    groundMap.SetTile(new Vector3Int(x, y, 0), grassTile);
                }
            }
        }
        PlaceOverlayTiles();
    }

    private void PlaceOverlayTiles()
    {
        MapGenerator.CurrentStage = "Placing Overlay Tiles";
        GameObject treeToPlace;
        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                if(map[x, y] == 2){
                    treeToPlace = Instantiate(treeList[Random.Range(0, 4)], new Vector3(x, y, 0), Quaternion.identity);
                    treeToPlace.transform.SetParent(overlayMap.transform);
                }
            }
        }
        PlaceObstacleTiles();
    }

    private void PlaceObstacleTiles()
    {
        MapGenerator.CurrentStage = "Placing Obstacle Tiles";
        GameObject rockToPlace, extractZone;
        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                if(map[x, y] == 4 || map[x, y] == 5 ){
                    obstaceleTile.SetTile(new Vector3Int(x, y, 0), waterTile);
                }else if(map[x, y] == 3){
                    rockToPlace = Instantiate(rockList[Random.Range(0, 11)], new Vector3(x, y, 0), Quaternion.identity);
                    rockToPlace.transform.SetParent(obstaceleTile.transform);
                }else if (map[x, y] == 6){
                    extractZone = Instantiate(extractPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
            }
        }
        
        finishedBuilding = true;
        PlaceBuildings();
        
    }

    private void PlaceBuildings()
    {
        MapGenerator.CurrentStage = "Placing Buildings";
        foreach(Vector2 coord in mapGen.s_BuildOrigin){ //later this can be randomized from a small list of small buildings, currently works
            GameObject smallBuildingToPlace = Instantiate(smallBuildingPrefab, new Vector3(coord.x, coord.y, 0), Quaternion.identity);
            //smallBuildingToPlace.transform.SetParent(overlayMap.transform);
        }
        foreach(Vector2 coord in mapGen.m_BuildOrigin){ //later this can be randomized from a small list of small buildings, currently works
            GameObject medBuildingToPlace = Instantiate(medBuildingPrefab, new Vector3(coord.x, coord.y, 0), Quaternion.identity);
            //smallBuildingToPlace.transform.SetParent(overlayMap.transform);
        }
        FinalizeMap();
    }

    private void FinalizeMap(){
        MapGenerator.playerTransform.position = MapGenerator.SpawnPosition;
        ambientSound.Play();
        loadingScreen.SetActive(false);
    }

}

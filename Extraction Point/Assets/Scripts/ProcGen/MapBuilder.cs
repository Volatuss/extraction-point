using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random=UnityEngine.Random;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] MapGenerator mapGen;
    [SerializeField] Tilemap groundMap, overlayMap, obstaceleTile;
    [SerializeField] RuleTile waterTile, grassTile;
    [SerializeField] List<GameObject> treeList;
    
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
        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                if(map[x, y] == 4 || map[x, y] == 5 ){
                    groundMap.SetTile(new Vector3Int(x, y, 0), waterTile);
                }
            }
        }
        
        finishedBuilding = true;
        FinalizeMap();
    }

    private void FinalizeMap(){
        MapGenerator.playerTransform.position = MapGenerator.SpawnPosition;
    }

}

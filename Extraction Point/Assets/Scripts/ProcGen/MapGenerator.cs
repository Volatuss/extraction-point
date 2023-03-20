using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    int[,] map; //grid of ints, 0 = empty, 1 = wall
    [SerializeField] int mapHeight, mapWidth, smoothingIterations = 5;
    [SerializeField] [Range(0, 100)] public int randomFillPercent, treeDensity, rockDensity; //density of walls to empty tiles
    [SerializeField] bool useRandomSeed = false;
    [SerializeField] String seed;

    private int xChunks, yChunks, totalChunks;
    int [,] chunkMap;

    private System.Random pseudoRandom;

    void Start()
    {
        pseudoRandom = new System.Random(seed.GetHashCode());
        GenerateMap();
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O)){
            GenerateMap();
        }

        if(Input.GetKeyDown(KeyCode.C)){
            Debug.Log(totalChunks);
            Debug.Log(chunkMap);
        }
    }

    private void SliceChunks(){ //smth broken here****
        xChunks = mapWidth / 32;
        yChunks = mapHeight / 32;
        totalChunks = yChunks * xChunks;
        chunkMap = new int[xChunks, yChunks];
        int chunkIndex = 1;
        for(int x = 0; x<xChunks; x++){
            for(int y = 0; y<yChunks; y++){
                chunkMap[x,y] = chunkIndex;
                chunkIndex++;
            }
        }
    }

    private int WhatChunk(int gridX, int gridY){
        int inChunkX = 0;
        int inChunkY = 0;
        for(int x = 0; x < xChunks; x++){
            if(gridX > x*32 && gridX <= (x + 1)*32){
                inChunkX = x;
            }
        }
        for(int y = 0; y < yChunks; y++){
            if(gridY > y*32 && gridY <= (y + 1)*32){
                inChunkY = y;
            }
        }
        return inChunkX;
    }

    private void GenerateMap()
    {
        map = new int[mapWidth, mapHeight];
        RandomFillMap();
        

        for(int i = 0; i < smoothingIterations; i++){
            SmoothMap();
        }   
        for(int i = 0; i< smoothingIterations * 2; i++){
            DrawBorders();
        }
        FillWater();
        //PlaceObstacles();



        PlaceBuildings();
        PlaceExtractionZones();
        PlaceSpawnPoint();
        SliceChunks();
    }

    private void PlaceSpawnPoint()
    {
        int randX = UnityEngine.Random.Range(5, mapWidth-5);
        int randY = UnityEngine.Random.Range(5, mapHeight-5);
        if(map[randX, randY] == 0){
            map[randX, randY] = 7;
            Debug.Log(randX + ", ");
            Debug.Log(randY);
            Debug.Log(WhatChunk(randX, randY));
        }else{
            PlaceSpawnPoint();
        }

    }

    private void PlaceExtractionZones()
    {
        
    }

    private void PlaceBuildings()
    {
        
    }

    private void RandomFillMap(){
        if(useRandomSeed){
            seed = Time.time.ToString();
        }

        

        for(int x = 0; x < mapWidth ; x++){
            for(int y = 0; y < mapHeight ; y++){
                if(x == 0 || x == mapWidth - 1 || y == 0 || y == mapHeight - 1){ //generating border tiles
                    map[x,y] = 1;
                }else{
                    map[x,y] = (pseudoRandom.Next(0, 100) < randomFillPercent) ? 1: 0;
                }                
            }
        }
    }

    private void SmoothMap(){
        for(int x = 0; x < mapWidth ; x++){
            for(int y = 0; y < mapHeight ; y++){
                int neighborWallTiles = GetNeighboringWallCount(x, y);
                if(neighborWallTiles > 4){
                    map[x,y] = 1;
                }else if(neighborWallTiles< 4) {
                    map[x,y] = 0;
                }

            }
        }
    }

    private int GetNeighboringWallCount(int gridX, int gridY){
        int wallCount = 0;
        for(int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX ++){ //loops through 3x3 grid centered on (gridX, gridY)
            for(int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY ++){
                if(neighborX >= 0  && neighborX < mapWidth && neighborY >= 0 && neighborY < mapHeight){
                    if(neighborX != gridX || neighborY != gridY){
                        wallCount += map[neighborX, neighborY]; //if wall (1) then it is added, else it adds 0
                    }
                }else{ //walls on edge
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    

    private void PlaceObstacles(){
        for(int x = 0; x < mapWidth ; x++){
            for(int y = 0; y < mapHeight ; y++){
                if(map[x, y] == 0){
                    map[x,y] = (pseudoRandom.Next(0, 100) < treeDensity) ? 2: map[x, y];
                    map[x,y] = (pseudoRandom.Next(0, 100) < rockDensity) ? 3: map[x, y];
                }
            }
        }
    }

    private void FillWater(){
        for(int x = 0; x < mapWidth ; x++){
            for(int y = 0; y < mapHeight ; y++){
                if(map[x, y] == 1){
                    map[x, y] = 4;
                }
            }
        }
    }

    private void DrawBorders(){
        for(int x = 0; x < mapWidth; x++){
            for(int y = 0; y < mapHeight; y++){
                if(map[x, y] == 1 && FillBorders(x, y)){
                    map[x,y] = 5;  
                }
            }
        }
        for(int x = mapWidth - 1; x > mapWidth ; x--){
            for(int y = mapHeight - 1; y > mapHeight; y--){
                if(map[x, y] == 1 && FillBorders(x, y)){
                    map[x,y] = 5;  
                }
            }
        }
        
    }
    private bool FillBorders(int gridX, int gridY){
        bool isBorder = false;
        for(int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX ++){ //loops through 3x3 grid centered on (gridX, gridY)
            for(int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY ++){
                if(neighborX >= 0  && neighborX < mapWidth && neighborY >= 0 && neighborY < mapHeight){
                    if(neighborX != gridX || neighborY != gridY){
                        if(map[neighborX, neighborY] == 5){
                            return true;
                        }
                    }
                }else{
                    return true;
                }
            }
        }

        return isBorder;
    }

    void OnDrawGizmos(){
        if(map == null){ return; }
        for(int x = 0; x < mapWidth ; x++){
            for(int y = 0; y < mapHeight ; y++){
                switch(map[x,y]){
                    case 0: //Normal ground tile
                        Gizmos.color = Color.white;
                        break;
                    case 1: //temp tile
                        Gizmos.color = Color.red;
                        break;
                    case 2: //Tree tile
                        Gizmos.color = Color.green;
                        break;
                    case 3: //Rock tile
                        Gizmos.color = Color.gray;
                        break;
                    case 4: //water tile
                        Gizmos.color = Color.blue;
                        break;
                    case 5: //border tile
                        Gizmos.color = Color.black;
                        break;
                    case 6: //Extract Zone
                        Gizmos.color = Color.magenta;
                        break;
                    case 7: //Spawn point
                        Gizmos.color = Color.cyan;
                        break;
                    default:
                        break;

                }
                Vector3 pos = new Vector3(-mapWidth/2 + x + .5f, -mapHeight/2 + y + .5f, 0);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
        
    }
}

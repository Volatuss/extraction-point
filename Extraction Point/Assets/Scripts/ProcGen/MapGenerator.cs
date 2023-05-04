using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static string CurrentStage = "";
    public static Vector2 SpawnPosition = Vector2.zero;
    public int[,] map; //grid of ints, 0 = empty, 1 = wall
    [SerializeField] [Range(0, 100)] public int randomFillPercent, treeDensity, rockDensity; //density of walls to empty tiles
    [SerializeField] bool useRandomSeed = false;
    [SerializeField] public int mapHeight, mapWidth, smoothingIterations = 7;
    [SerializeField] String seed;
    [HideInInspector] public int xChunks, yChunks, totalChunks, spawnPadding, chunkSize = 16, spawnChunk;
    
    public static Transform playerTransform = null;
    private int [,] chunkMap;
    public List<Vector2> s_BuildOrigin = new List<Vector2>(), m_BuildOrigin = new List<Vector2>(), l_BuildOrigin = new List<Vector2>();

    private int[] spawnChunks;
    private System.Random pseudoRandom;

    void Start()
    {
        CurrentStage = "Seeding";
        if(useRandomSeed){
            seed = Time.realtimeSinceStartup.ToString();
        }
        Debug.Log(seed);
        pseudoRandom = new System.Random(seed.GetHashCode());
        spawnPadding = 1/4 * mapWidth;
        GenerateMap();
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O)){
            GenerateMap();
        }

        if(Input.GetKeyDown(KeyCode.C)){
            Debug.Log(totalChunks);
            Debug.Log(chunkMap[3, 3]);
            
        }
    }
    
    public static void ResetStatics(){
        CurrentStage = "";
        SpawnPosition = Vector2.zero;
        MapBuilder.beginBuilding = false;
        MapBuilder.finishedBuilding = false;
        playerTransform = null;
    }

    private void GenerateMap()
    {
        CurrentStage = "GeneratingMap";
        map = new int[mapWidth, mapHeight];
        RandomFillMap();
        
        for(int i = 0; i < smoothingIterations; i++){
            SmoothMap();
        }   
        for(int i = 0; i< smoothingIterations * 2; i++){
            DrawBorders();
        }
        //Creating map data (chunks, zones, things that arent actually placing things)
        SliceChunks();
        CalculateBorderChunks();

        //Filling in map tiles, no obstacles yet
        FillWater();
        
        
        //Placing Objects in the map, comes last
        PlaceBuildings();
        PlaceExtractionZones();
        PlaceSpawnPoint();
        PlaceObstacles();
       
        //Should probably wait until all functions are completed, once that is done we will build the map
    }
    
    private void SliceChunks(){ //from left -> right, bot -> top
        xChunks = mapWidth / chunkSize;
        yChunks = mapHeight / chunkSize;
        totalChunks = yChunks * xChunks;
        chunkMap = new int[xChunks, yChunks];
        int chunkIndex = 0;
        for(int x = 0; x<xChunks; x++){
            for(int y = 0; y<yChunks; y++){
                chunkMap[y ,x] = chunkIndex;
                chunkIndex++;
            }
        }
    }

    private int WhatChunk(int gridX, int gridY){

        int testX = gridX / chunkSize, testY = gridY / chunkSize;

        int inChunkX = 0;
        int inChunkY = 0;
        for(int x = 0; x < xChunks; x++){
            if(gridX > x*chunkSize && gridX <= (x + 1)*chunkSize){
                inChunkX = x;
            }
        }
        for(int y = 0; y < yChunks; y++){
            if(gridY > y*chunkSize && gridY <= (y + 1)*chunkSize){
                inChunkY = y;
            }
        }
        return chunkMap[testX, testY];
    }

    private void PlaceSpawnPoint()
    {
        CurrentStage = "PlacingSpawnPoint";
        int randX = UnityEngine.Random.Range(5, mapWidth-5);
        int randY = UnityEngine.Random.Range(5, mapHeight-5);
        if(map[randX, randY] == 0  && BorderChunkCheck(randX, randY) && CheckSurroundTiles(8, 8, randX, randY)){
            map[randX, randY] = 7;
            spawnChunk = WhatChunk(randX, randY);
            Debug.Log("Spawn Chunk: " + spawnChunk + " At Coords: " + randX + ", " + randY);
            SpawnPosition = new Vector2(randX, randY);
        }else{
            PlaceSpawnPoint();
        }

    }

    private bool BorderChunkCheck( int x, int y ) // spawn chunks are the Nth outermost layers, need width, height, and spawn chunk band width 
    {   
        int chunkToCheck = WhatChunk(x, y);
        for(int i = 0; i < spawnChunks.Length ; i++){
            if(spawnChunks[i] == chunkToCheck){
                return true;
            }
        }
        
        return false;
    }

    private void CalculateBorderChunks() //allocates a 2 chunk band around the map for valid spawn chunks and extract Chunks
    {
        int count = 0;
        spawnChunks = new int[totalChunks - 2*(mapWidth/chunkSize)];
        
        for(int x = 0; x < mapWidth/chunkSize; x++) 
        {
            for(int y = 0; y < mapHeight/chunkSize; y++)
            {
                if(
                    x == 0 || y == 0 || 
                    y == mapWidth/chunkSize - 1 ||
                    x == mapWidth/chunkSize - 1 || 
                    x == 1 || y == 1 ||
                    y == mapWidth/chunkSize - 2 || 
                    x == mapWidth/chunkSize - 2 
                    )
                {
                    spawnChunks[count] = chunkMap[x, y];
                    count++;
                }
            }
        }   
        
    }

    private void PlaceExtractionZones()
    {
        CurrentStage = "PlacingExtracts";
        for(int i = 0; i < 2; i++){
            int randX = UnityEngine.Random.Range(5, mapWidth-5);
            int randY = UnityEngine.Random.Range(5, mapHeight-5);
            if(map[randX, randY] == 0  && BorderChunkCheck(randX, randY) && CheckSurroundTiles(5, 5, randX, randY)){
                map[randX, randY] = 6;
            }else{
                i--;
            }
        }
    }

    private bool CheckSurroundTiles(int tileCountX, int tileCountY, int x, int y){
        for(int i = - tileCountX; i < tileCountX; i++){
            for(int j = - tileCountY; j < tileCountY; j++){
                if(x - tileCountX < 0 || y - tileCountY < 0){ return false; }
                if(x-i < 0 || y-j < 0 || x+i >= mapWidth || y+j >= mapHeight){
                    return false;
                }else if(map[x + i, y + j] != 0){
                    return false;
                }
            }
        }
        return true;
    }

    private void ReserveArea( int x, int y, int xClear, int yClear){ //clear all tiles that arent ground or water
        for(int i = -xClear; i <= xClear; i ++){
            for(int j = -yClear; j <= yClear; j ++){ //sds
                map[i+x, j+y] = 10;
            }
        }
        map[x, y] = 8;
    }

    private void PlaceBuildings()
    {
        CurrentStage = "PlacingBuildings";
        int smallPlaced = 0, medPlaced = 0, largePlaced = 0; //number of placed 
        while(largePlaced < 1 ){
            int randX = UnityEngine.Random.Range(5, mapWidth-5);
            int randY = UnityEngine.Random.Range(5, mapHeight-5);
            if(map[randX, randY] == 0  && !BorderChunkCheck(randX, randY) && CheckSurroundTiles(16, 16, randX, randY)){
                map[randX, randY] = 8;
                l_BuildOrigin.Add(new Vector2(randX, randY));
                ReserveArea(randX, randY, 16, 16);
                largePlaced++;
            }
        }
        while(medPlaced < 2 ){
            int randX = UnityEngine.Random.Range(5, mapWidth-5);
            int randY = UnityEngine.Random.Range(5, mapHeight-5);
            if(map[randX, randY] == 0  && CheckSurroundTiles(16, 16, randX, randY)){
                map[randX, randY] = 8;
                m_BuildOrigin.Add(new Vector2(randX, randY));
                ReserveArea(randX, randY, 16, 16);
                medPlaced++;
            }
        }
        while(smallPlaced < 3 ){
            int randX = UnityEngine.Random.Range(5, mapWidth-5);
            int randY = UnityEngine.Random.Range(5, mapHeight-5);
            if(map[randX, randY] == 0  && CheckSurroundTiles(8, 8, randX, randY)){
                map[randX, randY] = 8;
                s_BuildOrigin.Add(new Vector2(randX, randY));
                ReserveArea(randX, randY, 8, 8);
                smallPlaced++;
            }
        }

    }

    private void RandomFillMap(){
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
                    if(CheckSurroundTiles(1,1,x,y)){
                        map[x,y] = (pseudoRandom.Next(0, 100) < treeDensity) ? 2: map[x, y];
                    }
                    if(CheckSurroundTiles(2,2,x,y)){
                        map[x,y] = (pseudoRandom.Next(0, 100) < rockDensity) ? 3: map[x, y];
                    }
                }
            }
        }

        MapBuilder.beginBuilding = true;
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

    /*void OnDrawGizmos(){
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
                    case 8: //building center
                        Gizmos.color = Color.yellow;
                        break;
                    case 10:
                        Gizmos.color = Color.red;
                        break;
                    default:
                        break;

                }
                Vector3 pos = new Vector3(-mapWidth + x - 10.5f, y, 0);
                Gizmos.DrawCube(pos, Vector3.one);
            }
        }
        
    }*/
}

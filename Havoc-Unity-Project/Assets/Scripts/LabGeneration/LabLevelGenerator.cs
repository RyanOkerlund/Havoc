﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 LevelGenerator Class:
 This class extends the Generator class to spawn a Level with a random rooms pattern.
 Works in conjuction with the RoomGenerator Class to create a unique dungeon level          
*/
public class LabLevelGenerator : Generator // Extends Generator Class
{
    public enum wallTypes { closed, open, door, empty }
    public struct wall {
        public Vector2 direction;
        public wallTypes wallType;
    }

    public int minNumRooms;
    public int maxNumRooms;
    public Room entranceRoom;
    public Room exitRoom;
    public Room borderRoom;
    public Room emptyRoom;
    public List<Room> rooms;

    private const int UP_INDEX = 0;
    private const int RIGHT_INDEX = 1;
    private const int DOWN_INDEX = 2;
    private const int LEFT_INDEX = 3;

    public struct LevelSpace {
        public Vector2Int gridPos;
        public UnityEngine.Vector2 spawnPos;
        public int numberOpenings;
        public Room room;
        // public Vector2 dirCameFrom;
        public wall[] walls;
        public LevelSpace[] adjacentRooms;
        public List<UnityEngine.Vector2> directionsToMove;
    }
    public LevelSpace[,] level; // The grid of the level
    private Transform roomList;

    public Vector2Int roomSize;
    public int hallwayWidth;
    public UnityEngine.Vector2 levelGridSize; // The size of the level in grid spaces

    public LabRoomGenerator labRoomGenPrefab; // The RoomGenerator prefab to make rooms

    // Runs all the code to make the level
    private void Start() {
        ValidateInput();
        Setup();
        GenerateLevel();        
        SpawnLevel();
    }

    #region Setup

    private void ValidateInput(){
        if (roomSize.x % 2 == 0) {
            roomSize.x -= 1;
        }
        if (roomSize.x == 1) {
            roomSize.x = 3;
        }
        if (roomSize.y % 2 == 0) {
            roomSize.y -= 1;
        }
        if (roomSize.y == 1) {
            roomSize.y = 3;
        }
        if (hallwayWidth % 2 == 0) {
            hallwayWidth -= 1;
        }
        if (hallwayWidth == 1) {
            hallwayWidth = 3;
        }
        labRoomGenPrefab.roomSize = roomSize;
        labRoomGenPrefab.hallwayWidth = hallwayWidth;
    }
    // Prepares the level for generation
    public void Setup() {
        roomList = transform.GetChild(0).transform;
        // Sets the level grid size in Unity world units
        worldUnitsPerOneGridCell = roomSize;
        gridSizeWorldUnits.x = worldUnitsPerOneGridCell.x * levelGridSize.x;
        gridSizeWorldUnits.y = worldUnitsPerOneGridCell.y * levelGridSize.y;        

        SetupGridSize();

        // Initialize the level grid
        level = new LevelSpace[gridWidth, gridHeight];

        // Set all the level grid spaces to empty
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (x == 0 || x == gridWidth - 1 || y == 0 || y == gridHeight - 1) {
                    level[x, y].room = borderRoom;
                }
                else {
                    level[x, y].room = emptyRoom;
                }
                level[x, y].gridPos = new Vector2Int(x, y);
                level[x, y].numberOpenings = 0;                
                level[x, y].walls = new wall[4];
                SetupWallDirections(x, y);
                level[x, y].adjacentRooms = new LevelSpace[4];
                level[x, y].directionsToMove = new List<UnityEngine.Vector2>();
            }
        }
    }

    private void SetupWallDirections(int x, int y) {    
        for (int i = 0; i < 4; i++) {
            wall wall = new wall();
            switch (i) {
                case UP_INDEX:
                    wall.direction = Vector2.up;
                    break;
                case RIGHT_INDEX:
                    wall.direction = Vector2.right;
                    break;
                case DOWN_INDEX:
                    wall.direction = Vector2.down;
                    break;
                default:
                    wall.direction = Vector2.left;
                    break;
            }
            if (level[x, y].room.type == Room.RoomTypes.border) {
                wall.wallType = wallTypes.closed;
            }
            else {
                wall.wallType = wallTypes.empty;
            }            
            level[x, y].walls[i] = wall;
        }  
    }
    #endregion

    private void GenerateLevel() {
        Vector2Int entrancePos = GenerateEntrance();
        GenerateRooms(entrancePos.x, entrancePos.y, entrancePos.x, entrancePos.y);
    }

    // Sets middle room to an entrance type
    private Vector2Int GenerateEntrance() {
        Vector2Int spawnPos = new Vector2Int(Mathf.FloorToInt(gridWidth / 2.0f), Mathf.FloorToInt(gridHeight / 2.0f));
        SetupRoom(spawnPos.x, spawnPos.y, entranceRoom);
        return spawnPos;
    }

    #region GenerateRooms
    // Use generators to spawn rooms
    private void GenerateRooms(int x, int y, int prevX, int prevY) {
        if (level[x, y].directionsToMove.Count == 0 || IsBoundaryRoom(level[x, y])) {
            return;
        }
        List<generator> localGenerators = new List<generator>();

        foreach (UnityEngine.Vector2 dir in level[x, y].directionsToMove) {
            SpawnGen(dir, level[x, y].gridPos, localGenerators);
        }
        MoveGen(localGenerators);

        // Change the location of each generator to a room
        foreach (generator targetGen in localGenerators) {
            if (level[(int)targetGen.pos.x, (int)targetGen.pos.y].room.type == Room.RoomTypes.empty) {
                if (HasLessThanMaxNumRooms()) {
                    SetupRoom((int)targetGen.pos.x, (int)targetGen.pos.y);             
                    GenerateRooms((int)targetGen.pos.x, (int)targetGen.pos.y, x, y);
                }
                else {
                    for (int i = 0; i < 4; i++) {
                        if (level[x, y].walls[i].direction == targetGen.dir) {
                            level[x, y].walls[i].wallType = wallTypes.closed;
                        }
                    }
                }
            }
        }
    }

    private bool IsBoundaryRoom(LevelSpace levelSpace) {
        return levelSpace.room.type == Room.RoomTypes.border;
    }

    private bool HasLessThanMinNumRooms() {
        int count = 0;
        foreach (LevelSpace levelSpace in level)
        {
            if (levelSpace.room.isCountedAsRoom)
            {
                count++;
            }
        }
        return count < minNumRooms;
    }

    // Returns the number of rooms in the level
    private bool HasLessThanMaxNumRooms() {
        int count = 0;
        foreach (LevelSpace levelSpace in level)
        {
            if (levelSpace.room.isCountedAsRoom)
            {
                count++;
            }
        }
        return count < maxNumRooms;
    }
    #endregion

    #region RoomSetup
    private void SetupRoom(int x, int y) {
        GetAdjacentRooms(x, y);
        SetupAdjacentWalls(x, y);  
        RandomizeRoom(x, y);  
        RandomizeRemainingWalls(x, y);
        GetDirectionsToMove(x, y);   
    }

    // For setting up room with a given room scriptable object (Entrance and Exit so far)
    private void SetupRoom(int x, int y, Room room) {
        GetAdjacentRooms(x, y);
        SetupAdjacentWalls(x, y);
        level[x, y].room = room;   
        RandomizeRemainingWalls(x, y);
        GetDirectionsToMove(x, y);        
    }

    public void GetAdjacentRooms(int x, int y) {
        for (int i = 0; i < 4; i++) {
            switch (i) {
                case UP_INDEX:
                    level[x, y].adjacentRooms[i] = level[x, y + 1];
                    break;
                case RIGHT_INDEX:
                    level[x, y].adjacentRooms[i] = level[x + 1, y];
                    break;
                case DOWN_INDEX:
                    level[x, y].adjacentRooms[i] = level[x, y - 1];
                    break;
                default:
                    level[x, y].adjacentRooms[i] = level[x - 1, y];
                    break;
            }
        }
    }

    private void SetupAdjacentWalls(int x, int y) {
        for (int i = 0; i < 4; i++) {
            switch (i) {
                case UP_INDEX:
                    if (!IsBoundaryRoom(level[x, y].adjacentRooms[UP_INDEX])) {
                        if (level[x, y].adjacentRooms[UP_INDEX].walls[DOWN_INDEX].wallType == wallTypes.door) {
                            level[x, y].walls[UP_INDEX].wallType = wallTypes.door;
                            level[x, y].numberOpenings++;
                        }
                        else if (level[x, y].adjacentRooms[UP_INDEX].walls[DOWN_INDEX].wallType == wallTypes.closed) {
                            level[x, y].walls[UP_INDEX].wallType = wallTypes.closed;
                        }
                    }
                    else {
                        level[x, y].walls[UP_INDEX].wallType = wallTypes.closed;
                    }                      
                    break;
                case RIGHT_INDEX:
                    if (!IsBoundaryRoom(level[x, y].adjacentRooms[RIGHT_INDEX])) {
                        if (level[x, y].adjacentRooms[RIGHT_INDEX].walls[LEFT_INDEX].wallType == wallTypes.door) {            
                            level[x, y].walls[RIGHT_INDEX].wallType = wallTypes.door;
                            level[x, y].numberOpenings++;
                        }
                        else if (level[x, y].adjacentRooms[RIGHT_INDEX].walls[LEFT_INDEX].wallType == wallTypes.closed) {            
                            level[x, y].walls[RIGHT_INDEX].wallType = wallTypes.closed;
                        }
                    }
                    else {
                        level[x, y].walls[RIGHT_INDEX].wallType = wallTypes.closed;
                    }                    
                    break; 
                case DOWN_INDEX:
                    if (!IsBoundaryRoom(level[x, y].adjacentRooms[DOWN_INDEX])) {
                        if (level[x, y].adjacentRooms[DOWN_INDEX].walls[UP_INDEX].wallType == wallTypes.door) {        
                            level[x, y].walls[DOWN_INDEX].wallType = wallTypes.door;
                            level[x, y].numberOpenings++;
                        } 
                        else if (level[x, y].adjacentRooms[DOWN_INDEX].walls[UP_INDEX].wallType == wallTypes.closed) {        
                            level[x, y].walls[DOWN_INDEX].wallType = wallTypes.closed;
                        }
                    }
                    else {
                        level[x, y].walls[DOWN_INDEX].wallType = wallTypes.closed;
                    }
                    break;            
                default:
                    if (!IsBoundaryRoom(level[x, y].adjacentRooms[LEFT_INDEX])) {
                        if (level[x, y].adjacentRooms[LEFT_INDEX].walls[RIGHT_INDEX].wallType == wallTypes.door) {        
                            level[x, y].walls[LEFT_INDEX].wallType = wallTypes.door;
                            level[x, y].numberOpenings++;
                        }
                        else if (level[x, y].adjacentRooms[LEFT_INDEX].walls[RIGHT_INDEX].wallType == wallTypes.closed) {        
                            level[x, y].walls[LEFT_INDEX].wallType = wallTypes.closed;
                        }
                    }
                    else {
                        level[x, y].walls[LEFT_INDEX].wallType = wallTypes.closed; 
                    }
                    break;
            }
        }
    }

    private void RandomizeRoom(int x, int y) {        
        bool hasChosenRoom = false;

        while (!hasChosenRoom) {
            int choice = Mathf.FloorToInt(Random.value * (float)(rooms.Count - 0.01));
            float rand = Random.value;
            if (rand <= rooms[choice].chanceToSpawn) {
                if (rooms[choice].type == Room.RoomTypes.hallway) {
                    if (IsValidHallwaySpace(x, y)) {
                        level[x, y].room = rooms[choice];
                        hasChosenRoom = true;
                        break;
                    }
                }
                else {
                    if (IsLowerThanMaxAllowed(rooms[choice])) {
                        level[x, y].room = rooms[choice];
                        hasChosenRoom = true;
                        break;
                    }
                }
            }
        }
    }

    private void RandomizeRemainingWalls(int x, int y) {
        while (level[x, y].numberOpenings < level[x, y].room.minNumDoors) {
            for (int i = 0; i < 4; i++) {
                float rand = Random.value;
                if (rand <= level[x, y].room.chanceToSpawnDoor && level[x, y].walls[i].wallType == wallTypes.empty) {
                    level[x, y].walls[i].wallType = wallTypes.door;
                    level[x, y].numberOpenings++;
                    break;
                }
            }
        }
        // Chance to spawn more doors
        for (int i = 0; i < 4; i++) {
            float rand = Random.value;
            if (rand <= level[x, y].room.chanceToSpawnDoor && level[x, y].walls[i].wallType == wallTypes.empty
                && level[x, y].numberOpenings < level[x, y].room.maxNumDoors) { 
                level[x, y].walls[i].wallType = wallTypes.door;
                level[x, y].numberOpenings++;
            }
        }

        for (int i = 0; i < 4; i++) {
            if (level[x, y].walls[i].wallType == wallTypes.empty) {
                level[x, y].walls[i].wallType = wallTypes.closed;
            }
        }
    }

    public void GetDirectionsToMove(int x, int y) {
        for (int i = 0; i < 4; i++) {
                if (level[x, y].walls[i].wallType == wallTypes.door && 
                    level[x, y].adjacentRooms[i].room.type == Room.RoomTypes.empty) {
                    switch (i) {
                        case UP_INDEX:
                        level[x, y].directionsToMove.Add(UnityEngine.Vector2.up);
                            break;
                        case RIGHT_INDEX:
                        level[x, y].directionsToMove.Add(UnityEngine.Vector2.right);
                            break;
                        case DOWN_INDEX:
                        level[x, y].directionsToMove.Add(UnityEngine.Vector2.down);
                            break;
                        default:
                        level[x, y].directionsToMove.Add(UnityEngine.Vector2.left);
                            break;
                    }
                }
        }
    }

    private bool IsValidHallwaySpace(int x, int y) {
        int count = 0;
        foreach (LevelSpace room in level[x, y].adjacentRooms) {
            if (room.room.type == Room.RoomTypes.empty) {
                count++;
            }
        }

        return count >= 1;
    }

    public bool IsLowerThanMaxAllowed(Room room) {
        int count = 0;
        for (int x = 1; x < gridWidth - 1; x++) {
            for (int y = 1; y < gridHeight - 1; y++) {
                if (level[x, y].room.type == room.type) {
                    count++;
                }
            }
        }
        
        return count < room.maxNumAllowedPerLevel;
    }
    #endregion

    #region GenerateExit
    // Set a random room in the level grid to an exit
    private void SetExit()
    {
        bool isExitSet = false;
        while (!isExitSet)
        {
            int x = Mathf.FloorToInt(Random.value * (gridWidth - 1));
            int y = Mathf.FloorToInt(Random.value * (gridHeight - 1));
            // So long as that location is a regular room and isn't next to the entrance and is next to another room, set exit
            if (level[x, y].room.type == Room.RoomTypes.room && IsNotNextToEntrance(x, y) && IsNextToAnotherRoom(x, y))
            {
                level[x, y].room.type = Room.RoomTypes.exit;
                isExitSet = true;
            }
        }
    }

    // Checks if the given location is next to the entrance room
    private bool IsNotNextToEntrance(int x, int y)
    {
        return level[x + 1, y].room.type != Room.RoomTypes.entrance && level[x - 1, y].room.type != Room.RoomTypes.entrance
            && level[x, y + 1].room.type != Room.RoomTypes.entrance && level[x, y - 1].room.type != Room.RoomTypes.entrance;
    }

    // Checks if the location is next to another room
    private bool IsNextToAnotherRoom(int x, int y)
    {
        return level[x + 1, y].room.type == Room.RoomTypes.room || level[x - 1, y].room.type == Room.RoomTypes.room ||
            level[x, y + 1].room.type == Room.RoomTypes.room || level[x, y - 1].room.type == Room.RoomTypes.room;
    }
    #endregion

    #region Spawning
    // Loops through each level grid space and spawns a room (RoomGenerator Class does most of the work)
    private void SpawnLevel()
    {
        for (int x = 1; x < gridWidth - 1; x++)
        {
            for (int y = 1; y < gridHeight - 1; y++)
            {
                if (level[x, y].room.type != Room.RoomTypes.border && level[x, y].room.type != Room.RoomTypes.empty) {
                    Spawn(level[x, y], x, y);
                }
            }
        }
    }

    // Instantiates a room at the given location
    private void Spawn(LevelSpace levelSpace, float xPos, float yPos)
    {
        UnityEngine.Vector2 offset = gridSizeWorldUnits / 2.0f;
        UnityEngine.Vector2 spawnPos = new UnityEngine.Vector2(xPos, yPos) * worldUnitsPerOneGridCell - offset;
        levelSpace.spawnPos = spawnPos;
        LabRoomGenerator roomToSpawn = Instantiate(labRoomGenPrefab, spawnPos, Quaternion.identity, roomList);
        roomToSpawn.init(levelSpace);
    } 
    #endregion
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 LevelGenerator Class:
 This class extends the Generator class to spawn a Level with a random rooms pattern.
 Works in conjuction with the RoomGenerator Class to create a unique dungeon level          
*/
public class LevelGenerator : Generator // Extends Generator Class
{
    public enum wallDirections { up, right, down, left }
    public enum wallTypes { closed, open, door }
    public struct wall {
        public wallDirections direction;
        public wallTypes wallType;
        public bool hasDoor;
    }
    public List<Room> rooms;

    private const int EMPTY_ROOM_INDEX = 0;
    private const int ROOM_INDEX = 1;
    private const int ENTRANCE_INDEX = 2;
    private const int EXIT_INDEX = 3;
    private const int BOSS_ROOM_INDEX = 4;
    private const int LORE_ROOM_INDEX = 5;
    private const int HALLWAY_INDEX = 6;
    private const int BORDER_INDEX = 7;

    private const int UP_INDEX = 0;
    private const int RIGHT_INDEX = 1;
    private const int DOWN_INDEX = 2;
    private const int LEFT_INDEX = 3;

    public struct LevelSpace {
        public Vector2Int gridPos;
        public Vector2 spawnPos;
        public int doorCount;
        public Room room;
        public wall[] walls;
        public LevelSpace[] adjacentRooms;
        public List<Vector2> directionsToMove;
    }
    public LevelSpace[,] level; // The grid of the level
    private Transform roomList;

    public Vector2 levelGridSize; // The size of the level in grid spaces
    public int offsetRoomPadding; // The space in between the rooms

    public LabRoomGenerator labRoomGenPrefab; // The RoomGenerator prefab to make rooms

    // Runs all the code to make the level
    private void Start()
    {
        Setup();
        GenerateLevel();        
        // SetExit();
        SpawnLevel();
    }

    // Prepares the level for generation
    public void Setup()
    {
        roomList = transform.GetChild(0).transform;
        // Sets the level grid size in Unity world units
        worldUnitsPerOneGridCell = labRoomGenPrefab.roomSize.x + offsetRoomPadding;
        gridSizeWorldUnits.x = worldUnitsPerOneGridCell * levelGridSize.x;
        gridSizeWorldUnits.y = worldUnitsPerOneGridCell * levelGridSize.y;        

        SetupGridSize();

        // Initialize the level grid
        level = new LevelSpace[gridWidth, gridHeight];

        // Set all the level grid spaces to empty
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (x == 0 || x == gridWidth - 1 || y == 0 || y == gridHeight - 1) {
                    level[x, y].room = rooms[BORDER_INDEX];
                }
                else {
                    level[x, y].room = rooms[EMPTY_ROOM_INDEX];
                }
                level[x, y].gridPos = new Vector2Int(x, y);
                level[x, y].doorCount = 0;                
                level[x, y].walls = new wall[4];
                SetupWallDirections(level[x, y]);
                level[x, y].adjacentRooms = new LevelSpace[4];
                level[x, y].directionsToMove = new List<Vector2>();
            }
        }
    }

    private void GenerateLevel() {
        Vector2Int entrancePos = GenerateEntrance();
        GenerateRooms(entrancePos.x, entrancePos.y);
    }
    // Sets middle room to an entrance type
    private Vector2Int GenerateEntrance() {
        Vector2Int spawnPos = new Vector2Int(Mathf.FloorToInt(gridWidth / 2.0f), Mathf.FloorToInt(gridHeight / 2.0f));
        level[spawnPos.x, spawnPos.y].room = rooms[ENTRANCE_INDEX];
        SetupRoom(spawnPos.x, spawnPos.y);
        return spawnPos;
    }

    #region GenerateRooms
    // Use generators to spawn rooms
    private void GenerateRooms(int x, int y) {
        if (level[x, y].directionsToMove.Count == 0 || isBoundaryRoom(level[x, y])) {
            return;
        }
        List<generator> localGenerators = new List<generator>();

        foreach (Vector2 dir in level[x, y].directionsToMove) {
            SpawnGen(dir, level[x, y].gridPos, localGenerators);
        }
        MoveGen(localGenerators);
        ClampGen(localGenerators);
        // Change the location of each generator to a room
        foreach (generator targetGen in localGenerators)
        {
            level[(int)targetGen.pos.x, (int)targetGen.pos.y].room = rooms[ROOM_INDEX];
            SetupRoom((int)targetGen.pos.x, (int)targetGen.pos.y);
            GenerateRooms((int)targetGen.pos.x, (int)targetGen.pos.y);          
        }
    }

    private bool isBoundaryRoom(LevelSpace levelSpace) {
        return levelSpace.room.type == Room.RoomTypes.border;
    }

    private bool isCovered() {
        return (float)NumberOfRooms() / (float)level.Length > percentGridCovered;
    }

    // Returns the number of rooms in the level
    private int NumberOfRooms()
    {
        int count = 0;
        foreach (LevelSpace levelSpace in level)
        {
            if (levelSpace.room.type == Room.RoomTypes.room)
            {
                count++;
            }
        }
        return count;
    } 
    #endregion

    #region RoomSetup
    private void SetupRoom(int x, int y) {
        GetAdjacentRooms(x, y);
        SetupAdjacentWalls(x, y);   
        RandomizeRemainingWalls(x, y);
        GetDirectionsToMove(x, y);    
    }

    private void SetupWallDirections(LevelSpace levelSpace) {    
        for (int i = 0; i < 4; i++) {
            wall wall = new wall();
            switch (i) {
                case UP_INDEX:
                    wall.direction = wallDirections.up;
                    break;
                case RIGHT_INDEX:
                    wall.direction = wallDirections.right;
                    break;
                case DOWN_INDEX:
                    wall.direction = wallDirections.down;
                    break;
                default:
                    wall.direction = wallDirections.left;
                    break;
            }
            wall.wallType = wallTypes.closed;
            wall.hasDoor = false;
            levelSpace.walls[i] = wall;
        }  
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
                    if (level[x, y].adjacentRooms[UP_INDEX].walls[DOWN_INDEX].hasDoor) {
                        level[x, y].walls[UP_INDEX].hasDoor = true;
                        level[x, y].walls[UP_INDEX].wallType = wallTypes.door;
                        level[x, y].doorCount++;
                    }       
                    break;
                case RIGHT_INDEX:
                    if (level[x, y].adjacentRooms[RIGHT_INDEX].walls[LEFT_INDEX].hasDoor) {
                        level[x, y].walls[RIGHT_INDEX].hasDoor = true;
                        level[x, y].walls[RIGHT_INDEX].wallType = wallTypes.door;
                        level[x, y].doorCount++;
                    }
                    break; 
                case DOWN_INDEX:
                    if (level[x, y].adjacentRooms[DOWN_INDEX].walls[UP_INDEX].hasDoor) {
                        level[x, y].walls[DOWN_INDEX].hasDoor = true;
                        level[x, y].walls[DOWN_INDEX].wallType = wallTypes.door;
                        level[x, y].doorCount++;
                    }     
                    break;            
                case LEFT_INDEX:
                    if (level[x, y].adjacentRooms[LEFT_INDEX].walls[RIGHT_INDEX].hasDoor) {
                        level[x, y].walls[LEFT_INDEX].hasDoor = true;
                        level[x, y].walls[LEFT_INDEX].wallType = wallTypes.door;
                        level[x, y].doorCount++;
                    }  
                    break;
                default:
                    break;
            }
        }
    }

    private void RandomizeRemainingWalls(int x, int y) {
        while (level[x, y].doorCount < level[x, y].room.minNumDoors) {
            for (int i = 0; i < 4; i++) {
                if (!(level[x, y].walls[i].hasDoor)) {
                    float rand = Random.value;
                    if (rand <= level[x, y].room.chanceToSpawnDoor) {
                        level[x, y].walls[i].hasDoor = true;
                        level[x, y].walls[i].wallType = wallTypes.door;
                        level[x, y].doorCount++;
                        break;
                    }
                }
            }
        }
        // Chance to spawn more doors
        for (int i = 0; i < 4; i++) {
            if (!(level[x, y].walls[i].hasDoor)) {
                float rand = Random.value;
                if (rand <= level[x, y].room.chanceToSpawnDoor) { 
                    level[x, y].walls[i].hasDoor = true;
                    level[x, y].walls[i].wallType = wallTypes.door;
                    level[x, y].doorCount++;
                }
            }
        }
    }

    public void GetDirectionsToMove(int x, int y) {
        for (int i = 0; i < 4; i++) {
                if (level[x, y].walls[i].hasDoor && level[x, y].adjacentRooms[i].room.type == Room.RoomTypes.empty) {
                    switch (i) {
                        case UP_INDEX:
                            level[x, y].directionsToMove.Add(Vector2.up);
                            break;
                        case RIGHT_INDEX:
                            level[x, y].directionsToMove.Add(Vector2.right);
                            break;
                        case DOWN_INDEX:
                            level[x, y].directionsToMove.Add(Vector2.down);
                            break;
                        default:
                            level[x, y].directionsToMove.Add(Vector2.left);
                            break;
                    }
                }
        }
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
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (level[x, y].room.type == Room.RoomTypes.room) {
                    Spawn(level[x, y], x, y);
                }
                else if (level[x, y].room.type == Room.RoomTypes.entrance) {
                    Spawn(level[x, y], x, y);
                }
                else {
                    continue;
                }
            }
        }
    }

    // Instantiates a room at the given location
    private void Spawn(LevelSpace levelSpace, float xPos, float yPos)
    {
        Vector2 offset = gridSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(xPos, yPos) * worldUnitsPerOneGridCell - offset;
        levelSpace.spawnPos = spawnPos;
        LabRoomGenerator roomToSpawn = Instantiate(labRoomGenPrefab, spawnPos, Quaternion.identity, roomList);
        roomToSpawn.init(levelSpace);
    } 
    #endregion
}
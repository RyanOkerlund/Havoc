using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Tilemaps used to spawn the room on a Tilemap
using UnityEngine.Tilemaps;

/*
 RoomGenerator Class:
 This class extends the Generator class to spawn a room with a random floor pattern.
 Works in conjuction with the LevelGenerator Class to create a unique dungeon level          
*/
public class RoomGenerator : Generator // Extends Generator Class
{
    LevelGenerator level = null; // The grid of the entire level
    public enum roomSpace { empty, floor, wall, door, entrancePortal, exitPortal } // Room space types
    public roomSpace[,] room; // The grid of the room

    public Vector2 roomPositionInLevel; // The room's position in the level
    public LevelGenerator.levelSpace roomType; // This room's type (see levelSpace enum in LevelGenerator Class)

    public List<TileBase> floorsPalette; // The list of tiles to draw from for floors 
    public List<TileBase> wallsPalette; // The list of tiles to draw from for walls
    public List<TileBase> doorsPalette; // The list of tiles to draw from for doors
    public List<TileBase> entrancePortalsPalette; // The list of tiles to draw from for entrance portals
    public List<TileBase> exitPortalsPalette; // The list of tiles to draw from for exit portals

    GameObject map; // The parent tilemap grid
    Tilemap floorTilemap, wallTilemap, doorTilemap, portalTilemap; // The respective tilemaps for each type of tile

    // Runs all the code to make the room
    private void Start()
    {
        Setup();
        GenerateFloor();
        GenerateAllDoors();
        GeneratePortal();
        SpawnRoom();
    }

    #region SetupRoom
    // Prepares the room for generation
    public void Setup()
    {
        // Get the reference for the level
        level = GetComponentInParent<LevelGenerator>();

        SetupTilemaps();
        SetupGridSize();

        // Initialize the room grid
        room = new roomSpace[gridWidth, gridHeight];

        // Set all the room spaces to a wall
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                room[x, y] = roomSpace.wall;
            }
        }

        SetupFirstGenerator();
    }

    // Get all the references for the respective tilemaps
    private void SetupTilemaps()
    {
        map = transform.GetChild(0).gameObject;
        floorTilemap = map.transform.GetChild(0).GetComponent<Tilemap>();
        wallTilemap = map.transform.GetChild(1).GetComponent<Tilemap>();
        doorTilemap = map.transform.GetChild(2).GetComponent<Tilemap>();
        portalTilemap = map.transform.GetChild(3).GetComponent<Tilemap>();
    }
    #endregion

    #region FloorGeneration
    // Use the generators to spawn the floor in the room
    private void GenerateFloor()
    {
        int iterations = 0;
        int maxIterations = 100000;
        while (iterations < maxIterations)
        {
            // Change the location of each generator to a floor
            foreach (generator targetGen in generators)
            {
                room[(int)targetGen.pos.x, (int)targetGen.pos.y] = roomSpace.floor;
            }

            // See Generator Class
            DestoryGen();
            SpawnGen();
            ChangeGenDir();
            MoveGen();
            ClampGen();

            // If enough of the room grid is floors then be done
            if ((float)NumberOfFloors() / (float)room.Length > percentGridCovered)
            {
                break;
            }

            iterations++;
        }
    }

    // Returns the number of floors in the room
    private int NumberOfFloors()
    {
        int count = 0;
        foreach (roomSpace space in room)
        {
            if (space == roomSpace.floor)
            {
                count++;
            }
        }
        return count;
    } 
    #endregion

    #region DoorGeneration
    // Create the doors for this room based upon adjacent rooms in the level
    private void GenerateAllDoors()
    {
        // Only do this if this room is part of level
        if (level != null)
        {
            List<Vector2> surroundingRoomsLocations = GetAdjacentRooms();
            foreach (Vector2 dir in surroundingRoomsLocations)
            {
                // Generate doors in each direciton where there is an adjacent room
                GenerateDoors(dir);
            } 
        }
    }

    // Returns a list of directions of the rooms adjacent to this room
    private List<Vector2> GetAdjacentRooms()
    {
        List<Vector2> adjacentRoomsLocations = new List<Vector2>();
        if (level.level[(int)roomPositionInLevel.x, (int)roomPositionInLevel.y] != LevelGenerator.levelSpace.empty)
        {
            if (level.level[(int)(roomPositionInLevel.x + 1), (int)roomPositionInLevel.y] != LevelGenerator.levelSpace.empty)
            {
                adjacentRoomsLocations.Add(Vector2.right);
            }
            if (level.level[(int)(roomPositionInLevel.x - 1), (int)roomPositionInLevel.y] != LevelGenerator.levelSpace.empty)
            {
                adjacentRoomsLocations.Add(Vector2.left);
            }
            if (level.level[(int)roomPositionInLevel.x, (int)(roomPositionInLevel.y + 1)] != LevelGenerator.levelSpace.empty)
            {
                adjacentRoomsLocations.Add(Vector2.up);
            }
            if (level.level[(int)roomPositionInLevel.x, (int)(roomPositionInLevel.y - 1)] != LevelGenerator.levelSpace.empty)
            {
                adjacentRoomsLocations.Add(Vector2.down);
            }
        }
        return adjacentRoomsLocations;
    }

    // Generate a door given a direction to an adjacent room
    private void GenerateDoors(Vector2 dir)
    {
        bool isDoorSpawned = false;
        if (dir == Vector2.right)
        {
            for (int x = gridWidth - 2; x > 0; x--)
            {
                for (int y = 0; y < gridHeight - 1; y++)
                {
                    if (room[x, y] == roomSpace.floor)
                    {
                        room[x + 1, y] = roomSpace.door;
                        isDoorSpawned = true;
                        break;
                    }
                }
                if (isDoorSpawned == true)
                {
                    break;
                }
            }
        }
        else if (dir == Vector2.left)
        {
            for (int x = 0; x < gridWidth - 1; x++)
            {
                for (int y = 0; y < gridHeight - 1; y++)
                {
                    if (room[x, y] == roomSpace.floor)
                    {
                        room[x - 1, y] = roomSpace.door;
                        isDoorSpawned = true;
                        break;
                    }
                }
                if (isDoorSpawned == true)
                {
                    break;
                }
            }
        }
        else if (dir == Vector2.up)
        {
            for (int y = gridHeight - 2; y > 0; y--)
            {
                for (int x = 0; x < gridWidth - 1; x++)
                {
                    if (room[x, y] == roomSpace.floor)
                    {
                        room[x, y + 1] = roomSpace.door;
                        isDoorSpawned = true;
                        break;
                    }
                }
                if (isDoorSpawned == true)
                {
                    break;
                }
            }
        }
        else if (dir == Vector2.down)
        {
            for (int y = 0; y < gridHeight - 1; y++)
            {
                for (int x = 0; x < gridWidth - 1; x++)
                {
                    if (room[x, y] == roomSpace.floor)
                    {
                        room[x, y - 1] = roomSpace.door;
                        isDoorSpawned = true;
                        break;
                    }
                }
                if (isDoorSpawned == true)
                {
                    break;
                }
            }
        }
    } 
    #endregion

    #region PortalGeneration
    // Sets a space in the middle of 8 floor spaces to a portal depending on this room's type
    private void GeneratePortal()
    {
        bool isPortalSpawned = false;
        while (!isPortalSpawned)
        {
            // Get a random x and y for a room grid space
            int x = Mathf.FloorToInt(Random.value * (gridWidth - 1));
            int y = Mathf.FloorToInt(Random.value * (gridHeight - 1));
            if (room[x, y] == roomSpace.floor && IsSurroundedByFloors(x, y))
            {
                if (roomType == LevelGenerator.levelSpace.entrance)
                {
                    room[x, y] = roomSpace.entrancePortal;
                    isPortalSpawned = true;
                }
                else if (roomType == LevelGenerator.levelSpace.exit)
                {
                    room[x, y] = roomSpace.exitPortal;
                    isPortalSpawned = true;
                }
                else
                {
                    break;
                }
            }
        }
    }

    // Checks if the given location is surrounded by floors so that portals don't generate next to a wall
    private bool IsSurroundedByFloors(int x, int y)
    {
        return room[x + 1, y] == roomSpace.floor && room[x - 1, y] == roomSpace.floor && room[x, y + 1] == roomSpace.floor && room[x, y - 1] == roomSpace.floor &&
            room[x + 1, y + 1] == roomSpace.floor && room[x + 1, y - 1] == roomSpace.floor && room[x - 1, y + 1] == roomSpace.floor && room[x - 1, y - 1] == roomSpace.floor;
    }
    #endregion

    #region ExtraWallRemoval
    private void RemoveExtraWalls()
    {
        for (int x = 1; x < gridWidth - 1; x++)
        {
            for (int y = 1; y < gridHeight - 1; y++)
            {
                if (room[x, y] == roomSpace.wall && IsNotAdjacentToFloor(x, y))
                {
                    room[x, y] = roomSpace.empty;
                }
            }
        }
    }

    private bool IsNotAdjacentToFloor(int x, int y)
    {
        return room[x + 2, y] != roomSpace.floor || room[x - 2, y] != roomSpace.floor || room[x, y + 2] != roomSpace.floor || room[x, y - 2] != roomSpace.floor ||
            room[x + 2, y + 2] != roomSpace.floor || room[x + 2, y - 2] != roomSpace.floor || room[x - 2, y + 2] != roomSpace.floor || room[x - 2, y - 2] != roomSpace.floor;
    }
    #endregion

    #region Spawning
    // Loops through each room grid space and spawns the tile it is designated to be on the correct tilemap layer
    private void SpawnRoom()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                switch (room[x, y])
                {
                    case roomSpace.empty:
                        continue;
                    case roomSpace.floor:
                        SpawnTile(x, y, floorTilemap, floorsPalette);
                        break;
                    case roomSpace.wall:
                        SpawnTile(x, y, wallTilemap, wallsPalette);
                        break;
                    case roomSpace.door:
                        SpawnTile(x, y, doorTilemap, doorsPalette);
                        break;
                    case roomSpace.entrancePortal:
                        SpawnTile(x, y, portalTilemap, entrancePortalsPalette);
                        break;
                    case roomSpace.exitPortal:
                        SpawnTile(x, y, portalTilemap, exitPortalsPalette);
                        break;
                }
            }
        }
    }

    // Spawns a tile at the given x,y coords, on the given tilemap, and given a list of tiles (palette)
    private void SpawnTile(float xPos, float yPos, Tilemap tilemapLayer, List<TileBase> palette)
    {
        Vector2 offset = gridSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(xPos, yPos) * worldUnitsPerOneGridCell - offset;
        tilemapLayer.SetTile(ToInt3(spawnPos), palette[0]); // Plan to implement randomness in tile selection
    }

    // Converts a Vector2 to a Vector3Int
    private Vector3Int ToInt3(Vector2 v)
    {
        return new Vector3Int((int)v.x, (int)v.y, 0);
    } 
    #endregion
}

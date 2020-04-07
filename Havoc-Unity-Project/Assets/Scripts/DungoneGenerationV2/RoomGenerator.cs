using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : Generator
{
    LevelGenerator level = null;
    public enum roomSpace { empty, floor, wall, door, entrancePortal, exitPortal }
    public roomSpace[,] room;

    public Vector2 roomPositionInLevel;
    public LevelGenerator.levelSpace roomType;

    public List<TileBase> floorsPalette;
    public List<TileBase> wallsPalette;
    public List<TileBase> doorsPalette;
    public List<TileBase> entrancePortalsPalette;
    public List<TileBase> exitPortalsPalette;

    GameObject map;
    Tilemap floorTilemap, wallTilemap, doorTilemap, portalTilemap;

    private void Start()
    {
        Setup();
        GenerateFloor();
        GenerateAllDoors();
        GeneratePortal();
        SpawnRoom();
    }

    #region SetupRoom
    public void Setup()
    {
        level = GetComponentInParent<LevelGenerator>();
        SetupTilemaps();

        SetupGridSize();

        room = new roomSpace[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                room[x, y] = roomSpace.wall;
            }
        }

        SetupFirstGenerator();
    }

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
    private void GenerateFloor()
    {
        int iterations = 0;
        int maxIterations = 100000;
        while (iterations < maxIterations)
        {
            foreach (generator targetGen in generators)
            {
                room[(int)targetGen.pos.x, (int)targetGen.pos.y] = roomSpace.floor;
            }

            DestoryGen();
            SpawnGen();
            ChangeGenDir();
            MoveGen();
            ClampGen();

            if ((float)NumberOfFloors() / (float)room.Length > percentGridCovered)
            {
                break;
            }

            iterations++;
        }
    }

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
    private void GenerateAllDoors()
    {
        if (level != null)
        {
            List<Vector2> surroundingRoomsLocations = GetAdjacentRooms();
            foreach (Vector2 dir in surroundingRoomsLocations)
            {
                GenerateDoors(dir);
            } 
        }
    }

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
    private void GeneratePortal()
    {
        bool isPortalSpawned = false;
        while (!isPortalSpawned)
        {
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

    private void SpawnTile(float xPos, float yPos, Tilemap tilemapLayer, List<TileBase> palette)
    {
        Vector2 offset = gridSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(xPos, yPos) * worldUnitsPerOneGridCell - offset;
        tilemapLayer.SetTile(ToInt3(spawnPos), palette[0]);
    }

    private Vector3Int ToInt3(Vector2 v)
    {
        return new Vector3Int((int)v.x, (int)v.y, 0);
    } 
    #endregion
}

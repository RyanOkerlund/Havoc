using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : Generator
{
    public enum roomSpace { empty, floor, wall, door, portal }
    public roomSpace[,] room;

    public List<TileBase> floorsPalette;
    public List<TileBase> wallsPalette;
    public List<TileBase> doorsPalette;

    GameObject map;
    Tilemap floorTilemap, wallTilemap;

    private void Start()
    {
        Setup();
        GenerateFloor();
        SpawnRoom();
    }

    public void Setup()
    {
        map = transform.GetChild(0).gameObject;
        floorTilemap = map.transform.GetChild(0).GetComponent<Tilemap>();
        wallTilemap = map.transform.GetChild(1).GetComponent<Tilemap>();

        SetupGridSize();

        room = new roomSpace[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                room[x, y] = roomSpace.wall;
            }
        }

        SetupGenerators();    
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
        }
    }

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
                        Spawn(x, y, floorTilemap, floorsPalette);
                        break;
                    case roomSpace.wall:
                        Spawn(x, y, wallTilemap, wallsPalette);
                        break;
                    case roomSpace.door:
                        Spawn(x, y, wallTilemap, doorsPalette);
                        break;
                        //case roomSpace.portal:
                        //    Spawn(x, y, portalObject, roomObject);
                        //    break;
                }
            }
        }
    }

    private void Spawn(float xPos, float yPos, Tilemap tilemapLayer, List<TileBase> palette)
    {
        Vector2 offset = gridSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(xPos, yPos) * worldUnitsPerOneGridCell - offset;
        tilemapLayer.SetTile(ToInt3(spawnPos), palette[0]);
    }

    private Vector3Int ToInt3(Vector2 v)
    {
        return new Vector3Int((int)v.x, (int)v.y, 0);
    }
}

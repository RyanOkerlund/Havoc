using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{
    enum roomSpace { empty, floor, wall, door, portal }
    roomSpace[,] room;
    int roomHeight, roomWidth;
    public Vector2 roomSizeWorldUnits = new Vector2(30, 30);
    float worldUnitsPerOneCell = 1f;
    float percentCovered = 0.4f;

    struct generator
    {
        public Vector2 pos;
        public Vector2 dir;
    }
    List<generator> generators;
    int maxGenerators = 10;

    float chanceToChangeDir = 0.5f;
    float chanceToDestoryGen = 0.05f;
    float chanceToSpawnGen = 0.05f;

    public List<TileBase> floorsPalette;
    public List<TileBase> wallsPalette;
    public List<TileBase> doorsPalette;
    GameObject map;
    Tilemap floorTilemap, wallTilemap;

    void Start()
    {
        Setup();
        GenerateFloor();
        //room[0, 29] = roomSpace.floor;
        //room[29, 29] = roomSpace.floor;
        //RemoveExtraWalls();
        //for (int x = 0; x < roomWidth; x++)
        //{
        //    for (int y = 0; y < roomHeight; y++)
        //    {

        //        Debug.Log("space type " + room[x, y].ToString());

        //    }
        //}
        GenerateDoors();
        SpawnRoom();
    }

    private void Setup()
    {
        map = transform.GetChild(0).gameObject;
        floorTilemap = map.transform.GetChild(0).GetComponent<Tilemap>();
        wallTilemap = map.transform.GetChild(1).GetComponent<Tilemap>();

        roomHeight = Mathf.FloorToInt(roomSizeWorldUnits.x / worldUnitsPerOneCell);
        roomWidth = Mathf.FloorToInt(roomSizeWorldUnits.y / worldUnitsPerOneCell);

        room = new roomSpace[roomHeight, roomWidth];

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                room[x, y] = roomSpace.wall;
            }
        }

        generators = new List<generator>();
        generator newGenerator = new generator();

        newGenerator.dir = RandomDirection();
        Vector2 spawnPos = new Vector2(Mathf.FloorToInt(roomWidth / 2.0f), Mathf.FloorToInt(roomHeight / 2.0f));
        newGenerator.pos = spawnPos;

        generators.Add(newGenerator);
    }

    private Vector2 RandomDirection()
    {
        int choice = Mathf.FloorToInt(Random.value * 3.99f);

        switch (choice)
        {
            case 0:
                return Vector2.right;
            case 1:
                return Vector2.up;
            case 2:
                return Vector2.left;
            default:
                return Vector2.down;
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

            int numGens = generators.Count; // might change in the loop DON'T CHANGE!
            for (int i = 0; i < numGens; i++)
            {
                if (Random.value < chanceToDestoryGen && generators.Count > 1)
                {
                    generators.RemoveAt(i);
                    break;
                }
            }

            for (int i = 0; i < generators.Count; i++)
            {
                if (Random.value < chanceToChangeDir)
                {
                    generator targetGen = generators[i];
                    targetGen.dir = RandomDirection();
                    generators[i] = targetGen;
                }
            }

            numGens = generators.Count; // might change in the loop DON'T CHANGE!
            for (int i = 0; i < numGens; i++)
            {
                if (Random.value < chanceToSpawnGen && generators.Count < maxGenerators)
                {
                    generator newGen = new generator();
                    newGen.dir = RandomDirection();
                    newGen.pos = generators[i].pos;
                    generators.Add(newGen);
                }
            }

            for (int i = 0; i < generators.Count; i++)
            {
                generator targetGen = generators[i];
                targetGen.pos += targetGen.dir;
                generators[i] = targetGen;
            }

            for (int i = 0; i < generators.Count; i++)
            {
                generator targetGen = generators[i];
                targetGen.pos.x = Mathf.Clamp(targetGen.pos.x, 1, roomWidth - 2);
                targetGen.pos.y = Mathf.Clamp(targetGen.pos.y, 1, roomHeight - 2);
                generators[i] = targetGen;
            }

            if ((float)NumberOfFloors() / (float)room.Length > percentCovered)
            {
                break;
            }
        }
    }

    //private void RemoveExtraWalls()
    //{
    //    for (int x = 0; x < roomWidth; x++)
    //    {
    //        for (int y = 0; y < roomHeight; y++)
    //        {
    //            if (room[x, y] == roomSpace.wall)
    //            {
    //                if (room[x + 1, y] == roomSpace.wall)
    //                {
    //                    if (room[x - 1, y] == roomSpace.wall)
    //                    {
    //                        if (room[x, y + 1] == roomSpace.wall)
    //                        {
    //                            if (room[x, y - 1] == roomSpace.wall)
    //                            {
    //                                if (room[x + 1, y + 1] == roomSpace.wall)
    //                                {
    //                                    if (room[x + 1, y - 1] == roomSpace.wall)
    //                                    {
    //                                        if (room[x - 1, y + 1] == roomSpace.wall)
    //                                        {
    //                                            if (room[x - 1, y - 1] == roomSpace.wall)
    //                                            {
    //                                                Debug.Log("Wall Detected at " + x.ToString() + ", " + y.ToString());
    //                                            }
    //                                        }
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }                
    //    }
    //}

    //private bool AllSurroudingTilesAreWalls(int x, int y)
    //{
    //    return ((room[x + 1, y] == roomSpace.wall) && (room[x - 1, y] == roomSpace.wall) && (room[x, y + 1] == roomSpace.wall) && (room[x, y - 1] == roomSpace.wall) &&
    //        (room[x + 1, y + 1] == roomSpace.wall) && (room[x + 1, y - 1] == roomSpace.wall) && (room[x - 1, y + 1] == roomSpace.wall) && (room[x - 1, y - 1] == roomSpace.wall));
    //}

    private void GenerateDoors()
    {

    }

    private void SpawnRoom()
    {
        Vector2 offset = roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(0, 0) * worldUnitsPerOneCell - offset;

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
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
        Vector2 offset = roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(xPos, yPos) * worldUnitsPerOneCell - offset;
        tilemapLayer.SetTile(ToInt3(spawnPos), palette[0]);
    }

    private Vector3Int ToInt3(Vector2 v)
    {
        return new Vector3Int((int)v.x, (int)v.y, 0);
    }
}

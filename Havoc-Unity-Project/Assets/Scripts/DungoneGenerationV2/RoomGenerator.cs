using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    enum roomSpace { empty, floor, wall, door, portal }
    roomSpace[,] room;
    int roomHeight, roomWidth;
    Vector2 roomSizeWorldUnits = new Vector2(30, 30);
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

    public GameObject tilePalette;
    GameObject map, floorTilemap, wallTilemap;
    //public GameObject floorObject, wallObject, doorObject, portalObject;

    public Tilemap 

    void Start()
    {
        Setup();
        GenerateFloor();
        SpawnRoom();
    }

    private void Setup()
    {
        map = this.transform.GetChild(0).gameObject;
        floorTilemap = map.transform.GetChild(0).gameObject;
        wallTilemap = map.transform.GetChild(1).gameObject;

        roomHeight = Mathf.FloorToInt(roomSizeWorldUnits.x / worldUnitsPerOneCell);
        roomWidth = Mathf.FloorToInt(roomSizeWorldUnits.y / worldUnitsPerOneCell);

        room = new roomSpace[roomHeight, roomWidth];

        for (int x = 0; x < roomWidth - 1; x++)
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
                targetGen.pos.x = Mathf.Clamp(targetGen.pos.x, 1, roomWidth - 3);
                targetGen.pos.y = Mathf.Clamp(targetGen.pos.y, 1, roomHeight - 2);
                generators[i] = targetGen;
            }

            if ((float)NumberOfFloors() / (float)room.Length > percentCovered)
            {
                break;
            }
        }
    }

    private void Spawn(float xPos, float yPos, GameObject objectToSpawn, GameObject roomObject)
    {
        Vector2 offset = roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(xPos, yPos) * worldUnitsPerOneCell - offset;
        floorTilemap.GetComponent<SpriteTileMode>
        GameObject space = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);
        space.transform.SetParent(roomObject.transform);
    }

    private void SpawnRoom()
    {
        GameObject roomObject = new GameObject();
        roomObject.name = "Room";

        Vector2 offset = roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(0, 0) * worldUnitsPerOneCell - offset;

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (room[x, y])
                {
                    case roomSpace.empty:
                        break;
                    case roomSpace.floor:
                        Spawn(x, y, floorObject, roomObject);
                        break;
                    case roomSpace.wall:
                        Spawn(x, y, wallObject, roomObject);
                        break;
                    case roomSpace.door:
                        Spawn(x, y, doorObject, roomObject);
                        break;
                    case roomSpace.portal:
                        Spawn(x, y, portalObject, roomObject);
                        break;
                }
            }
        }
    }
}

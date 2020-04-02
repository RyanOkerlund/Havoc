using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : Generator
{
    public enum levelSpace { empty, firstRoom, regularRoom, bossRoom, loreRoom }
    public levelSpace[,] level;

    public int maxNumRooms = 5;
    public int minNumRooms = 1;

    public RoomGenerator roomGenPrefab;
    public GameObject player;
    RoomGenerator firstRoom;

    public bool isLastRoom = false;
    public bool isFirstRoom = true;

    private void Start()
    {
        Setup();
        GenerateLevel();
        SpawnLevel();
    }

    public void Setup()
    {
        SetupGridSize();

        level = new levelSpace[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                level[x, y] = levelSpace.empty;
            }
        }

        SetupGenerators();

    }

    private void GenerateLevel()
    {
        int numSpawnedRooms = 0;
        while (numSpawnedRooms < maxNumRooms)
        {
            foreach (generator targetGen in generators)
            {
                level[(int)targetGen.pos.x, (int)targetGen.pos.y] = levelSpace.regularRoom;
            }

            DestoryGen();
            SpawnGen();
            ChangeGenDir();
            MoveGen();

            numSpawnedRooms++;
        }
    }

    private void SpawnLevel()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                switch (level[x, y])
                {
                    case levelSpace.empty:
                        continue;
                    case levelSpace.regularRoom:
                        Spawn(x, y);
                        break;
                    //case space.wall:
                    //    Spawn(x, y, wallTilemap, wallsPalette);
                    //    break;
                    //case space.door:
                    //    Spawn(x, y, wallTilemap, doorsPalette);
                    //    break;
                    //case roomSpace.portal:
                    //    Spawn(x, y, portalObject, roomObject);
                    //    break;
                }
            }
        }
    }

    private void Spawn(float xPos, float yPos)
    {
        Vector2 offset = gridSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(xPos, yPos) * worldUnitsPerOneGridCell - offset;
        Instantiate(roomGenPrefab, spawnPos, Quaternion.identity, this.transform);
    }
}

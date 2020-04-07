using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : Generator
{
    public enum levelSpace { empty, room, entrance, exit, bossRoom, loreRoom }
    public levelSpace[,] level;

    public Vector2 levelSizeWorldUnits;

    public int maxNumRooms = 5;
    public int minNumRooms = 1;

    public RoomGenerator roomGenPrefab;
    RoomGenerator firstRoom;

    private void Start()
    {
        Setup();
        GenerateLevel();
        SetEntrance();
        SetExit();
        SpawnLevel();
    }

    public void Setup()
    {
        gridSizeWorldUnits.x = roomGenPrefab.gridSizeWorldUnits.x * levelSizeWorldUnits.x;
        gridSizeWorldUnits.y = roomGenPrefab.gridSizeWorldUnits.y * levelSizeWorldUnits.y;
        worldUnitsPerOneGridCell = roomGenPrefab.gridSizeWorldUnits.x;

        SetupGridSize();

        level = new levelSpace[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                level[x, y] = levelSpace.empty;
            }
        }

        SetupFirstGenerator();
    }

    #region GenerateRooms
    private void GenerateLevel()
    {
        int iterations = 0;
        int maxIterations = 100000;
        while (iterations < maxIterations)
        {
            foreach (generator targetGen in generators)
            {
                level[(int)targetGen.pos.x, (int)targetGen.pos.y] = levelSpace.room;
            }

            DestoryGen();
            SpawnGen();
            ChangeGenDir();
            MoveGen();

            if ((float)NumberOfRooms() / (float)level.Length > percentGridCovered)
            {
                break;
            }
            iterations++;
        }
    }

    private int NumberOfRooms()
    {
        int count = 0;
        foreach (levelSpace room in level)
        {
            if (room != levelSpace.empty)
            {
                count++;
            }
        }
        return count;
    } 
    #endregion

    private void SetEntrance()
    {
        Vector2 spawnPos = new Vector2(Mathf.FloorToInt(gridWidth / 2.0f), Mathf.FloorToInt(gridHeight / 2.0f));
        level[(int)spawnPos.x, (int)spawnPos.y] = levelSpace.entrance;
    }

    #region GenerateExit
    private void SetExit()
    {
        bool isExitSet = false;
        while (!isExitSet)
        {
            int x = Mathf.FloorToInt(Random.value * (gridWidth - 1));
            int y = Mathf.FloorToInt(Random.value * (gridHeight - 1));
            if (level[x, y] == levelSpace.room && IsNotNextToEntrance(x, y) && IsNextToAnotherRoom(x, y))
            {
                level[x, y] = levelSpace.exit;
                isExitSet = true;
            }
        }
    }

    private bool IsNotNextToEntrance(int x, int y)
    {
        return level[x + 1, y] != levelSpace.entrance && level[x - 1, y] != levelSpace.entrance
            && level[x, y + 1] != levelSpace.entrance && level[x, y - 1] != levelSpace.entrance;
    }

    private bool IsNextToAnotherRoom(int x, int y)
    {
        return level[x + 1, y] == levelSpace.room || level[x - 1, y] == levelSpace.room ||
            level[x, y + 1] == levelSpace.room || level[x, y - 1] == levelSpace.room;
    }
    #endregion

    #region Spawning
    private void SpawnLevel()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                roomGenPrefab.roomPositionInLevel.x = x;
                roomGenPrefab.roomPositionInLevel.y = y;
                switch (level[x, y])
                {
                    case levelSpace.empty:
                        continue;
                    case levelSpace.room:
                        roomGenPrefab.roomType = levelSpace.room;
                        Spawn(x, y);
                        break;
                    case levelSpace.entrance:
                        roomGenPrefab.roomType = levelSpace.entrance;
                        Spawn(x, y);
                        break;
                    case levelSpace.exit:
                        roomGenPrefab.roomType = levelSpace.exit;
                        Spawn(x, y);
                        break;
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
    #endregion
}
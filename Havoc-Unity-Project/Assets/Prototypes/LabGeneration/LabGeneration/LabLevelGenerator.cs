using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabLevelGenerator : Generator // Extends Generator Class
{
    public enum labLevelSpace { empty, room, entrance, exit, bossRoom, loreRoom } // Level space types (i.e. room types)
    public labLevelSpace[,] labLevel; // The grid of the level

    public Vector2 labLevelGridSize; // The size of the level in grid spaces
    public int offsetLabRoomPadding; // The space in between the rooms

    public LabRoomSpawner labRoomSpawnerPrefab; // The RoomGenerator prefab to make rooms

    // Runs all the code to make the level
    private void Start()
    {
        Setup();
        GenerateLevel();
        SetEntrance();
        SetExit();
        SpawnLevel();
    }

    // Prepares the level for generation
    public void Setup()
    {
        // Sets the level grid size in Unity world units
        worldUnitsPerOneGridCell = labRoomSpawnerPrefab.gridSizeWorldUnits.x + offsetLabRoomPadding;
        gridSizeWorldUnits.x = worldUnitsPerOneGridCell * labLevelGridSize.x;
        gridSizeWorldUnits.y = worldUnitsPerOneGridCell * labLevelGridSize.y;        

        SetupGridSize();

        // Initialize the level grid
        labLevel = new labLevelSpace[gridWidth, gridHeight];

        // Set all the level grid spaces to empty
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                labLevel[x, y] = labLevelSpace.empty;
            }
        }

        // // Set the horizontal and vertical location of doors for the level randomly
        // roomGenPrefab.horizontalDoorsYLocation = Mathf.FloorToInt(Random.Range(1f, roomGenPrefab.gridSizeWorldUnits.y));
        // roomGenPrefab.verticalDoorsXLocation = Mathf.FloorToInt(Random.Range(1f, roomGenPrefab.gridSizeWorldUnits.x));

        SetupFirstGenerator();
    }

    #region GenerateRooms
    // Use generators to spawn rooms
    private void GenerateLevel()
    {
        int iterations = 0;
        int maxIterations = 100000;
        while (iterations < maxIterations)
        {
            // Change the location of each generator to a room
            foreach (generator targetGen in generators)
            {
                labLevel[(int)targetGen.pos.x, (int)targetGen.pos.y] = labLevelSpace.room;
            }

            // See Generator Class
            DestoryGen();
            SpawnGen();
            ChangeGenDir();
            MoveGen();
            ClampGen();

            // If enough of the room grid is rooms then be done
            if ((float)NumberOfRooms() / (float)labLevel.Length > percentGridCovered)
            {
                break;
            }
            iterations++;
        }
    }

    // Returns the number of rooms in the level
    private int NumberOfRooms()
    {
        int count = 0;
        foreach (labLevelSpace room in labLevel)
        {
            if (room != labLevelSpace.empty)
            {
                count++;
            }
        }
        return count;
    } 
    #endregion

    // Sets middle room to an entrance type
    private void SetEntrance()
    {
        Vector2 spawnPos = new Vector2(Mathf.FloorToInt(gridWidth / 2.0f), Mathf.FloorToInt(gridHeight / 2.0f));
        labLevel[(int)spawnPos.x, (int)spawnPos.y] = labLevelSpace.entrance;
    }

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
            if (labLevel[x, y] == labLevelSpace.room && IsNotNextToEntrance(x, y) && IsNextToAnotherRoom(x, y))
            {
                labLevel[x, y] = labLevelSpace.exit;
                isExitSet = true;
            }
        }
    }

    // Checks if the given location is next to the entrance room
    private bool IsNotNextToEntrance(int x, int y)
    {
        return labLevel[x + 1, y] != labLevelSpace.entrance && labLevel[x - 1, y] != labLevelSpace.entrance
            && labLevel[x, y + 1] != labLevelSpace.entrance && labLevel[x, y - 1] != labLevelSpace.entrance;
    }

    // Checks if the location is next to another room
    private bool IsNextToAnotherRoom(int x, int y)
    {
        return labLevel[x + 1, y] == labLevelSpace.room || labLevel[x - 1, y] == labLevelSpace.room ||
            labLevel[x, y + 1] == labLevelSpace.room || labLevel[x, y - 1] == labLevelSpace.room;
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
                labRoomSpawnerPrefab.labRoomPosInLevel.x = x;
                labRoomSpawnerPrefab.labRoomPosInLevel.y = y;
                switch (labLevel[x, y])
                {
                    case labLevelSpace.empty:
                        continue;
                    case labLevelSpace.room:
                        labRoomSpawnerPrefab.labRoomType = labLevelSpace.room;
                        Spawn(x, y);
                        break;
                    case labLevelSpace.entrance:
                        labRoomSpawnerPrefab.labRoomType = labLevelSpace.entrance;
                        Spawn(x, y);
                        break;
                    case labLevelSpace.exit:
                        labRoomSpawnerPrefab.labRoomType = labLevelSpace.exit;
                        Spawn(x, y);
                        break;
                    // This is where you add more room types (i.e. boss rooms, lore rooms, etc)
                }
            }
        }
    }

    // Instantiates a room at the given location
    private void Spawn(float xPos, float yPos)
    {
        Vector2 offset = gridSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(xPos, yPos) * worldUnitsPerOneGridCell - offset;
        Instantiate(labRoomSpawnerPrefab, spawnPos, Quaternion.identity, this.transform);
    } 
    #endregion
}
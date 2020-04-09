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
    public enum levelSpace { empty, room, entrance, exit, bossRoom, loreRoom } // Level space types (i.e. room types)
    public levelSpace[,] level; // The grid of the level

    public Vector2 levelGridSize; // The size of the level in grid spaces
    public int offsetRoomPadding; // The space in between the rooms

    public RoomGenerator roomGenPrefab; // The RoomGenerator prefab to make rooms

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
        worldUnitsPerOneGridCell = roomGenPrefab.gridSizeWorldUnits.x + offsetRoomPadding;
        gridSizeWorldUnits.x = worldUnitsPerOneGridCell * levelGridSize.x;
        gridSizeWorldUnits.y = worldUnitsPerOneGridCell * levelGridSize.y;        

        SetupGridSize();

        // Initialize the level grid
        level = new levelSpace[gridWidth, gridHeight];

        // Set all the level grid spaces to empty
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
                level[(int)targetGen.pos.x, (int)targetGen.pos.y] = levelSpace.room;
            }

            // See Generator Class
            DestoryGen();
            SpawnGen();
            ChangeGenDir();
            MoveGen();
            ClampGen();

            // If enough of the room grid is rooms then be done
            if ((float)NumberOfRooms() / (float)level.Length > percentGridCovered)
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

    // Sets middle room to an entrance type
    private void SetEntrance()
    {
        Vector2 spawnPos = new Vector2(Mathf.FloorToInt(gridWidth / 2.0f), Mathf.FloorToInt(gridHeight / 2.0f));
        level[(int)spawnPos.x, (int)spawnPos.y] = levelSpace.entrance;
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
            if (level[x, y] == levelSpace.room && IsNotNextToEntrance(x, y) && IsNextToAnotherRoom(x, y))
            {
                level[x, y] = levelSpace.exit;
                isExitSet = true;
            }
        }
    }

    // Checks if the given location is next to the entrance room
    private bool IsNotNextToEntrance(int x, int y)
    {
        return level[x + 1, y] != levelSpace.entrance && level[x - 1, y] != levelSpace.entrance
            && level[x, y + 1] != levelSpace.entrance && level[x, y - 1] != levelSpace.entrance;
    }

    // Checks if the location is next to another room
    private bool IsNextToAnotherRoom(int x, int y)
    {
        return level[x + 1, y] == levelSpace.room || level[x - 1, y] == levelSpace.room ||
            level[x, y + 1] == levelSpace.room || level[x, y - 1] == levelSpace.room;
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

    // Instantiates a room at the given location
    private void Spawn(float xPos, float yPos)
    {
        Vector2 offset = gridSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(xPos, yPos) * worldUnitsPerOneGridCell - offset;
        Instantiate(roomGenPrefab, spawnPos, Quaternion.identity, this.transform);
    } 
    #endregion
}
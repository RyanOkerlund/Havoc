using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    enum roomType { empty, regularRoom, bossRoom, loreRoom }
    int maxNumRooms = 5;
    int minNumRooms = 3;

    Vector2 distanceBetweenRooms;

    struct levelGenerator
    {
        public Vector2 pos;
        public Vector2 startDir;
        public Vector2 nextDir;
    }
    levelGenerator levelGen;

    float chanceToChangeLevelGenDir = 0.8f;
    float chanceToDestoryLevelGen = 0.5f;

    public RoomGenerator roomGenPrefab;    

    private void Start()
    {
        Setup();
        GenerateLevel();
    }

    private void Setup()
    {
        distanceBetweenRooms = new Vector2(roomGenPrefab.roomSizeWorldUnits.x, roomGenPrefab.roomSizeWorldUnits.y);

        levelGen = new levelGenerator();
        levelGen.startDir = RandomDirection();
        levelGen.nextDir = NextDirection(levelGen.startDir);

        Vector2 startPos = new Vector2(Mathf.FloorToInt(roomGenPrefab.roomSizeWorldUnits.x / 2.0f), Mathf.FloorToInt(roomGenPrefab.roomSizeWorldUnits.y / 2.0f));
        levelGen.pos = startPos;

        Instantiate(roomGenPrefab, levelGen.pos, Quaternion.identity, this.transform);
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

    private Vector2 NextDirection(Vector2 lastDir)
    {
        int choice = Mathf.FloorToInt(Random.value * 2.99f);

        if (lastDir == Vector2.right)
        {
            switch (choice)
            {
                case 0:
                    return Vector2.up;
                case 1:
                    return Vector2.down;
                default:
                    return Vector2.right;
            }
        } 
        else if (lastDir == Vector2.up)
        {
            switch (choice)
            {
                case 0:
                    return Vector2.left;
                case 1:
                    return Vector2.right;
                default:
                    return Vector2.up;
            }
        }
        else if (lastDir == Vector2.left)
        {
            switch (choice)
            {
                case 0:
                    return Vector2.down;
                case 1:
                    return Vector2.up;
                default:
                    return Vector2.left;
            }
        }
        else if (lastDir == Vector2.down)
        {
            switch (choice)
            {
                case 0:
                    return Vector2.right;
                case 1:
                    return Vector2.left;
                default:
                    return Vector2.down;
            }
        }
        else
        {
            return lastDir;
        }
    }

    private void GenerateLevel()
    {
        int numSpawnedRooms = 0;
        while (numSpawnedRooms < maxNumRooms)
        {
            if (Random.value < chanceToDestoryLevelGen && numSpawnedRooms > minNumRooms)
            {
                break;
            }
            if (Random.value < chanceToChangeLevelGenDir)
            {
                levelGen.nextDir = NextDirection(levelGen.nextDir);
            }
            levelGen.pos += levelGen.nextDir * roomGenPrefab.roomSizeWorldUnits.x;
            Instantiate(roomGenPrefab, levelGen.pos, Quaternion.identity, this.transform);
            numSpawnedRooms++;
        }
    }
}

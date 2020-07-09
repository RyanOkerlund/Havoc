using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LabRoomGenerator : MonoBehaviour
{
    LevelGenerator.LevelSpace levelSpace;

    public Vector2 roomSize;

    Grid map; // The parent tilemap grid
    Tilemap floorTilemap, wallTilemap, doorTilemap, portalTilemap; // The respective tilemaps for each type of tile

    public void init(LevelGenerator.LevelSpace levelSpaceFromLevel) {
        levelSpace = levelSpaceFromLevel;
        name = levelSpace.room.name + " at [" + levelSpace.gridPos.x + ", " + levelSpace.gridPos.y + "]";
        Setup();
        GenerateWalls();
        GenerateFloors();
        GenerateDoors();
    }

    private void Setup() {
        SetupTilemaps();
    }

    private void SetupTilemaps() {
        map = transform.GetComponentInChildren<Grid>();
        floorTilemap = map.transform.GetChild(0).GetComponent<Tilemap>();
        wallTilemap = map.transform.GetChild(1).GetComponent<Tilemap>();
        doorTilemap = map.transform.GetChild(2).GetComponent<Tilemap>();
        portalTilemap = map.transform.GetChild(3).GetComponent<Tilemap>();
    }

    private void GenerateWalls() {
        for (int x = 0; x < roomSize.x; x++) {
            for (int y = 0; y < roomSize.y; y++) {
                Vector2 spawnPos = new Vector2(x, y);
                SpawnTile(spawnPos, wallTilemap, levelSpace.room.wallTile);
            }
        }
    }

    private void GenerateFloors() {
        for (int x = 1; x < roomSize.x - 1; x++) {
            for (int y = 1; y < roomSize.y - 1; y++) {               
                Vector2 spawnPos = new Vector2(x, y);
                SpawnTile(spawnPos, wallTilemap, null);
                SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
            }
        }
    }

    private void GenerateDoors() {
        foreach (LevelGenerator.wall wall in levelSpace.walls) {
            if (wall.hasDoor) {
                // Debug.Log("Spawning door on " + wall.direction);
                Vector2 spawnPos;
                if (wall.direction == LevelGenerator.wallDirections.up) {
                    spawnPos = new Vector2(roomSize.x / 2, roomSize.y - 1);
                }
                else if (wall.direction == LevelGenerator.wallDirections.right) {
                    spawnPos = new Vector2(roomSize.x - 1, roomSize.y / 2);
                }
                else if (wall.direction == LevelGenerator.wallDirections.down) {
                    spawnPos = new Vector2(roomSize.x / 2, 0);
                }
                else {
                    spawnPos = new Vector2(0, roomSize.y / 2);
                }
                SpawnTile(spawnPos, wallTilemap, null);
                SpawnTile(spawnPos, doorTilemap, levelSpace.room.doorTile);
            }
            // Don't know if this works at all.
            else if (wall.wallType == LevelGenerator.wallTypes.open) {
                Vector2 spawnPos;
                if (wall.direction == LevelGenerator.wallDirections.up) {
                    for (int i = 1; i < roomSize.x; i++) {
                        spawnPos = new Vector2(levelSpace.spawnPos.x + i, levelSpace.spawnPos.y + roomSize.y);
                        SpawnTile(spawnPos, wallTilemap, null);
                        SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                    }
                }
                else if (wall.direction == LevelGenerator.wallDirections.right) {
                    for (int j = 1; j < roomSize.y; j++) {
                        spawnPos = new Vector2(levelSpace.spawnPos.x + roomSize.x, levelSpace.spawnPos.y + j);
                        SpawnTile(spawnPos, wallTilemap, null);
                        SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                    }
                }
                else if (wall.direction == LevelGenerator.wallDirections.down) {
                    for (int i = 1; i < roomSize.x; i++) {
                        spawnPos = new Vector2(levelSpace.spawnPos.x + i, levelSpace.spawnPos.y);
                        SpawnTile(spawnPos, wallTilemap, null);
                        SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                    }
                }
                else {
                    for (int j = 1; j < roomSize.x; j++) {
                        spawnPos = new Vector2(levelSpace.spawnPos.x, levelSpace.spawnPos.y + j);
                        SpawnTile(spawnPos, wallTilemap, null);
                        SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                    }
                }
            }
        }
        // Debug.Log("Doors Generated");
    }

    private void SpawnTile(Vector2 spawnPos, Tilemap tilemapLayer, TileBase tile)
    {
        tilemapLayer.SetTile(ToInt3(spawnPos), tile); // Plan to implement randomness in tile selection
    }

    // Converts a Vector2 to a Vector3Int
    private Vector3Int ToInt3(Vector2 v)
    {
        return new Vector3Int((int)v.x, (int)v.y, 0);
    }


    /*
    0.5 Setup the tile maps and such
    1. Constructed at location, with a room type (First an entrance)
    2. Assign wall types based on adjacent rooms' corresponding wall types from the lab level gen (if not empty)
        2.5 Randomly assign any remaining wall types with weights (i.e. empty adjacent rooms)
    3. Tell the lab level gen what directions to move for next rooms based on wall type
    4. Generate the floor based on wall type
    5. Generate doors based on wall types
    6. Generate entrances/exits based on room type
    */
}
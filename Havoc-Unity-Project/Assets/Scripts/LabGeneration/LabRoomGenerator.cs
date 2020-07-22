﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LabRoomGenerator : MonoBehaviour
{
    LabLevelGenerator.LevelSpace levelSpace;

    public Vector2 roomSize;

    Grid map; // The parent tilemap grid
    Tilemap floorTilemap, wallTilemap, doorTilemap, portalTilemap; // The respective tilemaps for each type of tile

    public void init(LabLevelGenerator.LevelSpace levelSpaceFromLevel) {
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
        if (levelSpace.room.type != Room.RoomTypes.hallway) {
            for (int x = 0; x < roomSize.x; x++) {
                for (int y = 0; y < roomSize.y; y++) {
                    Vector2 spawnPos = new Vector2(x, y);
                    SpawnTile(spawnPos, wallTilemap, levelSpace.room.wallTile);
                }
            }
        }
        else {
            for (int x = 5; x < roomSize.x - 5; x++) {
                for (int y = 5; y < roomSize.y - 5; y++) {               
                    Vector2 spawnPos = new Vector2(x, y);
                    SpawnTile(spawnPos, wallTilemap, levelSpace.room.wallTile);
                }
            }
            foreach (LabLevelGenerator.wall wall in levelSpace.walls) {
                if (wall.wallType == LabLevelGenerator.wallTypes.door) {
                    if (wall.direction == Vector2.up) {
                        for (int x = 5; x < roomSize.x - 5; x++) {
                            for (int y = 7; y < roomSize.y; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, wallTilemap, levelSpace.room.wallTile);
                            }
                        }
                    }
                    else if (wall.direction == Vector2.right) {
                        for (int x = 7; x < roomSize.x; x++) {
                            for (int y = 5; y < roomSize.y - 5; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, wallTilemap, levelSpace.room.wallTile);
                            }
                        }
                    }
                    else if (wall.direction == Vector2.down) {
                        for (int x = 5; x < roomSize.x - 5; x++) {
                            for (int y = 0; y < roomSize.y - 7; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, wallTilemap, levelSpace.room.wallTile);
                            }
                        }
                    }
                    else {
                        for (int x = 0; x < roomSize.x - 7; x++) {
                            for (int y = 5; y < roomSize.y - 5; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, wallTilemap, levelSpace.room.wallTile);
                            }
                        }
                    }
                }
            }
        }
    }

    private void GenerateFloors() {
        if (levelSpace.room.type != Room.RoomTypes.hallway) {
            for (int x = 1; x < roomSize.x - 1; x++) {
                for (int y = 1; y < roomSize.y - 1; y++) {               
                    Vector2 spawnPos = new Vector2(x, y);
                    SpawnTile(spawnPos, wallTilemap, null);
                    SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                }
            }
        }
        else {
            for (int x = 6; x < roomSize.x - 6; x++) {
                for (int y = 6; y < roomSize.y - 6; y++) {               
                    Vector2 spawnPos = new Vector2(x, y);
                    SpawnTile(spawnPos, wallTilemap, null);
                    SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                }
            }
            foreach (LabLevelGenerator.wall wall in levelSpace.walls) {
                if (wall.wallType == LabLevelGenerator.wallTypes.door) {
                    if (wall.direction == Vector2.up) {
                        for (int x = 6; x < roomSize.x - 6; x++) {
                            for (int y = 7; y < roomSize.y - 1; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, wallTilemap, null);
                                SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                            }
                        }
                    }
                    else if (wall.direction == Vector2.right) {
                        for (int x = 7; x < roomSize.x - 1; x++) {
                            for (int y = 6; y < roomSize.y - 6; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, wallTilemap, null);
                                SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                            }
                        }
                    }
                    else if (wall.direction == Vector2.down) {
                        for (int x = 6; x < roomSize.x - 6; x++) {
                            for (int y = 1; y < roomSize.y - 7; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, wallTilemap, null);
                                SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                            }
                        }
                    }
                    else {
                        for (int x = 1; x < roomSize.x - 7; x++) {
                            for (int y = 6; y < roomSize.y - 6; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, wallTilemap, null);
                                SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                            }
                        }
                    }
                }
            }
        }
    }

    private void GenerateDoors() {
        foreach (LabLevelGenerator.wall wall in levelSpace.walls) {
            if (wall.wallType == LabLevelGenerator.wallTypes.door) {
                // Debug.Log("Spawning door on " + wall.direction);
                Vector2 spawnPos;
                if (wall.direction == Vector2.up) {
                    spawnPos = new Vector2(roomSize.x / 2, roomSize.y - 1);
                }
                else if (wall.direction == Vector2.right) {
                    spawnPos = new Vector2(roomSize.x - 1, roomSize.y / 2);
                }
                else if (wall.direction == Vector2.down) {
                    spawnPos = new Vector2(roomSize.x / 2, 0);
                }
                else {
                    spawnPos = new Vector2(0, roomSize.y / 2);
                }
                SpawnTile(spawnPos, wallTilemap, null);
                SpawnTile(spawnPos, doorTilemap, levelSpace.room.doorTile);
            }
            // Don't know if this works at all.
            else if (wall.wallType == LabLevelGenerator.wallTypes.open) {
                Vector2 spawnPos;
                if (wall.direction == Vector2.up) {
                    for (int i = 1; i < roomSize.x; i++) {
                        spawnPos = new Vector2(levelSpace.spawnPos.x + i, levelSpace.spawnPos.y + roomSize.y);
                        SpawnTile(spawnPos, wallTilemap, null);
                        SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                    }
                }
                else if (wall.direction == Vector2.right) {
                    for (int j = 1; j < roomSize.y; j++) {
                        spawnPos = new Vector2(levelSpace.spawnPos.x + roomSize.x, levelSpace.spawnPos.y + j);
                        SpawnTile(spawnPos, wallTilemap, null);
                        SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                    }
                }
                else if (wall.direction == Vector2.down) {
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
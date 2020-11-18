using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LabRoomGenerator : MonoBehaviour
{
    LabLevelGenerator.LevelSpace levelSpace;

    [HideInInspector] public Vector2Int roomSize;
    [HideInInspector] public int hallwayWidth;
    
    public RuleTile roomWalls;
    public RuleTile hallwayWalls;

    int upperBound, rightBound, lowerBound, leftBound, verticalHalf, horizontalHalf;
        
    Grid map; // The parent tilemap grid
    Tilemap floorTilemap, wallTilemap, doorTilemap, portalTilemap; // The respective tilemaps for each type of tile

    public void init(LabLevelGenerator.LevelSpace levelSpaceFromLevel) {
        levelSpace = levelSpaceFromLevel;
        name = levelSpace.room.name + " at [" + levelSpace.gridPos.x + ", " + levelSpace.gridPos.y + "]";
        Setup();
        GenerateFloors();
        GenerateWalls();
        GenerateDoors();
    }

    private void Setup() {
        SetupTilemaps();
        verticalHalf = (int)((roomSize.y - 1) / 2);
        horizontalHalf = (int)((roomSize.x - 1) / 2);
        upperBound = verticalHalf + (Mathf.CeilToInt(hallwayWidth / 2) + 1);
        rightBound = horizontalHalf + (Mathf.CeilToInt(hallwayWidth / 2) + 1);
        lowerBound = verticalHalf - (Mathf.CeilToInt(hallwayWidth / 2) + 1);
        leftBound = horizontalHalf - (Mathf.CeilToInt(hallwayWidth / 2) + 1);
        SetupRoomWallTile();
        SetupHallwayWallTile();
    }

    private void SetupTilemaps() {
        map = transform.GetComponentInChildren<Grid>();
        floorTilemap = map.transform.GetChild(0).GetComponent<Tilemap>();
        wallTilemap = map.transform.GetChild(1).GetComponent<Tilemap>();
        doorTilemap = map.transform.GetChild(2).GetComponent<Tilemap>();
        portalTilemap = map.transform.GetChild(3).GetComponent<Tilemap>();
    }

    // Sets the neighbor tiling rules for the room walls tileset.
    private void SetupRoomWallTile() {
        for (int i = 0; i < roomWalls.m_TilingRules.Count; i++) {
            Dictionary<Vector3Int, int> neighbors = new Dictionary<Vector3Int, int>();
            switch (i) {
                case 0: // Top wall normal rule
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -(roomSize.y - 1), 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 1: // Side wall normal rule
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(roomSize.x - 1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 2: // Bottom wall normal rule
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, roomSize.y - 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 3: // Top door opening rule
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(-horizontalHalf, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(2, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -(roomSize.y - 1), 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 4: // Side door opening (top) rule
                    neighbors.Add(new Vector3Int(0, verticalHalf, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(roomSize.x - 1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -2, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 5: // Side door opening (bottom) rule
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, -verticalHalf, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, 2, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(roomSize.x - 1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 6: // Bottom door opening rule
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(-horizontalHalf, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, roomSize.y - 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(2, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 7: // Top wall door across rule
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(-(roomSize.y - 1), 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(1, -(roomSize.y - 1), 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, -(roomSize.y - 1), 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 8: // Side wall door across rule
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(roomSize.x - 1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(roomSize.x - 1, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(roomSize.x - 1, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 9: // Bottom wall door across rule
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, roomSize.y - 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(1, roomSize.y - 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, roomSize.y - 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 10: // Top inside corner rule
                    neighbors.Add(new Vector3Int(1, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 11: // Bottom inside corner rule
                    neighbors.Add(new Vector3Int(1, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                default:
                    break;
            }
            roomWalls.m_TilingRules[i].ApplyNeighbors(neighbors);
        }
    }

    private void SetupHallwayWallTile() {
        for (int i = 0; i < hallwayWalls.m_TilingRules.Count; i++) {
            Dictionary<Vector3Int, int> neighbors = new Dictionary<Vector3Int, int>();
            switch (i) {
                case 0: // Top wall normal rule
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -(hallwayWidth + 1), 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 1: // Side wall normal rule
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(hallwayWidth + 1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 2: // Bottom wall normal rule
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, hallwayWidth + 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 3: // Top wall across rule
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -upperBound, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 4: // Side wall across rule 
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(rightBound, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);  
                    break;
                case 5: // Bottom wall across rule
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, upperBound, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This); 
                    break;
                case 6:
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -(roomSize.y - 1), 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 7:
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(roomSize.x - 1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 8:
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, roomSize.y - 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This); 
                    break;
                case 9: // Top wall door across rule
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, -upperBound, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(1, -upperBound, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, -upperBound, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 10: // Side wall door across rule
                    neighbors.Add(new Vector3Int(rightBound, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(rightBound, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(rightBound, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 11: // Bottom wall door across rule
                    neighbors.Add(new Vector3Int(0, upperBound, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(1, upperBound, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, upperBound, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 12: // Top door opening rule
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(2, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    if (hallwayWidth == 3) {
                        neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                        neighbors.Add(new Vector3Int(-1, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    }
                    else {
                        neighbors.Add(new Vector3Int(-1 * hallwayWidth % 3, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                        neighbors.Add(new Vector3Int(-1 * hallwayWidth % 3, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    }
                    break;
                case 13: // Side door opening (top) rule
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, -2, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    if (hallwayWidth == 3) {
                        neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                        neighbors.Add(new Vector3Int(1, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    }
                    else {
                        neighbors.Add(new Vector3Int(0, 1 * hallwayWidth % 3, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                        neighbors.Add(new Vector3Int(1, 1 * hallwayWidth % 3, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    }
                    break;
                case 14: // Side door opening (bottom) rule
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, 2, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    if (hallwayWidth == 3) {
                        neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                        neighbors.Add(new Vector3Int(1, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    }
                    else {
                        neighbors.Add(new Vector3Int(0, -1 * hallwayWidth % 3, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                        neighbors.Add(new Vector3Int(1, -1 * hallwayWidth % 3, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    }
                    break;
                case 15: // Bottom door opening rule
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(2, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    if (hallwayWidth == 3) {
                        neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                        neighbors.Add(new Vector3Int(-1, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    }
                    else {
                        neighbors.Add(new Vector3Int(-1 * hallwayWidth % 3, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                        neighbors.Add(new Vector3Int(-1 * hallwayWidth % 3, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    }
                    break;
                case 16: // Top outside corner rule
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(hallwayWidth + 1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -(hallwayWidth + 1), 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 17: // Bottom outside corner rule
                    neighbors.Add(new Vector3Int(0, hallwayWidth + 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(hallwayWidth + 1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(-1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 18: // Top inside corner rule
                    neighbors.Add(new Vector3Int(1, -1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(0, -1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                case 19: // Bottom inside corner rule
                    neighbors.Add(new Vector3Int(1, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
                    neighbors.Add(new Vector3Int(0, 1, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    neighbors.Add(new Vector3Int(1, 0, 0), RuleTile.TilingRuleOutput.Neighbor.This);
                    break;
                default:
                    break;
            }
            hallwayWalls.m_TilingRules[i].ApplyNeighbors(neighbors);
        }
    }

    private void GenerateFloors() {
        if (levelSpace.room.type != Room.RoomTypes.hallway) {
            for (int x = 0; x < roomSize.x; x++) {
                for (int y = 0; y < roomSize.y; y++) {               
                    Vector2 spawnPos = new Vector2(x, y);
                    SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                }
            }
        }
        else {
            
            for (int x = leftBound; x < rightBound + 1; x++) {
                for (int y = lowerBound; y < upperBound + 1; y++) {               
                    Vector2 spawnPos = new Vector2(x, y);
                    SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                }
            }
            foreach (LabLevelGenerator.wall wall in levelSpace.walls) {
                if (wall.wallType == LabLevelGenerator.wallTypes.door) {
                    if (wall.direction == Vector2.up) {
                        for (int x = leftBound; x < rightBound + 1; x++) {
                            for (int y = verticalHalf; y < roomSize.y; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                            }
                        }
                    }
                    else if (wall.direction == Vector2.right) {
                        for (int x = horizontalHalf; x < roomSize.x; x++) {
                            for (int y = lowerBound; y < upperBound + 1; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                            }
                        }
                    }
                    else if (wall.direction == Vector2.down) {
                        for (int x = leftBound; x < rightBound + 1; x++) {
                            for (int y = 0; y < verticalHalf + 1; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                            }
                        }
                    }
                    else {
                        for (int x = 0; x < horizontalHalf + 1; x++) {
                            for (int y = lowerBound; y < upperBound + 1; y++) {
                                Vector2 spawnPos = new Vector2(x, y);
                                SpawnTile(spawnPos, floorTilemap, levelSpace.room.floorTile);
                            }
                        }
                    }
                }
            }
        }
    }

    private void GenerateWalls() {
        if (levelSpace.room.type != Room.RoomTypes.hallway) {
            for (int x = 0; x < roomSize.x; x++) {
                for (int y = 0; y < roomSize.y; y++) {
                    if (x == 0 || x == roomSize.x - 1 || y == 0 || y == roomSize.y - 1) {
                        Vector2 spawnPos = new Vector2(x, y);
                        SpawnTile(spawnPos, wallTilemap, levelSpace.room.wallTile);
                    }
                }
            }
        }
        else {
            foreach (LabLevelGenerator.wall wall in levelSpace.walls) {
                if (wall.wallType == LabLevelGenerator.wallTypes.door) {
                    if (wall.direction == Vector2.up) {
                        for (int y = upperBound; y < roomSize.y; y++) {
                            SpawnTile(new Vector2(leftBound, y), wallTilemap, levelSpace.room.wallTile);
                            SpawnTile(new Vector2(rightBound, y), wallTilemap, levelSpace.room.wallTile);
                        }
                        for (int i = 0; i < hallwayWidth; i++) {
                            SpawnTile(new Vector2(leftBound + 1 + i, roomSize.y - 1), wallTilemap, levelSpace.room.wallTile);
                        }
                    }
                    else if (wall.direction == Vector2.right) {
                        for (int x = rightBound; x < roomSize.x; x++) {
                            SpawnTile(new Vector2(x, lowerBound), wallTilemap, levelSpace.room.wallTile);
                            SpawnTile(new Vector2(x, upperBound), wallTilemap, levelSpace.room.wallTile);
                        }
                        for (int i = 0; i < hallwayWidth; i++) {
                            SpawnTile(new Vector2(roomSize.x - 1, lowerBound + 1 + i), wallTilemap, levelSpace.room.wallTile);
                        }
                    }
                    else if (wall.direction == Vector2.down) {
                        for (int y = 0; y < lowerBound + 1; y++) {
                            SpawnTile(new Vector2(leftBound, y), wallTilemap, levelSpace.room.wallTile);
                            SpawnTile(new Vector2(rightBound, y), wallTilemap, levelSpace.room.wallTile);
                        }
                        for (int i = 0; i < hallwayWidth; i++) {
                            SpawnTile(new Vector2(leftBound + 1 + i, 0), wallTilemap, levelSpace.room.wallTile);
                        }
                    }
                    else {
                        for (int x = 0; x < leftBound + 1; x++) {
                            SpawnTile(new Vector2(x, lowerBound), wallTilemap, levelSpace.room.wallTile);
                            SpawnTile(new Vector2(x, upperBound), wallTilemap, levelSpace.room.wallTile);
                        }
                        for (int i = 0; i < hallwayWidth; i++) {
                            SpawnTile(new Vector2(0, lowerBound + 1 + i), wallTilemap, levelSpace.room.wallTile);
                        }
                    }
                }
                else {
                    if (wall.direction == Vector2.up) {
                        for (int x = leftBound; x < rightBound + 1; x++) {
                            SpawnTile(new Vector2(x, upperBound), wallTilemap, levelSpace.room.wallTile);
                        }
                    }
                    else if (wall.direction == Vector2.right) {
                        for (int y = lowerBound; y < upperBound + 1; y++) {
                            SpawnTile(new Vector2(rightBound, y), wallTilemap, levelSpace.room.wallTile);
                        }
                    }
                    else if (wall.direction == Vector2.down) {
                        for (int x = leftBound; x < rightBound + 1; x++) {
                            SpawnTile(new Vector2(x, lowerBound), wallTilemap, levelSpace.room.wallTile);
                        }
                    }
                    else {
                        for (int y = lowerBound; y < upperBound + 1; y++) {
                            SpawnTile(new Vector2(leftBound, y), wallTilemap, levelSpace.room.wallTile);
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
        }
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
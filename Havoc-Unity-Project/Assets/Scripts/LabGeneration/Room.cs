﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Room", menuName = "Room")]
public class Room : ScriptableObject
{    
    public string roomName;
    public enum RoomTypes { empty, room, entrance, exit, bossRoom, loreRoom, hallway, border }
    public RoomTypes type;
    public bool isCountedAsRoom;

    [Space]
    public int minNumDoors;
    public int maxNumDoors;
    public int maxNumAllowedPerLevel;

    [Space]
    public float chanceToSpawn;
    public float chanceToSpawnDoor;

    [Space]
    public TileBase floorTile;
    public TileBase wallTile;
    public TileBase doorTile;
}

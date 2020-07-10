using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabLevelGenerator : MonoBehaviour
{
//     public Vector2Int levelSize;
//     public LabRoomGenerator[,] labLevelGrid;
//     public Vector2 entranceLocation;

//     public List<Room> roomTypes; // 0. Entrance

//     public LabRoomGenerator labRoomGenPrefab;
//     public Vector2 roomSize;
//     public Transform roomList;

//     private const int UP_INDEX = 0;
//     private const int RIGHT_INDEX = 1;
//     private const int DOWN_INDEX = 2;
//     private const int LEFT_INDEX = 3;

//     private void Start() {
//         Setup();        
//     }

//     public void Setup() {
//         labLevelGrid = new LabRoomGenerator[levelSize.x, levelSize.y];
//         labRoomGenPrefab.roomSize = roomSize;
//         labRoomGenPrefab.labLevelGenPrefab = this;
//         LabRoomGenerator entrance = SpawnEntrance();
//         GenerateLevel(entrance, entranceLocation, 0, levelSize.x / 2, levelSize.y / 2);
//     }

//     public LabRoomGenerator SpawnEntrance() {
//         LabRoomGenerator entrance = SpawnRoom(roomTypes[0], "Entrance", entranceLocation, levelSize.x / 2, levelSize.y / 2);
//         return entrance;             
//     }

//     public LabRoomGenerator SpawnRoom(Room room, string roomName, Vector2 physicalLocation, int gridXPos, int gridYPos) {
//         labRoomGenPrefab.room = room;
//         labRoomGenPrefab.physicalLocation = physicalLocation;
//         labRoomGenPrefab.gridXPos = gridXPos;
//         labRoomGenPrefab.gridYPos = gridYPos;                          

//         labLevelGrid[gridXPos, gridYPos] = Instantiate(labRoomGenPrefab, ToInt3(physicalLocation), Quaternion.identity);
//         labLevelGrid[gridXPos, gridYPos].name = roomName;
//         labLevelGrid[gridXPos, gridYPos].transform.parent = roomList;

//         return labLevelGrid[gridXPos, gridYPos];        
//     }

//     public void GenerateLevel(LabRoomGenerator labRoom, Vector2 location, int roomCount, int gridXPos, int gridYPos) {
//         if (labRoom.directionsToMove.Count == 0 || roomCount > 10) {
//             return;
//         }

//         foreach (Vector2 dir in labRoom.directionsToMove) {
//             roomCount++;
//             location += dir;
//             gridXPos += (int) dir.x;
//             gridYPos += (int) dir.y;
//             LabRoomGenerator nextRoom = SpawnRoom(roomTypes[0], "Room " + roomCount, location, gridXPos, gridYPos);
//             GenerateLevel(nextRoom, location, roomCount, gridXPos, gridYPos);
//         }
//     }

//     private Vector3Int ToInt3(Vector2 v) {
//         return new Vector3Int((int)v.x, (int)v.y, 0);
//     }
//     /*
//     0.5 Setup math stuff (room)
//     1. Spawn entrance room
//     2. Get the next rooms' directions from the lab room gen
//     3. 
//     */
}

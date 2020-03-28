using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryRoomSpawner : RoomSpawner
{
    private void Start()
    {
        
    }

    //private void EntryRoomSpawn()
    //{
    //    if (!hasSpawned)
    //    {
    //        if (openingDirection == 1)
    //        {
    //            // Need to spawn a room with a BOTTOM door
    //            rand = Random.Range(0, .bottomRooms.Length);
    //            Instantiate(base.GetRoomTemplates().bottomRooms[rand], transform.position, Quaternion.identity);
    //        }
    //        else if (openingDirection == 2)
    //        {
    //            // Need to spawn a room with a TOP door
    //            rand = Random.Range(0, base.GetRoomTemplates().topRooms.Length);
    //            Instantiate(base.GetRoomTemplates().topRooms[rand], transform.position, Quaternion.identity);
    //        }
    //        else if (openingDirection == 3)
    //        {
    //            // Need to spawn a room with a LEFT door
    //            rand = Random.Range(0, base.GetRoomTemplates().leftRooms.Length);
    //            Instantiate(base.GetRoomTemplates().leftRooms[rand], transform.position, Quaternion.identity);
    //        }
    //        else if (openingDirection == 4)
    //        {
    //            // Need to spawn a room with a RIGHT door
    //            rand = Random.Range(0, base.GetRoomTemplates().rightRooms.Length);
    //            Instantiate(base.GetRoomTemplates().rightRooms[rand], transform.position, Quaternion.identity);
    //        }
    //        hasSpawned = true;
    //    }
    //}
}

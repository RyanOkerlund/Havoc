using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    // 1 = need bottom door
    // 2 = need top door
    // 3 = need left door 
    // 4 = need right door

    public float waitTimeToDestroy = 4f;

    private RoomTemplates templates;
    private int rand;
    private bool hasSpawned = false;

    private void Start()
    {
        Destroy(gameObject, waitTimeToDestroy);
        templates = GameObject.FindGameObjectWithTag("Dungeon").GetComponent<DungeonManager>().templates;
        Invoke("Spawn", 0.1f);
    }

    private void Spawn()
    {
        GameObject spawnedRoom;
        if (!hasSpawned)
        {
            if (openingDirection == 1)
            {
                // Need to spawn a room with a BOTTOM door
                rand = Random.Range(0, templates.bottomRooms.Length);
                spawnedRoom = Instantiate(templates.bottomRooms[rand], transform.position, Quaternion.identity);
                spawnedRoom.transform.SetParent(GameObject.FindGameObjectWithTag("Rooms").transform);
            }
            else if (openingDirection == 2)
            {
                // Need to spawn a room with a TOP door
                rand = Random.Range(0, templates.topRooms.Length);
                spawnedRoom = Instantiate(templates.topRooms[rand], transform.position, Quaternion.identity);
                spawnedRoom.transform.SetParent(GameObject.FindGameObjectWithTag("Rooms").transform);
            }
            else if (openingDirection == 3)
            {
                // Need to spawn a room with a LEFT door
                rand = Random.Range(0, templates.leftRooms.Length);
                spawnedRoom = Instantiate(templates.leftRooms[rand], transform.position, Quaternion.identity);
                spawnedRoom.transform.SetParent(GameObject.FindGameObjectWithTag("Rooms").transform);
            }
            else if (openingDirection == 4)
            {
                // Need to spawn a room with a RIGHT door
                rand = Random.Range(0, templates.rightRooms.Length);
                spawnedRoom = Instantiate(templates.rightRooms[rand], transform.position, Quaternion.identity);
                spawnedRoom.transform.SetParent(GameObject.FindGameObjectWithTag("Rooms").transform);
            }
            hasSpawned = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject closedRoom;
        if (collision.CompareTag("SpawnPoint"))
        {
            if (collision.GetComponent<RoomSpawner>().hasSpawned == false && !hasSpawned)
            {
                // Spawn closed room
                closedRoom = Instantiate(templates.closedRoom, transform.position, Quaternion.identity);
                closedRoom.transform.SetParent(GameObject.FindGameObjectWithTag("Rooms").transform);
                Destroy(this.gameObject);
            }
            hasSpawned = true;
        }
    }
}

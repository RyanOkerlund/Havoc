using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public RoomTemplates templates;
    public List<GameObject> rooms;

    public float waitTime;
    private bool spawnedBoss;
    public GameObject boss;    

    private void Start()
    {
        Invoke("AddRooms", 1.5f);
    }

    private void Update()
    {
        if (waitTime <= 0 && !spawnedBoss)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                if (i == rooms.Count - 1)
                {
                    Instantiate(boss, rooms[i].transform.position, Quaternion.identity);
                    spawnedBoss = true;
                }
            }
        }
        else
        {
            waitTime -= Time.deltaTime;
        }
    }

    private void AddRooms()
    {
        for (int i = 0; i < this.transform.GetChild(0).childCount; i++)
        {
            rooms.Add(this.transform.GetChild(0).GetChild(i).gameObject);
        }
    }
}

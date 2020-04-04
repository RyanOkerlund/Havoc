using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Generator : MonoBehaviour
{
    public Vector2 gridSizeWorldUnits;
    [HideInInspector] public int gridWidth, gridHeight;

    public float worldUnitsPerOneGridCell;
    public float percentGridCovered;

    public struct generator
    {
        public Vector2 pos;
        public Vector2 dir;
    }

    public List<generator> generators;
    public int maxGenerators;

    public float chanceToChangeDir;
    public float chanceToDestoryGen;
    public float chanceToSpawnGen;

    public void SetupGridSize()
    {
        gridWidth = Mathf.FloorToInt(gridSizeWorldUnits.x / worldUnitsPerOneGridCell);
        gridHeight = Mathf.FloorToInt(gridSizeWorldUnits.y / worldUnitsPerOneGridCell);
    }

    public void SetupFirstGenerator()
    {
        generators = new List<generator>();
        generator newGenerator = new generator();

        newGenerator.dir = RandomDirection();
        Vector2 spawnPos = new Vector2(Mathf.FloorToInt(gridWidth / 2.0f), Mathf.FloorToInt(gridHeight / 2.0f));
        newGenerator.pos = spawnPos;

        generators.Add(newGenerator);
    }

    public Vector2 RandomDirection()
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

    public void DestoryGen()
    {
        int numGens = generators.Count; // might change in the loop DON'T CHANGE!
        for (int i = 0; i < numGens; i++)
        {
            if (Random.value < chanceToDestoryGen && generators.Count > 1)
            {
                generators.RemoveAt(i);
                break;
            }
        }
    }

    public void SpawnGen()
    {
        int numGens = generators.Count; // might change in the loop DON'T CHANGE!
        for (int i = 0; i < numGens; i++)
        {
            if (Random.value < chanceToSpawnGen && generators.Count < maxGenerators)
            {
                generator newGen = new generator();
                newGen.dir = RandomDirection();
                newGen.pos = generators[i].pos;
                generators.Add(newGen);
            }
        }
    }

    public void ChangeGenDir()
    {
        for (int i = 0; i < generators.Count; i++)
        {
            if (Random.value < chanceToChangeDir)
            {
                generator targetGen = generators[i];
                targetGen.dir = RandomDirection();
                generators[i] = targetGen;
            }
        }
    }

    public void MoveGen()
    {
        for (int i = 0; i < generators.Count; i++)
        {
            generator targetGen = generators[i];
            targetGen.pos += targetGen.dir;
            generators[i] = targetGen;
        }
    }

    public void ClampGen()
    {
        for (int i = 0; i < generators.Count; i++)
        {
            generator targetGen = generators[i];
            targetGen.pos.x = Mathf.Clamp(targetGen.pos.x, 1, gridWidth - 2);
            targetGen.pos.y = Mathf.Clamp(targetGen.pos.y, 1, gridHeight - 2);
            generators[i] = targetGen;
        }
    }
}

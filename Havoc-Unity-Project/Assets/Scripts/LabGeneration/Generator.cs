using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Generator Class:
This class is an abstract class describing main variables and features of a drunken-walker generation algorithm.
Creates generators on a specified grid to "walk" about and generate whatever is chosen.                  
*/
public abstract class Generator : MonoBehaviour
{
    // The "grid" is implemented in classes that extend this class!!!
    [HideInInspector] public Vector2 gridSizeWorldUnits; // The physical x, y space the grid takes up (actual values in Unity)
    [HideInInspector] public int gridWidth, gridHeight; // The width and height of the grid in number of grid spaces

    [HideInInspector] public Vector2 worldUnitsPerOneGridCell; // The size of each space in the grid

    public struct generator // The generator itself. Stores a position in the grid and a direction
    {
        public Vector2 pos;
        public Vector2 dir;
    }

    public List<generator> generators; // List of generators currently in the grid

    // Define the gridWidth and gridHeight
    public void SetupGridSize()
    {
        gridWidth = Mathf.FloorToInt(gridSizeWorldUnits.x / worldUnitsPerOneGridCell.x);
        gridHeight = Mathf.FloorToInt(gridSizeWorldUnits.y / worldUnitsPerOneGridCell.y);
    }

    // Tries to spawn new generators as long as the max isn't reached
    public void SpawnGen(Vector2 dir, Vector2 pos, List<generator> genList)
    {
        generator newGen = new generator();
        newGen.dir = dir;
        newGen.pos = pos;
        genList.Add(newGen);
    }

    // Moves all generators in their direciton on grid space
    public void MoveGen(List<generator> genList)
    {
        for (int i = 0; i < genList.Count; i++)
        {
            generator targetGen = genList[i];
            targetGen.pos += targetGen.dir;
            genList[i] = targetGen;
        }
    }

    // Prevents generator from moving to an edge of the grid
    public void ClampGen(List<generator> genList)
    {
        for (int i = 0; i < genList.Count; i++)
        {
            generator targetGen = genList[i];
            targetGen.pos.x = Mathf.Clamp(targetGen.pos.x, 1, gridWidth - 2);
            targetGen.pos.y = Mathf.Clamp(targetGen.pos.y, 1, gridHeight - 2);
            genList[i] = targetGen;
        }
    }
}

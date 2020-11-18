using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RuleTileTest : MonoBehaviour
{
    public RuleTile ruleTile;

    void Start() {
        Setup();
    }

    private void Setup() {
        RuleTile.TilingRule rule0 = ruleTile.m_TilingRules[0];
        Dictionary<Vector3Int, int> dictNeighbors = new Dictionary<Vector3Int, int>();
        dictNeighbors.Add(new Vector3Int(-15, 1, 0), RuleTile.TilingRuleOutput.Neighbor.NotThis);
        rule0.ApplyNeighbors(dictNeighbors);
    }
}

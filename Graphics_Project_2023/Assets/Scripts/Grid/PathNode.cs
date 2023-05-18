using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {
    private GridPathSystem gridPathSystem;
    private GridPosition gridPosition;

    private int gCost;
    private int hCost;
    private int fCost;
    private PathNode previousPathNode;

    private bool isWalkable = true;

    public PathNode(GridPathSystem gridPathSystem, GridPosition gridPosition) {
        this.gridPathSystem = gridPathSystem;
        this.gridPosition = gridPosition;
    }

    public override string ToString() {
        return gridPosition.ToString();
    }

    public int GetGCost() {
        return gCost;
    }
    public int GetHCost() {
        return hCost;
    }
    public int GetFCost() {
        return fCost;
    }

    public void SetGCost(int gCost) {
        this.gCost = gCost;
    }
    public void SetHCost(int hCost) {
        this.hCost = hCost;
    }
    public void SetFCost() {
        this.fCost = gCost + hCost;
    }

    public void SetPreviousNode(PathNode pathNode) {
        previousPathNode = pathNode;
    }

    public PathNode GetPreviousNode() {
        return previousPathNode;
    }

    public void ResetPreviousNode() {
        previousPathNode = null;
    }

    public GridPosition GetGridPosition() {
        return gridPosition;
    }


    public bool IsWalkable() {
        return isWalkable;
    }

    public void SetIsWalkable(bool isWalkable) {
        this.isWalkable = isWalkable;
    }
}

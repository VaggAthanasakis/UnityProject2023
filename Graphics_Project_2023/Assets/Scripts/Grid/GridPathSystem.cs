using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridPathSystem {
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private int width;
    private int height;
    private float cellSize;
    private PathNode[,] gridPathArray;

    

    /* instantiation of the grid */
    public GridPathSystem(int width, int height, float cellSize) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridPathArray = new PathNode[width, height];

        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode newNode = new PathNode(this, gridPosition);
                /* Make the edge nodes no walkable */
                if (x == 0) {
                    newNode.SetIsWalkable(false);
                }
                if (z == 0) {
                    newNode.SetIsWalkable(false);
                }
                if (x == 99) {
                    newNode.SetIsWalkable(false);
                }
                if (z == 79) {
                    newNode.SetIsWalkable(false);
                }
                /* Make the inside limits no walkable */
                if (x == 69 && (z >= 45 && z <= 58)) { newNode.SetIsWalkable(false); }
                if (z == 45 && (x >= 0 && x <= 69)) { newNode.SetIsWalkable(false); }
                if (x == 62 && (z >= 28 && z <= 45)) { newNode.SetIsWalkable(false); }
                if (z == 28 && (x >= 49 && x <= 62)) { newNode.SetIsWalkable(false); }
                if (x == 49 && (z <= 14 && z >= 28)) { newNode.SetIsWalkable(false); }
                if (z == 27 && x >= 77) { newNode.SetIsWalkable(false); }
                if (x == 77 && z <= 27) { newNode.SetIsWalkable(false); }

                /* Make the nodes that are below the houses, no walkable */
                if ((z >=0 && z <= 8) && (x >= 0 && x <= 9)) { newNode.SetIsWalkable(false); }
                if ((x >= 0 && x <= 14) && (z >= 14 && z <= 34)) { newNode.SetIsWalkable(false); }
                if ((x >= 14 && x <= 62) && (z >= 34 && z <= 44)) { newNode.SetIsWalkable(false); }
                if ((x >= 51 && x <= 62) && (z >= 29 && z <= 45)) { newNode.SetIsWalkable(false); }
                if ((x >= 39 && x <= 49) && (z >= 14 && z <= 29)) { newNode.SetIsWalkable(false); }
                if ((x >= 21 && x <= 31) && (z >= 23 && z <= 30)) { newNode.SetIsWalkable(false); }
                if ((x >= 69 && x <= 81) && (z >= 52 && z <= 61)) { newNode.SetIsWalkable(false); }
                if((x >= 22 && x <= 53) && (z <= 5)) { newNode.SetIsWalkable(false); }
                if ((x >= 72) && (z >= 68)) { newNode.SetIsWalkable(false); }
                if (x >= 89 && z >= 55) { newNode.SetIsWalkable(false); }
                gridPathArray[x, z] = newNode;

            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / cellSize),
            Mathf.RoundToInt(worldPosition.z / cellSize)
        );
    }

    public void CreateDebugObjects(Transform debugPrefab, Transform container) {
        for (int x = 0; x < width; x++) {
            for (int z = 0; z < height; z++) {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity, container);
                PathfindingDebugObject pathDebugObject = debugTransform.GetComponent<PathfindingDebugObject>();
                pathDebugObject.SetPathfindingObject(GetPathNode(gridPosition));
            }
        }
    }

    /* Made some changes for out of bounds exception */
    public PathNode GetPathNode(GridPosition gridPosition) {
        /* if it is out of bounds at the positives */
        if (gridPosition.x >= gridPathArray.GetLength(0) || gridPosition.z >= gridPathArray.GetLength(1)) {
            return null;
        }
        /* if it is out of bounds to the negatives (out of the world) */
        if (gridPosition.x < 0 || gridPosition.z < 0 ) {
            return null;
        }
        return gridPathArray[gridPosition.x, gridPosition.z];
    }

    public int GetWidth() {  
        return width;
    }

    public int GetHeight() {
        return height;
    }

    /* combatSearch is true if we search for combat mode */
    public List<GridPosition> FindPath(GridPosition startPosition, GridPosition endPosition, bool combatSearch) {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = GetPathNode(startPosition);
        PathNode endNode = GetPathNode(endPosition);
        if (endNode == null) return null;
        if (!endNode.IsWalkable() && !combatSearch) {
            Debug.Log("Cannot Move There!");
            UI_Manager.Instance.SetGameInfo("Cannot Move There!");
            return null;
        }

        openList.Add(startNode);
         
        for (int x = 0; x < GetWidth(); x++) {
            for (int z = 0; z < GetHeight(); z++) {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = GetPathNode(gridPosition);

                pathNode.SetGCost(int.MaxValue);
                pathNode.SetHCost(0);
                pathNode.SetFCost();
            }
        }
        startNode.SetGCost(0);
        startNode.SetHCost(CalculateDistance(startPosition, endPosition));
        startNode.SetFCost();


        while (openList.Count > 0) {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            //End
            if (currentNode == endNode) {
                /*  */
                startNode.SetIsWalkable(true);
                if (combatSearch) {
                    endNode.SetIsWalkable(true);
                }
                else {
                    endNode.SetIsWalkable(false);
                }
                //Debug.Log("Returning 1");
                return CalculatePath(startNode, endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
                if (closedList.Contains(neighbourNode))
                    continue;
             
                if (!neighbourNode.IsWalkable() && !combatSearch) {
                    closedList.Add(neighbourNode);
                    continue;
                }

                /* If there are gameObjects at this node, then set it noWalkable */
                if (CheckForColliderAtNode(neighbourNode)) {
                    neighbourNode.SetIsWalkable(false);
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tenativeGCost = currentNode.GetGCost() + CalculateDistance(currentNode.GetGridPosition(), neighbourNode.GetGridPosition());

                if (tenativeGCost < neighbourNode.GetGCost()) {
                    neighbourNode.SetPreviousNode(currentNode);
                    neighbourNode.SetGCost(tenativeGCost);
                    neighbourNode.SetHCost(CalculateDistance(neighbourNode.GetGridPosition(), endPosition));
                    neighbourNode.SetFCost();

                    if (!openList.Contains(neighbourNode))
                        openList.Add(neighbourNode);
                }
            }
        }

        return null;
    }

    private bool CheckForColliderAtNode(PathNode node) {
        GridPosition nodePos = node.GetGridPosition();
        Vector3 worldPos = PathFinding.Instance.Grid().GetWorldPosition(nodePos);
        bool hasHit = Physics.Raycast(worldPos, Vector3.up, out RaycastHit hit, float.MaxValue, PathFinding.Instance.gameObjectsLayerMask);
        if (hasHit) {
            return true;
        }
        return false;
    }


    public int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB) {
        GridPosition gridPositionDistance = (gridPositionA - gridPositionB);

        /*int distance = Mathf.Abs(gridPositionDistance.x) + Mathf.Abs(gridPositionDistance.z);
        return distance * MOVE_STRAIGHT_COST;*/

        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;

    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList) {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].GetFCost() < lowestFCostPathNode.GetFCost()) {
                lowestFCostPathNode = pathNodeList[i];
            }
        }
        return lowestFCostPathNode;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        List<PathNode> neighbourList = new List<PathNode>();

        GridPosition gridPosition = currentNode.GetGridPosition();

        if (gridPosition.x - 1 >= 0) {
            // Left
            neighbourList.Add(GetPathNode(new GridPosition(gridPosition.x - 1, gridPosition.z + 0)));
            if (gridPosition.z - 1 >= 0) {
                // Left Down
                neighbourList.Add(GetPathNode(new GridPosition(gridPosition.x - 1, gridPosition.z - 1)));
            }

            if (gridPosition.z + 1 < GetHeight()) {
                // Left Up
                neighbourList.Add(GetPathNode(new GridPosition(gridPosition.x - 1, gridPosition.z + 1)));
            }
        }

        if (gridPosition.x + 1 < GetWidth()) {
            // Right
            neighbourList.Add(GetPathNode(new GridPosition(gridPosition.x + 1, gridPosition.z + 0)));
            if (gridPosition.z - 1 >= 0) {
                // Right Down
                neighbourList.Add(GetPathNode(new GridPosition(gridPosition.x + 1, gridPosition.z - 1)));
            }
            if (gridPosition.z + 1 < GetHeight()) {
                // Right Up
                neighbourList.Add(GetPathNode(new GridPosition(gridPosition.x + 1, gridPosition.z + 1)));
            }
        }

        if (gridPosition.z - 1 >= 0) {
            // Down
            neighbourList.Add(GetPathNode(new GridPosition(gridPosition.x + 0, gridPosition.z - 1)));
        }
        if (gridPosition.z + 1 < GetHeight()) {
            // Up
            neighbourList.Add(GetPathNode(new GridPosition(gridPosition.x + 0, gridPosition.z + 1)));
        }

        return neighbourList;
    }

    private List<GridPosition> CalculatePath(PathNode startNode,PathNode endNote) {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNote);

        PathNode currentNode = endNote;
        while (currentNode.GetPreviousNode() != null && currentNode != startNode) {
            pathNodeList.Add(currentNode.GetPreviousNode());
            currentNode = currentNode.GetPreviousNode();
        }
        pathNodeList.Reverse();

        List<GridPosition> pathPositionsList = new List<GridPosition>();
        foreach (PathNode pathNode in pathNodeList) {
            pathPositionsList.Add(pathNode.GetGridPosition());
        }

        return pathPositionsList;
    }
}

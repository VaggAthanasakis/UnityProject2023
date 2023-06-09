using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGameObject : MonoBehaviour {
    // Start is called before the first frame update
    private void Start() {
        SetSpawnedNodeNoWalkable();
    }

    /* Create a method that will make the node in which the object is above, no walkable */
    private void SetSpawnedNodeNoWalkable() {
        GridPosition position = PathFinding.Instance.GetGridPosition(this.transform.position);
        PathNode node = PathFinding.Instance.Grid().GetPathNode(position);
        if (node != null) {
            node.SetIsWalkable(false);
        }
    
    }


}

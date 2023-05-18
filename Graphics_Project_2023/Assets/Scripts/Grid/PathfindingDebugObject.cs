using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PathfindingDebugObject : MonoBehaviour
{
    [SerializeField] private TMP_Text _poistionText;

    [SerializeField] private TMP_Text _gCostText;
    [SerializeField] private TMP_Text _hCostText;
    [SerializeField] private TMP_Text _fCostText;

    [SerializeField] private bool _isWalkable;

    private PathNode pathNode;

    public void SetPathfindingObject(PathNode pathNode)
    {
        this.pathNode = pathNode;
        
    }

    private void Update()
    {
        _poistionText.text = pathNode.ToString();
        _gCostText.text = pathNode.GetGCost().ToString();
        _hCostText.text = pathNode.GetHCost().ToString();
        _fCostText.text = pathNode.GetFCost().ToString();
        this.pathNode.SetIsWalkable(pathNode.IsWalkable());
        _isWalkable = this.pathNode.IsWalkable();
        //pathNode.SetIsWalkable(_isWalkable);

    }
}
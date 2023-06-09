using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour
{
    public static LevelGrid Instance { get; private set; }

    [SerializeField] private Transform gridDebugObjectPrefab;

    private GridSystem gridSystem;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //Grid Creation

        //10 x 100 Grid , 1 unit tile size
        gridSystem = new GridSystem(20, 20, 1f);
        //Create Debug Objects on each grid tile
        gridSystem.CreateDebugObjects(gridDebugObjectPrefab, this.transform); 
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Heroes hero)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.AddUnit(hero);
    }

    public List<Heroes> GetUnitListAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Heroes unit)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Heroes hero, GridPosition fromGridPosition, GridPosition toGridPosition)
    {   
        //Left from this Grid Position
        RemoveUnitAtGridPosition(fromGridPosition, hero);
        //Went to this Grid Posiition
        AddUnitAtGridPosition(toGridPosition, hero);
    }

    //Returns the grid position when given a world Vector3 position
    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return gridSystem.GetGridPosition(worldPosition);
    }

}
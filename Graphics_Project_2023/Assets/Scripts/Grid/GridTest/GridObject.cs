using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridSystem gridSystem;
    private GridPosition gridPosition;
    private List<Heroes> unitList;

    public GridObject(GridSystem gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitList = new List<Heroes>();
    }

    public override string ToString()
    {
        string unitString = "";
        foreach (Heroes unit in unitList)
        {
            unitString += unit + "\n";
        }

        return gridPosition.ToString() + "\n" + unitString;
    }

    public void AddUnit(Heroes unit)
    {
        unitList.Add(unit);
    }

    public void RemoveUnit(Heroes unit)
    {
        unitList.Remove(unit);
    }

    public List<Heroes> GetUnitList()
    {
        return unitList;
    }

}
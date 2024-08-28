using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placementObjs = new();

    public void AddObject(Vector3Int gridPos, Vector2Int objectSize, int id, int placedObjectIndex)
    {
        List<Vector3Int> positionToPlacement = CalculatePosition(gridPos, objectSize);
        PlacementData data = new PlacementData(positionToPlacement, id, placedObjectIndex);

        foreach(var pos in positionToPlacement)
        {
            if(placementObjs.ContainsKey(pos))
            {
                throw new Exception($"Dictionary already contains this cell position {pos}");
            }

            placementObjs[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePosition(Vector3Int gridPos, Vector2Int objectSize)
    {
        List<Vector3Int> result = new();
        
        for(int x = 0; x < objectSize.x; x++)
        {
            for(int y = 0; y < objectSize.y; y++)
            {
                result.Add(gridPos + new Vector3Int(x, 0, y));
            }
        }

        return result;
    }

    public bool IfCanPlaceObject(Vector3Int gridPos, Vector2Int objectSize)
    {
        List<Vector3Int> positionToPlacement = CalculatePosition(gridPos, objectSize);

        foreach(var pos in positionToPlacement) 
        {
            if (placementObjs.ContainsKey(pos)) return false;
        }
        return true;
    }
}

public class PlacementData
{ 
    public List<Vector3Int> placementPosition;
    public int Id { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> placementPosition, int id, int placedObjectIndex)
    {
        this.placementPosition = placementPosition;
        this.Id = id;
        this.PlacedObjectIndex = placedObjectIndex;
    }
}

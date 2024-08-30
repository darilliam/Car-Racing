using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuilldingState
{
    private Grid _grid;
    private int _gameObjectIndex = -1;
    private PreviewRoadPartsController _previewRoadPartsController;
    private ObjectPlacer _objectPlacer;
    private GridData _gridData;
    private GridData _roadData;

    public RemovingState(Grid grid,
                         PreviewRoadPartsController previewRoadPartsController,
                         ObjectPlacer objectPlacer,
                         GridData gridData,
                         GridData roadData)
    {
        _grid = grid;
        _previewRoadPartsController = previewRoadPartsController;
        _objectPlacer = objectPlacer;
        _gridData = gridData;
        _roadData = roadData;

        _previewRoadPartsController.StartShowingPreviewOfRemoving();
    }

    public void EnndState()
    {
        _previewRoadPartsController.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPos)
    {
        GridData selectedData = null;

        if(_roadData.IfCanPlaceObject(gridPos, Vector2Int.one) == false)
        {
            selectedData = _roadData;
        }
        else if(_gridData.IfCanPlaceObject(gridPos, Vector2Int.one) == false)
        {
            selectedData = _gridData;
        }

        if (selectedData != null)
        {
            _gameObjectIndex = selectedData.GetRepresentationIndex(gridPos);

            if(_gameObjectIndex == -1) return;

            selectedData.RemoveObject(gridPos);
            _objectPlacer.RemoveObject(_gameObjectIndex);
        }

        Vector3 cellPos = _grid.CellToWorld(gridPos);
        _previewRoadPartsController.UpdatePosition(cellPos, IfSelectionIsValid(gridPos));
    }

    private bool IfSelectionIsValid(Vector3Int gridPos)
    {
        return !(_roadData.IfCanPlaceObject(gridPos, Vector2Int.one) && _gridData.IfCanPlaceObject(gridPos, Vector2Int.one));
    }

    public void UpdateState(Vector3Int gridPos)
    {
        bool validity = IfSelectionIsValid(gridPos);
        _previewRoadPartsController.UpdatePosition(_grid.CellToWorld(gridPos), validity);
    }
}

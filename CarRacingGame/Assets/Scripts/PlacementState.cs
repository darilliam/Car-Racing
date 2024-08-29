using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementState : IBuilldingState
{
    private Grid _grid;
    private int _id;
    private ObjectsDatabase _objectsDatabase;
    private int _selectedObjectIndex = -1;
    private PreviewRoadPartsController _previewRoadPartsController;
    private ObjectPlacer _objectPlacer;
    private GridData _gridData;
    private GridData _roadData;

    public PlacementState(Grid grid,
                          int id,
                          ObjectsDatabase objectsDatabase,
                          PreviewRoadPartsController previewRoadPartsController,
                          ObjectPlacer objectPlacer,
                          GridData gridData,
                          GridData roadData)
    {
        _grid = grid;
        _id = id;
        _objectsDatabase = objectsDatabase;
        _previewRoadPartsController = previewRoadPartsController;
        _objectPlacer = objectPlacer;
        _gridData = gridData;
        _roadData = roadData;

        _selectedObjectIndex = _objectsDatabase.objectData.FindIndex(data => data.Id == id);

        if (_selectedObjectIndex > -1)
        {
            _previewRoadPartsController.StartShowingPreviewOfPlacement(_objectsDatabase.objectData[_selectedObjectIndex].Prefab, _objectsDatabase.objectData[_selectedObjectIndex].Size);
        }
        else
        {
            throw new Exception($"No object with ID {id}");
        }
    }

    public void EnndState()
    {
        _previewRoadPartsController.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPos)
    {
        bool ifPlacementValid = CheckPlacementValidation(gridPos, _selectedObjectIndex);
        if (ifPlacementValid == false) return;

        int idx = _objectPlacer.PlaceObject(_objectsDatabase.objectData[_selectedObjectIndex].Prefab, _grid.CellToWorld(gridPos));

        GridData selectedData = _objectsDatabase.objectData[_selectedObjectIndex].Id == 0 ? _gridData : _roadData;
        selectedData.AddObject(gridPos, _objectsDatabase.objectData[_selectedObjectIndex].Size, _objectsDatabase.objectData[_selectedObjectIndex].Id, idx);

        _previewRoadPartsController.UpdatePosition(_grid.CellToWorld(gridPos), false);
    }

    private bool CheckPlacementValidation(Vector3Int gridPos, int selectedObjectIndex)
    {
        GridData selectedData = _objectsDatabase.objectData[selectedObjectIndex].Id == 0 ? _gridData : _roadData;

        return selectedData.IfCanPlaceObject(gridPos, _objectsDatabase.objectData[selectedObjectIndex].Size);
    }

    public void UpdateState(Vector3Int gridPos)
    {
        bool ifPlacementValid = CheckPlacementValidation(gridPos, _selectedObjectIndex);

        _previewRoadPartsController.UpdatePosition(_grid.CellToWorld(gridPos), ifPlacementValid);
    }
}

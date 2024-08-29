using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    [SerializeField] private GameObject mouseTag;
    [SerializeField] private Grid grid;
    [SerializeField] private PlacementInputManager inputManager;
    [SerializeField] private ObjectsDatabase objectsDatabase;
    [SerializeField] private GameObject gridPlaneVisualization;
    [SerializeField] private PreviewRoadPartsController previewRoadPartsController;
    [SerializeField] private ObjectPlacer objectPlacer;

    private GridData _gridData; 
    private GridData _roadData;
    private Vector3Int _lastDetectedPos = Vector3Int.zero;
    private IBuilldingState _builldingState;

    private void Start()
    {
        StopPlacement();
        _gridData = new GridData();
        _roadData = new GridData();
    }

    public void StartPlacement(int id)
    {
        StopPlacement();
        gridPlaneVisualization.SetActive(true);

        _builldingState = new PlacementState(grid, id, objectsDatabase, previewRoadPartsController, objectPlacer, _gridData, _roadData);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI()) return;

        Vector3 mousePos = inputManager.GetSelectedGridPosition();
        Vector3Int gridPos = grid.WorldToCell(mousePos);

       _builldingState.OnAction(gridPos);
    }

    private void StopPlacement()
    {
        if(_builldingState == null) return;
        gridPlaneVisualization.SetActive(false);
        _builldingState.EnndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        _lastDetectedPos = Vector3Int.zero;
        _builldingState = null;
    }

    void Update()
    {
        if (_builldingState == null) return;

        Vector3 mousePos = inputManager.GetSelectedGridPosition();

        if (mousePos != Vector3.zero)
        {
            Vector3Int gridPos = grid.WorldToCell(mousePos);

            if (_lastDetectedPos != gridPos)
            {
                _builldingState.UpdateState(gridPos);
                _lastDetectedPos = gridPos;
            }
        }
    }
}

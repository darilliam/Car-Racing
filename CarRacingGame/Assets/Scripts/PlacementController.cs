using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    [SerializeField] private GameObject mouseTag;
    [SerializeField] private GameObject cellTag;
    [SerializeField] private Grid grid;
    [SerializeField] private PlacementInputManager inputManager;
    [SerializeField] private ObjectsDatabase objectsDatabase;
    [SerializeField] private int selectedObjectIndex = -1;
    [SerializeField] private GameObject gridPlaneVisualization;

    private GridData _gridData; 
    private GridData _roadData;
    private Renderer _previewRenderer;
    private List<GameObject> _placedObjects = new List<GameObject>();

    private void Start()
    {
        StopPlacement();
        _gridData = new GridData();
        _roadData = new GridData();
        _previewRenderer = cellTag.GetComponent<Renderer>();
    }

    public void StartPlacement(int id)
    {
        StopPlacement();
        selectedObjectIndex = objectsDatabase.objectData.FindIndex(data => data.Id == id);
        gridPlaneVisualization.SetActive(true);
        cellTag.SetActive(true);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if(inputManager.IsPointerOverUI()) return;

        Vector3 mousePos = inputManager.GetSelectedGridPosition();
        Vector3Int gridPos = grid.WorldToCell(mousePos);

        bool ifPlacementValid = CheckPlacementValidation(gridPos, selectedObjectIndex);
        if(ifPlacementValid == false) return;

        GameObject newGameObject = Instantiate(objectsDatabase.objectData[selectedObjectIndex].Prefab);
        newGameObject.transform.position = grid.CellToWorld(gridPos);
        Vector3 pos = cellTag.transform.position;
        pos.y = 0.7f;
        newGameObject.transform.position = pos;

        _placedObjects.Add(newGameObject);

        GridData selectedData = objectsDatabase.objectData[selectedObjectIndex].Id == 0 ? _gridData : _roadData;
        selectedData.AddObject(gridPos, objectsDatabase.objectData[selectedObjectIndex].Size, objectsDatabase.objectData[selectedObjectIndex].Id, _placedObjects.Count - 1);

    }

    private void StopPlacement()
    {
        selectedObjectIndex= -1;
        gridPlaneVisualization.SetActive(false);
        cellTag.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }

    private bool CheckPlacementValidation(Vector3Int gridPos, int selectedObjectIndex)
    {
        GridData selectedData = objectsDatabase.objectData[selectedObjectIndex].Id == 0 ? _gridData : _roadData;

        return selectedData.IfCanPlaceObject(gridPos, objectsDatabase.objectData[selectedObjectIndex].Size);
    }

    void Update()
    {
        if (selectedObjectIndex < 0) return;

        Vector3 mousePos = inputManager.GetSelectedGridPosition();

        if (mousePos != Vector3.zero)
        {
            Vector3Int gridPos = grid.WorldToCell(mousePos);

            bool ifPlacementValid = CheckPlacementValidation(gridPos, selectedObjectIndex);
            _previewRenderer.material.color = ifPlacementValid ? Color.white : Color.red;

            mouseTag.transform.position = mousePos;
            cellTag.transform.position = grid.CellToWorld(gridPos);
            Vector3 pos = cellTag.transform.position;
            pos.x += 5;
            pos.z += 5;
            pos.y = 0.6f;
            cellTag.transform.position = pos;
        }
    }
}

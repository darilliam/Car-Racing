using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    [SerializeField] private GameObject mouseTag;
    [SerializeField] private GameObject cellTag;
    [SerializeField] private Grid grid;
    [SerializeField] private PlacementInputManager inputManager;

    void Update()
    {
        Vector3 mousePos = inputManager.GetSelectedGridPosition();

        if (mousePos != Vector3.zero)
        {
            Vector3Int gridPos = grid.WorldToCell(mousePos);
            mouseTag.transform.position = mousePos;
            cellTag.transform.position = grid.CellToWorld(gridPos);
            Vector3 pos = cellTag.transform.position;
            pos.y = 0.0f;
            cellTag.transform.position = pos;
        }
    }
}

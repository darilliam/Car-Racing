using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacementInputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementMask;

    public event Action OnClicked, OnExit;


    private Vector3 _finalPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClicked?.Invoke();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        { 
            OnExit?.Invoke(); 
        }
    }

    public bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public Vector3 GetSelectedGridPosition()
    {
        Vector3 mosePos = Input.mousePosition;
        mosePos.z = sceneCamera.nearClipPlane;
        Ray ray = sceneCamera.ScreenPointToRay(mosePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementMask))
        {
            _finalPosition = hit.point;
        }
        else
        {
            _finalPosition = Vector3.zero;
        }

        return _finalPosition;
    }
}

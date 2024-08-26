using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementInputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementMask;

    private Vector3 _finalPosition;

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

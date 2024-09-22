using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationButtonController : MonoBehaviour
{
    private GameObject _currentObject;
    private float _rotationAngle = 0; 

    public void SetCurrentObject(GameObject currentObject)
    {
        _currentObject = currentObject;
    }

    public void RotateLeft()
    {
        if (_currentObject != null)
        {
            _rotationAngle -= 90;
            _currentObject.transform.rotation = Quaternion.Euler(0, _rotationAngle, 0);
        }
    }

    public void RotateRight()
    {
        if (_currentObject != null)
        {
            _rotationAngle += 90;
            _currentObject.transform.rotation = Quaternion.Euler(0, _rotationAngle, 0);
        }
    }
}

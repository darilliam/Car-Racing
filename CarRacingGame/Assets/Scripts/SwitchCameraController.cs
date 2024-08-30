using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCameraController : MonoBehaviour
{
    [SerializeField] private GameObject upperCamera;
    [SerializeField] private GameObject carCamera;

    private int _controlerNum = 0;

    private void CameraManager()
    {
        if(_controlerNum == 0)
        {
            UpperCameraActive();
            _controlerNum = 1;
        }
        else if(_controlerNum == 1) 
        {
            _controlerNum = 0;
        }
        else throw new Exception($"Wrong controler number, current {_controlerNum}");
    }

    public void CarCameraActive()
    {
        carCamera.SetActive(true);
        upperCamera.SetActive(false);
    }

    public void UpperCameraActive()
    {
        carCamera.SetActive(false);
        upperCamera.SetActive(true);
    }
}

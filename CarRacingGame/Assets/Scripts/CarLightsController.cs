using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarLightsController : MonoBehaviour
{
    public enum Side
    {
        Front,
        Back
    }

    [Serializable]
    public struct Light
    {
        public GameObject light;
        public Material lightMaterial;
        public Side side;
    }

    public Light[] lights;
    public bool isBackLightOn;
    public Color backLightOnColor;
    public Color backLightOffColor;

    private void Start()
    {
        isBackLightOn = false;
    }

    public void OperateBackLights()
    {
        if(isBackLightOn)
        {
            // Turn on lights
            foreach(Light light in lights) 
            {
                if(light.side == Side.Back && light.light.activeInHierarchy == false)
                {
                    light.light.SetActive(true);
                    light.lightMaterial.color = backLightOnColor;
                }
            }
        }
        else
        {
            // Turn off lights
            foreach (Light light in lights)
            {
                if (light.side == Side.Back && light.light.activeInHierarchy == true)
                {
                    light.light.SetActive(false);
                    light.lightMaterial.color = backLightOffColor;
                }
            }
        }
    }
}

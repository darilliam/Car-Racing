using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    // Array of wheel colliders
    public WheelCollider[] wheels = new WheelCollider[4];
    public GameObject[] wheelMesh = new GameObject[4];
    public int motorTorque = 100;
    public float steeringMax = 25;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        AnimateWheel();

        if (Input.GetKey(KeyCode.W))
        {
            for(int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = motorTorque;
            }
        }

        if(Input.GetAxis("Horizontal") != 0)
        {
            for (int i = 0; i < wheels.Length -2; i++)
            {
                wheels[i].steerAngle = Input.GetAxis("Horizontal") * steeringMax;
            }
        }
        else
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                wheels[i].steerAngle = 0;
            }
        }
    }

    private void AnimateWheel()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for(int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }
}

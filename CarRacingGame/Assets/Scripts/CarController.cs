using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    internal enum DriveType
    {
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }
    [SerializeField]private DriveType driveType;

    // Array of wheel colliders
    public WheelCollider[] wheels = new WheelCollider[4];
    public GameObject[] wheelMesh = new GameObject[4];
    public int motorTorque = 100;
    public float steeringMax = 25;

    [Header("Car Specs")]
    public float wheelBase; // in meters
    public float rearTrack; // in meters
    public float turnRadius; // in meters

    //public float ackermannAngleLeft;
    //public float ackermannAngleRight;


    private InputManager inputManager;

    // Start is called before the first frame update
    void Start()
    {
        Getobjects();
    }


    private void FixedUpdate()
    {
        AnimateWheel();
        CarMovement();
        SteerCar();
    }

    private void CarMovement()
    {
        //float totalPower;

        if (driveType == DriveType.allWheelDrive)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = inputManager.vertical * (motorTorque / 4);
            }
        }
        else if (driveType == DriveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = inputManager.vertical * (motorTorque / 2);
            }
        }
        else
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                wheels[i].motorTorque = inputManager.vertical * (motorTorque / 2);
            }
        }
    }

    private void SteerCar()
    {
        // The Ackermann steering formula calculates the correct steering angles for the wheels of a vehicle to ensure
        // they follow concentric circles, reducing tire wear and improving handling during turns.

        if(inputManager.horizontal > 0) // is turning right
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * inputManager.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * inputManager.horizontal;
        }
        else if(inputManager.horizontal < 0) // is turning left
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * inputManager.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * inputManager.horizontal;
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
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

    private void Getobjects()
    {
        inputManager = GetComponent<InputManager>();
    }
}

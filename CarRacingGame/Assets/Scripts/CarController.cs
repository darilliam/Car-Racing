using System;
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
    [SerializeField]
    private DriveType driveType;

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelMesh;
        public WheelCollider wheelCollider;
        public bool isForwardWheel;
    }

    // Array of wheels
    public Wheel[] wheels;
    public int motorTorque = 100;
    public float steeringMax = 25;

    [Header("Car Specs")]
    public float wheelBase; // in meters
    public float rearTrack; // in meters
    public float turnRadius; // in meters
    public float downForceValue = 50;
    // public float handbrakePower;
    public float brakeForce;


    // Kilometres per hour
    public float KPH;

    public float[] slip = new float[4];

    private InputManager _inputManager;
    private Rigidbody _carRb;
    private float _brakeInput;

    [SerializeField]
    private Transform _centerOfMass;

    // Start is called before the first frame update
    void Start()
    {
        Getobjects();
    }


    private void FixedUpdate()
    {
        DownForce();
        AnimateWheel();
        CarMovement();
        Brake();
        SteerCar();
        Friction();
        CheckInput();
    }

    private void CarMovement()
    {

        if (driveType == DriveType.allWheelDrive)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].wheelCollider.motorTorque = _inputManager.vertical * (motorTorque / 4);
            }
        }
        else if (driveType == DriveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].wheelCollider.motorTorque = _inputManager.vertical * (motorTorque / 2);
            }
        }
        else
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                wheels[i].wheelCollider.motorTorque = _inputManager.vertical * (motorTorque / 2);
            }
        }


        // Kilometres per hour
        KPH = _carRb.velocity.magnitude * 3.6f; 

        /*
        if(_inputManager.handbrake)
        {
            wheels[2].wheelCollider.brakeTorque = wheels[3].wheelCollider.brakeTorque = handbrakePower;
        }
        else
        {
            wheels[2].wheelCollider.brakeTorque = wheels[3].wheelCollider.brakeTorque = 0;
        }
        */
    }

    private void SteerCar()
    {
        // The Ackermann steering formula calculates the correct steering angles for the wheels of a vehicle to ensure
        // they follow concentric circles, reducing tire wear and improving handling during turns.

        if(_inputManager.horizontal > 0) // is turning right
        {
            wheels[0].wheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * _inputManager.horizontal;
            wheels[1].wheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * _inputManager.horizontal;
        }
        else if(_inputManager.horizontal < 0) // is turning left
        {
            wheels[0].wheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * _inputManager.horizontal;
            wheels[1].wheelCollider.steerAngle = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * _inputManager.horizontal;
        }
        else
        {
            wheels[0].wheelCollider.steerAngle = 0;
            wheels[1].wheelCollider.steerAngle = 0;
        }

    }

    private void DownForce()
    {
        _carRb.AddForce(-transform.up * downForceValue * _carRb.velocity.magnitude);
    }

    private void Brake()
    {
        foreach(Wheel wheel in wheels)
        {
            if(wheel.isForwardWheel)
            {
                wheel.wheelCollider.brakeTorque = brakeForce * _brakeInput * 0.7f;
            }
            else
            {
                wheel.wheelCollider.brakeTorque = brakeForce * _brakeInput * 0.3f;
            }
        }
    }

    
    private void CheckInput()
    {
        float movingDirectional = Vector3.Dot(transform.forward, _carRb.velocity);

        if ((movingDirectional < -0.5f && _inputManager.vertical > 0) || (movingDirectional > 0.5f && _inputManager.vertical < 0))
        {
            _brakeInput = Math.Abs(_inputManager.vertical);
        }
        else
        {
            _brakeInput = 0;
        }
    }
    

    
    private void Friction()
    {
        for(int i = 0; i < wheels.Length; i++)
        {
            WheelHit wheelHit;
            wheels[i].wheelCollider.GetGroundHit(out wheelHit);

            // Returns the slip or friction loss if we go sideways (drift)
            slip[i] = wheelHit.forwardSlip;
        }
    }
    

    private void AnimateWheel()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for(int i = 0; i < 4; i++)
        {
            wheels[i].wheelCollider.GetWorldPose(out wheelPosition, out wheelRotation);
            wheels[i].wheelMesh.transform.position = wheelPosition;
            wheels[i].wheelMesh.transform.rotation = wheelRotation;
        }
    }

    private void Getobjects()
    {
        _inputManager = GetComponent<InputManager>();
        _carRb = GetComponent<Rigidbody>();
        _carRb.centerOfMass = _centerOfMass.position;
    }
}

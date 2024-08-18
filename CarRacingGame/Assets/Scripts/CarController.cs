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
    [SerializeField] private DriveType driveType;

    [Serializable]
    public struct Wheel
    {
        public GameObject wheelMesh;
        public WheelCollider wheelCollider;
        public GameObject wheelEffectsObj;
        public ParticleSystem driftSmoke;
        public bool isForwardWheel;
    }

    // Array of wheels
    public Wheel[] wheels;
    public int motorTorque = 100;
    public float steeringMax = 25;

    [Header("Car Specs")]
    [SerializeField] private float wheelBase; // in meters
    [SerializeField] private float rearTrack; // in meters
    [SerializeField] private float turnRadius; // in meters
    [SerializeField] private float maxSpeedKPH;
    [SerializeField] private float downForceValue = 50.0f;
    // public float handbrakePower;
    [SerializeField] private float brakeAcceleration = 50.0f;
    [SerializeField] private float brakeForce;
    [SerializeField] private Transform _centerOfMass;

    [Header("Variables")]
    [SerializeField] private float handBrakeFrictionMultiplier = 2f;


    [Header("DEBUG")]
    // Kilometres per hour
    public float KPH;
    [SerializeField] private float[] slip = new float[4];

    // [HideInInspector] public bool playPauseDriftSmoke = false;

    private InputManager _inputManager;
    private CarLightsController _carLightsController;
    private Rigidbody _carRb;
    private float _brakeInput;
    private WheelFrictionCurve _forwardFriction;
    private WheelFrictionCurve _sidewaysFriction;
    private float _driftFactorValue;


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
        WheelEffects();
        CheckInput();
        LimitMaxSpeed();
        AdjustTraction();
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

    private void LimitMaxSpeed()
    {
        // Current speed in kilometres per hour
        float currentSpeedKPH = _carRb.velocity.magnitude * 3.6f;

        if (currentSpeedKPH > maxSpeedKPH)
        {
            // Limit speed by reducing forward force
            _carRb.velocity = (_carRb.velocity.normalized * maxSpeedKPH) / 3.6f;
        }
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

            if(_inputManager.brakePedal)
            {
                wheel.wheelCollider.brakeTorque = 300 * brakeAcceleration * Time.deltaTime;
                _carLightsController.isBackLightOn = true;
                _carLightsController.OperateBackLights();
            }
            else
            {
                wheel.wheelCollider.brakeTorque = 0;
                _carLightsController.isBackLightOn = false;
                _carLightsController.OperateBackLights();
            }
        }
    }

    private void AdjustTraction()
    {
        // Time factor for smoothing the transition from normal driving to drifting
        float driftSmothFactor = .7f * Time.deltaTime;

        // Check if the brake pedal is being pressed
        if (_inputManager.brakePedal)
        {
            // Store the current friction parameters for later adjustment
            _sidewaysFriction = wheels[0].wheelCollider.sidewaysFriction;
            _forwardFriction = wheels[0].wheelCollider.forwardFriction;

            float velocity = 0;
            // Smoothly adjust friction values to facilitate drifting
            _sidewaysFriction.extremumValue = _sidewaysFriction.asymptoteValue = _forwardFriction.extremumValue = _forwardFriction.asymptoteValue =
                Mathf.SmoothDamp(_forwardFriction.asymptoteValue, _driftFactorValue * handBrakeFrictionMultiplier,ref velocity ,driftSmothFactor );

            // Apply the adjusted friction parameters to all wheels
            for (int i = 0; i < 4; i++) {
                wheels[i].wheelCollider.sidewaysFriction = _sidewaysFriction;
                wheels[i].wheelCollider.forwardFriction = _forwardFriction;
            }

            // Restore higher traction for the front wheels after the drift
            _sidewaysFriction.extremumValue = _sidewaysFriction.asymptoteValue = _forwardFriction.extremumValue = _forwardFriction.asymptoteValue =  1.1f;

            // Apply the updated traction to the front wheels for stability
            for (int i = 0; i < 2; i++) {
                wheels[i].wheelCollider.sidewaysFriction = _sidewaysFriction;
                wheels[i].wheelCollider.forwardFriction = _forwardFriction;
            }

            // Apply force to the car in the forward direction during the drift
            _carRb.AddForce(transform.forward * (KPH / 400) * 40000 );
        }
        // Executed when brake pedal is not being held
        else
        {
            // Store the current friction parameters
            _forwardFriction = wheels[0].wheelCollider.forwardFriction;
			_sidewaysFriction = wheels[0].wheelCollider.sidewaysFriction;

            // Adjust friction based on the car's speed to control traction
            _forwardFriction.extremumValue = _forwardFriction.asymptoteValue = _sidewaysFriction.extremumValue = _sidewaysFriction.asymptoteValue = 
                ((KPH * handBrakeFrictionMultiplier) / 300) + 1;

            // Apply the adjusted friction parameters to all wheels
            for (int i = 0; i < 4; i++) {
				wheels[i].wheelCollider.forwardFriction = _forwardFriction;
				wheels[i].wheelCollider.sidewaysFriction = _sidewaysFriction;

			}
        }

        // Loop through the rear wheels to check the amount of slip
        // This helps to dynamically adjust the drift factor based on the car's slip
        for (int i = 2;i<4 ;i++){

            WheelHit wheelHit;

            wheels[i].wheelCollider.GetGroundHit(out wheelHit);

            // Adjust the drift factor based on the amount of sideways slip and input direction
            if (wheelHit.sidewaysSlip < 0)
            {
                _driftFactorValue = (1 + -_inputManager.horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);
            }

            if (wheelHit.sidewaysSlip > 0)
            {
                _driftFactorValue = (1 + _inputManager.horizontal) * Mathf.Abs(wheelHit.sidewaysSlip);
            }
		}
    }

    private void WheelEffects()
    {
        foreach(var wheel in wheels)
        {
            if(_inputManager.brakePedal && !wheel.isForwardWheel && wheel.wheelCollider.isGrounded == true && _carRb.velocity.magnitude >= 10.0f)
            {
                wheel.wheelEffectsObj.GetComponentInChildren<TrailRenderer>().emitting = true;

                wheel.driftSmoke.Emit(1);
            }
            else
            {
                wheel.wheelEffectsObj.GetComponentInChildren<TrailRenderer>().emitting = false;
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
        _carRb.centerOfMass = _centerOfMass.localPosition;

        _carLightsController = GetComponent<CarLightsController>();
    }
}

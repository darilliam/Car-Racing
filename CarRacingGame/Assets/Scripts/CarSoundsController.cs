using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSoundsController : MonoBehaviour
{

    public float minSpeed;
    public float maxSpeed;
    public float minPitch;
    public float maxPitch;


    private Rigidbody _carRb;
    private AudioSource _carAudioSource;
    private float _currentSpeed;
    private float _pitchFromCar;

    private void Start()
    {
        _carAudioSource = GetComponent<AudioSource>();
        _carRb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CarEngineSound();
    }

    private void CarEngineSound()
    {
        _currentSpeed = _carRb.velocity.magnitude;
        _pitchFromCar = _carRb.velocity.magnitude / 60.0f;

        if(_currentSpeed < minSpeed)
        {
            _carAudioSource.pitch = minPitch;
        }

        if(_currentSpeed > minSpeed && _currentSpeed < maxSpeed)
        {
            _carAudioSource.pitch = maxPitch + _pitchFromCar;
        }

        if(_currentSpeed > maxSpeed)
        {
            _carAudioSource.pitch = maxPitch;
        }
    }
}

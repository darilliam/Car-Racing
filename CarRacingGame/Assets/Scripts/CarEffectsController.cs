using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffectsController : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] driftSmokes;
    private CarController _carController;
    private bool _smokeFlag = false;

    private void Start()
    {
        _carController = GetComponent<CarController>();
    }

    private void FixedUpdate()
    {
        if(_carController.playPauseDriftSmoke) StartDriftSmokes();
        else StopDriftSmokes();

        if(_smokeFlag)
        {
            for(int i = 0; i < driftSmokes.Length; i++)
            {
                var emission = driftSmokes[i].emission;
                emission.rateOverTime = ((int)_carController.KPH * 10 <= 2000) ? (int)_carController.KPH * 10 : 2000;
            }
        }

    }

    public void StartDriftSmokes()
    {
        if (_smokeFlag) return;

        for(int i = 0; i < driftSmokes.Length; i++)
        {
            var emission = driftSmokes[i].emission;
            emission.rateOverTime = ((int)_carController.KPH * 2 >= 2000) ? (int)_carController.KPH * 2 : 2000;
            driftSmokes[i].Play();
        }

        _smokeFlag = true;
    }

    public void StopDriftSmokes()
    {
        if(!_smokeFlag) return;

        for (int i = 0; i < driftSmokes.Length; i++)
        {
            driftSmokes[i].Stop();
        }

        _smokeFlag = false;
    }
}

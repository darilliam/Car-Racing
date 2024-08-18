using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float vertical;
    public float horizontal;
    // public bool handbrake;
    public bool brakePedal;

    private void FixedUpdate()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        // handbrake = (Input.GetAxis("Jump") != 0)? true : false;
        // brakePedal = (Input.GetKey(KeyCode.Space)) ? true : false;

        brakePedal = (Input.GetKey(KeyCode.J)) ? true : false;
    }
}

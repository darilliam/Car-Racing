using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float smoothnessMove;
    public float smoothnessRotation;

    public Vector3 moveOffset;
    public Vector3 rotateOffset;

    public Transform playerTarget;

    private GameObject player;
    private CarController carController;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        carController = player.GetComponent<CarController>();
    }

    private void FixedUpdate()
    {
        FollowPlayer();

        // Modify the camera speed according to the car kph 
        smoothnessMove = (carController.KPH >= 50) ? 20 : carController.KPH / 4;
    }

    private void FollowPlayer()
    {
        MovementHandler();
        RotationHandler();
    }

    private void MovementHandler()
    {
        Vector3 playerPos = new Vector3();
        playerPos = playerTarget.TransformPoint(moveOffset);

        transform.position = Vector3.Lerp(transform.position, playerPos, smoothnessMove * Time.deltaTime);
    }

    private void RotationHandler()
    {
        var direction = playerTarget.position - transform.position;
        var rotation = new Quaternion();

        rotation = Quaternion.LookRotation(direction + rotateOffset, Vector3.up);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothnessRotation * Time.deltaTime);
    }
}

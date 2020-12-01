﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Transform centerOfMass;

    public WheelCollider wheelColliderLeftFront;
    public WheelCollider wheelColliderRightFront;
    public WheelCollider wheelColliderLeftRear;
    public WheelCollider wheelColliderRightRear;

    public Transform wheelLeftFront;
    public Transform wheelRightFront;
    public Transform wheelLeftRear;
    public Transform wheelRightRear;

    public float motorTorque = 300f;
    public float brakeTorque = 450f;
    public float decelerationForce = 50f;
    public float maxSteerAngle = 20f;
    public float downForceCoefficient = 2.5f;
    public Rigidbody body;
    public Vector3 respawnPosition;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.centerOfMass = centerOfMass.localPosition;
    }

    void FixedUpdate()
    {
        Accleration(Input.GetAxis("Vertical"));
        Steering(Input.GetAxis("Horizontal"));
        Break();
        AddDownForce();
    }

    void Update()
    {
        var position = Vector3.zero;
        var rotation = Quaternion.identity;

        wheelColliderLeftFront.GetWorldPose(out position, out rotation);
        wheelLeftFront.position = position;
        wheelLeftFront.rotation = rotation * Quaternion.Euler(0, 180, 0);

        wheelColliderRightFront.GetWorldPose(out position, out rotation);
        wheelRightFront.position = position;
        wheelRightFront.rotation = rotation;

        wheelColliderLeftRear.GetWorldPose(out position, out rotation);
        wheelLeftRear.position = position;
        wheelLeftRear.rotation = rotation * Quaternion.Euler(0, 180, 0);

        wheelColliderRightRear.GetWorldPose(out position, out rotation);
        wheelRightRear.position = position;
        wheelRightRear.rotation = rotation;
    }

    // Car action
    public void Accleration(float verticalInput)
    {
        if (verticalInput != 0f)
        {

            if (body.velocity.magnitude < 15)
            {
                body.AddForce(body.transform.forward * 10000.0f * verticalInput);
            }

            wheelColliderLeftRear.motorTorque = verticalInput * motorTorque;
            wheelColliderRightRear.motorTorque = verticalInput * motorTorque;
            wheelColliderLeftRear.brakeTorque = 0;
            wheelColliderRightRear.brakeTorque = 0;
            wheelColliderLeftFront.brakeTorque = 0;
            wheelColliderRightFront.brakeTorque = 0;
        }
        else
        {
            wheelColliderLeftRear.brakeTorque = decelerationForce;
            wheelColliderRightRear.brakeTorque = decelerationForce;
            wheelColliderLeftFront.brakeTorque = decelerationForce;
            wheelColliderRightFront.brakeTorque = decelerationForce;
        }
        // Debug.Log(wheelColliderRightRear.brakeTorque);

    }

    public void Break()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            wheelColliderLeftRear.brakeTorque = brakeTorque;
            wheelColliderRightRear.brakeTorque = brakeTorque;
            wheelColliderLeftFront.brakeTorque = brakeTorque;
            wheelColliderRightFront.brakeTorque = brakeTorque;
        }
        // Debug.Log(wheelColliderRightRear.brakeTorque);
    }

    public void Steering(float horizontalInput)
    {
        wheelColliderLeftFront.steerAngle = horizontalInput * maxSteerAngle;
        wheelColliderRightFront.steerAngle = horizontalInput * maxSteerAngle;
    }

    private void AddDownForce()
    {
        float force = downForceCoefficient * body.velocity.sqrMagnitude;
        body.AddForce(-force * transform.up);
    }

    public void Respawn()
    {
        body.MovePosition(respawnPosition);
        transform.position = respawnPosition;
    }

    public float LocalSpeed()
    {
        float dot = Vector3.Dot(transform.forward, body.velocity);
        float speed = body.velocity.magnitude;
        return dot < 0 ? -speed : speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ItemPositive")
        {
            //print("Hello, World!");
            body.AddForce(body.transform.forward * 500000.0f);
        }

        if (other.tag == "ItemNegative")
        {
            body.AddForce(-body.transform.forward * 500000.0f);
        }
    }
}

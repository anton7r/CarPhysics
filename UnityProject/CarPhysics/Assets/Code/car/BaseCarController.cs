using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCarController : MonoBehaviourX
{

    public WheelCollider wheelCFL, wheelCFR, wheelCRL, wheelCRR;
    public List<Wheel> wheels = new List<Wheel>();
    private float maxSteerDeg = 30;
    public float downForce = 50;
    private float torque = 0;
    [Header("Car stats")]
    public float brakePower = 10000;
    public CarType carType;
    public GameObject cameraTarget;
    public AnimationCurve enginePowerCurve;
    public float MaxRPM = 30000;
    private Rigidbody rb;

    private GameObject centerOfMass;

    private float engineRPM;
    private float smoothTime = 0.01f;
    [Header("Gears")]
    public float[] gears;
    public int gearNum = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cameraTarget == null)
        {
            Debug.LogWarning("Camera target not refrenced in '" + gameObject.name + "' BaseCarController script, so we are using the cars gameobject as the camera target.");
            cameraTarget = gameObject;
        }

        /*
        centerOfMass = GameObject.Find("CenterOfMass");
        if (centerOfMass == null)
        {
            Debug.LogWarning("No child gameObject was found with the name 'CenterOfMass', so we are using the cars gameobject as the center of mass which may be unrealistic.");
            centerOfMass = gameObject;
        }
        rb.centerOfMass = centerOfMass.transform.localPosition;
        */

        updatePosition();
        initWheels();
    }

    void FixedUpdate()
    {

        //TODO: ADD maxSteerDeg reduction when going faster (traction control) to prevent wheel spinouts aka drifting and sliding
        //TODO: ADD acceleration and torque forces related to it.

        float vertInput = -Input.GetAxis("Vertical");
        float horzInput = Input.GetAxis("Horizontal");

        //float driftValue = Vector3.Dot(rb.velocity, transform.forward);
        //float driftAngle = Mathf.Acos(driftValue) * Mathf.Rad2Deg;
        //Debug.Log("Drifting: " + (!float.IsNaN(driftAngle)));

        applyDownForce();
        steering(horzInput);
        accelerate(vertInput);
        updateWheels();

        wheelCRL.brakeTorque = wheelCRR.brakeTorque = (Input.GetKey(KeyCode.Space)) ? brakePower : 0;
        enginePower(vertInput);
        updatePosition();
    }

    void accelerate(float vertInput)
    {
        float accelForce = torque;

        switch (carType)
        {
            case CarType.AWD:
                {
                    float perWheel = accelForce / 4;

                    wheelCFL.motorTorque = perWheel;
                    wheelCFR.motorTorque = perWheel;

                    wheelCRL.motorTorque = perWheel;
                    wheelCRR.motorTorque = perWheel;
                    break;
                }
            case CarType.FWD:
                {
                    float perWheel = accelForce / 2;

                    wheelCFL.motorTorque = perWheel;
                    wheelCFR.motorTorque = perWheel;
                    break;
                }
            case CarType.RWD:
                {
                    float perWheel = accelForce / 2;

                    wheelCRL.motorTorque = perWheel;
                    wheelCRR.motorTorque = perWheel;
                    break;
                }
        }
    }

    void enginePower(float vertInput)
    {
        float WheelRPM = getWheelRPM();
        torque = enginePowerCurve.Evaluate(engineRPM) * (gears[gearNum]) * vertInput;
        float velocity = 0.0f;
        engineRPM = Mathf.Min(Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(WheelRPM) * 3.6f * (gears[gearNum])), ref velocity, smoothTime), MaxRPM);

        GameManager.UpdateRPM(engineRPM);

    }

    float getWheelRPM()
    {
        float total = 0;
        int count = 0;

        foreach (Wheel wheel in wheels)
        {
            total += wheel.collider.rpm;
            count++;
        }

        return (count != 0) ? (total / count) : 0;
    }

    private void addWheel(WheelCollider wheelCollider)
    {
        wheels.Add(new Wheel(this.gameObject, wheelCollider));
    }

    private void initWheels()
    {
        addWheel(wheelCFL);
        addWheel(wheelCFR);
        addWheel(wheelCRL);
        addWheel(wheelCRR);

        foreach (Wheel wheel in wheels)
        {
            wheel.collider.ConfigureVehicleSubsteps(1, 12, 15);
        }

        updateWheels();
    }


    void updateWheels()
    {
        foreach (Wheel wheel in wheels)
        {
            wheel.Update();
        }
    }

    void steering(float horzInput)
    {
        float steer = maxSteerDeg * horzInput;
        wheelCFL.steerAngle = steer;
        wheelCFR.steerAngle = steer;
    }

    void applyDownForce()
    {
        rb.AddForce(-transform.up * downForce * rb.velocity.magnitude);
    }

    private void updatePosition()
    {
        GameManager.UpdateSpeed(rb.velocity.magnitude * 3.6F);
    }

    public GameObject getCameraTargetPosition()
    {
        return cameraTarget;
    }

    public GameObject getCameraLookTarget()
    {
        return gameObject;
    }


}

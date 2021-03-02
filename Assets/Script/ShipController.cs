using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    //For scene exporter Tool
    public struct ShipControlsStats {
        public float rotorMaxAngle;
        public float forwardForce;
        public float slowingVelFactor;
        public float brakingVelFactor;
        public float angleOfRoll;
        public float angleOfPitch;
        public float rotationMultiplicator;
        public float propultionMultiplicator;
        public float hoverHeight;
        public float maxGroundDist;
        public float hoverForce;
        public float bounceForce;
        public float terminalVelocity;
        public float hoverGravity;
        public float drag;
        public float hoverPID_P;
        public float hoverPID_I;
        public float hoverPID_D;
    }
    public ShipControlsStats GetShipControlsStats() {
        ShipControlsStats stats = new ShipControlsStats();
        stats.rotorMaxAngle = rotorMaxAngle;
        stats.forwardForce = forwardForce;
        stats.slowingVelFactor = slowingVelFactor;
        stats.brakingVelFactor = brakingVelFactor;
        stats.angleOfRoll = angleOfRoll;
        stats.angleOfPitch = angleOfPitch;
        stats.rotationMultiplicator = rotationMultiplicator;
        stats.propultionMultiplicator = propultionMultiplicator;
        stats.hoverHeight = hoverHeight;
        stats.maxGroundDist = maxGroundDist;
        stats.hoverForce = hoverForce;
        stats.bounceForce = bounceForce;
        stats.terminalVelocity = terminalVelocity;
        stats.hoverGravity = hoverGravity;
        stats.drag = drag;
        stats.hoverPID_P = hoverPID.pCoeff;
        stats.hoverPID_I = hoverPID.iCoeff;
        stats.hoverPID_D = hoverPID.dCoeff;
        
        return stats;
    }

    [Header("Drive Settings")]
    [SerializeField] private float forwardForce = 150f;
    [SerializeField] private float slowingVelFactor = .99f;
    [SerializeField] private float brakingVelFactor = .95f;
    [SerializeField] private float angleOfRoll = 10f;
    [SerializeField] private float angleOfPitch = 15f;
    [SerializeField] private float rotationMultiplicator = 2f;
    [SerializeField] private float propultionMultiplicator = 1.2f;
    [SerializeField] private float rotorMaxAngle = 30f;

    [Header("Hover Settings")]
    [SerializeField] private float hoverHeight = 8f;
    [SerializeField] private float maxGroundDist = 10f;
    [SerializeField] private float hoverForce = 300f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private PIDController hoverPID;

    [Header("Physics Settings")]
    [SerializeField] private float bounceForce = 100f;
    [SerializeField] private Transform shipBody;
    [SerializeField] private float terminalVelocity = 100f;
    [SerializeField] private float hoverGravity = 20f;
    [SerializeField] private float fallGravity = 80f;
    [SerializeField] private float drag;
    [SerializeField] private bool isOnGround;
    
    [SerializeField] private ShipComponents shipComponents;

    private float startHoverHeight;

    private void Start()
    {
        startHoverHeight = hoverHeight;

        drag = forwardForce / terminalVelocity;
    }

    private void Update()
    {
        Rotors();
    }

    private void FixedUpdate()
    {
        Debug.Log(Time.fixedDeltaTime);
        hoverHeight = startHoverHeight + Mathf.PingPong(Time.time, 1);
        CalculateHover();
        CalculateThrust();
    }

    private void OnCollisionEnter(Collision other)
    {
        //if(Vector3.Angle(Vector3.up, other.GetContact(0).normal) > 80f)
        //    shipComponents.shipRigidbody.AddForce(other.GetContact(0).normal * bounceForce, ForceMode.Impulse);
    }

    void CalculateHover()
    {
        Vector3 groundNormal;

        Ray ray = new Ray(transform.position, -transform.up);

        RaycastHit hitInfo;

        isOnGround = Physics.Raycast(ray, out hitInfo, maxGroundDist, groundLayer);

        if (isOnGround)
        {
            float height = hitInfo.distance;

            groundNormal = hitInfo.normal.normalized;

            float forcePercent = hoverPID.Seek(hoverHeight, height);

            Vector3 force = groundNormal * hoverForce * forcePercent;

            Vector3 gravity = -groundNormal * hoverGravity * height;

            shipComponents.shipRigidbody.AddForce(force, ForceMode.Acceleration);
            shipComponents.shipRigidbody.AddForce(gravity, ForceMode.Acceleration);
        }
        else
        {
           /* foreach (Transform t in stabPoints) {
                float stabValue = stabPID.Seek(t.position.y - transform.position.y, 0);
                shipComponents.shipshipComponents.shipRigidbody.AddForceAtPosition(Vector3.up * stabValue, t.position);
            }*/

            groundNormal = Vector3.up;
            Vector3 gravity = -groundNormal * fallGravity;

            shipComponents.shipRigidbody.AddForce(gravity, ForceMode.Acceleration);

        }

        float rotationSpeed = 0.0f;
        
        if (isOnGround)
        {
            rotationSpeed = 10f;
        }
        else
        {
            rotationSpeed = 1.0f;
        }

        Vector3 projection = Vector3.ProjectOnPlane(transform.forward, groundNormal);
        Quaternion rotation = Quaternion.LookRotation(projection, groundNormal);

        shipComponents.shipRigidbody.MoveRotation(Quaternion.Lerp(shipComponents.shipRigidbody.rotation, rotation, Time.deltaTime * rotationSpeed));


        float angle = angleOfRoll * -shipComponents.shipInput.rudder * shipComponents.shipInput.GetIntensity();
        float pitchAngle = angleOfPitch * shipComponents.shipInput.thruster * shipComponents.shipInput.GetIntensity();
        Quaternion bodyRotation = transform.rotation * Quaternion.Euler(pitchAngle, 0f, angle);

        shipBody.rotation = Quaternion.Lerp(shipBody.rotation, bodyRotation, Time.deltaTime * 10f);
    }

    void CalculateThrust()
    {
        float rotationTorque = (shipComponents.shipInput.rudder * rotationMultiplicator) - shipComponents.shipRigidbody.angularVelocity.y ;

        shipComponents.shipRigidbody.AddRelativeTorque(0f, rotationTorque, 0f, ForceMode.VelocityChange);

        float sidewaySpeed = Vector3.Dot(shipComponents.shipRigidbody.velocity, transform.right);

        Vector3 sideFriction = -transform.right * (sidewaySpeed / Time.fixedDeltaTime);

        shipComponents.shipRigidbody.AddForce(sideFriction, ForceMode.Acceleration);

        if (shipComponents.shipInput.thruster <= 0f)
        {
            shipComponents.shipRigidbody.velocity *= slowingVelFactor;
        }

        if (!isOnGround)
        {
            return;
        }

        if (shipComponents.shipInput.isBreaking)
        {
            shipComponents.shipRigidbody.velocity *= brakingVelFactor;
        }

        float propulsion = (forwardForce * shipComponents.shipInput.thruster * propultionMultiplicator) - (drag * Mathf.Clamp(shipComponents.shipStats.GetForwardSpeed(), 0f, terminalVelocity)* propultionMultiplicator);
        shipComponents.shipRigidbody.AddForce(transform.forward * propulsion, ForceMode.Acceleration);
    }

    /// <summary>
    /// Incline rotor visual
    /// </summary>
    void Rotors() {
        Vector3 rightRotation = Vector3.zero;

        rightRotation.x = shipComponents.shipInput.GetJoystickAxis(ShipInput.Joystick.Right, ShipInput.Axis.Vertical) * rotorMaxAngle;
        rightRotation.z = -shipComponents.shipInput.GetJoystickAxis(ShipInput.Joystick.Right, ShipInput.Axis.Horizontal) * rotorMaxAngle;

        shipComponents.rightRotor.localRotation = Quaternion.Euler(rightRotation);

        Vector3 leftRotation = Vector3.zero;

        leftRotation.x = shipComponents.shipInput.GetJoystickAxis(ShipInput.Joystick.Left, ShipInput.Axis.Vertical) * rotorMaxAngle;
        leftRotation.z = -shipComponents.shipInput.GetJoystickAxis(ShipInput.Joystick.Left, ShipInput.Axis.Horizontal) * rotorMaxAngle;

        shipComponents.leftRotor.localRotation = Quaternion.Euler(leftRotation);
    }


    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            Vector3 upwardForceFromCollision = Vector3.Dot(other.impulse, transform.up) * transform.up;
            shipComponents.shipRigidbody.AddForce(-upwardForceFromCollision, ForceMode.Impulse);
        }
    }

    public float GetSpeedPercentage()
    {
        return shipComponents.shipRigidbody.velocity.magnitude / terminalVelocity;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCameraLucaSmoothDamp : MonoBehaviour
{
    [SerializeField] Vector3 targetPosition;
    [SerializeField] Transform ship;
    [SerializeField] Rigidbody shipBody;
    [SerializeField] Vector3 cameraPosition;

    [SerializeField] float angularLateralMult;
    [SerializeField] float angularBackwardMult;
    [SerializeField] float angularForwardTargetMult;

    [SerializeField] float linearUpwardMult;
    [SerializeField] float linerarBackwardMult;
    [SerializeField] float linerarBackwardDiv;
    [SerializeField] float fallMultiplicator = 0.1f;
    [SerializeField] float fallTargetMultiplicator = -0.1f;


    [SerializeField] float angularLerp;
    [SerializeField] float angularTargetLerp;

    [SerializeField] float linearLerp;
    [SerializeField] float lerpPosition;

    [Header("Visual Debug")]
    private float angularLateralMovement;
    private float angularBackwardMovement;
    private float angularForwardTarget;
    private float linearUpwardMovement;
    private float linerarBackwardMovement;
    private Vector3 angularAddVector;
    private Vector3 angularAddTargetVector;
    private Vector3 linearAddVector;
    private Vector3 newPosition;
    private Vector3 positionVelocity;
    private float fallAddition;
    private float fallTargetAddition;
    private float dotProductPosVelo;
    private float dotProductPosVeloMult;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var angularVelocity = shipBody.angularVelocity;
        angularLateralMovement = angularLateralMult * angularVelocity.y;
        angularBackwardMovement = angularBackwardMult * angularVelocity.y;
        var velocity = shipBody.velocity;
        linearUpwardMovement = linearUpwardMult * velocity.magnitude;
        linerarBackwardMovement = linerarBackwardMult * velocity.magnitude;
        dotProductPosVelo = Vector3.Dot(transform.position - ship.position, velocity);
        if (dotProductPosVelo > 100)
        {
            dotProductPosVeloMult = dotProductPosVelo * linerarBackwardDiv;
            linerarBackwardMovement = linerarBackwardMovement * dotProductPosVeloMult;
        }
        newPosition = ship.position;
        angularAddVector = Vector3.Lerp(angularAddVector, Vector3.right * angularLateralMovement + Vector3.back * Mathf.Abs(angularBackwardMovement), angularLerp);
        linearAddVector = Vector3.Lerp(linearAddVector, Vector3.up * linearUpwardMovement + Vector3.back * linerarBackwardMovement, linearLerp);
        newPosition += ship.rotation * (cameraPosition + angularAddVector + linearAddVector);
        fallAddition = (fallMultiplicator * -Mathf.Abs(shipBody.velocity.y));
        newPosition += Vector3.up * fallAddition;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref positionVelocity, lerpPosition);

        angularForwardTarget = angularForwardTargetMult * angularVelocity.y;
        fallTargetAddition = (fallTargetMultiplicator * -Mathf.Abs(shipBody.velocity.y));
        angularAddTargetVector = Vector3.Lerp(angularAddTargetVector, Vector3.forward * Mathf.Abs(angularForwardTarget) + Vector3.back * Mathf.Abs(fallTargetAddition), angularTargetLerp);
        transform.LookAt(ship.position + ship.rotation * (targetPosition + angularAddTargetVector));
    }

}

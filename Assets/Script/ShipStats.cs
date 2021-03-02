using UnityEngine;

public class ShipStats : MonoBehaviour
{
    [SerializeField] private ShipComponents shipComponents;
    [SerializeField] private float forwardSpeed;
    [SerializeField] private float lateralSpeed;
    [SerializeField] private RaycastHit groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private float groundedDistance = 20f;

    public float GetForwardSpeed() {
        return forwardSpeed;
    }

    public float GetLateralSpeed() {
        return lateralSpeed;
    }

    public bool Grounded() {
        return groundCheck.distance < groundedDistance;
    }

    public RaycastHit GroundCheck() {
        return groundCheck;
    }

    private void FixedUpdate() {
        forwardSpeed = Vector3.Dot(shipComponents.shipRigidbody.velocity, transform.forward);
        lateralSpeed = Vector3.Dot(shipComponents.shipTransform.right, shipComponents.shipRigidbody.velocity);
        Physics.Raycast(shipComponents.shipModelTransform.position, -Vector3.up, out groundCheck, Mathf.Infinity, groundLayer);
    }
}

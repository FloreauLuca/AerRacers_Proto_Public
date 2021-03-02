using UnityEngine;

public class ShipComponents : MonoBehaviour
{
    public Transform shipTransform;
    public Transform shipModelTransform;
    public Rigidbody shipRigidbody;
    public ShipInput shipInput;
    public ShipStats shipStats;
    public ShipController shipController;
    //public ShipModelRotation shipModelRotation;
    public Transform rightRotor;
    public Transform leftRotor;
}

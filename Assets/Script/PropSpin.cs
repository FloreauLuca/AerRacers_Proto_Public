using UnityEngine;

public class PropSpin : MonoBehaviour
{
    [SerializeField] private float propSpeed;

    void FixedUpdate()
    {
        transform.Rotate(new Vector3(0,0,propSpeed));
    }
}

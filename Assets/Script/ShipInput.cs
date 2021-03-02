using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShipInput : MonoBehaviour {

    [SerializeField] private string quitButtonName = "Cancel";

    [HideInInspector] private float vehiclePowerForceInput;
    [HideInInspector] private float vehicleRotationForceInput;

    [SerializeField] private bool rightBumper = false;

    [HideInInspector] private Vector2 leftJoystick;
    [HideInInspector] private Vector2 rightJoystick;

    [SerializeField] private Vector2 joystickDeadzone;

    [Header("Joysticks")]
    [SerializeField] private Direction rightJoystickDirection;
    [SerializeField] private Direction leftJoystickDirection;
    [SerializeField] private Gesture currentGesture;

    public float rudder;
    public float thruster;
    public bool isBreaking;

    public enum Joystick {
        Right,
        Left
    }

    public enum Axis {
        Horizontal,
        Vertical
    }

    public enum Gesture {
        Default,
        Forward,
        Backward,
        TurnLeftForward,
        TurnLeftBackward,
        TurnRightForward,
        TurnRightBackward,
        RotateLeft,
        RotateRight,
    }

    public enum Direction {
        Center,
        Forward,
        ForwardLeft,
        ForwardRight,
        Backward,
        BackwardLeft,
        BackwardRight,
        Left,
        Right,
    }

    /// <summary>
    /// Return True if joystick IS NOT inside the deadzone
    /// </summary>
    public bool isJoystickActive(Joystick joystick) {
        switch (joystick) {
            case Joystick.Right:
                return (rightJoystick.y > joystickDeadzone.y || rightJoystick.y < -joystickDeadzone.y);
            case Joystick.Left:
                return (leftJoystick.y > joystickDeadzone.y || leftJoystick.y < -joystickDeadzone.y);
            default:
                return false;
        }
    }

    /// <summary>
    /// Return True if joystick IS in designated deadzone
    /// </summary>
    public bool isJoystickAxisInDeadzone(Joystick joystick, Axis axis) {
        switch (axis) {
            case Axis.Horizontal:
                return (GetJoystick(joystick).x < joystickDeadzone.x && GetJoystick(joystick).x > -joystickDeadzone.x);
            case Axis.Vertical:
                return (GetJoystick(joystick).y < joystickDeadzone.y && GetJoystick(joystick).y > -joystickDeadzone.y);
            default:
                return true;
        }
    }

    /// <summary>
    /// Return Vector2 of joystick position
    /// </summary>
    public Vector2 GetJoystick(Joystick joystick) {
        switch (joystick) {
            case Joystick.Right:
                return rightJoystick;
            case Joystick.Left:
                return leftJoystick;
            default:
                return Vector2.zero;
        }
    }

    /// <summary>
    /// Return float of joystick on desired axis
    /// </summary>
    public float GetJoystickAxis(Joystick joystick, Axis axis) {
        switch (axis) {
            case Axis.Vertical:
                return GetJoystick(joystick).y;
            case Axis.Horizontal:
                return GetJoystick(joystick).x;
            default:
                return 0;
        }
    }

    /// <summary>
    /// Return the Direction of desired joystick
    /// </summary>
    public Direction GetJoystickDirection(Joystick joystick) {
        //Forward
        if(!isJoystickAxisInDeadzone(joystick, Axis.Vertical) && GetJoystick(joystick).y > joystickDeadzone.y && isJoystickAxisInDeadzone(joystick, Axis.Horizontal)) {
            return Direction.Forward;
        }
        //Forward Left
        else if (!isJoystickAxisInDeadzone(joystick, Axis.Vertical) && GetJoystick(joystick).y > joystickDeadzone.y && GetJoystick(joystick).x < joystickDeadzone.x) {
            return Direction.ForwardLeft;
        }
        //Forward Right
        else if (!isJoystickAxisInDeadzone(joystick, Axis.Vertical) && GetJoystick(joystick).y > joystickDeadzone.y && GetJoystick(joystick).x > joystickDeadzone.x) {
            return Direction.ForwardRight;
        }
        //Backward
        else if (!isJoystickAxisInDeadzone(joystick, Axis.Vertical) && GetJoystick(joystick).y < joystickDeadzone.y && isJoystickAxisInDeadzone(joystick, Axis.Horizontal)) {
            return Direction.Backward;
        }
        //Forward Left
        else if (!isJoystickAxisInDeadzone(joystick, Axis.Vertical) && GetJoystick(joystick).y < joystickDeadzone.y && GetJoystick(joystick).x < joystickDeadzone.x) {
            return Direction.BackwardLeft;
        }
        //Forward Right
        else if (!isJoystickAxisInDeadzone(joystick, Axis.Vertical) && GetJoystick(joystick).y < joystickDeadzone.y && GetJoystick(joystick).x > joystickDeadzone.x) {
            return Direction.BackwardRight;
        }
        //Left
        else if (!isJoystickAxisInDeadzone(joystick, Axis.Horizontal) && GetJoystick(joystick).x < joystickDeadzone.x && isJoystickAxisInDeadzone(joystick, Axis.Vertical)) {
            return Direction.Left;
        }
        //Right
        else if (!isJoystickAxisInDeadzone(joystick, Axis.Horizontal) && GetJoystick(joystick).x > joystickDeadzone.x && isJoystickAxisInDeadzone(joystick, Axis.Vertical)) { 
            return Direction.Right;
        }
        else {
            return Direction.Center;
        }
    }

    /// <summary>
    /// Return the current Gesture performed by the two joysticks
    /// </summary>
    public Gesture GetCurrentGesture()
    {
        Direction leftJoystickDir = GetJoystickDirection(Joystick.Left);
        Direction rightJoystickDir = GetJoystickDirection(Joystick.Right);

        //Default
        if (leftJoystickDir == Direction.Center && rightJoystickDir == Direction.Center) {
            return Gesture.Default;
        }
        //Forward
        else if (leftJoystickDir == Direction.Forward && rightJoystickDir == Direction.Forward) {
            return Gesture.Forward;
        }
        //Backward
        else if (leftJoystickDir == Direction.Backward && rightJoystickDir == Direction.Backward) {
            return Gesture.Backward;
        }
        //Turn Left Forward
        else if(((leftJoystickDir == Direction.Left || leftJoystickDir == Direction.ForwardLeft) && (rightJoystickDir == Direction.Forward || rightJoystickDir == Direction.ForwardLeft || rightJoystickDir == Direction.Left)) 
                ||
                (leftJoystickDir == Direction.Forward) && (rightJoystickDir == Direction.Left || rightJoystickDir == Direction.ForwardLeft) 
                ||
                (leftJoystickDir == Direction.Center) && (rightJoystickDir == Direction.ForwardLeft || rightJoystickDir == Direction.Forward || rightJoystickDir == Direction.ForwardRight)) {
            return Gesture.TurnLeftForward;
        }
        //Turn Left Backward
        else if (((leftJoystickDir == Direction.Left || leftJoystickDir == Direction.BackwardLeft) && (rightJoystickDir == Direction.Backward || rightJoystickDir == Direction.BackwardLeft))) {
            return Gesture.TurnLeftBackward;
        }
        //Turn Right Forward
        else if (((leftJoystickDir == Direction.Forward || leftJoystickDir == Direction.ForwardRight || leftJoystickDir == Direction.Right) && (rightJoystickDir == Direction.Right || rightJoystickDir == Direction.ForwardRight))
                 ||
                 ((leftJoystickDir == Direction.Right || leftJoystickDir == Direction.ForwardRight) && (rightJoystickDir == Direction.Forward))
                 ||
                 (leftJoystickDir == Direction.ForwardLeft || leftJoystickDir == Direction.ForwardRight || leftJoystickDir == Direction.Forward) && (rightJoystickDir == Direction.Center)) {
            return Gesture.TurnRightForward;
        }
        //Turn Right Backward
        else if (((leftJoystickDir == Direction.Right || leftJoystickDir == Direction.BackwardRight) && (rightJoystickDir == Direction.Backward || rightJoystickDir == Direction.BackwardRight))) {
            return Gesture.TurnRightBackward;
        }
        //Rotate Left
        else if ((leftJoystickDir == Direction.Backward || leftJoystickDir == Direction.BackwardLeft || leftJoystickDir == Direction.BackwardRight) && 
                 (rightJoystickDir == Direction.Forward || rightJoystickDir == Direction.ForwardLeft || rightJoystickDir == Direction.ForwardRight)) {
            return Gesture.RotateLeft;
        }
        //Rotate Right
        else if ((leftJoystickDir == Direction.Forward || leftJoystickDir == Direction.ForwardLeft || leftJoystickDir == Direction.ForwardRight) && 
                 (rightJoystickDir == Direction.Backward || rightJoystickDir == Direction.BackwardLeft || rightJoystickDir == Direction.BackwardRight)) {
            return Gesture.RotateRight;
        }
        //Default
        else {
            return Gesture.Default;
        }
    }

    public float GetJoystickAngle(Joystick joystick)
    {
        return Vector2.Angle(Vector2.up, GetJoystick(joystick)); ;
    }

    /// <summary>
    /// Return the joystick magnitude
    /// </summary>
    public float GetJoystickMagnitude(Joystick joystick)
    {
        return GetJoystick(joystick).magnitude;
    }

    public float GetIntensity() {
        float rightIntensity = Mathf.Abs(GetJoystickMagnitude(ShipInput.Joystick.Right));
        float leftIntensity = Mathf.Abs(GetJoystickMagnitude(ShipInput.Joystick.Left));

        return ((rightIntensity + leftIntensity) * .5f);
    }

    public bool GetBoostButton()
    {
        return rightBumper;
    }

    void Update() {
        rightJoystick = new Vector2(Input.GetAxis("RightStick_H"), Input.GetAxis("RightStick_V"));
        leftJoystick = new Vector2(Input.GetAxis("LeftStick_H"), Input.GetAxis("LeftStick_V"));

        rightBumper = Input.GetKeyDown(KeyCode.Joystick1Button0);

        rightJoystickDirection = GetJoystickDirection(Joystick.Right);
        leftJoystickDirection = GetJoystickDirection(Joystick.Left);
        currentGesture = GetCurrentGesture();

        if (Input.GetButtonDown(quitButtonName)) {
            Application.Quit();
        }

        float intensity = GetIntensity();
        float turnIntensity = (GetJoystickAngle(Joystick.Left) / 180) + (GetJoystickAngle(Joystick.Right) / 180);
        isBreaking = false;

        switch (currentGesture)
        {
            case Gesture.Default:
                thruster = 0;
                rudder = 0;
                break;

            case Gesture.Forward:
                thruster = intensity;
                rudder = 0;
                break;

            case Gesture.TurnLeftForward:
                thruster = intensity;
                rudder = -intensity*(turnIntensity);
                break;

            case Gesture.TurnLeftBackward:
                thruster = -intensity * 0.25f;
                rudder = -intensity * (turnIntensity * 0.5f);
                break;

            case Gesture.TurnRightForward:
                thruster = intensity;
                rudder = intensity* (turnIntensity);
                break;

            case Gesture.TurnRightBackward:
                thruster = -intensity * .25f;
                rudder = intensity * (turnIntensity * 0.5f);
                break;

            case Gesture.RotateLeft:
                thruster = 0;
                rudder = -intensity;
                break;

            case Gesture.RotateRight:
                thruster = 0;
                rudder = intensity;
                break;

            case Gesture.Backward:
                thruster = -intensity;
                rudder = 0;
                isBreaking = true;
                break;
        }

        //rudder = GetJoystick(Joystick.Left).x;
        //thruster = GetJoystick(Joystick.Left).y;
        //isBreaking = Input.GetKey(KeyCode.A);
    }
}

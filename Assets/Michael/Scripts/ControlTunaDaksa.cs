using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlTunaDaksa : MonoBehaviour
{
    public float moveSpeed = 1f;    // How fast to move
    public float turnSpeed = 10f;  // Degrees per second

    private Vector2 inputVector;

    public void JoystickControl(Vector2 vector)
    {
        // Called by joystick's onJoystickVectorChange event
        inputVector = vector;
    }

    private void Update()
    {
        // Move forward/back
        Vector3 forwardMove = transform.forward * inputVector.y * moveSpeed * Time.deltaTime;
        transform.position += forwardMove;

        // Rotate left/right
        float rotation = inputVector.x * turnSpeed * Time.deltaTime;
        transform.Rotate(0f, rotation, 0f);
    }
}

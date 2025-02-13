using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 20f;          // Movement speed when pressing forward/backward
    public float frictionCoefficient = 5f; // Strength of friction when grounded
    public float maxSpeed = 15f;           // Maximum velocity the character can reach
    public float maxForce = 25f;           // Maximum total acceleration applied
    public float jumpForce = 8f;           // Force applied for jumping
    [SerializeField]
    public CharacterController cc;         // Reference to CharacterController

    [SerializeField]
    Camera cam;


    [SerializeField]
    AnimationCurve jumpForceCurve;

    private Vector3 velocity;              // Current velocity (includes all forces)
    private Vector3 acceleration;          // Accumulated forces (gravity, movement, friction)
    
    void Update()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        // Rotate character based on input
        // transform.Rotate(0, hAxis * 60 * Time.deltaTime, 0);

        // Gravity handling (always apply gravity, even if grounded)
        acceleration = new Vector3(0, -9.81f, 0);

        // Apply movement input (forward/backward movement)
        Vector3 cameraForward = cam.gameObject.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        acceleration += cameraForward * vAxis * moveSpeed;

        Vector3 cameraRight = cam.gameObject.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();
        acceleration += cameraRight * hAxis * moveSpeed;

        // Apply friction as an acceleration (opposite direction of horizontal velocity)
        ApplyFriction();

        if (cc.isGrounded)
        {
            // Handle jump (apply upward force if jumping)
            if (Input.GetKeyDown(KeyCode.Space))
            {
                velocity.y = jumpForce;
            }
        } 
        else 
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                velocity.y = 0;
            }
        }

        // Clamp acceleration to avoid excessive values
        acceleration = Vector3.ClampMagnitude(acceleration, maxForce);

        // Apply the acceleration to velocity
        velocity += acceleration * Time.deltaTime;

        // Apply maximum speed limit to velocity
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Move the character
        cc.Move(velocity * Time.deltaTime);

        // Debug.Log((velocity * Time.deltaTime).magnitude);
        if (cc.velocity.sqrMagnitude > 0.01f) {
            Vector3 direction = new Vector3(velocity.x, 0, velocity.z);
            transform.forward = direction.normalized;
        }

        // Reset acceleration for the next frame (don't carry over from previous frame)
        acceleration = Vector3.zero;
    }

    void ApplyFriction()
    {
        // Calculate horizontal velocity (ignore the vertical component)
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);

        if (horizontalVelocity.magnitude > 0) // Only apply friction if there's horizontal velocity
        {
            // Friction acceleration is the opposite direction of velocity
            Vector3 frictionAcceleration = -horizontalVelocity.normalized * frictionCoefficient;

            // Add the friction force as part of the total acceleration
            acceleration += frictionAcceleration;
        }
    }
}

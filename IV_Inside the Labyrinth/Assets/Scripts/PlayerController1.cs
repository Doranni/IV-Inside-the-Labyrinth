using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float jumpingUpForce;
    [SerializeField] private float jumpingAsideForce;
    [SerializeField] private float gravityMultiplier;

    private Rigidbody playerRB;

    private float rotationInput;
    private float verticalInput;
    private bool isOnGround;
    private bool isDoubleJumpAvailable = false;
    private bool isOnDash;
    private float acceleration = 1f;
    private float maxAcceleration = 2f;

    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        Physics.gravity *= gravityMultiplier;
    }

    void Update()
    {
        // Moving - user input
        rotationInput = Input.GetAxis("Mouse X");
        verticalInput = Input.GetAxisRaw("Vertical");

        Jumping();        
        Dashing();
        
    }

    private void Dashing()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isOnDash = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            isOnDash = false;
        }
        if (isOnDash && isOnGround && acceleration < maxAcceleration)
        {
            acceleration += Time.deltaTime;
        }
        else if (!isOnDash && isOnGround && acceleration > 1f)
        {
            acceleration -= Time.deltaTime;
        }
        else if (isOnDash && acceleration > maxAcceleration)
        {
            acceleration = maxAcceleration;
        }
        else if (!isOnDash && acceleration < 1f)
        {
            acceleration = 1f;
        }
    }

    private void Jumping()
    {
        // Jumping up
        if (isOnGround && Input.GetKeyDown(KeyCode.Space))
        {
            playerRB.AddForce(Vector3.up * jumpingUpForce, ForceMode.Impulse);
            isDoubleJumpAvailable = true;
        }
        // Double jump
        else if (isDoubleJumpAvailable && Input.GetKeyDown(KeyCode.Space))
        {
            playerRB.AddForce(Vector3.up * jumpingUpForce, ForceMode.Impulse);
            isDoubleJumpAvailable = false;
        }
        // Jumping aside
        else if (isOnGround && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)))
        {
            playerRB.AddForce(transform.right * -1 * jumpingAsideForce, ForceMode.Impulse);
        }
        else if (isOnGround && (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            playerRB.AddForce(transform.right * jumpingAsideForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // Rotating
        playerRB.MoveRotation(playerRB.rotation * Quaternion.Euler(Vector3.up * rotationInput * rotationSpeed * Time.fixedDeltaTime));

        // Runing forward
        playerRB.MovePosition(transform.position + transform.forward * verticalInput * movementSpeed * acceleration * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;
    public LayerMask layerMask;

    float timeForMovement = 0;
    float timeForRotation = 0;
    float minRotationTime;
    float maxRotationTime;
    RotationSide side;
    bool needToRotate = false;
    bool checkForward;

    Rigidbody enemyRB;
    Vector3 enemyExtents;

    // Start is called before the first frame update
    void Start()
    {
        enemyRB = GetComponent<Rigidbody>();
        enemyExtents = GetComponent<Collider>().bounds.extents;
        minRotationTime = 30 / rotationSpeed;
        maxRotationTime = 180 / rotationSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        checkForward = Physics.CheckBox(transform.position + Vector3.forward, enemyExtents, Quaternion.identity, layerMask);

        // Rotating 
        if (needToRotate)
        {
            if (timeForRotation <= 0)
            {
                needToRotate = false;
                timeForMovement = Random.Range(1f, 8f);
            }
            else
            {
                
                timeForRotation -= Time.deltaTime;
            }
        }
        // Moving forward
        else
        {
            if (timeForMovement <= 0 || checkForward)
            {
                needToRotate = true;
                side = (RotationSide)Random.Range(0, 2);
                timeForRotation = Random.Range(minRotationTime, maxRotationTime);
            }
            else
            {
                timeForMovement -= Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        if (needToRotate)
        {
            if (side == RotationSide.left)
            {
                enemyRB.MoveRotation(transform.rotation * Quaternion.Euler(Vector3.down * rotationSpeed * Time.fixedDeltaTime));
            }
            else
            {
                enemyRB.MoveRotation(transform.rotation * Quaternion.Euler(Vector3.up * rotationSpeed * Time.fixedDeltaTime));
            }
            
        }
        else
        {
            enemyRB.MovePosition(transform.position + transform.forward * movementSpeed * Time.fixedDeltaTime);
        }
    }

    enum RotationSide
    {
        left,
        right
    }

    // TODO: to fix moving and rotating
    // TODO: reaction to the player
}

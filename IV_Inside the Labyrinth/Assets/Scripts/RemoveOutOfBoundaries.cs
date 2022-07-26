using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveOutOfBoundaries : MonoBehaviour
{
    [SerializeField] private Vector3 minBorder, maxBorder;

    // Start is called before the first frame update
    void Start()
    {
        if (minBorder == Vector3.zero || maxBorder == Vector3.zero)
        {
            // TODO: to refactor this script
            Collider planeCollider = GameObject.Find("Plane").GetComponent<Collider>();
            Collider cubeCollider = GameObject.Find("Cube").GetComponent<Collider>();
            Vector3 planeCenter = planeCollider.bounds.center;
            Vector3 planeExtents = planeCollider.bounds.extents;
            Vector3 cubeCenter = cubeCollider.bounds.center;
            Vector3 cubeExtents = cubeCollider.bounds.extents;
            float offset = 1f;
            minBorder = (minBorder != Vector3.zero) ? minBorder : new Vector3(planeCenter.x - planeExtents.x - offset,
                cubeCenter.y - cubeExtents.y - offset, planeCenter.z - planeExtents.z - offset);
            maxBorder = (maxBorder != Vector3.zero) ? maxBorder : new Vector3(planeCenter.x + planeExtents.x + offset,
                cubeCenter.y + cubeExtents.y + offset, planeCenter.z + planeExtents.z + offset);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > maxBorder.x || transform.position.x < minBorder.x ||
            transform.position.y > maxBorder.y || transform.position.y < minBorder.y ||
            transform.position.z > maxBorder.z || transform.position.z < minBorder.z)
        {
            Destroy(gameObject);
        }
    }
}

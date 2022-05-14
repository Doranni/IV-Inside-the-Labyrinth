using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed;

    public Transform firsrViewPoint, thirdViewMinPoint, thirdViewMaxPoint, mapViewPoint, currentViewPoint;

    (Vector3, Vector3) firstView, thirdViewMin, thirdViewMax, mapView, currentView;

    float posYStep, posZStep, rotXStep;

    ViewMode viewMode = ViewMode.thirdViewMode;

    // Start is called before the first frame update
    void Start()
    {
        if (firsrViewPoint == null)
        {
            firstView.Item1 = new Vector3(0, 1.5f, 0);
            firstView.Item2 = new Vector3(11, 0, 0);
        }
        else
        {
            firstView.Item1 = firsrViewPoint.localPosition;
            firstView.Item2 = firsrViewPoint.localRotation.eulerAngles;
        }

        if (thirdViewMinPoint == null)
        {
            thirdViewMin.Item1 = new Vector3(0, 2.3f, -2);
            thirdViewMin.Item2 = new Vector3(15, 0, 0);
        }
        else
        {
            thirdViewMin.Item1 = thirdViewMinPoint.localPosition;
            thirdViewMin.Item2 = thirdViewMinPoint.localRotation.eulerAngles;
        }

        if (thirdViewMaxPoint == null)
        {
            thirdViewMax.Item1 = new Vector3(0, 8, -7);
            thirdViewMax.Item2 = new Vector3(27, 0, 0);
        }
        else
        {
            thirdViewMax.Item1 = thirdViewMaxPoint.localPosition;
            thirdViewMax.Item2 = thirdViewMaxPoint.localRotation.eulerAngles;
        }

        if (mapViewPoint == null)
        {
            mapView.Item1 = new Vector3(0, 120, 0);
            mapView.Item2 = new Vector3(90, 0, 0);
        }
        else
        {
            mapView.Item1 = mapViewPoint.localPosition;
            mapView.Item2 = mapViewPoint.localRotation.eulerAngles;
        }

        if (currentViewPoint == null)
        {
            currentView = thirdViewMin;
        }
        else
        {
            currentView.Item1 = currentViewPoint.localPosition;
            currentView.Item2 = currentViewPoint.localRotation.eulerAngles;
        }

        transform.localPosition = currentView.Item1;
        transform.localRotation = Quaternion.Euler(currentView.Item2);

        posYStep = Mathf.Abs(thirdViewMax.Item1.y - thirdViewMin.Item1.y);
        posZStep = Mathf.Abs(thirdViewMax.Item1.z - thirdViewMin.Item1.z);
        rotXStep = Mathf.Abs(thirdViewMax.Item2.x - thirdViewMin.Item2.x);

        if (zoomSpeed == 0)
        {
            zoomSpeed = 5;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: to create smooth view toggle

        // Toggle First person view
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (viewMode == ViewMode.firstViewMode)
            {
                transform.localPosition = currentView.Item1;
                transform.localRotation = Quaternion.Euler(currentView.Item2);
                viewMode = ViewMode.thirdViewMode;
            }
            else
            {
                transform.localPosition = firstView.Item1;
                transform.localRotation = Quaternion.Euler(firstView.Item2);
                viewMode = ViewMode.firstViewMode;
            }
        }

        // Toggle Map view
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (viewMode == ViewMode.mapViewMode)
            {
                transform.localPosition = currentView.Item1;
                transform.localRotation = Quaternion.Euler(currentView.Item2);
                viewMode = ViewMode.thirdViewMode;
            }
            else
            {
                transform.localPosition = mapView.Item1;
                transform.localRotation = Quaternion.Euler(mapView.Item2);
                viewMode = ViewMode.mapViewMode;
            }
            
        }
    }

    private void LateUpdate()
    {
        // Camera zoom due to ScrollWheel input
        // Zoom forward
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && viewMode == ViewMode.thirdViewMode)
        {
            transform.localPosition = new Vector3(0,
                Mathf.MoveTowards(transform.localPosition.y, thirdViewMax.Item1.y,
                posYStep * Time.deltaTime * zoomSpeed),
                Mathf.MoveTowards(transform.localPosition.z, thirdViewMax.Item1.z,
                posZStep * Time.deltaTime * zoomSpeed));

            transform.localRotation = Quaternion.Euler(Vector3.MoveTowards(transform.localRotation.eulerAngles,
                thirdViewMax.Item2, rotXStep * Time.deltaTime * zoomSpeed));

            currentView.Item1 = transform.localPosition;
            currentView.Item2 = transform.localRotation.eulerAngles;
        }

        // Zoom back
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && viewMode == ViewMode.thirdViewMode)
        {
            transform.localPosition = new Vector3(0,
                Mathf.MoveTowards(transform.localPosition.y, thirdViewMin.Item1.y,
                posYStep * Time.deltaTime * zoomSpeed),
                Mathf.MoveTowards(transform.localPosition.z, thirdViewMin.Item1.z,
                posZStep * Time.deltaTime * zoomSpeed));

            transform.localRotation = Quaternion.Euler(Vector3.MoveTowards(transform.localRotation.eulerAngles,
                thirdViewMin.Item2, rotXStep * Time.deltaTime * zoomSpeed));

            currentView.Item1 = transform.localPosition;
            currentView.Item2 = transform.localRotation.eulerAngles;
        }
    }

    enum ViewMode
    {
        firstViewMode,
        thirdViewMode,
        mapViewMode
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float thirdViewZoomSpeed, mapViewZoomSpeed;

    public Transform firsrViewPoint, thirdViewMinPoint, thirdViewMaxPoint,
        mapViewMinPoint, mapViewMaxPoint;

    // Preset of positions and rotations for all camera views
    (Vector3, Vector3) firstView, thirdViewMin, thirdViewMax, mapViewMin, mapViewMax,
        currentThirdView, currentMapView;

    // Steps' values for smooth Camera zoom
    float posYStep, posZStep, rotXStep, mapZoomStep;

    float scrollSweelInput;

    ViewMode viewMode = ViewMode.thirdViewMode;

    // Start is called before the first frame update
    void Start()
    {
        // Adjusting Camera values
        firstView = InitViewPoint(firsrViewPoint, (new Vector3(0, 0.6f, 0.2f), new Vector3(8, 0, 0)));
        thirdViewMin = InitViewPoint(thirdViewMinPoint, (new Vector3(0, 2, -2), new Vector3(12, 0, 0)));
        thirdViewMax = InitViewPoint(thirdViewMaxPoint, (new Vector3(0, 10, -10), new Vector3(23, 0, 0)));
        mapViewMin = InitViewPoint(mapViewMinPoint, (new Vector3(0, 10, 0), new Vector3(90, 0, 0)));
        mapViewMax = InitViewPoint(mapViewMaxPoint, (new Vector3(0, 100, 0), new Vector3(90, 0, 0)));
        currentThirdView = thirdViewMax;
        currentMapView = mapViewMax;

        transform.localPosition = currentThirdView.Item1;
        transform.localRotation = Quaternion.Euler(currentThirdView.Item2);

        posYStep = Mathf.Abs(thirdViewMax.Item1.y - thirdViewMin.Item1.y);
        posZStep = Mathf.Abs(thirdViewMax.Item1.z - thirdViewMin.Item1.z);
        rotXStep = Mathf.Abs(thirdViewMax.Item2.x - thirdViewMin.Item2.x);
        mapZoomStep = Mathf.Abs(mapViewMax.Item1.y - mapViewMin.Item1.y);

        if (thirdViewZoomSpeed == 0)
        {
            thirdViewZoomSpeed = 2.5f;
        }
        if (mapViewZoomSpeed == 0)
        {
            mapViewZoomSpeed = 2.5f;
        }
    }

    // Initializing Camera view points with Transform position and rotation, or with altValue if Transform is null
    private (Vector3, Vector3) InitViewPoint(Transform viewPoint, (Vector3, Vector3) altValue)
    {
        if ((viewPoint == null) || (viewPoint.localPosition == Vector3.zero
            && viewPoint.localRotation == Quaternion.identity))
        {
            return altValue;
        }
        else
        {
            return (viewPoint.localPosition, viewPoint.localRotation.eulerAngles);
        }
    }

    // Update is called once per frame
    void Update()
    {
        scrollSweelInput = Input.GetAxis("Mouse ScrollWheel");

        // Toggle First person view
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (viewMode == ViewMode.firstViewMode)
            {
                transform.localPosition = currentThirdView.Item1;
                transform.localRotation = Quaternion.Euler(currentThirdView.Item2);
                viewMode = ViewMode.thirdViewMode;
            }
            else
            {
                if (viewMode == ViewMode.thirdViewMode)
                {
                    currentThirdView.Item1 = transform.localPosition;
                    currentThirdView.Item2 = transform.localRotation.eulerAngles;
                }
                else if (viewMode == ViewMode.mapViewMode)
                {
                    currentMapView.Item1 = transform.localPosition;
                    currentMapView.Item2 = transform.localRotation.eulerAngles;
                }

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
                currentMapView.Item1 = transform.localPosition;
                currentMapView.Item2 = transform.localRotation.eulerAngles;
                transform.localPosition = currentThirdView.Item1;
                transform.localRotation = Quaternion.Euler(currentThirdView.Item2);
                viewMode = ViewMode.thirdViewMode;
            }
            else
            {
                if (viewMode == ViewMode.thirdViewMode)
                {
                    currentThirdView.Item1 = transform.localPosition;
                    currentThirdView.Item2 = transform.localRotation.eulerAngles;
                }

                transform.localPosition = currentMapView.Item1;
                transform.localRotation = Quaternion.Euler(currentMapView.Item2);
                viewMode = ViewMode.mapViewMode;
            }
            
        }
    }

    private void LateUpdate()
    {
        // Camera zoom due to ScrollWheel input
        if (viewMode == ViewMode.thirdViewMode)
        {
            if (scrollSweelInput > 0)
            {
                transform.localPosition = new Vector3(0,
                    Mathf.MoveTowards(transform.localPosition.y, thirdViewMax.Item1.y,
                    posYStep * Time.deltaTime * thirdViewZoomSpeed),
                    Mathf.MoveTowards(transform.localPosition.z, thirdViewMax.Item1.z,
                    posZStep * Time.deltaTime * thirdViewZoomSpeed));

                transform.localRotation = Quaternion.Euler(Vector3.MoveTowards(transform.localRotation.eulerAngles,
                    thirdViewMax.Item2, rotXStep * Time.deltaTime * thirdViewZoomSpeed));
            }
            else if (scrollSweelInput < 0)
            {
                transform.localPosition = new Vector3(0,
                    Mathf.MoveTowards(transform.localPosition.y, thirdViewMin.Item1.y,
                    posYStep * Time.deltaTime * thirdViewZoomSpeed),
                    Mathf.MoveTowards(transform.localPosition.z, thirdViewMin.Item1.z,
                    posZStep * Time.deltaTime * thirdViewZoomSpeed));

                transform.localRotation = Quaternion.Euler(Vector3.MoveTowards(transform.localRotation.eulerAngles,
                    thirdViewMin.Item2, rotXStep * Time.deltaTime * thirdViewZoomSpeed));
            }
        }
        else if (viewMode == ViewMode.mapViewMode)
        {
            if (scrollSweelInput > 0)
            {
                transform.localPosition = new Vector3(0,
                    Mathf.MoveTowards(transform.localPosition.y, mapViewMax.Item1.y,
                    mapZoomStep * Time.deltaTime * mapViewZoomSpeed), 0);
            }
            else if (scrollSweelInput < 0)
            {
                transform.localPosition = new Vector3(0,
                    Mathf.MoveTowards(transform.localPosition.y, mapViewMin.Item1.y,
                    mapZoomStep * Time.deltaTime * mapViewZoomSpeed), 0);
            }
        }
    }

    enum ViewMode
    {
        firstViewMode,
        thirdViewMode,
        mapViewMode
    }
}

using UnityEngine;
using System.Collections;

public class GrabPack : MonoBehaviour
{
    public float grabDistance = 2f;
    public float grabSpeed = 5f;
    public LayerMask grabbableLayer;
    public LayerMask scannerlayer;

    public Transform grabPackHandLeft;
    public Transform grabPackHandRight;

    private GameObject grabbedObjectLeft;
    private GameObject grabbedObjectRight;
    public bool isGrabbingLeft = false;
    private bool isGrabbingRight = false;
    private bool isRetractingLeft = false;
    private bool isRetractingRight = false;
    private Camera mainCamera;
    public Transform PlayerCamera;
    private Transform originalParentLeft;
    private Transform originalParentRight;

    public GameObject OGPOS;
    public GameObject OGPOS2;

    public GameObject LINEPOS;
    public GameObject LINEPOS2;

    public GameObject HandLocationLeft;
    public GameObject HandLocationRight;

    private LineRenderer lineRendererLeft;
    private LineRenderer lineRendererRight;

    public AudioClip launch;
    public AudioClip retract;

    public AudioSource audio;

    public bool scannerright;
    public bool scannerleft;

    private void Start()
    {
        mainCamera = Camera.main;

        // Create LineRenderer components
        lineRendererLeft = grabPackHandLeft.gameObject.AddComponent<LineRenderer>();
        lineRendererLeft.material = new Material(Shader.Find("Sprites/Default"));
        lineRendererLeft.startWidth = 0.05f;
        lineRendererLeft.endWidth = 0.05f;
        lineRendererLeft.startColor = Color.black;
        lineRendererLeft.endColor = Color.black;

        lineRendererRight = grabPackHandRight.gameObject.AddComponent<LineRenderer>();
        lineRendererRight.material = new Material(Shader.Find("Sprites/Default"));
        lineRendererRight.startWidth = 0.05f;
        lineRendererRight.endWidth = 0.05f;
        lineRendererRight.startColor = Color.black;
        lineRendererRight.endColor = Color.black;
    }

    private void Update()
    {
        if (isGrabbingRight)
        {
            grabPackHandRight.transform.position = HandLocationRight.transform.position;
        }

        if (isGrabbingLeft)
        {
            grabPackHandLeft.transform.position = HandLocationLeft.transform.position;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isGrabbingLeft)
            {
                TryGrabObjectLeft();
            }
            else if (isGrabbingLeft && !isRetractingLeft)
            {
                StartCoroutine(RetractHandLeft());
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!isGrabbingRight)
            {
                TryGrabObjectRight();
            }
            else if (isGrabbingRight && !isRetractingRight)
            {
                StartCoroutine(RetractHandRight());
            }
        }

        if (isGrabbingLeft && !scannerleft && Input.GetMouseButton(0))
        {
            PullObject(grabPackHandLeft, HandLocationLeft, grabbedObjectLeft);
            UpdateLineRenderer(lineRendererLeft, LINEPOS, grabPackHandLeft.position);
        }

        if (isGrabbingRight && !scannerright && Input.GetMouseButton(1))
        {
            PullObject(grabPackHandRight, HandLocationRight, grabbedObjectRight);
            UpdateLineRenderer(lineRendererRight, LINEPOS2, grabPackHandRight.position);
        }


    }

    private void LateUpdate()
    {
        if (isGrabbingLeft)
        {
            // Update the line renderer positions for the left hand
            UpdateLineRenderer(lineRendererLeft, LINEPOS, grabPackHandLeft.position);
        }
        else
        {
            // Disable line renderer for the left hand when not grabbing
            lineRendererLeft.enabled = false;
        }

        if (isGrabbingRight)
        {
            // Update the line renderer positions for the right hand
            UpdateLineRenderer(lineRendererRight, LINEPOS2, grabPackHandRight.position);
        }
        else
        {
            // Disable line renderer for the right hand when not grabbing
            lineRendererRight.enabled = false;
        }
    }

    private void TryGrabObjectLeft()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, grabDistance, grabbableLayer))
        {
            if (!IsObstructed(hit.point))
            {
                Transform hitTransform = hit.collider.transform;

                grabbedObjectLeft = hit.collider.gameObject;
                HandLocationLeft.transform.position = hit.point;
                HandLocationLeft.transform.SetParent(hitTransform);

                isGrabbingLeft = true;

                // Unparent left grab pack hand from its current parent
                originalParentLeft = grabPackHandLeft.parent;
                grabPackHandLeft.SetParent(null);

                audio.PlayOneShot(launch, 2.0f);

                scannerleft = false;
            }
        }
        else if (Physics.Raycast(ray, out hit, grabDistance, scannerlayer))
        {
            Transform hitTransform = hit.collider.transform;

            grabbedObjectLeft = hit.collider.gameObject;
            HandLocationLeft.transform.position = hit.point;
            HandLocationLeft.transform.SetParent(hitTransform);

            isGrabbingLeft = true;

            // Unparent left grab pack hand from its current parent
            originalParentLeft = grabPackHandLeft.parent;
            grabPackHandLeft.SetParent(null);

            audio.PlayOneShot(launch, 2.0f);

            scannerleft = true;

        }
    }


    private void TryGrabObjectRight()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, grabDistance, grabbableLayer))
        {
            if (!IsObstructed(hit.point))
            {
                Transform hitTransform = hit.collider.transform;

                grabbedObjectRight = hit.collider.gameObject;
                HandLocationRight.transform.position = hit.point;
                HandLocationRight.transform.SetParent(hitTransform);

                isGrabbingRight = true;

                // Unparent right grab pack hand from its current parent
                originalParentRight = grabPackHandRight.parent;
                grabPackHandRight.SetParent(null);

                audio.PlayOneShot(launch, 2.0f);

                scannerright = false;
            }
        }
        else if (Physics.Raycast(ray, out hit, grabDistance, scannerlayer))
        {
            Transform hitTransform = hit.collider.transform;

            grabbedObjectRight = hit.collider.gameObject;
            HandLocationRight.transform.position = hit.point;
            HandLocationRight.transform.SetParent(hitTransform);

            isGrabbingRight = true;

            // Unparent right grab pack hand from its current parent
            originalParentRight = grabPackHandRight.parent;
            grabPackHandRight.SetParent(null);

            audio.PlayOneShot(launch, 2.0f);

            scannerright = true;
        }
    }



    private bool IsObstructed(Vector3 point)
    {
        // Cast a ray from the camera to the target point
        Vector3 cameraToTargetDirection = (point - mainCamera.transform.position).normalized;
        float distanceToTarget = Vector3.Distance(mainCamera.transform.position, point);

        Ray ray = new Ray(mainCamera.transform.position, cameraToTargetDirection);
        RaycastHit[] hits = Physics.RaycastAll(ray, distanceToTarget);

        // Check if any of the hits are walls or obstacles
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.CompareTag("Obstacle"))
            {
                return true; // The target point is obstructed
            }
        }

        return false; // The target point is not obstructed
    }

    private void ReleaseGrabbedObjectLeft()
    {
        grabbedObjectLeft.GetComponent<Rigidbody>().isKinematic = false;
        grabbedObjectLeft = null;
        isGrabbingLeft = false;

        // Re-parent left grab pack hand to its original parent
        grabPackHandLeft.SetParent(originalParentLeft);

        grabPackHandLeft.position = OGPOS.transform.position;
        grabPackHandLeft.rotation = OGPOS.transform.rotation;
        UpdateLineRenderer(lineRendererRight, LINEPOS, grabPackHandLeft.position);


        audio.PlayOneShot(retract, 2.0f);
    }

    private void ReleaseGrabbedObjectRight()
    {
        grabbedObjectRight.GetComponent<Rigidbody>().isKinematic = false;
        grabbedObjectRight = null;
        isGrabbingRight = false;
        UpdateLineRenderer(lineRendererRight, LINEPOS2, grabPackHandRight.position);

        // Re-parent right grab pack hand to its original parent
        grabPackHandRight.SetParent(originalParentRight);

        grabPackHandRight.position = OGPOS2.transform.position;
        grabPackHandRight.rotation = OGPOS2.transform.rotation;


        audio.PlayOneShot(retract, 2.0f);
    }

    private void PullObject(Transform grabPackHand, GameObject handLocation, GameObject grabbedObject)
    {

        Vector3 grabPackPosition = transform.position;
        Vector3 objectPosition = grabbedObject.transform.position;
        Vector3 direction = (grabPackPosition - objectPosition).normalized;
        Vector3 newPosition = objectPosition + direction * grabSpeed * Time.deltaTime;

        grabPackHand.position = handLocation.transform.position;
        grabPackHand.rotation = PlayerCamera.transform.rotation;

        grabbedObject.transform.position = newPosition;

    }

    private IEnumerator RetractHandLeft()
    {
        isRetractingLeft = true;

        while (Vector3.Distance(grabPackHandLeft.position, HandLocationLeft.transform.position) > 0.1f)
        {
            Vector3 newPos = Vector3.Lerp(grabPackHandLeft.position, HandLocationLeft.transform.position, grabSpeed * Time.deltaTime);
            grabPackHandLeft.position = newPos;

            yield return null;
        }

        isRetractingLeft = false;


        ReleaseGrabbedObjectLeft();
    }

    private IEnumerator RetractHandRight()
    {
        isRetractingRight = true;

        while (Vector3.Distance(grabPackHandRight.position, HandLocationRight.transform.position) > 0.1f)
        {
            Vector3 newPos = Vector3.Lerp(grabPackHandRight.position, HandLocationRight.transform.position, grabSpeed * Time.deltaTime);
            grabPackHandRight.position = newPos;

            yield return null;
        }

        isRetractingRight = false;

        ReleaseGrabbedObjectRight();
    }

    private void UpdateLineRenderer(LineRenderer lineRenderer, GameObject startPos, Vector3 endPos)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, startPos.transform.position);
        lineRenderer.SetPosition(1, endPos);
    }

    private bool CheckObstruction(Ray ray, Vector3 hitPoint)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, Vector3.Distance(mainCamera.transform.position, hitPoint));
        foreach (RaycastHit hit in hits)
        {
            // Ignore the grabbed objects and their colliders
            if (hit.collider.gameObject == grabbedObjectLeft || hit.collider.gameObject == grabbedObjectRight)
                continue;

            // If any collider is between the camera and the hit point, return true
            if (hit.collider.bounds.Contains(hitPoint))
                return true;
        }

        return false;
    }
}











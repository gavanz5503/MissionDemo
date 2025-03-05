using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip rubberBandSnap;
    // fields set in the Unity Inspector game
    // Inscribed fields are set in the Unity Inspector pane
    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    // Rubber band properties
    private LineRenderer lineRenderer;
    public Transform leftAnchor, rightAnchor;
    public GameObject rubberBandGO; // Reference to the RubberBand GameObject
    

    // fields set dynamically
    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;

    void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }


        // Initialize Line Renderer
        rubberBandGO = transform.Find("RubberBand").gameObject;
        lineRenderer = rubberBandGO.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("No Line Renderer found on RubberBand!");
            return;
        }

        // Find the Left and Right Anchors
        leftAnchor = transform.Find("LeftAnchor");
        rightAnchor = transform.Find("RightAnchor");

        if (leftAnchor == null || rightAnchor == null)
        {
            Debug.LogError("LeftAnchor or RightAnchor missing from Slingshot!");
            return;
        }

        // Configure Line Renderer properties
        lineRenderer.positionCount = 3; // Left anchor,projectile, right anchor
        //lineRenderer.startWidth = 0.05f;
        //lineRenderer.endWidth = 0.05f;
        lineRenderer.enabled = false; // Hide until used
    }

    void OnMouseEnter()
    {
        // print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    void OnMouseExit()
    {
        // print("Slingshot:OnMouseExit()");
        launchPoint.SetActive( false);
    }

    void OnMouseDown()
    {
        // The player has pressed the mouse button while over Slingshot
        aimingMode = true;
        // Instantiate a Projectile
        projectile = Instantiate(projectilePrefab) as GameObject;
        // Start it at the launchPoint
        projectile.transform.position = launchPos;
        // Set it to isKinematic for now
        projectile.GetComponent<Rigidbody>().isKinematic = true;

        // Show and update rubber band
        lineRenderer.enabled = true;
        UpdateRubberBand();
    }

    void Update()
    {
        // If Slingshot is not in aimingMode, don't run this code
        if (!aimingMode) return;

        // Get the current mouse positiion in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Find the delta from the launchPos to the mousePos3D
        Vector3 mouseDelta = mousePos3D - launchPos;
        // Limit mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }
        // Move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        // Update rubber band while aiming
        UpdateRubberBand();

        if (Input.GetMouseButtonUp(0))
        { 
            // The mouse has been released
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.linearVelocity = -mouseDelta * velocityMult;

            if (rubberBandSnap != null)
            {
                audioSource.PlayOneShot(rubberBandSnap);
            }

            // Switch to slingshot view immediately before setting POI
            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);

            FollowCam.POI = projectile; // Set the _MainCamera POI
            // Add a ProjectileLine to the Projectile
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();

            // Hide rubber band after release
            lineRenderer.enabled = false;
        }
    }

    void UpdateRubberBand()
    {
        if (projectile == null) return;

        // Ensure the Line Renderer has exactly 3 points
        lineRenderer.positionCount = 3;

        // Set the anchor positions correctly
        lineRenderer.SetPosition(0, leftAnchor.position);

        // Ensure the projectile stays centered between anchors
        Vector3 middlePos = projectile.transform.position;

        // Slightly adjust the middle position to prevent weird artifacts
        middlePos.z = (leftAnchor.position.z + rightAnchor.position.z) / 2;

        lineRenderer.SetPosition(1, middlePos);
        lineRenderer.SetPosition(2, rightAnchor.position);
    }

}

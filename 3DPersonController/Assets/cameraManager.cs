using UnityEngine;
using UnityEngine.InputSystem;

public class cameraManager : MonoBehaviour
{

    InputManager inputManager;

    public Transform targetTransform;   //Object camera follows
    public Transform cameraPivot;       //Object camera use to pivot
    public Transform cameraTransform;   //Transform of camera object
    public LayerMask collisionLayers;   //Layer camera collides with

    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;    //Camera transformation speed
    private Vector3 cameraVectorPosition;   //Cannot edit camera z pos from transform

    public float cameraCollisionOffset = 0.2f;  //Camera distance when collided
    public float minimumCollisionOffset = 0.2f; //Camera 
    public float cameraCollisionRadius=0.2f;    //Camera collision
    public float cameraFollowSpeed = 0.2f;  //Camera follow speed
    public float cameraLookSpeed = 2;       //Camera horizontal
    public float cameraPivotSpeed = 2;      //Camera vertical

    public float lookAngle;
    public float pivotAngle;

    public float minimumPivotAngle=-35;
    public float maximumPivotAngle=35;

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        //Deprecated FindObjectOfType
        targetTransform = FindFirstObjectByType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;

    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();
    }
    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed); //Smooth interpolation
        //Ref passes object as reference. ie pointer(like) but memory safe
        transform.position = targetPosition;
    }
    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;

        lookAngle = lookAngle+(inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle); //limit angle

        //Target rotation
        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        //Pivot rotation
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
        //transform.rotation = targetRotation;
    }
    

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit; //Stores whether hit when using raycast
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        //Create sphere collision at the (location, radius, direction, ray(bool), target)
        //send sphere to ray direction to the target position (Player)
        if (Physics.SphereCast(cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point); //Distance camera position and ray hit point
            targetPosition =- (distance - cameraCollisionOffset);
        }
        if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
        {
            targetPosition = targetPosition - minimumCollisionOffset;
        }

        //Move the camera z
        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f); //(orig, target, time)
        cameraTransform.localPosition=cameraVectorPosition;
    }

}

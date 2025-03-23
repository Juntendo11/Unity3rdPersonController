using UnityEngine;
using UnityEngine.InputSystem;

public class cameraManager : MonoBehaviour
{

    InputManager inputManager;

    public Transform targetTransform;   //Object camera follows
    public Transform cameraPivot;       //Object camera use to pivot

    private Vector3 cameraFollowVelocity = Vector3.zero;    //Camera transformation speed
    public float cameraFollowSpeed = 0.2f;  //Camera follow speed
    public float cameraLookSpeed = 2;
    public float cameraPivotSpeed = 2;

    public float lookAngle;
    public float pivotAngle;

    public float minimumPivotAngle=-35;
    public float maximumPivotAngle=35;

    private void Awake()
    {
        inputManager = FindFirstObjectByType<InputManager>();
        //Deprecated FindObjectOfType
        targetTransform = FindFirstObjectByType<PlayerManager>().transform;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
    }
    private void FollowTarget()
    {
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed); //Smooth interpolation
        //Ref passes object as reference. ie pointer(like) but memory safe

        transform.position = targetPosition;

    }
    private void RotateCamera()
    {
        lookAngle = lookAngle+(inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle); //limit angle

        //Target rotation
        Vector3 rotation = Vector3.zero;
        rotation.y = lookAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        transform.rotation = targetRotation;

        //Pivot rotation
        rotation = Vector3.zero;
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        cameraPivot.localRotation = targetRotation;
    }
}

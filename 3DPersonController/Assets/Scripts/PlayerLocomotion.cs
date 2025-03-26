using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    InputManager inputManager;
    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody playerRigidbody;

    [Header("Falling")]
    public float inAirTimer;

    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset=0.5f;   //Offset plane so not intersecting immediately
    public LayerMask groundLayer;
    public float maxDistance = 0.5f;

    [Header("Movement flags")]
    public bool isSprinting;
    public bool isGrounded;
    public bool isJumping;

    [Header("Movement Speeds")]
    public float walkingSpeed = 1.5f;
    public float runningSpeed = 5;
    public float sprintingSpeed = 7;
    public float rotationSpeed = 15;

    [Header("Jump Speeds")]
    public float jumpHeight = 3;
    public float gravityIntensity = -15;

    private void Awake()    //Call before start (Finishied variable)
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        //Check falling first

        HandleFallingAndLanding();
        if (playerManager.isInteracting)
        {
            return;
        }
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        if (isJumping)
        {
            //Cancel rotation when jumping
            return;
        }
        moveDirection = cameraObject.forward * inputManager.verticalInput;//Movement Input
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        //Check sprinting
        if(isSprinting)
        {
            moveDirection = moveDirection * sprintingSpeed;
        }
        else
        {
            if (inputManager.moveAmount >= 0.5f)
            {
                moveDirection = moveDirection * runningSpeed;
            }
            else
            {
                moveDirection = moveDirection * walkingSpeed;
            }
        }

        //moveDirection = moveDirection * runningSpeed;
        Vector3 movementVelocity = moveDirection;
        playerRigidbody.linearVelocity = movementVelocity;
    }

    private void HandleRotation()
    {

        if(isJumping)
        {
            //Cancel rotation when jumping
            return;
        }
        Vector3 targetDirection = Vector3.zero;
        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;
        if(targetDirection==Vector3.zero)
        {
            targetDirection = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 raycastOrigin = transform.position; //At the bottom of player
        raycastOrigin.y = raycastOrigin.y + rayCastHeightOffset;

        if(!isGrounded && isJumping) //If not grounded and dont want falling animation when jumping
        {
            if(!playerManager.isInteracting)    //If not interacting
            {
                //Falling Stuck until condition breaks
                animatorManager.PlayerTargetAnimation("Falling", true);
            }

            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);  //
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);   //Down*speed*acc(time)
        }

        if (Physics.SphereCast(raycastOrigin, 0.2f, -Vector3.up, out hit, maxDistance,groundLayer))
        {
            if(!isGrounded && playerManager.isInteracting) //Detect ground and not grounded
            {
                animatorManager.PlayerTargetAnimation("Land", true);    //Play land anim
            }
            inAirTimer = 0;
            isGrounded = true;
            playerManager.isInteracting = false;    //Set isInteracting when grounded
        }
        else
        {
            isGrounded = false;
        }
    }

    public void HandleJumping()
    {
        if(isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayerTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity; //Conserve the move speed but add height
            playerRigidbody.linearVelocity = playerVelocity;
        }
    }
}

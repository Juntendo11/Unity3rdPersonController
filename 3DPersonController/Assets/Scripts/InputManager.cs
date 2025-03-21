using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager: MonoBehaviour
{
    PlayerControls playerControls;
    AnimatorManager animatorManager;
    public Vector2 movementInput;
    private float moveAmount;
    public float verticalInput;
    public float horizontalInput;
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
    }
    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();  //Set ref to controls
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        }
        playerControls.Enable();

    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
    public void HandleAllInputs()
    {
        HandleMovementInput();
        //HandleJumpingInput
        //HandleActionInput
    }
    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        //Since the animation tree values are 0-1, the value must be positive
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));        //Clamps between 0 and 1
        animatorManager.UpdateAnimatorValues(0, moveAmount);
    }

}

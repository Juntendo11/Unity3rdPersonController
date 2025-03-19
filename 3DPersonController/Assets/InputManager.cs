using Unity.VisualScripting;
using UnityEngine;

public class NewEmptyCSharpScript
{
    PlayerControls playerControls;
    public Vector2 movementInput;

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

}

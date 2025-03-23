using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputManager inputManager;
    cameraManager cameraManager;
    PlayerLocomotion playerLocomotion;

    private void Awake()
    {
        //When put on player, the component will be on the same gameobject
        inputManager = GetComponent<InputManager>();
        cameraManager = FindFirstObjectByType<cameraManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
    }
    private void FixedUpdate()
    {   //Called every desired fps (30FPS etc)
        //Rigidbody works better with fixed update
        playerLocomotion.HandleAllMovement();
    }

    private void LateUpdate()
    {
        //Called the next frame
        cameraManager.HandleAllCameraMovement();
    }
}

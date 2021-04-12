using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    // gets the inputs from the input handler object created in the Unity engine
    public class InputHandler : MonoBehaviour
    {
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        public bool xInput;
        
        private PlayerControls inputActions;
        private PlayerManager playerManager;
        private CheckSurroundings checkSurroundings;

        private Vector2 movementInput;
        private Vector2 cameraInput;

        private void Awake()
        {
            checkSurroundings = GetComponent<CheckSurroundings>();
            playerManager = GetComponent<PlayerManager>();
        }

        // enables input actions manager
        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();
                inputActions.PlayerMovement.Movement.performed +=
                    inputActions => movementInput = inputActions.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += camIn => cameraInput = camIn.ReadValue<Vector2>();
            }
            
            inputActions.Enable();
        }

        // disables the input action manager
        public void OnDisable()
        {
            inputActions.Disable();
        }

        // gets called every frame
        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleInteractionInput(delta);
        }

        // gets input from the input actions manager
        private void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(vertical) + Mathf.Abs(horizontal));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;
        }

        private void HandleInteractionInput(float delta)
        {
            xInput = inputActions.PlayerActions.Interact.phase == UnityEngine.InputSystem.InputActionPhase.Started;
            if (xInput)
            {
                if (playerManager.canOpenChest)
                {
                    checkSurroundings.OpenChest();
                }
            }
        }
    }
}
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

        private PlayerControls inputActions;
        private CameraHandler cameraHandler;

        private Vector2 movementInput;
        private Vector2 cameraInput;
        private bool iscameraHandlerNotNull;

        // finds the camera handler script so that the camera can be controller
        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
            iscameraHandlerNotNull = cameraHandler != null;
        }

        // follows the player and points at them
        // since the camera can collide, it should be in FixedUpdate
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;

            if (iscameraHandlerNotNull)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, mouseX, mouseY);
            }
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
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    // this class handles everything that has to do with the player so there are not so many update functions
    public class PlayerManager : MonoBehaviour
    {
        // gets all the scripts
        private CameraHandler cameraHandler;
        private InputHandler inputHandler;
        private PlayerLocomotion playerLocomotion;
        
        private Animator anim;
        private bool iscameraHandlerNotNull;
        
        public bool isInteracting;

        [Header("Player Flags")]
        public bool isInAir;
        public bool isGrounded;
        
        // Collects all of the scripts used for the player
        void Start()
        {
            inputHandler = GetComponent<InputHandler>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            anim = GetComponentInChildren<Animator>();

            isInteracting = false;
        }
        
        // finds the camera handler script so that the camera can be controller
        private void Awake()
        {
            cameraHandler = CameraHandler.singleton;
            iscameraHandlerNotNull = cameraHandler != null;
        }
        
        // handles the collision scripts that have to do with the player
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            
            if (iscameraHandlerNotNull)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }
            else
            {
                cameraHandler = CameraHandler.singleton;
                iscameraHandlerNotNull = cameraHandler != null;
            }
        }

        // Update is called once per frame
        void Update()
        {
            float delta = Time.deltaTime;
            inputHandler.TickInput(delta);
            playerLocomotion.HandleMovement(delta);
        }

        private void LateUpdate()
        {
            float delta = Time.deltaTime;
            if (isInAir)
            {
                playerLocomotion.inAirTimer += delta;
            }
        }
    }
}
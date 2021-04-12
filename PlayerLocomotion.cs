using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    // controls the players movement
    public class PlayerLocomotion : MonoBehaviour
    {
        private PlayerManager playerManager;
        private Transform cameraObject;
        private InputHandler inputHandler;
        private Vector3 moveDirection;

        [HideInInspector] public Transform playerTransform;
        [HideInInspector] public PlayerAnimatorHandler animatorHandler;

        [Header("Ground and Air detection")]
        [SerializeField]
        private float groundDetectionRayStartPoint = .5f;
        [SerializeField]
        private float minRequiredFallHeight = 1f;
        [SerializeField]
        private float groundDetectionRayDistance = .2f;
        public float inAirTimer;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Movement Stats")]
        [SerializeField] 
        private float movementSpeed = 5;
        [SerializeField] 
        private float rotationSpeed = 10;
        [SerializeField]
        private float fallSpeed = 45;
        
        // finds all the components and game objects relevant to the movement of the player
        void Start()
        {
            playerManager = GetComponent<PlayerManager>();
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            cameraObject = Camera.main.transform;
            playerTransform = transform;
            animatorHandler = GetComponentInChildren<PlayerAnimatorHandler>();
            animatorHandler.Initialize();

            playerManager.isGrounded = true;
        }

        // the actual code for moving the player
        #region Movement

        private Vector3 normalVector;
        private Vector3 targetPosition;

        public void HandleMovement(float delta)
        {
            moveDirection = cameraObject.forward * inputHandler.vertical;
            moveDirection += cameraObject.right * inputHandler.horizontal;
            moveDirection.y = 0;
            moveDirection.Normalize();

            float speed = movementSpeed;
            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;
            
            // makes the player appear to walk faster the more the user is trying to move them
            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0);

            if (animatorHandler.canRotate)
            {
                HandleRotation(delta);
            }
        }

        // makes the player turn when the input is moving a different direction
        private void HandleRotation(float delta)
        {
            Vector3 targetDir = Vector3.zero;
            float moveOverride = inputHandler.moveAmount;

            targetDir = cameraObject.forward * inputHandler.vertical;
            targetDir += cameraObject.right * inputHandler.horizontal;
            
            targetDir.Normalize();
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = playerTransform.forward;

            float rs = rotationSpeed;
            Quaternion targetRot = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(playerTransform.rotation, targetRot, rs * delta);

            playerTransform.rotation = targetRotation;
        }

        #endregion
    }
}
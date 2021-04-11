using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace Controls
{
    // controls the main camera
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        private Transform thisTransform;
        private Vector3 cameraTransformPosition;
        private LayerMask ignoreLayers;
        private Vector3 cameraFollowVelocity = Vector3.zero;

        public static CameraHandler singleton;

        public float lookSpeed = .1f;
        public float followSpeed = .1f;
        public float pivotSpeed = .03f;

        private float targetPosition;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35f;
        public float maximumPivot = 35f;

        public float cameraSphereRadius = .2f;
        public float cameraCollisionOffset = .2f;
        public float minimumCollisionOffset = .2f;

        // finds the gameobjects connected to the camera and the camera's components
        private void Awake()
        {
            singleton = this;
            thisTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
        }

        // keeps a set distance from the player
        public void FollowTarget(float delta)
        {
            Vector3 targetPosition =
                Vector3.SmoothDamp(thisTransform.position, targetTransform.position, ref cameraFollowVelocity, delta / followSpeed);
            thisTransform.position = targetPosition;
            
            HandleCameraCollisions(delta);
        }

        // allows the camera to move around and faces towards the player
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            lookAngle += (mouseXInput * lookSpeed) / delta;
            pivotAngle -= (mouseYInput * lookSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);
            
            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            thisTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;
            
            targetRotation = Quaternion.Euler(rotation);
            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        // stops camera from going into walls.
        // if this is done in the normal engine, finding the angle of the player relative to the camera stops working
        private void HandleCameraCollisions(float delta)
        {
            targetPosition = defaultPosition;
            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast(cameraPivotTransform.position, cameraSphereRadius, direction, out hit,
                Mathf.Abs(targetPosition), ignoreLayers))
            {
                float distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = cameraCollisionOffset - distance;
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta);
            cameraTransform.localPosition = cameraTransformPosition;
        }
    }
}
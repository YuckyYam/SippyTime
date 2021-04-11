using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    // controls the animator attached to the player model
    public class PlayerAnimatorHandler : MonoBehaviour
    {
        public Animator anim;
        private int vertical;
        private int horizontal;
        public bool canRotate;

        // finds the animator allows the parameter names to be changed
        public void Initialize()
        {
            anim = GetComponent<Animator>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        // updates parameters in the animator, like how fast the player is walking
        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement)
        {
            // vertical movement represents how much the player is trying to move in their forward direction
            #region Vertical
            // fuzzy logic to clamp the input and make it more discrete
            float v = 0;

            if ((verticalMovement > 0) && (verticalMovement <= .55f))
            {
                v = .5f;
            }
            else if (verticalMovement > .55f)
            {
                v = 1;
            }
            else if ((verticalMovement < 0) && (verticalMovement >= -.55f))
            {
                v = -.5f;
            }
            else if (verticalMovement < -.55f)
            {
                v = -1;
            }
            else
            {
                v = 0;
            }

            #endregion

            // horizontal movement represents how much the player is trying to turn
            #region Horizontal
            // fuzzy logic to clamp the input and make it more discrete
            float h = 0;

            if ((horizontalMovement > 0) && (horizontalMovement <= .55f))
            {
                h = .5f;
            }
            else if (horizontalMovement > .55f)
            {
                h = 1;
            }
            else if ((horizontalMovement < 0) && (horizontalMovement >= -.55f))
            {
                h = -.5f;
            }
            else if (horizontalMovement < -.55f)
            {
                h = -1;
            }
            else
            {
                h = 0;
            }

            #endregion

            anim.SetFloat(vertical, v, .1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, .1f, Time.deltaTime);
        }

        // allows the player to rotate
        public void CanRotate()
        {
            canRotate = true;
        }

        // stops the player from rotating
        public void StopRotation()
        {
            canRotate = false;
        }
    }
}
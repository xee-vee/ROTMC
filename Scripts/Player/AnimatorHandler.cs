using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace lxxj 
{
    public class AnimatorHandler : AnimatorManager
    {
        #region Variables
        InputHandler inputHandler;
        PlayerLocomotion playerLocomotion;
        PlayerManager playerManager;
        int vertical;
        int horizontal;
        #endregion

        // Custom component finding method to be used in player locomotion script
        public void Initialize()
        {
            anim = GetComponent<Animator>();
            inputHandler = GetComponent<InputHandler>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            playerManager = GetComponent<PlayerManager>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        // Updates the animator component's values to accurately follow player movement
        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            #region Vertical
            float v = 0f;

            if (verticalMovement > 0 && verticalMovement < 0.55f)
            {
                v = 0.5f;
            }
            else if (verticalMovement >= 0.55f)
            {
                v = 1f;
            }
            else if (verticalMovement < 0 && verticalMovement > -0.55f)
            {
                v = -0.5f;
            }
            else if (verticalMovement <= -0.55f)
            {
                v = -1f;
            }
            #endregion

            #region Horizontal
            float h = 0f;

            if (horizontalMovement > 0 && horizontalMovement < 0.55f)
            {
                h = 0.5f;
            }
            else if (horizontalMovement >= 0.55f)
            {
                h = 1f;
            }
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
            {
                h = -0.5f;
            }
            else if (horizontalMovement <= -0.55f)
            {
                h = -1f;
            }
            #endregion

            if (isSprinting)
            {
                v = 2f;
                h = horizontalMovement;
            }

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        // Public methods that update the bools within the Unity animator
        #region Animator bool updates
        public void CanRotate()
        {
            anim.SetBool("canRotate", true);
        }

        public void StopRotation()
        {
            anim.SetBool("canRotate", false);
        }

        public void EnableCombo()
        {
            anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            anim.SetBool("canDoCombo", false);
        }

        public void EnableIsInvulnerable()
        {
            anim.SetBool("isInvulnerable", true);
        }

        public void DisableIsInvulnerable()
        {
            anim.SetBool("isInvulnerable", false);
        }
        #endregion

        // Keeps rigidbody movement synchronised with animator movement
        private void OnAnimatorMove()
        {
            if (!playerManager.isInteracting)
            {
                return;
            }

            float delta = Time.deltaTime;
            if (delta == 0f)
            {
                return;
            }

            playerLocomotion.rigidbody.drag = 0f;

            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0f;

            Vector3 velocity = deltaPosition / delta;
            playerLocomotion.rigidbody.velocity = velocity;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using lxxj;
using UnityEditor;
using UnityEngine;

namespace lxxj
{
    public class PlayerManager : CharacterManager
    {
        #region Variables
        InputHandler inputHandler;
        Animator anim;
        CameraHandler cameraHandler;
        PlayerLocomotion playerLocomotion;
        InteractableUI interactableUI;
        PlayerStats playerStats;
        AnimatorHandler animatorHandler;
        public GameObject interactableUIGameObject;
        public GameObject interactableGameObject;

        public bool isInteracting;

        [Header("Player Flags")]
        public bool isSprinting;
        public bool isInAir;
        public bool isInvulnerable;
        public bool isGrounded;
        public bool canDoCombo;

        private Vector3 sphereCastOrigin;
        private Vector3 sphereCastDirection;
        private float sphereCastRadius = 3f;
        private float sphereCastMaxDistance = 5f;
        private bool drawGizmos = true;
        #endregion

        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
            interactableUI = FindObjectOfType<InteractableUI>();
            inputHandler = GetComponent<InputHandler>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            anim = GetComponent<Animator>();
            playerStats = GetComponent<PlayerStats>();
            animatorHandler = GetComponent<AnimatorHandler>();
        }

        void Update()
        {
            // Delta time needed for all updates
            float delta = Time.deltaTime;

            // Setting bools from animator (strings need to be exact)
            isInteracting = anim.GetBool("isInteracting");
            canDoCombo = anim.GetBool("canDoCombo");
            isInvulnerable = anim.GetBool("isInvulnerable");
            animatorHandler.canRotate = anim.GetBool("canRotate");
            anim.SetBool("isInAir", isInAir);

            // Movement updates
            inputHandler.TickInput(delta);
            playerLocomotion.PlayerRollingAndSprinting(delta);
            playerLocomotion.PlayerJump();
            CheckForInteractableObject();
            playerStats.RegenStamina();
        }

        // Physics logic handled in FixedUpdate, runs more regularly than regulat update method
        private void FixedUpdate()
        {
            float delta = Time.fixedDeltaTime;
            playerLocomotion.PlayerMovement(delta);
            playerLocomotion.PlayerFalling(delta, playerLocomotion.moveDirection);
            playerLocomotion.PlayerRotation(delta);
        }
    
        // Resetting input bools in LateUpdate, as this ensures they are only true once per frame
        private void LateUpdate()
        {
            inputHandler.rollFlag = false;
            inputHandler.rb_Input = false;
            inputHandler.rt_Input = false;
            inputHandler.a_Input = false;
            inputHandler.jump_Input = false;

            float delta = Time.fixedDeltaTime;

            if (cameraHandler != null)
            {
                cameraHandler.FollowPlayer(delta);
                cameraHandler.RotateCamera(delta, inputHandler.mouseX, inputHandler.mouseY);
            }

            if (isInAir)
            {
                playerLocomotion.inAirTimer = playerLocomotion.inAirTimer + Time.deltaTime;
            }
        }
    
        // Checks whether the player is in range of an interactable object, displays text if so
        public void CheckForInteractableObject()
        {
            RaycastHit hit;
            Vector3 sphereCastOrigin = transform.position + Vector3.up * 2f; // add Y offset
            Vector3 sphereCastDirection = transform.forward;

            if (Physics.SphereCast(sphereCastOrigin, sphereCastRadius, sphereCastDirection, out hit, sphereCastMaxDistance))
            {
                if (hit.collider.CompareTag("Interactable"))
                {
                    Interactable interactableObject = hit.collider.GetComponent<Interactable>();

                    if (interactableObject != null)
                    {
                        interactableUI.interactableText.text = interactableObject.interactableText;
                        interactableUIGameObject.SetActive(true);

                        if (inputHandler.a_Input)
                        {
                            // Add logic for interacting with the object on button (not used yet)
                        }
                    }
                }
            }
            else if (interactableUIGameObject != null)
            {
                interactableUIGameObject.SetActive(false);
            }
        }

        // Useful Gizmo debugging for interacting with objects in scene view
        private void OnDrawGizmos()
        {
            if (drawGizmos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(sphereCastOrigin, sphereCastRadius);
                Gizmos.DrawLine(sphereCastOrigin, sphereCastOrigin + sphereCastDirection * sphereCastMaxDistance);
                Gizmos.DrawWireSphere(sphereCastOrigin + sphereCastDirection * sphereCastMaxDistance, sphereCastRadius);
            }
        }
    }

}

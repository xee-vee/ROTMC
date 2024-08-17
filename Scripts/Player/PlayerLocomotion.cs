using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Animations;

namespace lxxj
{
    public class PlayerLocomotion : MonoBehaviour
    {
        #region Variables
        Transform cameraObject; 
        InputHandler inputHandler;
        public Vector3 moveDirection;
        CameraHandler cameraHandler;
        PlayerStats playerStats;
        PlayerManager playerManager;
        StaminaBar staminaBar;
        [HideInInspector] public Transform myTransform;
        [HideInInspector] public AnimatorHandler animatorHandler;

        public new Rigidbody rigidbody;
        public GameObject normalCamera;

        [Header("Ground & Air Detection Stats")]
        [SerializeField] float groundDetectionRayStartPoint = 0.5f;
        [SerializeField] float minimumDistanceNeededToBeginFall = 1f;
        [SerializeField] float groundDirectionRayDistance = 0.2f;
        LayerMask ignroreForGroundCheck;
        public float inAirTimer;


        [Header("Movement Stats")]
        [SerializeField] float movementSpeed = 5f;
        [SerializeField] float walkingSpeed = 2.5f;
        [SerializeField] float rotationSpeed = 10f;
        [SerializeField] float sprintSpeed = 7f;
        [SerializeField] float fallSpeed = 45f;
        [SerializeField] int rollStaminaCost = 30;
        [SerializeField] int backstepStaminaCost = 10;
        [SerializeField] int sprintStaminaCost = 1;
        #endregion

        private void Awake()
        {
            cameraHandler = FindObjectOfType<CameraHandler>();
        }

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            inputHandler = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            playerManager = GetComponent<PlayerManager>();
            playerStats = GetComponent<PlayerStats>();
            staminaBar = FindObjectOfType<StaminaBar>();
            cameraObject = Camera.main.transform;
            myTransform = transform;
            animatorHandler.Initialize();

            playerManager.isGrounded = true;
            ignroreForGroundCheck = ~(1 << 2 | 1 << 10);
        }

        #region Movement
        Vector3 normalVector;
        Vector3 targetPosition;

        // Logic for rotating the player based on movement inputs, with different behaviour for sprinting & lock on
        public void PlayerRotation(float delta)
        {
            if (!animatorHandler.canRotate) { return; }

            Vector3 targetDirection = Vector3.zero;
            Quaternion targetRotation;

            if (inputHandler.lockOnFlag)
            {
                if (inputHandler.sprintFlag || inputHandler.rollFlag)
                {
                    targetDirection = cameraHandler.cameraTransform.forward * inputHandler.vertical +
                                    cameraHandler.cameraTransform.right * inputHandler.horizontal;
                }
                else
                {
                    targetDirection = cameraHandler.currentLockOnTarget.transform.position - transform.position;
                }
            }
            else
            {
                targetDirection = cameraObject.forward * inputHandler.vertical +
                                cameraObject.right * inputHandler.horizontal;
            }

            targetDirection.y = 0;
            targetDirection = targetDirection == Vector3.zero ? transform.forward : targetDirection;
            targetDirection.Normalize();

            targetRotation = Quaternion.LookRotation(targetDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * delta);
        }


        // Logic for moving the player based on inputs, also for animation states & stamina costs
        public void PlayerMovement(float delta)
        {
            if (inputHandler.rollFlag || animatorHandler.anim.GetBool("isInteracting"))
            {
                return;
            }

            moveDirection = cameraObject.forward * inputHandler.vertical + cameraObject.right * inputHandler.horizontal;
            moveDirection.Normalize();
            moveDirection.y = 0;

            float speed = inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f ? sprintSpeed : movementSpeed;

            if (inputHandler.sprintFlag && inputHandler.moveAmount > 0.5f)
            {
                playerManager.isSprinting = true;
                playerStats.DrainStamina(sprintStaminaCost);
            }
            else
            {
                playerManager.isSprinting = false;
                speed = inputHandler.moveAmount < 0.5f ? walkingSpeed : movementSpeed;
            }

            moveDirection *= speed;

            Vector3 projectedVelocity = Vector3.ProjectOnPlane(moveDirection, normalVector);
            rigidbody.velocity = projectedVelocity;

            if (inputHandler.lockOnFlag && !inputHandler.sprintFlag)
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.vertical, inputHandler.horizontal, playerManager.isSprinting);
            }
            else
            {
                animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, 0, playerManager.isSprinting);
            }
        }

        // Inverts movement (redundant, part of a debugging session)
        public void InvertMovementDirection()
        {
            moveDirection *= -1;
        }

        // Logic for rolling & sprinting, includes backstep/roll check, stamina & animation
        public void PlayerRollingAndSprinting(float delta)
        {
            if (animatorHandler.anim.GetBool("isInteracting") || playerStats.currentStamina <= 0)
            {
                return;
            }

            if (inputHandler.rollFlag)
            {
                moveDirection = cameraObject.forward * inputHandler.vertical + cameraObject.right * inputHandler.horizontal;
                moveDirection.Normalize();

                if (inputHandler.moveAmount > 0)
                {
                    playerStats.TakeStaminaDamage(rollStaminaCost);
                    animatorHandler.PlayTargetAnimation("Rolling_02", true);

                    moveDirection.y = 0;
                    myTransform.rotation = Quaternion.LookRotation(moveDirection);
                }
                else
                {
                    playerStats.TakeStaminaDamage(backstepStaminaCost);
                    animatorHandler.PlayTargetAnimation("BackStep", true);
                }
            }
        }


        // Logic for player behaviour during falling 
        public void PlayerFalling(float delta, Vector3 moveDirection)
        {
            playerManager.isGrounded = false;
            RaycastHit hit;
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;

            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f))
            {
                moveDirection = Vector3.zero;
            }

            if (playerManager.isInAir)
            {
                rigidbody.AddForce(-Vector3.up * fallSpeed);
                rigidbody.AddForce(moveDirection * fallSpeed / 20f);
            }

            Vector3 dir = moveDirection.normalized;
            origin += dir * groundDirectionRayDistance;

            targetPosition = myTransform.position;

            UnityEngine.Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red, 0.1f, false);

            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, ignroreForGroundCheck))
            {
                normalVector = hit.normal;
                Vector3 tp = hit.point;
                playerManager.isGrounded = true;
                targetPosition.y = tp.y;

                if (playerManager.isInAir)
                {
                    if (inAirTimer > 0.5f)
                    {
                        UnityEngine.Debug.Log("You were in the air for " + inAirTimer);
                        animatorHandler.PlayTargetAnimation("Land", true);
                    }
                    else
                    {
                        animatorHandler.PlayTargetAnimation("Empty", false);
                    }

                    inAirTimer = 0;
                    playerManager.isInAir = false;
                }
            }
            else
            {
                if (playerManager.isGrounded)
                {
                    playerManager.isGrounded = false;
                }

                if (!playerManager.isInAir)
                {
                    if (!playerManager.isInteracting)
                    {
                        animatorHandler.PlayTargetAnimation("Falling", true);
                    }

                    rigidbody.velocity = rigidbody.velocity.normalized * (movementSpeed / 2);
                    playerManager.isInAir = true;
                }
            }

            if (playerManager.isGrounded)
            {
                if (playerManager.isInteracting || inputHandler.moveAmount > 0)
                {
                    myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, Time.deltaTime / 0.1f);
                }
                else
                {
                    myTransform.position = targetPosition;
                }
            }

        }

        // Logic for player jump, including animation
        public void PlayerJump()
        {
            if (playerManager.isInteracting) { return; }
            if (playerStats.currentStamina <= 0) { return; }

            if (inputHandler.jump_Input)
            {
                if (inputHandler.moveAmount > 0)
                {
                    moveDirection = cameraObject.forward * inputHandler.vertical;
                    moveDirection += cameraObject.right * inputHandler.horizontal;
                    animatorHandler.PlayTargetAnimation("Jump", true);
                    moveDirection.y = 0;
                    Quaternion jumpRotation = Quaternion.LookRotation(moveDirection);
                    myTransform.rotation = jumpRotation;
                }
            }
        }
       
        #endregion
    }
}

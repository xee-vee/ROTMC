using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace lxxj {
    public class InputHandler : MonoBehaviour
    {
        #region Variables
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;
        public bool b_Input;
        public bool rb_Input;
        public bool rt_Input;
        public bool jump_Input;
        public bool a_Input;
        public bool lockOnInput;
        public bool right_Stick_Right_Input;
        public bool right_Stick_Left_Input;
        public bool x_Input;
        public bool resetInput;
        public bool rollFlag;
        public bool sprintFlag;
        public bool comboFlag;
        public bool lockOnFlag; 
        public bool cameraIsTopDownFlag;
        public float rollInputTimer;    
        bool invertMovement = false;
        public bool isInactive = false;

        [Header("Analytics readings")]
        public bool rollFrameFlag;
        PlayerControls inputActions;
        PlayerAttacker playerAttacker;
        PlayerInventory playerInventory;
        PlayerManager playerManager;
        PlayerStats playerStats;
        CameraHandler cameraHandler;
        CameraSwitch cameraSwitch;
        GameSession gameSession;
        Vector2 HandlemoveInput; 
        Vector2 cameraInput;
        private float inactivityTimer = 0f; 
        private float inactivityThreshold = 120f; 
        #endregion

        private void Update()
        {
            CheckInactivity(Time.deltaTime);
        }
        
        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
            playerAttacker = GetComponent<PlayerAttacker>();
            playerInventory = GetComponent<PlayerInventory>();
            playerManager = GetComponent<PlayerManager>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            cameraSwitch = FindObjectOfType<CameraSwitch>();
            gameSession = FindObjectOfType<GameSession>();
        }

        // Setting the corresponding variables to true whenever the input is pressed
        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();

                inputActions.PlayerMovement.Movement.performed += condition => HandlemoveInput = condition.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.performed += condition => cameraInput = condition.ReadValue<Vector2>();

                // Attack Inputs
                inputActions.PlayerActions.RB.performed += condition => rb_Input = true;
                inputActions.PlayerActions.RT.performed += condition => rt_Input = true;

                // Interaction Input (not used)
                inputActions.PlayerActions.A.performed += condition => a_Input = true;

                // Jump Input
                inputActions.PlayerActions.Jump.performed += condition => jump_Input = true;

                // Lock On Input
                inputActions.PlayerActions.LockOn.performed += condition => lockOnInput = true;

                // Right Stick Lock On Input
                inputActions.PlayerActions.LockOnRight.performed += condition => right_Stick_Right_Input = true;
                inputActions.PlayerActions.LockOnLeft.performed += condition => right_Stick_Left_Input = true;

                // Camera Switch Input (not used)
                inputActions.PlayerActions.CameraSwitch.performed += condition => x_Input = true;

                // Roll Input
                inputActions.PlayerActions.Roll.performed += condition => b_Input = true;
                inputActions.PlayerActions.Roll.canceled += condition => b_Input = false;

                // Reset Level Input
                inputActions.PlayerActions.ResetGame.performed += condition => resetInput = true;
            }

            inputActions.Enable();
        }

        // Disables all input functionality
        private void OnDisable()
        {
            inputActions.Disable();
        }

        // Custom checking method to be used in Update in PlayerManager
        public void TickInput(float delta)
        {
            MoveInput(delta);
            RollInput(delta);
            AttackInput(delta);
            LockOnInput();
            CameraSwitch();
            ManualGameReset();
            CheckInactivity(delta);
        }

        // Toggle movement inversion
        public void ToggleMovementInversion(bool invert) 
        {
            invertMovement = invert;
        }

        // Sets player movement based on x & y input values
        private void MoveInput(float delta)
        {
           float horizontalInput = HandlemoveInput.x;
            float verticalInput = HandlemoveInput.y;

            // Apply inversion logic if enabled
            if (invertMovement)
            {
                horizontalInput = -horizontalInput;
                verticalInput = -verticalInput;
            }

            horizontal = horizontalInput;
            vertical = verticalInput;

            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX = cameraInput.x;
            mouseY = cameraInput.y;

            // Reset inactivity timer on any movement input
            if (horizontal != 0f || vertical != 0f || mouseX != 0f || mouseY != 0f)
            {
                ResetInactivityTimer();
            }

        }

        // Handles logic for rolling & sprinting
        private void RollInput(float delta)
        {
            if (b_Input)
            {
                rollInputTimer += delta;

                // Use stamina if no stamina do nothing
                if (playerStats.currentStamina <= 0)
                {
                    b_Input = false;
                    sprintFlag = false;
                }
                else
                {
                    // Sprint
                    if (moveAmount > 0.5f)
                    {
                        sprintFlag = true;
                    }

                    // Reset inactivity timer on roll input
                    ResetInactivityTimer();
                }
            }
            else
            {
                sprintFlag = false;

                // Roll
                if (rollInputTimer > 0f && rollInputTimer < 0.5f)
                {
                    rollFrameFlag = true;
                    rollFlag = true;
                }
                rollInputTimer = 0f;
            }

        }

        // Handles logic for both light & heavy attacks, including combo
        private void AttackInput(float delta)
        {
            // Light attack
            if (rb_Input)
            {
                if (playerManager.canDoCombo)
                {
                    comboFlag = true;
                    playerAttacker.WeaponCombo(playerInventory.rightWeapon);
                    comboFlag = false;
                }
                else if (!playerManager.isInteracting)
                {
                    playerAttacker.LightAttack(playerInventory.rightWeapon);
                }

                // Reset inactivity timer on attack input
                ResetInactivityTimer();
            }

            // Heavy attack
            if (rt_Input && !playerManager.canDoCombo && !playerManager.isInteracting)
            {
                playerAttacker.HeavyAttack(playerInventory.rightWeapon);

                // Reset inactivity timer on attack input
                ResetInactivityTimer();
            }
        }

        // Handles lock on input logic
        private void LockOnInput()
        {
            // Lock on
            if (lockOnInput)
            {
                lockOnInput = false;

                if (!lockOnFlag)
                {
                    cameraHandler.LockOn();
                    if (cameraHandler.nearestLockOnTarget != null)
                    {
                        cameraHandler.currentLockOnTarget = cameraHandler.nearestLockOnTarget;
                        lockOnFlag = true;
                    }
                }
                else
                {
                    lockOnFlag = false;
                    cameraHandler.ClearLockOn();
                }

                // Reset inactivity timer on lock-on input
                ResetInactivityTimer();
            }

            // Move lock on targets left
            if (lockOnFlag && right_Stick_Left_Input)
            {
                right_Stick_Left_Input = false;
                cameraHandler.LockOn();

                if (cameraHandler.leftLockOnTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.leftLockOnTarget;
                }

                // Reset inactivity timer on lock-on input
                ResetInactivityTimer();
            }

            // Move lock on targets right
            if (lockOnFlag && right_Stick_Right_Input)
            {
                right_Stick_Right_Input = false;
                cameraHandler.LockOn();

                if (cameraHandler.rightLockOnTarget != null)
                {
                    cameraHandler.currentLockOnTarget = cameraHandler.rightLockOnTarget;
                }

                // Reset inactivity timer on lock-on input
                ResetInactivityTimer();
            }

            // Adjust camera height
            cameraHandler.CHangeCameraHeight();

        }

        // Handles input for switching cameras, no longer used but functionality may be useful in future
        private void CameraSwitch()
        {
            if (x_Input)
            {
                x_Input = false;

                if (!cameraIsTopDownFlag)
                {
                    // Switch camera to top down view
                    cameraSwitch.SwitchToTopDown();
                    cameraIsTopDownFlag = true;
                }
                else
                {
                    // Switch camera back to third person view
                    cameraSwitch.SwitchToThirdPerson();
                    cameraIsTopDownFlag = false;
                }

                // Reset inactivity timer on camera switch input
                ResetInactivityTimer();
            }
        }

        // Resets the game when reset input is pressed
        private void ManualGameReset()
        {
            if (resetInput)
            {
                gameSession.ResetGame();

                // Reset inactivity timer on reset input
                ResetInactivityTimer();
            }
        }

        // Used to check whether the player is inactive or not
        private void CheckInactivity(float delta)
        {
            inactivityTimer += delta;

            if (inactivityTimer >= inactivityThreshold)
            {
                isInactive = true;
                inactivityTimer = 0; 
            }
        }

        // Resets inactivity timer, used in almost almost all input logic above
        private void ResetInactivityTimer()
        {
            inactivityTimer = 0f;
        }
    }
}

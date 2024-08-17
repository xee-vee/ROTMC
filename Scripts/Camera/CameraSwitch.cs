using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class CameraSwitch : MonoBehaviour
    {
        #region Variables
        public GameObject topDownCamera;
        public GameObject thirdPersonCamera;
        public GameObject topDownPlayerLighting;
        public GameObject topDownEnemyLighting;
        PlayerLocomotion playerLocomotion;
        CameraHandler cameraHandler;
        InputHandler inputHandler;
        public bool isInTopDown;
        public static CameraSwitch instance;
        #endregion
        
        private void Awake()
        {
            inputHandler = FindObjectOfType<InputHandler>();
            cameraHandler = FindObjectOfType<CameraHandler>();
            playerLocomotion = FindObjectOfType<PlayerLocomotion>();
        }

        // Switch to top-down camera
        public void SwitchToTopDown()
        {
            isInTopDown = true;

            // Reset camera position back to initial position in order to maintain player movement controls
            cameraHandler.transform.position = new Vector3(0, 0, 0);
            cameraHandler.transform.rotation = Quaternion.Euler(0, 0, 0);

            // Activate top down camera and deactivate third person one
            topDownCamera.SetActive(true);
            thirdPersonCamera.SetActive(false);

            // Remove any lock-on logic
            cameraHandler.ClearLockOn();
            inputHandler.lockOnFlag = false;
            inputHandler.lockOnInput = false;

        }

        // Switch to third person
        public void SwitchToThirdPerson()
        {
            isInTopDown = false;
            
            // Activate third person camera, and disable top-down
            topDownCamera.SetActive(false);
            thirdPersonCamera.SetActive(true);

        }
    }
}

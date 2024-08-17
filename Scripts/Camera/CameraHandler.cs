using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace lxxj
{
    public class CameraHandler : MonoBehaviour
    {
        #region Variables
        InputHandler inputHandler;
        PlayerManager playerManager;
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        private Transform myTransform;
        private Vector3 cameraTransformPosition;
        private LayerMask ignoreLayers;
        private LayerMask environmentLayer;
        private Vector3 cameracameraMoveSpeed = Vector3.zero;
        CameraSwitch cameraSwitch;
        public static CameraHandler singleton;
        public float cameraViewSpeed = 0.1f;
        public float cameraMoveSpeed = 0.1f;
        public float cameraPivotMoveSpeed = 0.03f;
        private float targetPosition;
        private float defaultPosition;
        private float viewAngle;
        private float pivotAngle;
        public float minimumPivot = -35;
        public float maximumPivot = 35;
        public float cameraRaycastRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;
        public float lockedPivotPosition = 2.5f;
        public float unlockedPivotPosition = 1.65f;
        public CharacterManager currentLockOnTarget;
        List<CharacterManager> availableTargets = new List<CharacterManager>();
        public float maxLockOnDistance = 30f;
        public CharacterManager nearestLockOnTarget;
        public CharacterManager leftLockOnTarget;
        public CharacterManager rightLockOnTarget;
        #endregion

        /* Implementing a singleton pattern in Awake so all
        scripts are accessing the same version of this camera */
        private void Awake()
        {
            singleton = this;

            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;

            // Layers for the camera to ignore collision
            ignoreLayers = ~(1 << 2 | 1 << 9 | 1 << 10);

            targetTransform = FindObjectOfType<PlayerManager>().transform;
            inputHandler = FindObjectOfType<InputHandler>();
            playerManager = FindObjectOfType<PlayerManager>();
            cameraSwitch = FindObjectOfType<CameraSwitch>();
        }
        private void Start()
        {
            environmentLayer = LayerMask.NameToLayer("Environment");
        }

        // Keeping the camera following the player game object, also calling collision checker
        public void FollowPlayer(float delta)
        {
            Vector3 targetPosition = Vector3.SmoothDamp(myTransform.position, targetTransform.position, ref cameracameraMoveSpeed, delta / cameraMoveSpeed);

            myTransform.position = targetPosition;

            CameraCollide(delta);
        }

        // Handles camera rotation, checks if using lock-on ability
        public void RotateCamera(float delta, float mouseXInput, float mouseYInput)
        {
            if (cameraSwitch.isInTopDown) { return; }
            
            // If player is not using lock-on, rotate camera based on mouse movement
            if (!inputHandler.lockOnFlag && currentLockOnTarget == null)
            {
                viewAngle += mouseXInput * (cameraViewSpeed / delta);
                pivotAngle -= mouseYInput * (cameraPivotMoveSpeed / delta);
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

                Quaternion viewRotation = Quaternion.Euler(0f, viewAngle, 0f);
                myTransform.rotation = viewRotation;

                Quaternion pivotRotation = Quaternion.Euler(pivotAngle, 0f, 0f);
                cameraPivotTransform.localRotation = pivotRotation;

            }

            // Otherwise the camera always rotates to face the target
            else
            {
                Vector3 direction = (currentLockOnTarget.transform.position - transform.position).normalized;
                direction.y = 0f;

                transform.rotation = Quaternion.LookRotation(direction);

                direction = (currentLockOnTarget.transform.position - cameraPivotTransform.position).normalized;

                Quaternion pivotRotation = Quaternion.LookRotation(direction);
                Vector3 pivotEuler = pivotRotation.eulerAngles;
                pivotEuler.y = 0f;
                cameraPivotTransform.localEulerAngles = pivotEuler;

            }
        }

        // Making sure camera doesnt collide with specified layers and moving it if it does
        private void CameraCollide(float delta)
        {
            targetPosition = defaultPosition;

            RaycastHit hit;
            Vector3 direction = (cameraTransform.position - cameraPivotTransform.position).normalized;

            if (Physics.SphereCast(cameraPivotTransform.position, cameraRaycastRadius, direction, out hit, Mathf.Abs(targetPosition), ignoreLayers))
            {
                float distance = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPosition = -(distance - cameraCollisionOffset);
                targetPosition = Mathf.Max(targetPosition, -minimumCollisionOffset);
            }

            if (Mathf.Abs(targetPosition) < minimumCollisionOffset)
            {
                targetPosition = -minimumCollisionOffset;
            }

            cameraTransformPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, delta / 0.2f);
            cameraTransform.localPosition = cameraTransformPosition;


        }

        // Tracks enemies and allows for lock on based on their location
        public void LockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestLeftTargetDistance = -Mathf.Infinity;
            float shortestRightTargetDistance = Mathf.Infinity;

            Collider[] potentialTargets = Physics.OverlapSphere(targetTransform.position, 26);

            foreach (Collider collider in potentialTargets)
            {
                CharacterManager character = collider.GetComponent<CharacterManager>();

                if (character != null)
                {
                    Vector3 directionToTarget = character.transform.position - targetTransform.position;
                    float distanceToTarget = directionToTarget.magnitude;
                    float angleToTarget = Vector3.Angle(directionToTarget, cameraTransform.forward);
                    RaycastHit hit;

                    if (character.transform.root != targetTransform.root 
                        && angleToTarget > -50 && angleToTarget < 50 
                        && distanceToTarget <= maxLockOnDistance)
                    {
                        if (Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit))
                        {
                            Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position);

                            if (hit.transform.gameObject.layer != environmentLayer)
                            {
                                availableTargets.Add(character);
                            }
                        }
                    }
                }
            }

            foreach (CharacterManager target in availableTargets)
            {
                float distanceToTarget = Vector3.Distance(targetTransform.position, target.transform.position);

                if (distanceToTarget < shortestDistance)
                {
                    shortestDistance = distanceToTarget;
                    nearestLockOnTarget = target;
                }

                if (inputHandler.lockOnFlag)
                {
                    Vector3 relativePosition = inputHandler.transform.InverseTransformPoint(target.transform.position);
                    float leftTargetDistance = relativePosition.x;
                    float rightTargetDistance = relativePosition.x;

                    if (relativePosition.x <= 0.0f && leftTargetDistance > shortestLeftTargetDistance && target != currentLockOnTarget)
                    {
                        shortestLeftTargetDistance = leftTargetDistance;
                        leftLockOnTarget = target;
                    }
                    else if (relativePosition.x >= 0.0f && rightTargetDistance < shortestRightTargetDistance && target != currentLockOnTarget)
                    {
                        shortestRightTargetDistance = rightTargetDistance;
                        rightLockOnTarget = target;
                    }
                }
            }

        }

        // Clears all lock-on targets
        public void ClearLockOn()
        {
            availableTargets.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
        }

        // Adjust the camera height if using lock-on
        public void CHangeCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 targetPosition = currentLockOnTarget != null ? new Vector3(0, lockedPivotPosition) : new Vector3(0, unlockedPivotPosition);

            cameraPivotTransform.localPosition = Vector3.SmoothDamp(cameraPivotTransform.localPosition, targetPosition, ref velocity, Time.deltaTime);

        }
    }
}

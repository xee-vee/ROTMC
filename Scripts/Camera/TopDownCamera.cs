using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class TopDownCamera : MonoBehaviour
    {
        #region Variables
        public Transform FollowTarget;
        public Vector3 TargetOffset;
        public float MoveSpeed = 2f;
        public static TopDownCamera instance;
        private Transform _myTransform;
        #endregion

        /* Implementing a singleton pattern in Awake so all
        scripts are accessing the same version of this camera */
        private void Awake()
        {
            instance = this;
            _myTransform = transform;
        }

        // Setting the top-down camera viewpoint
        private void Start () 
        {
            _myTransform.rotation = Quaternion.Euler(0, 0, 0);
        }

        /* If follow target is set (in unity editor), 
        move camera towards this transform smoothly */
        private void LateUpdate () 
        {
            if (FollowTarget != null) 
            {
                _myTransform.position = Vector3.Lerp(_myTransform.position, 
                    FollowTarget.position + TargetOffset, MoveSpeed * Time.deltaTime);
            }
        }
    }
}

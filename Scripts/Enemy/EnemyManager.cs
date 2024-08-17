using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.AI;

namespace lxxj
{
    public class EnemyManager : CharacterManager
    {   
        #region Variables
        public Rigidbody enemyRigidbody;
        public float rotationSpeed = 15f;
        [SerializeField] public float enemyMoveSpeed = 0.45f;
        public float maxAttackRange = 4f;
        public NavMeshAgent navMeshAgent;
        public CharacterStats currentTarget;
        public State currentState;
        public bool isPerformingAction;
        EnemyLocomotionManager enemyLocomotionManager;
        EnemyAnimatorManager enemyAnimatorManager;
        EnemyStats enemyStats;

        [Header("AI Settings")]
        public float detectionRadius = 20f;

        // Enemy FoV widens based on these stats
        public float minimumDetectionAngle = -50f;
        public float maximumDetectionAngle = 50f;
        public float currentRecoveryTime = 0;
        #endregion
        
        private void Awake()
        {
            enemyLocomotionManager = GetComponent<EnemyLocomotionManager>();
            enemyAnimatorManager = GetComponent<EnemyAnimatorManager>();
            enemyStats = GetComponent<EnemyStats>();
            navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            enemyRigidbody = GetComponent<Rigidbody>();

        }
        
        // Ensuring enemy can't move when it loads
        private void Start()
        {
            enemyRigidbody.isKinematic = false;
        }

        private void Update()
        {
            RecoveryTimeLogic();
        }

        private void FixedUpdate()
        {
            StateMachineLogic();
        }

        // Run the tick method of the current state, if current state returns a new state then transition to it
        private void StateMachineLogic()
        {
            if (enemyStats.isDead) { return; }

            if (currentState != null)
            {
                State nextState = currentState.Tick(this, enemyStats, enemyAnimatorManager);

                if (nextState != null)
                {
                    SwitchToNextState(nextState);
                }
            }
        }

        // Logic for switching state
        private void SwitchToNextState(State state)
        {
            currentState = state;
        }

        // Count down recovery time every frame
        private void RecoveryTimeLogic()
        {
            if (currentRecoveryTime > 0)
            {
                currentRecoveryTime -= Time.deltaTime;
            }

            if (isPerformingAction)
            {
                if (currentRecoveryTime <= 0)
                {
                    isPerformingAction = false;
                }
            }
        }

        // Useful debugging Gizmos to show AI detection range & angle
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
            Vector3 fovLine1 = Quaternion.AngleAxis(maximumDetectionAngle, transform.up) * transform.forward * detectionRadius;
            Vector3 fovLine2 = Quaternion.AngleAxis(minimumDetectionAngle, transform.up) * transform.forward * detectionRadius;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, fovLine1);
            Gizmos.DrawRay(transform.position, fovLine2); 
        }
    }
}
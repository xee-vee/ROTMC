using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class PursueTargetState : State
    {
        public CombatStanceState combatStanceState;

        // Overridden Tick method defines specific logic for this state, returns another state
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            RotateToTarget(enemyManager);

            if (enemyManager.isPerformingAction) 
            { 
                enemyAnimatorManager.anim.SetFloat("Vertical", 0f, 0.1f, Time.deltaTime);
                return this; 
            }

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
            float distanceToTarget = targetDirection.magnitude;
            float angleToTarget = Vector3.Angle(targetDirection, enemyManager.transform.forward);

            // Update vertical movement if not in attack range
            if (distanceToTarget > enemyManager.maxAttackRange)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", enemyManager.enemyMoveSpeed, 0.1f, Time.deltaTime);
            }

            RotateToTarget(enemyManager);
            enemyManager.navMeshAgent.transform.localPosition = Vector3.zero;
            enemyManager.navMeshAgent.transform.localRotation = Quaternion.identity;

            // Move to combat stance if within attack range
            if (distanceToTarget <= enemyManager.maxAttackRange)
            {
                return combatStanceState;
            }

            // Otherwise stay in this state
            return this;

        }

        private void RotateToTarget(EnemyManager enemyManager)
        {
            // Rotate manually
            if (enemyManager.isPerformingAction)
            {
                Vector3 direction = enemyManager.currentTarget.transform.position - enemyManager.transform.position;
                direction.y = 0f;

                if (direction.sqrMagnitude == 0f)
                {
                    direction = enemyManager.transform.forward;
                }
                else
                {
                    direction.Normalize();
                }

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed * Time.deltaTime);
            }

            // Rotate with pathfinding
            else
            {
                Vector3 relativeDirection = transform.InverseTransformDirection(enemyManager.navMeshAgent.desiredVelocity);
                Vector3 targetVelocity = enemyManager.enemyRigidbody.velocity;

                enemyManager.navMeshAgent.enabled = true;
                enemyManager.navMeshAgent.SetDestination(enemyManager.currentTarget.transform.position);
                enemyManager.enemyRigidbody.velocity = targetVelocity;

                Quaternion targetRotation = enemyManager.navMeshAgent.transform.rotation;
                enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyManager.rotationSpeed * Time.deltaTime);
            }

        }
    }
}

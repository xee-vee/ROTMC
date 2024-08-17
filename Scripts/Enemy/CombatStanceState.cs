using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class CombatStanceState : State
    {
        public AttackState attackState;
        public PursueTargetState pursueTargetState;

        // Overridden Tick method defines specific logic for this state, returns another state
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            float distanceToTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);

            // Stop vertical movement if performing an action
            if (enemyManager.isPerformingAction)
            {
                enemyAnimatorManager.anim.SetFloat("Vertical", 0f, 0.1f, Time.deltaTime);
            }

            // If ready and within attack range, transition to attack state
            if (enemyManager.currentRecoveryTime <= 0f && distanceToTarget <= enemyManager.maxAttackRange)
            {
                return attackState;
            }
            // If target is out of attack range, transition to pursue state
            else if (distanceToTarget > enemyManager.maxAttackRange)
            {
                return pursueTargetState;
            }
            // Otherwise, remain in the current state
            return this;
        }
    }
}
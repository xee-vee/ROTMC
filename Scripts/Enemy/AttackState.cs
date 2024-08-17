using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class AttackState : State
    {
        public CombatStanceState combatStanceState;
        public EnemyAttackAction[] enemyAttacks;
        public EnemyAttackAction currentAttack;

        // Overridden Tick method defines specific logic for this state, returns another state
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {

            Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            float distanceToTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, enemyManager.transform.position);
            float angleToTarget = Vector3.Angle(targetDirection, transform.forward);

            if (enemyManager.isPerformingAction)
            {
                return combatStanceState;
            }

            if (currentAttack != null)
            {
                if (distanceToTarget < currentAttack.minDistanceNeededToAttack)
                {
                    return this;
                }
                else if (distanceToTarget < currentAttack.maxDistanceNeededToAttack)
                {
                    if (angleToTarget <= currentAttack.maxAttackAngle && angleToTarget >= currentAttack.minAttackAngle)
                    {
                        if (enemyManager.currentRecoveryTime <= 0 && !enemyManager.isPerformingAction)
                        {
                            enemyAnimatorManager.anim.SetFloat("Vertical", 1, 0.1f, Time.deltaTime);
                            enemyAnimatorManager.anim.SetFloat("Horizontal", 1, 0.1f, Time.deltaTime);
                            enemyAnimatorManager.PlayTargetAnimation(currentAttack.actionAnimation, true);
                            enemyManager.isPerformingAction = true;
                            enemyManager.currentRecoveryTime = currentAttack.recoveryTime;
                            currentAttack = null;

                            return combatStanceState;
                        }
                    }
                }
            }
            else
            {
                NewAttack(enemyManager);
            }

            return combatStanceState; 

        }

        // Selects an attack from a list of scriptable objects created in Unity
        private void NewAttack(EnemyManager enemyManager)
        {
            Vector3 targetDirection = enemyManager.currentTarget.transform.position - transform.position;
            float viewableAngle = Vector3.Angle(targetDirection, transform.forward);
            float distanceToTarget = Vector3.Distance(enemyManager.currentTarget.transform.position, transform.position);

            int totalScore = 0;

            // Calculate the total score of valid attacks
            foreach (var attack in enemyAttacks)
            {
                if (distanceToTarget <= attack.maxDistanceNeededToAttack && distanceToTarget >= attack.minDistanceNeededToAttack)
                {
                    if (viewableAngle <= attack.maxAttackAngle && viewableAngle >= attack.minAttackAngle)
                    {
                        totalScore += attack.attackScore;
                    }
                }
            }

            // Select an attack based on a random value within the total score
            int randomValue = Random.Range(0, totalScore);
            int cumulativeScore = 0;

            foreach (var attack in enemyAttacks)
            {
                if (distanceToTarget <= attack.maxDistanceNeededToAttack && distanceToTarget >= attack.minDistanceNeededToAttack)
                {
                    if (viewableAngle <= attack.maxAttackAngle && viewableAngle >= attack.minAttackAngle)
                    {
                        if (currentAttack != null)
                        {
                            return;
                        }

                        cumulativeScore += attack.attackScore;

                        if (cumulativeScore > randomValue)
                        {
                            currentAttack = attack;
                        }
                    }
                }
            }
        }
    
    }
}

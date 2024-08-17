using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class IdleState : State
    {
        public PursueTargetState pursueTargetState;
        public LayerMask detectionLayer;
        // Overridden Tick method defines specific logic for this state, returns another state
        public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager)
        {
            /* Look for a target, switch to pursue target state if target is found,
            if no target return this and continue looking for target */
            #region Enemy Target Detection

            enemyManager.currentTarget = null;
            Collider[] detectedColliders = Physics.OverlapSphere(transform.position, enemyManager.detectionRadius, detectionLayer);

            foreach (Collider detectedCollider in detectedColliders)
            {
                CharacterStats detectedCharacterStats = detectedCollider.GetComponent<CharacterStats>();

                if (detectedCharacterStats != null)
                {
                    Vector3 directionToTarget = detectedCharacterStats.transform.position - transform.position;
                    float angleToTarget = Vector3.Angle(directionToTarget, transform.forward);

                    if (angleToTarget > enemyManager.minimumDetectionAngle && angleToTarget < enemyManager.maximumDetectionAngle)
                    {
                        enemyManager.currentTarget = detectedCharacterStats;
                        break;
                    }
                }           
            }

            #endregion

            #region Switch State

            if (enemyManager.currentTarget != null)
            {
                return pursueTargetState;
            }
            else
            {
                return this;
            }
            #endregion
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class EnemyAnimatorManager : AnimatorManager
    {
        EnemyManager enemyManager;

        private void Awake()
        {
            anim.GetComponent<Animator>();
            enemyManager = GetComponent<EnemyManager>();
        }

        // Keeps enemy movement smooth to the animation
        private void OnAnimatorMove()
        {
            // Check for pause
            float delta = Time.deltaTime;
            if (delta == 0) { return; } 

            enemyManager.enemyRigidbody.drag = 0;
            Vector3 deltaPosition = anim.deltaPosition;
            deltaPosition.y = 0;
            Vector3 velocity = deltaPosition / delta;
            enemyManager.enemyRigidbody.velocity = velocity / 1.5f;
        }

    }
}

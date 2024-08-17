using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public abstract class State : MonoBehaviour
    {
        /* Base class, the Tick method is overridden on every state that inherits from
        this base state, and the Tick of the current state is called in the EnemyManager Update method */
        public abstract State Tick(EnemyManager enemyManager, EnemyStats enemyStats, EnemyAnimatorManager enemyAnimatorManager);
    }
}

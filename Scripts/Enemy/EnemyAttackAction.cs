using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    // Easy attack action creation in Unity editor
    [CreateAssetMenu(menuName = "AI/Enemy Actions/Attack Action")]

    public class EnemyAttackAction : EnemyActions
    {
        // Variables for each attack action, this is scalable
        public int attackScore = 3;
        public float recoveryTime = 1.5f;

        public float maxAttackAngle = 90;
        public float minAttackAngle = -90;

        public float minDistanceNeededToAttack = 0;
        public float maxDistanceNeededToAttack = 6;
    }
}

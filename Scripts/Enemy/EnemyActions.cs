using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class EnemyActions : ScriptableObject
    {
        /* Scriptable object is used to store data independent of a gameobject in Unity,
        allows for creation of multiple different enemy actions, currently only used for attacks */
        public string actionAnimation;
    }
}

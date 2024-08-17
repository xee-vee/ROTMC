using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item
    {
        // Scriptable object for weapons, able to create them in Unity project menu
        
        public GameObject modelPrefab;
        public bool isUnarmed;

        [Header("Two Handed Attack Animations")]
        public string Two_Handed_Light_Attack_1;
        public string Two_Handed_Light_Attack_2;
        public string Two_Handed_Heavy_Attack_1;

        [Header("Stamina Costs")]
        public int baseStamina;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;

    }
}

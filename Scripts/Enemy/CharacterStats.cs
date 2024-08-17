using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class CharacterStats : MonoBehaviour
    {
        // Abstract class holidng common stats for multiple characters
        public bool isDead = false;
        public int healthLevel = 10;
        public int maxHealth = 999;
        public int currentHealth;
        public float maxStamina;
        public float currentStamina;
        public int staminaLevel = 10;

    }
}

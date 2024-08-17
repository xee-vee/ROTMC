using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class EnemyStats : CharacterStats
    {
        #region Variables
        EnemyManager enemyManager;
        Animator animator;
        public Vector3 startingPosition;
        CameraHandler cameraHandler;
        PlayerStats playerStats;
        InputHandler inputHandler;
        private int hitCounter = 0;
        #endregion

        private void Awake()
        {
            inputHandler = FindObjectOfType<InputHandler>();
            playerStats = FindObjectOfType<PlayerStats>();
            startingPosition = transform.position;
            enemyManager = GetComponent<EnemyManager>();
            animator = GetComponent<Animator>();
            cameraHandler = FindObjectOfType<CameraHandler>();
        }

        void Start()
        {
            maxHealth = SetMaxHealth();
            currentHealth = maxHealth;
        }

        // Setting max health relevant to health level from character stats base class
        private int SetMaxHealth()
        {
            maxHealth = healthLevel * 10;
            return maxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (isDead) { return; }

            // Take damage
            currentHealth -= damage;

            // Increment the hit counter    
            hitCounter++; 

            // Play damage animation only on every second hit
            if (hitCounter % 2 == 0)
            {
                animator.Play("Damage_01");
            }

            // Enemy death
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isDead = true;
                animator.Play("Death");

                // Heals player when enemy dies
                playerStats.Heal(25);
                    
                // Logic for stopping lock on to target when target dead
                cameraHandler.ClearLockOn();
                inputHandler.lockOnFlag = false;
                inputHandler.lockOnInput = false;

                // Logic for gaining souls would be added here
            }
        }  
    }
}

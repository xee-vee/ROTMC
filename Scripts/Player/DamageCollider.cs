using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace lxxj
{
    public class DamageCollider : MonoBehaviour
    {
        Collider damageCollider;
        public int currentWeaponDamage = 25;
        public int heavyAttackMultiplier = 2;   

        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.gameObject.SetActive(true);
            damageCollider.isTrigger = true;
            damageCollider.enabled = false;
        }

        // Used in weapon manager for opening and closing damage colliders
        #region Open & close damage collider
        public void EnableDamageCollider()
        {
            damageCollider.enabled = true;
        }

        public void DisableDamageCollider()
        {
            damageCollider.enabled = false;
        }
        #endregion

        // Handles logic for what happens when the collider triggers with game objects tag
        private void OnTriggerEnter(Collider collision)
        {
            PlayerAttacker playerAttacker = FindObjectOfType<PlayerAttacker>();

            if (playerAttacker == null) return; 

            int damage = currentWeaponDamage;
            
            // Specifies if the attack is a heavy attack (x2 damage)
            if (playerAttacker.isHeavyAttack)
            {
                damage *= heavyAttackMultiplier;
                playerAttacker.isHeavyAttack = false; 
            }

            if (collision.tag == "Player")
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();

                if (playerStats != null)
                {
                    playerStats.TakeDamage(damage);
                }
            }
            else if (collision.tag == "Enemy")
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

                if (enemyStats != null)
                {
                    enemyStats.TakeDamage(damage);
                }
            }
        }
    }
}

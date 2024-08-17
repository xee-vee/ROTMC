using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class PlayerAttacker : MonoBehaviour
    {
        #region Variables
        AnimatorHandler animatorHandler;
        InputHandler inputHandler;
        WeaponSlotManager weaponSlotManager;
        PlayerStats playerStats;
        public string lastAttack;
        public bool isHeavyAttack;
        #endregion
        
        private void Awake()
        {
            animatorHandler = GetComponent<AnimatorHandler>();
            weaponSlotManager = GetComponent<WeaponSlotManager>();
            inputHandler = GetComponent<InputHandler>();
            playerStats = GetComponent<PlayerStats>();
        }

        // Handles logic for enabling attack combo
        public void WeaponCombo(WeaponItem weapon)
        {
            if (playerStats.currentStamina > 0 && inputHandler.comboFlag)
            {
                animatorHandler.anim.SetBool("canDoCombo", false);

                if (lastAttack == weapon.Two_Handed_Light_Attack_1)
                {
                    animatorHandler.PlayTargetAnimation(weapon.Two_Handed_Light_Attack_2, true);
                }
            }
        }

        
        // Logic for light attack
        public void LightAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0) { return; }
            
            isHeavyAttack = false;
            weaponSlotManager.attackingWeapon = weapon;
            animatorHandler.PlayTargetAnimation(weapon.Two_Handed_Light_Attack_1, true);
            lastAttack = weapon.Two_Handed_Light_Attack_1;
        }
        
        // Logic for heavy attack
        public void HeavyAttack(WeaponItem weapon)
        {
            if (playerStats.currentStamina <= 0) { return; }

            isHeavyAttack = true;
            weaponSlotManager.attackingWeapon = weapon;
            animatorHandler.PlayTargetAnimation(weapon.Two_Handed_Heavy_Attack_1, true);
            lastAttack = weapon.Two_Handed_Heavy_Attack_1;

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{

    public class WeaponSlotManager : MonoBehaviour
    {
        #region Variables
        WeaponHolderSlot leftHandSlot;
        WeaponHolderSlot rightHandSlot;
        DamageCollider leftHandDamageCollider;
        DamageCollider rightHandDamageCollider;
        public WeaponItem attackingWeapon;
        PlayerStats playerStats;
        #endregion

        // Finding weapon slots in the player model
        private void Awake()
        {
            playerStats = GetComponentInParent<PlayerStats>();

            foreach (var weaponSlot in GetComponentsInChildren<WeaponHolderSlot>())
            {
                if (weaponSlot.isLeftHandSlot)
                {
                    leftHandSlot = weaponSlot;
                }
                else if (weaponSlot.isRightHandSlot)
                {
                    rightHandSlot = weaponSlot;
                }

                // Exit early if both slots are assigned
                if (leftHandSlot != null && rightHandSlot != null)
                {
                    break;
                }
            }
        }

        // Spawning weapon in right hand (left not used once again, could be added later)
        public void SpawnWeaponSlot(WeaponItem weaponItem, bool isLeft)
        {
            if (isLeft)
            {
                leftHandSlot.currentWeapon = weaponItem;
                leftHandSlot.SpawnWeaponModel(weaponItem);
                SpawnLeftWeaponDamageCollider();
            }
            else
            {
                rightHandSlot.currentWeapon = weaponItem;
                rightHandSlot.SpawnWeaponModel(weaponItem);
                SpawnRightWeaponDamageCollider();
            }
        }

        // Enableing and closing damage colliders for mid animation damage output
        #region Handle Weapon Damage Collider

        private void SpawnLeftWeaponDamageCollider()
        {
            // Only using 2H weapon so no need for left hand slot

            // leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }

        private void SpawnRightWeaponDamageCollider()
        {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
        }

        public void EnableRightDamageCollider()
        {
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void EnableLeftDamageCollider()
        {
            leftHandDamageCollider.EnableDamageCollider();
        }

        public void DisableRightHandDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }

        public void DisableLeftHandDamageCollider()
        {
            leftHandDamageCollider.DisableDamageCollider();
        }

        #endregion

        // Drain stamina based on weapon stats from item
        #region Handle Weapon Stamina Drain

        public void DrainStaminaLightAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.lightAttackMultiplier));
        }

        public void DrainStaminaHeavyAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStamina * attackingWeapon.heavyAttackMultiplier));
        }

        #endregion

    }
}

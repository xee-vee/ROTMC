using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace lxxj
{
    public class EnemyWeaponSlotManager : MonoBehaviour
    {
        #region Variables
        public WeaponItem rightHandWeapon;
        public WeaponItem leftHandWeapon;
        WeaponHolderSlot rightHandSlot;        
        WeaponHolderSlot leftHandSlot;
        DamageCollider leftHandDamageCollider;
        DamageCollider rightHandDamageCollider;
        #endregion

        // Finds weapon holder slots for each hand
        private void Awake()
        {
            foreach (var weaponSlot in GetComponentsInChildren<WeaponHolderSlot>())
            {
                if (weaponSlot.isLeftHandSlot && leftHandSlot == null)
                {
                    leftHandSlot = weaponSlot;
                }
                else if (weaponSlot.isRightHandSlot && rightHandSlot == null)
                {
                    rightHandSlot = weaponSlot;
                }

                // Break early if both slots are found
                if (leftHandSlot != null && rightHandSlot != null)
                {
                    break;
                }
            }
        }
        private void Start()
        {
            SpawnBothWeapons();
        }

        // Loads the weapon into slot based on isLeft bool
        public void SpawnWeapon(WeaponItem weapon, bool isLeft)
        {
            if (isLeft)
            {
                // leftHandSlot.LoadWeaponModel(weapon);
                // LoadWeaponDamageCollider(true);      
            }
            else
            {
                rightHandSlot.currentWeapon = weapon;
                rightHandSlot.SpawnWeaponModel(weapon);
                SpawnWeaponDamageCollider(false);      
            }
        }

        // Loads weapon into right hand, logic can be added for left too
        public void SpawnBothWeapons()
        {
            if (rightHandWeapon != null)
            {
                SpawnWeapon(rightHandWeapon, false);
            }
            // If you wanted to add logic for the other hand it would be below

            // if (leftHandWeapon != null)
            // {
            //     LoadWeaponOnSlot(leftHandWeapon,true);
            // }
        }
       
        // Loads the damage collider (box collider in this case) for both weapon slots
        public void SpawnWeaponDamageCollider(bool isLeft)
        {
            if (isLeft)
            {
                leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();  
            }
            else
            {
                rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();  

            }
        }

        // Open or close damage collider functions (can be used in unity animation events)
        #region Open Damage Colliders
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

        // Can add stam functionality for enemy, for now empty to stop console errors
        #region Enemy Stamina Costs
        public void DrainStaminaLightAttack()
        {
            
        }
        public void DrainStaminaHeavyAttack()
        {
            
        }
        #endregion
        
        // Can potentially add combo functionality here
        #region Enemy Combo Management
        public void EnableCombo()
        {
            // anim.SetBool("canDoCombo", true);
        }

        public void DisableCombo()
        {
            // anim.SetBool("canDoCombo", false);
        }
        #endregion

        // Can potentially add more unique rotation logic here, for now it is handled in state machine
        #region Enemy Rotation
        public void CanRotate()
        {

        }

        public void StopRotation()
        {
            
        }
        #endregion
    }
}

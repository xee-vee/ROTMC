using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class PlayerInventory : MonoBehaviour
    {
        // Inventory system not in place, placeholder for future development
        WeaponSlotManager weaponSlotManager;
        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        private void Awake()
        {
            weaponSlotManager = GetComponent<WeaponSlotManager>();
        }

        private void Start()
        {
            weaponSlotManager.SpawnWeaponSlot(rightWeapon, false);
            weaponSlotManager.SpawnWeaponSlot(leftWeapon, true);
        }
    }
}

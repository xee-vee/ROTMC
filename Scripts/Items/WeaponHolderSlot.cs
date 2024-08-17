using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{

    public class WeaponHolderSlot : MonoBehaviour
    {
        // Class not used in full, some methods are placeholder for inventory system if developed in future

        #region Variables
        public Transform parentOverride;
        public bool isLeftHandSlot;
        public bool isRightHandSlot;
        public WeaponItem currentWeapon;
        public GameObject currentWeaponModel;
        #endregion

        // Deactivates weapon model
        public void DeactivateWeapon()
        {
            if (currentWeaponModel != null)
            {
                currentWeaponModel.SetActive(false);
            }
        }

        // Unloads and deleted weapon in slot
        public void DeactivateAndDestroyWeapon()
        {
            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }
        
        // Loads weapon (used)
        public void SpawnWeaponModel(WeaponItem weaponItem)
        {
            DeactivateAndDestroyWeapon();

            if (weaponItem == null)
            {
                DeactivateWeapon();
                return;
            }

            GameObject model = Instantiate(weaponItem.modelPrefab);
            if (model != null)
            {
                Transform parentTransform = parentOverride != null ? parentOverride : transform;

                model.transform.SetParent(parentTransform);
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;

                currentWeaponModel = model;
            }

        }
    }

}

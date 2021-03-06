using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    // this handles what the player actually has in their hand
    public class WeaponHolderSlot : MonoBehaviour
    {
        public Transform parentOverride;
        public bool isLeftHandSlot;
        public bool isRightHandSlot;

        public GameObject currentWeaponModel;

        // removes the weapon model prefab from the player's hand by disabling it
        public void UnloadWeapon()
        {
            if (currentWeaponModel != null)
            {
                currentWeaponModel.SetActive(false);
            }
        }

        // removes the weapon model prefab from the player's hand by unloading it
        public void UnloadWeaponAndDestroy()
        {
            if (currentWeaponModel != null)
            {
                Destroy(currentWeaponModel);
            }
        }

        // puts the weapon model prefab into the player's hand
        public void LoadWeaponModel(WeaponItem weaponItem)
        {
            UnloadWeaponAndDestroy();
            
            if (weaponItem == null)
            {
                UnloadWeapon();
                return;
            }
            
            GameObject model = Instantiate(weaponItem.prefab) as GameObject;
            if (model != null)
            {
                if (parentOverride != null)
                {
                    model.transform.parent = parentOverride;
                }
                else
                {
                    model.transform.parent = transform;
                }
                
                model.transform.localPosition = Vector3.zero;
                model.transform.localRotation = Quaternion.identity;
                model.transform.localScale = Vector3.one;
            }

            currentWeaponModel = model;
        }
    }
}
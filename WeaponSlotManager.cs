using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    public class WeaponSlotManager : MonoBehaviour
    {
        private WeaponHolderSlot leftHandSlot;
        private WeaponHolderSlot rightHandSlot;

        private void Awake()
        {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach (var weaponHolder in weaponHolderSlots)
            {
                if (weaponHolder.isLeftHandSlot)
                {
                    leftHandSlot = weaponHolder;
                }
                else if (weaponHolder.isRightHandSlot)
                {
                    rightHandSlot = weaponHolder;
                }
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            if (isLeft)
            {
                leftHandSlot.LoadWeaponModel(weaponItem);
            }
            else
            {
                rightHandSlot.LoadWeaponModel(weaponItem);
            }
        }
    }
}
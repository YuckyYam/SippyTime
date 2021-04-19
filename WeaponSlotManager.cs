using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    // handles what the player has in their hands and where those things are placed
    public class WeaponSlotManager : MonoBehaviour
    {
        private WeaponHolderSlot leftHandSlot;
        private WeaponHolderSlot rightHandSlot;

        // finds weapons that the player might have and stores their existence in this script
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

        // puts the player's weapon in the equipped slot and in the player model's hands
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
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    // this is an object that handles all of the item objects that the player will have
    public class PlayerInventory : MonoBehaviour
    {
        private WeaponSlotManager weaponSlotManager;
        
        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon;

        // gets the weapon slot manager, which handles what the player actually has in their hands
        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }

        // puts the predetermines weapons in the player's hand, if there are any
        private void Start()
        {
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }
    }
}
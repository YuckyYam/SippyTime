using System;
using System.Collections;
using System.Collections.Generic;
using Controls;
using UnityEditor;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Controls
{
    // This object is connected to the player and checks to see if there are objects nearby for the player to interact
    // with, rather than every object having its own script to check for the player
    public class CheckSurroundings : MonoBehaviour
    {
        public GameObject openedChest;

        private PlayerStats playerStats;
        private PlayerInventory playerInventory;
        private WeaponSlotManager weaponSlotManager;
        private PlayerManager playerManager;
        private Transform playerTransform;
        public float chestRadius = 4f;
        public GameObject currentChest;

        public float sippyChance = .5f;
        public int maxSippies = 4;
        public int minSippies = 1;

        public WeaponItem rightHandWeapons;
        public WeaponItem leftHandWeapons;

        // instantiates properties to self
        private void Start()
        {
            playerTransform = transform;
            // checking if an object is null is expensive for the CPU, so instead we check if it is the player instead
            currentChest = gameObject;
        }

        // instantiates properties after the other components have been instantiated
        private void Awake()
        {
            playerStats = GetComponent<PlayerStats>();
            playerManager = GetComponent<PlayerManager>();
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
            playerInventory = GetComponent<PlayerInventory>();
        }

        // finds every object in a chestRadius distance from the player and determines if each is a player
        public void Check()
        {
            bool foundChest = false;
            Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, chestRadius);
            foreach (Collider collision in hitColliders)
            {
                if (collision.gameObject.CompareTag("Chest"))
                {
                    foundChest = true;
                    if (currentChest == gameObject)
                    {
                        currentChest = collision.gameObject;
                        playerManager.canOpenChest = true;
                        // TODO add gui element to tell the player that they can open the chest
                    }
                }
            }

            if (!foundChest)
            {
                currentChest = gameObject;
                playerManager.canOpenChest = false;
                // TODO remove gui element to tell the player that they can no longer open the chest
            }
        }

        // gets called when the player presses the interaction button next to a chest
        public void OpenChest()
        {
            // tells the input handler to not run this method until the player finds another chest
            playerManager.canOpenChest = false;

            bool rewardIsSippies = (sippyChance >= Random.Range(0f,1f));
            if (rewardIsSippies)
            {
                int sippyNum = Random.Range(minSippies, maxSippies);
                Debug.Log("the number of sippies you got was " + sippyNum);
                // gives the sippies to the gui code
                playerStats.AddSippies(sippyNum);
            }
            else
            {
                bool rewardIsSword = (1f >= Random.Range(0f,1f)); // TODO implement shields and blocking then change this to (.5f >= ...)
                if (rewardIsSword)
                {
                    Debug.Log("you got a sword");
                    // gives sword to player if they do not have one. upgrades it if they do
                    if (playerInventory.rightWeapon == null) // TODO change to check if it has the unarmed weapon
                    {
                        weaponSlotManager.LoadWeaponOnSlot(rightHandWeapons, false);
                    }
                    else
                    {
                        // TODO upgrade held sword
                    }
                }
                else
                {
                    Debug.Log("you got a shield");
                    // gives shield to player if they do not have one. upgrades it if they do
                    if (playerInventory.leftWeapon == null) // TODO change to check if it has the unarmed weapon
                    {
                        weaponSlotManager.LoadWeaponOnSlot(leftHandWeapons, true);
                    }
                    else
                    {
                        // TODO upgrade held shield
                    }
                }
            }

            // replaces chest with open chest that doesn't have the chest tag
            Vector3 newChestPosition = currentChest.transform.position;
            Quaternion newChestRotation = currentChest.transform.rotation;
            Destroy(currentChest);
            Instantiate(openedChest, newChestPosition, newChestRotation);

            // resets currentChest
            currentChest = gameObject;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    // defines what an item is so that it can go in the inventory of the player
    public class Item : ScriptableObject
    {
        [Header("Item Info")] public Sprite itemIcon;
        public string itemName;
        public bool isShield;
    }
}
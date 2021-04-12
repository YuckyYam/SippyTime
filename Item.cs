using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    public class Item : ScriptableObject
    {
        [Header("Item Info")] public Sprite itemIcon;
        public string itemName;
        public bool isShield;
    }
}
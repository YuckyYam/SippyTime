using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    // creates a weapon class that can be used to more easily add weapons to Unity
    [CreateAssetMenu(menuName = "Item/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject prefab;
        public bool isUnarmed;
    }
}
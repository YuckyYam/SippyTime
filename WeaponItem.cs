using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controls
{
    [CreateAssetMenu(menuName = "Item/Weapon Item")]
    public class WeaponItem : Item
    {
        public GameObject prefab;
        public bool isUnarmed;
    }
}
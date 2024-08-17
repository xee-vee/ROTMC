using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace lxxj
{
    public class Item : ScriptableObject
    {
        // Scriptable object used for multiple items

        [Header("Item Information")]
        public Sprite itemIcon;
        public string itemName;
    }

}


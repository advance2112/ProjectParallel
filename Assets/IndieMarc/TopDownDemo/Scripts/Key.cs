using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Key script
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.TopDown
{

    public class Key : MonoBehaviour
    {

        [FormerlySerializedAs("key_index")] public int keyIndex = 0;
        [FormerlySerializedAs("key_value")] public int keyValue = 1;

        private string uniqueID;
        private CarryItem carryItem;

        void Start()
        {
            carryItem = GetComponent<CarryItem>();
            carryItem.onTake += OnTake;
            carryItem.onDrop += OnDrop;
        }

        private void OnTake(GameObject triggerer)
        {
            
        }

        private void OnDrop(GameObject triggerer)
        {
            
        }

        public bool TryOpenDoor(GameObject door)
        {
            if (door.GetComponent<Door>() && door.GetComponent<Door>().CanKeyUnlock(this) && !door.GetComponent<Door>().IsOpened())
            {
                door.GetComponent<Door>().UnlockWithKey(keyValue);
                Destroy(gameObject);
                return true;
            }
            return false;
        }
    }

}
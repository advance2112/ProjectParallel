using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace IndieMarc.TopDown
{
    public class FixOffset : MonoBehaviour
    {
        [FormerlySerializedAs("order_min")] public int orderMin = 0;
        [FormerlySerializedAs("order_max")] public int orderMax = 0;

        [FormerlySerializedAs("z_offset")]
        [Header("Use 'Tools/Fix Z Offsets'")]
        [Tooltip("Use Tools/Fix Z Offsets  to offset all the assets with this script in the scene. This will avoid Z fighting for rendering and lighting.")]
        public float zOffset = 0f;

        void Awake()
        {
            if (GetComponent<SpriteRenderer>())
                GetComponent<SpriteRenderer>().sortingOrder = Random.Range(orderMin, orderMax + 1);
            enabled = false; //Just for editor, don't run in-game
        }

    }

}
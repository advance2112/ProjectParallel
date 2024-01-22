using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Allow using Sorting Layer and Sorting Order on 3D MeshRenderers
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.TopDown
{

    public class Sprite3D : MonoBehaviour
    {

        [FormerlySerializedAs("sort_layer")] public string sortLayer = "Default";
        [FormerlySerializedAs("sort_order")] public int sortOrder = 0;
        [FormerlySerializedAs("z_offset")] public float zOffset = 0f;
        [FormerlySerializedAs("fix_offset")] public bool fixOffset = false;

        void Awake()
        {
            GetComponent<MeshRenderer>().sortingLayerID = SortingLayer.NameToID(sortLayer);
            GetComponent<MeshRenderer>().sortingOrder = sortOrder;
            enabled = false;
        }
    }

}
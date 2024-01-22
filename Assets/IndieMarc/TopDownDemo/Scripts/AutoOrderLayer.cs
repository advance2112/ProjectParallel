using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// This script automatically updates the sorting order and X rotation, so that it faces the camera and sort based on the Y value
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>
/// 
namespace IndieMarc.TopDown
{
    public class AutoOrderLayer : MonoBehaviour
    {
        [FormerlySerializedAs("auto_sort")] [Header("Auto Order")]
        public bool autoSort = true;
        public float offset;
        [FormerlySerializedAs("sort_refresh_rate")] public float sortRefreshRate = 0.1f;

        [FormerlySerializedAs("auto_rotate")] [Header("Auto Rotate")]
        public bool autoRotate = true;
        [FormerlySerializedAs("rotate_offset")] public float rotateOffset;

        private SpriteRenderer render;
        private MeshRenderer meshRender;
        private Renderer particleRender;
        private float timer = 0f;

        private static List<AutoOrderLayer> orderList = new List<AutoOrderLayer>();
        private List<AutoOrderLayerChild> childList = new List<AutoOrderLayerChild>();

        private void Awake()
        {
            if (!orderList.Contains(this))
            {
                orderList.Add(this);
            }

            render = GetComponent<SpriteRenderer>();
            meshRender = GetComponent<MeshRenderer>();
            timer = Random.Range(0f, 0.2f) + 1f;

            if (GetComponent<MeshRenderer>())
                GetComponent<MeshRenderer>().sortingLayerName = "Perspective";

            ParticleSystem particle = GetComponent<ParticleSystem>();
            if (particle) particleRender = particle.GetComponent<Renderer>();

        }

        private void Start()
        {
            RefreshRender();

            if (gameObject.isStatic)
                enabled = false; //No need to update static
        }

        private void OnDestroy()
        {
            orderList.Remove(this);
        }

        void Update()
        {
            if (autoSort)
            {
                timer += Time.deltaTime;
                if (timer < sortRefreshRate)
                    return;
                timer -= sortRefreshRate;

                RefreshSort();
            }
        }

        public int GetSortOrder()
        {
            return Mathf.RoundToInt((transform.position.y + offset) * 100f) * -1;
        }

        public float GetRotateZOffset()
        {
            return -rotateOffset * transform.lossyScale.y;
        }

        public void AddChild(AutoOrderLayerChild child)
        {
            childList.Add(child);
        }

        public void RefreshSort()
        {
            int sortingOrder = GetSortOrder();

            if (render != null && render.sortingOrder != sortingOrder)
                render.sortingOrder = sortingOrder;
            if (meshRender != null && meshRender.sortingOrder != sortingOrder)
                meshRender.sortingOrder = sortingOrder;
            if (particleRender != null && particleRender.sortingOrder != sortingOrder)
                particleRender.sortingOrder = sortingOrder;

            //update childd NOW
            foreach (AutoOrderLayerChild child in childList)
            {
                if (child != null)
                    child.SetOrder(sortingOrder);
            }
        }

        public void RefreshAutoRotate()
        {
            Camera cam = FollowCamera.GetCamera();
            float currentAngle = transform.rotation.eulerAngles.z;
            if (!cam.orthographic)
            {
                //Projection
                transform.rotation = Quaternion.Euler(cam.transform.rotation.eulerAngles.x, 0f, currentAngle);
                transform.position = new Vector3(transform.position.x, transform.position.y, GetRotateZOffset());
            }
            else
            {
                //Ortho
                transform.rotation = Quaternion.Euler(0f, 0f, currentAngle);
                transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
            }
        }

        public void RefreshRender()
        {
            //Rotation
            if (autoRotate)
            {
                RefreshAutoRotate();
            }

            if (autoSort)
            {
                RefreshSort();
            }

            //Refresh child
            foreach (AutoOrderLayerChild child in childList)
            {
                child.RefreshRender();
            }
        }

        public static void RefreshRenderAll()
        {
            foreach (AutoOrderLayer item in orderList)
            {
                item.RefreshRender();
            }
        }
    }

}
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
    public class AutoOrderLayerChild : MonoBehaviour
    {
        [Header("Auto Order Child")]
        public AutoOrderLayer parent;
        [FormerlySerializedAs("order_offset")] public int orderOffset;

        [FormerlySerializedAs("auto_rotate")] [Header("Auto Rotate")]
        public bool autoRotate = false;
        [FormerlySerializedAs("rotate_offset")] public float rotateOffset;

        private SpriteRenderer render;
        private MeshRenderer meshRender;
        private Renderer particleRender;
        private Canvas canvas;
        private Vector3 startRot;

        void Awake()
        {
            render = GetComponent<SpriteRenderer>();
            meshRender = GetComponent<MeshRenderer>();
            canvas = GetComponent<Canvas>();
            startRot = transform.rotation.eulerAngles;
            parent.AddChild(this);

            if (GetComponent<MeshRenderer>())
                GetComponent<MeshRenderer>().sortingLayerName = "Perspective";

            ParticleSystem particle = GetComponent<ParticleSystem>();
            if (particle) particleRender = particle.GetComponent<Renderer>();
        }

        private void Start()
        {
            RefreshRender();
        }

        public void SetOrder(int order)
        {
            if (render != null)
                render.sortingOrder = order + orderOffset;
            if (meshRender != null)
                meshRender.sortingOrder = order + orderOffset;
            if (particleRender != null)
                particleRender.sortingOrder = order + orderOffset;
            if (canvas != null)
                canvas.sortingOrder = order + orderOffset;
        }

        public void RefreshRender()
        {
            //Rotation
            if (autoRotate)
            {
                Camera cam = FollowCamera.GetCamera();

                if (!cam.orthographic)
                {
                    transform.rotation = Quaternion.Euler(startRot + Vector3.right * cam.transform.rotation.eulerAngles.x);
                    transform.position = new Vector3(transform.position.x, transform.position.y, -rotateOffset * transform.lossyScale.y);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(startRot);
                    transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
                }
            }
        }
    }

}
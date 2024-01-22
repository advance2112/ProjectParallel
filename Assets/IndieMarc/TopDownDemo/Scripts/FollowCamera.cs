using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Top-down camera script
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.TopDown
{

    public class FollowCamera : MonoBehaviour
    {
        [Header("Camera Target")]
        public GameObject target;
        [FormerlySerializedAs("target_offset")] public Vector3 targetOffset;
        [FormerlySerializedAs("camera_speed")] public float cameraSpeed = 5f;
        
        private PlayerCharacter targetCharacter;
        private Camera cam;
        private Vector3 curPos;
        private GameObject lockTarget = null;

        private Vector3 shakeVector = Vector3.zero;
        private float shakeTimer = 0f;
        private float shakeIntensity = 1f;

        private static FollowCamera instance;

        void Awake()
        {
            instance = this;
            cam = GetComponent<Camera>();
        }

        void LateUpdate()
        {
            GameObject camTarget = target;

            if (lockTarget != null)
                camTarget = lockTarget;

            if (camTarget != null)
            {
                //Find target
                Vector3 targetPos = camTarget.transform.position + targetOffset;
                
                //Check if need to move
                Vector3 diff = targetPos - transform.position;
                if (diff.magnitude > 0.1f)
                {
                    //Move camera
                    transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref curPos, 1f / cameraSpeed, Mathf.Infinity, Time.deltaTime);
                }
            }

            //Shake FX
            if (shakeTimer > 0f)
            {
                shakeTimer -= Time.deltaTime;
                shakeVector = new Vector3(Mathf.Cos(shakeTimer * Mathf.PI * 8f) * 0.02f, Mathf.Sin(shakeTimer * Mathf.PI * 7f) * 0.02f, 0f);
                transform.position += shakeVector * shakeIntensity;
            }
        }

        public float GetFrustrumHeight()
        {
            if (cam.orthographic)
                return 2f * cam.orthographicSize;
            else
                return 2.0f * Mathf.Abs(transform.position.z) * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }

        public float GetFrustrumWidth()
        {
            return GetFrustrumHeight() * cam.aspect;
        }

        public void LockCameraOn(GameObject ltarget)
        {
            lockTarget = ltarget;
        }

        public void UnlockCamera()
        {
            lockTarget = null;
        }

        public void Shake(float intensity = 2f, float duration = 0.5f)
        {
            shakeIntensity = intensity;
            shakeTimer = duration;
        }

        public static FollowCamera Get()
        {
            return instance;
        }

        public static Camera GetCamera()
        {
            if(instance)
                return instance.cam;
            return null;
        }
    }

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

/// <summary>
/// Door script
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>
/// 

namespace IndieMarc.TopDown
{

    public class Door : MonoBehaviour
    {
        [FormerlySerializedAs("nb_switches_required")] [Header("Door")]
        public int nbSwitchesRequired = 1;
        [FormerlySerializedAs("reversed_side")] public bool reversedSide = false;
        [FormerlySerializedAs("opened_at_start")] public bool openedAtStart = false;
        [FormerlySerializedAs("reset_on_death")] public bool resetOnDeath = true;

        [FormerlySerializedAs("open_speed")] public float openSpeed;
        [FormerlySerializedAs("close_speed")] public float closeSpeed;
        [FormerlySerializedAs("max_move")] public float maxMove;

        [FormerlySerializedAs("key_can_open")] [Header("Key Door")]
        public bool keyCanOpen;
        [FormerlySerializedAs("key_index")] public int keyIndex;

        [FormerlySerializedAs("lever_state_required")] [Header("Lever Door")]
        public LeverState leverStateRequired;
        public GameObject[] levers;

        [FormerlySerializedAs("audio_door_open")] [Header("Audio")]
        public AudioClip audioDoorOpen;
        [FormerlySerializedAs("audio_door_close")] public AudioClip audioDoorClose;
        [FormerlySerializedAs("audio_door_close_hard")] public AudioClip audioDoorCloseHard;
        
        private Vector3 initialPos;
        private int nbKeysInside;
        private int audioLastPlayed;
        private bool initialOpened;
        private Vector3 targetPos;
        private bool shouldOpen;

        private AudioSource audioSource;
        
        private List<Lever> leverList = new List<Lever>();

        private static List<Door> doorList = new List<Door>();

        void Awake()
        {
            doorList.Add(this);
        }

        void OnDestroy()
        {
            doorList.Remove(this);
        }

        void Start()
        {
            initialPos = transform.position;
            initialPos.z = 0f;
            initialOpened = openedAtStart;
            audioSource = GetComponent<AudioSource>();
            targetPos = transform.position;
            shouldOpen = openedAtStart;

            foreach (GameObject lever in levers)
                InitSwitch(lever);
            
            ResetOne();
        }

        private void SetOpenedInstant()
        {
            Vector3 moveDir = GetMoveDir();
            transform.position = initialPos + moveDir * maxMove;
            targetPos = transform.position;
        }

        private void InitSwitch(GameObject swt)
        {
            if (swt != null)
            {
                if (swt.GetComponent<Lever>())
                    leverList.Add(swt.GetComponent<Lever>());
            }
        }

        void FixedUpdate()
        {
            //Get nb switch on
            int nbSwitch = GetNbSwitches();

            //keys
            nbSwitch += nbKeysInside;

            //Open door
            bool activated = (nbSwitch >= nbSwitchesRequired);
            Vector3 moveDir = GetMoveDir();
            shouldOpen = openedAtStart ? !activated : activated;
            targetPos = transform.position;
            
            if (shouldOpen)
            {
                Vector3 diff = transform.position - initialPos;
                if (openSpeed >= 0.01f && diff.magnitude < maxMove)
                {
                    targetPos = initialPos + moveDir.normalized * maxMove;
                    targetPos.z = 0f;

                    if (audioSource.enabled && !audioSource.isPlaying && audioLastPlayed != 1)
                    {
                        audioLastPlayed = 1;
                        audioSource.clip = audioDoorOpen;
                        audioSource.Play();
                    }
                }
            }
            else
            {
                Vector3 diff = transform.position - initialPos;
                float dotProd = Vector3.Dot(diff, moveDir);

                if (closeSpeed >= 0.01f && dotProd > 0.001f && diff.magnitude > 0.01f)
                {
                    targetPos = initialPos;
                    targetPos.z = 0f;

                    if (audioSource.enabled && !audioSource.isPlaying && audioLastPlayed != 2)
                    {
                        audioLastPlayed = 2;
                        audioSource.clip = closeSpeed > 5.1f ? audioDoorCloseHard : audioDoorClose;
                        audioSource.Play();
                    }
                }
            }
        }

        private void Update()
        {
            Vector3 moveDir = targetPos - transform.position;
            if (moveDir.magnitude > 0.01f)
            {
                float speed = shouldOpen ? openSpeed : closeSpeed;
                float moveDist = Mathf.Min(speed * Time.deltaTime, moveDir.magnitude);
                transform.position += moveDir.normalized * moveDist;
            }
        }

        private Vector3 GetMoveDir()
        {
            float doorDir = reversedSide ? -1f : 1f;
            Vector3 oriDir = Vector3.right;
            Vector3 moveDir = Quaternion.AngleAxis(transform.eulerAngles.z, Vector3.forward) * oriDir * doorDir;
            moveDir.z = 0f;
            return moveDir.normalized;
        }

        public bool IsOpened()
        {
            float dist = (transform.position - initialPos).magnitude;
            return dist > (maxMove / 2f);
        }

        private int GetNbSwitches()
        {
            int nbSwitch = 0;
            
            // Lever ------------
            foreach (Lever lever in leverList)
            {
                if (lever)
                    nbSwitch += (lever.state == leverStateRequired) ? lever.doorValue : 0;
            }
            
            return nbSwitch;
        }

        public void Open()
        {
            openedAtStart = true;
        }

        public void Close()
        {
            openedAtStart = false;
        }

        public void Toggle()
        {
            openedAtStart = !openedAtStart;
        }

        public bool CanKeyUnlock(Key key)
        {
            return (keyCanOpen && key.keyIndex == keyIndex);
        }

        public void UnlockWithKey(int value)
        {
            nbKeysInside += value;
        }

        public void ResetOne()
        {
            openedAtStart = initialOpened;
            if (openedAtStart)
                SetOpenedInstant();
            else
                transform.position = initialPos;
            targetPos = transform.position;
            shouldOpen = openedAtStart;
        }

        public static void ResetAll()
        {
            foreach (Door door in doorList)
            {
                if (door.resetOnDeath)
                    door.ResetOne();
            }
        }
    }

}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// Lever script
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>
/// 

namespace IndieMarc.TopDown
{

    public enum LeverState
    {
        Left, Center, Right, Disabled
    }

    public class Lever : MonoBehaviour
    {
        [FormerlySerializedAs("lever_center")] public Sprite leverCenter;
        [FormerlySerializedAs("lever_left")] public Sprite leverLeft;
        [FormerlySerializedAs("lever_right")] public Sprite leverRight;
        [FormerlySerializedAs("lever_disabled")] public Sprite leverDisabled;

        [FormerlySerializedAs("can_be_center")] public bool canBeCenter;
        public LeverState state;
        [FormerlySerializedAs("door_value")] public int doorValue = 1;
        [FormerlySerializedAs("no_return")] public bool noReturn = false;
        [FormerlySerializedAs("reset_on_dead")] public bool resetOnDead = true;

        private SpriteRenderer render;
        private LeverState startState;
        private LeverState prevState;
        private float timer = 0f;

        public UnityAction onTriggerLever;

        private static List<Lever> levers = new List<Lever>();

        private void Awake()
        {
            levers.Add(this);
            render = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            startState = state;
            prevState = state;
            ChangeSprite();
        }

        void Update()
        {

            timer += Time.deltaTime;

            if (state != prevState)
            {
                ChangeSprite();
                prevState = state;
            }
        }

        private void OnDestroy()
        {
            levers.Remove(this);
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            if (coll.gameObject.GetComponent<PlayerCharacter>())
            {
                if (state == LeverState.Disabled)
                    return;
                
                Activate();
            }
        }

        public void Activate()
        {
            //Can't activate twice very fast
            if (timer < 0f)
                return;

            if (!noReturn || state == startState)
            {
                timer = -0.8f;

                //Change state
                if (state == LeverState.Left)
                {
                    state = (canBeCenter) ? LeverState.Center : LeverState.Right;
                }
                else if (state == LeverState.Center)
                {
                    state = LeverState.Right;
                }
                else if (state == LeverState.Right)
                {
                    state = LeverState.Left;
                }
                
                //Audio
                GetComponent<AudioSource>().Play();

                //Trigger
                if (onTriggerLever != null)
                    onTriggerLever.Invoke();
            }
        }

        private void ChangeSprite()
        {
            if (state == LeverState.Left)
            {
                render.sprite = leverLeft;
            }
            if (state == LeverState.Center)
            {
                render.sprite = leverCenter;
            }
            if (state == LeverState.Right)
            {
                render.sprite = leverRight;
            }
            if (state == LeverState.Disabled)
            {
                render.sprite = leverDisabled;
            }

            if (noReturn && state != startState)
            {
                render.sprite = leverDisabled;
            }
        }
        
        public void ResetOne()
        {
            if (resetOnDead)
            {
                state = startState;
            }
        }

        public static void ResetAll()
        {
            foreach (Lever lever in levers)
            {
                lever.ResetOne();
            }
        }
    }

}
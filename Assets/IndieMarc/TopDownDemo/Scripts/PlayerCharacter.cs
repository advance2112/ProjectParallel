using System.Collections;
using System.Collections.Generic;
using Advance.ProjectParallel;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace IndieMarc.TopDown
{
    public class PlayerCharacter : Character
    {
        public int playerID;
        
        private bool disableControls = false;
        private PlayerControls controls;

        void Awake()
        {
            Initialize("Player", false);
            onDeath = () =>
            {
                SceneManager.LoadScene("TopDownDemo");
            };
        }

        void Start()
        {
            controls = PlayerControls.Get(playerID);
        }

        //Handle physics
        void FixedUpdate()
        {
            HandleMovement();
            HandleShootTimer();
        }

        //Handle render and controls
        void Update()
        {
            hitTimer += Time.deltaTime;
            DoHealth();
            if (!disableControls)
            {
                SetMoveInput();
                DoRotation();
                DoShootInput();
            }
        }

        protected override void DoShootInput()
        {
            shootInput = useWeapon && controls.GetShoot();
        }

        protected override void DoRotation()
        {
            transform.eulerAngles = new Vector3(0, 0, controls.GetRotate());
        }

        protected override void SetMoveInput()
        {
            moveInput = controls.GetMove();
        }
    }
}

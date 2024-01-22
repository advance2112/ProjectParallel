using System.Collections;
using System.Collections.Generic;
using IndieMarc.TopDown;
using UnityEngine;
using UnityEngine.Events;

namespace Advance.ProjectParallel
{
    public class Enemy : Character
    {
        [Header("Behavior")]
        public bool kamikaze;
        public float followDistance;
        
        private GameObject player;

        void Awake()
        {
            Initialize("Enemy", true);
            onDeath = () =>
            {
                Destroy(gameObject);
            };
            if (kamikaze)
            {
                followDistance = 0;
            }
        }

        void Start()
        {
            player = GameObject.Find("CharacterTopDown");
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
            SetMoveInput();
            DoRotation();
            DoShootInput();
        }
        
        protected override void DoShootInput()
        {
            shootInput = useWeapon;
        }

        protected override void DoRotation()
        {
            Vector3 playerPosition = player.transform.position;
            Vector3 transformPosition = transform.position;

            Vector3 rotateInput = (playerPosition - transformPosition).normalized;
            
            float angleInRadians = Mathf.Atan2(rotateInput.y, rotateInput.x);
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
            
            transform.eulerAngles = new Vector3(0, 0, angleInDegrees + 270);
        }

        protected override void SetMoveInput()
        {
            Vector3 playerPosition = player.transform.position;
            Vector3 transformPosition = transform.position;
            float distance = Vector2.Distance(playerPosition, transformPosition);
            float scalar;
            if (distance > followDistance + 0.5f)
            {
                scalar = 1f;
            }
            else if (distance < followDistance - 0.5f)
            {
                scalar = -1f;
            }
            else
            {
                scalar = 0;
            }
            moveInput = scalar * (playerPosition - transformPosition).normalized;
        }
    }
}
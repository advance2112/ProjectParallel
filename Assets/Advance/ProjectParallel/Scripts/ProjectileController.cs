using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using IndieMarc.TopDown;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Advance.ProjectParallel
{
    public class ProjectileController : MonoBehaviour
    {
        private Vector2 direction;
        private float speed;
        private float damage;
        
        private Rigidbody2D rigid;
        private SpriteRenderer sprite;

        private float lifetime = 5f;

        public void Initialize(Vector2 initDirection, float initSpeed, float initDamage, Vector2 pos, bool enemy, Color color)
        {
            direction = initDirection;
            speed = initSpeed;
            damage = initDamage;
            transform.position = pos;
            sprite.color = color;
            if (enemy)
            {
                gameObject.layer = LayerMask.NameToLayer("Enemy Projectiles");
                
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Player Projectiles");
            }
            
            Destroy(gameObject, lifetime);
        }
        
        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
        }

        void FixedUpdate()
        {
            Vector2 move = new Vector2();
            float desiredSpeedX = direction.x * speed;
            move.x = Mathf.MoveTowardsAngle(direction.x, desiredSpeedX, 1000 * Time.fixedDeltaTime);
            float desiredSpeedY = direction.y * speed;
            move.y = Mathf.MoveTowardsAngle(direction.y, desiredSpeedY, 1000 * Time.fixedDeltaTime);

            rigid.velocity = move;
            
            float angleInRadians = Mathf.Atan2(direction.y, direction.x);
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg;

            transform.eulerAngles = new Vector3(0, 0, angleInDegrees + 270);
        }

        public void ForceFixedUpdate()
        {
            FixedUpdate();
        }
        
        void OnCollisionEnter2D(Collision2D collision)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            PlayerCharacter player = collision.gameObject.GetComponent<PlayerCharacter>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }
            
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
            Destroy(gameObject);
        }
        
        // Update is called once per frame
        void Update()
        {
            
        }
    }
}

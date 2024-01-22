using System.Collections.Generic;
using IndieMarc.TopDown;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Advance.ProjectParallel
{
    public abstract class Character : MonoBehaviour
    {
        [Header("Stats")]
        public float maxHp = 50f;

        [Header("Status")]
        public bool invulnerable = false;

        [Header("Movement")] 
        public bool isStationary;
        public float moveAccel = 1f;
        public float moveDeccel = 1f;
        public float moveMax = 1f;

        [Header("Collision")]
        public bool explodeOnCollision;
        public float damageDealtOnCollision;

        [Header("Weapon")]
        public bool useWeapon;
        public int minFramesBetweenShots;
        public float projectileSpeed;
        public float projectileDamage;

        [Header("Object References")]
        public ProjectileController projectilePrefab;
        public HealthBar healthBar;

        public UnityAction onDeath;
        public UnityAction onHit;

        private Rigidbody2D rigid;
        private SpriteRenderer sprite;

        private float hp;
        private bool isDead = false;
        private Vector2 move;
        protected bool shootInput;
        private int framesSinceLastShoot;
        protected Vector2 moveInput;
        protected float hitTimer = 0f;
        private bool isEnemy;

        protected void Initialize(string layer, bool enemy)
        {
            rigid = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            hp = maxHp;
            isEnemy = enemy;
            gameObject.layer = LayerMask.NameToLayer(layer);
            if (isStationary)
            {
                rigid.mass = 1000000;
            }
        }
        
        protected void HandleShootTimer()
        {
            if (framesSinceLastShoot > minFramesBetweenShots)
            {
                if (shootInput)
                {
                    framesSinceLastShoot = 0;
                    Shoot();
                }
            }
            else
            {
                framesSinceLastShoot++;
            }
        }

        protected void HandleMovement()
        {
            if (isStationary)
            {
                return;
            }
            
            float desiredSpeedX = Mathf.Abs(moveInput.x) > 0.1f ? moveInput.x * moveMax : 0f;
            float accelerationX = Mathf.Abs(moveInput.x) > 0.1f ? moveAccel : moveDeccel;
            move.x = Mathf.MoveTowards(move.x, desiredSpeedX, accelerationX * Time.fixedDeltaTime);
            float desiredSpeedY = Mathf.Abs(moveInput.y) > 0.1f ? moveInput.y * moveMax : 0f;
            float accelerationY = Mathf.Abs(moveInput.y) > 0.1f ? moveAccel : moveDeccel;
            move.y = Mathf.MoveTowards(move.y, desiredSpeedY, accelerationY * Time.fixedDeltaTime);
            
            rigid.velocity = move;
        }

        protected void DoHealth()
        {
            healthBar.SetDamage(hp/maxHp);
        }

        protected virtual void DoShootInput()
        {
        }

        protected virtual void DoRotation()
        {
        }

        protected virtual void SetMoveInput()
        {
        }

        private void Shoot()
        {
            ProjectileController projectile = Instantiate(projectilePrefab);
            projectile.enabled = false;
            var thisTransform = transform;
            
            float angleInRadians = (thisTransform.eulerAngles.z + 90) * Mathf.Deg2Rad;
            float x = Mathf.Cos(angleInRadians);
            float y = Mathf.Sin(angleInRadians);
            
            projectile.Initialize(new Vector2(x, y), projectileSpeed, projectileDamage, thisTransform.position, isEnemy, sprite.color);
            projectile.ForceFixedUpdate();
            projectile.enabled = true;
        }

        public void HealDamage(float heal)
        {
            if (!isDead)
            {
                hp += heal;
                hp = Mathf.Min(hp, maxHp);
            }
        }

        public void TakeDamage(float damage)
        {
            if (!isDead && !invulnerable && hitTimer > 0f)
            {
                hp -= damage;
                hitTimer = 0f;
                
                if (hp <= 0f)
                {
                    Kill();
                }
                else
                {
                    if (onHit != null)
                        onHit.Invoke();
                }
            }
        }

        public void Kill()
        {
            if (!isDead)
            {
                isDead = true;
                rigid.velocity = Vector2.zero;
                move = Vector2.zero;
                moveInput = Vector2.zero;

                if (onDeath != null)
                    onDeath.Invoke();
            }
        }
        
        public void Teleport(Vector3 pos)
        {
            transform.position = pos;
            move = Vector2.zero;
        }

        public Vector2 GetMove()
        {
            return move;
        }

        public bool IsDead()
        {
            return isDead;
        }
        
        void OnCollisionEnter2D(Collision2D collision)
        {
            Character character = collision.gameObject.GetComponent<Character>();

            if (character != null && character.isEnemy != isEnemy)
            {
                character.TakeDamage(damageDealtOnCollision);
                if (explodeOnCollision)
                {
                    Kill();
                }
            }
        }
    }
}
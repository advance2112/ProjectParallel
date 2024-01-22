using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/// <summary>
/// Script for allowing character to carry items
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.TopDown
{

    public class CarryItem : MonoBehaviour
    {
        [FormerlySerializedAs("item_type")] public string itemType;
        [FormerlySerializedAs("rotate_item")] public bool rotateItem;
        [FormerlySerializedAs("carry_size")] public float carrySize = 1f;
        [FormerlySerializedAs("carry_offset")] public Vector2 carryOffset = Vector2.zero;
        [FormerlySerializedAs("carry_angle_deg")] public float carryAngleDeg = 0f;
        [FormerlySerializedAs("reset_on_death")] public bool resetOnDeath;
        
        [HideInInspector]
        public UnityAction<GameObject> onTake;
        [HideInInspector]
        public UnityAction<GameObject> onDrop;

        private CharacterHoldItem bearer;
        private Vector3 initialPos;
        private Vector3 startSize;
        private Quaternion startRot;
        private int startOrder;
        private int startLayer;
        private bool startAutoOrder;
        private bool triggerAtStart;
        private Vector3 lastMotion = Vector3.right;
        
        private SpriteRenderer spriteRender;
        private Collider2D collide;
        private AutoOrderLayer autoSort;
        private AudioSource audioSource;
        private float overObstacleCount = 0f;
        private bool throwing = false;
        private CharacterHoldItem lastBearer;
        private float destroyTimer = 0f;
        private float takeTimer = 0f;
        private float flipX = 1f;
        private bool destroyed = false;

        private Vector3 targetPos;
        private Quaternion targetRotation;

        private static List<CarryItem> itemList = new List<CarryItem>();

        private void Awake()
        {
            itemList.Add(this);

            collide = GetComponent<Collider2D>();
            spriteRender = GetComponentInChildren<SpriteRenderer>();
            autoSort = GetComponent<AutoOrderLayer>();
            audioSource = GetComponent<AudioSource>();
            initialPos = transform.position;
            triggerAtStart = collide.isTrigger;
            startSize = transform.localScale;
            startRot = transform.rotation;
            startOrder = spriteRender.sortingOrder;
            startLayer = spriteRender.sortingLayerID;
            startAutoOrder = autoSort.autoSort;
        }

        private void OnDestroy()
        {
            itemList.Remove(this);
        }

        void Start()
        {

        }

        void Update()
        {
            takeTimer += Time.deltaTime;

            if (overObstacleCount > 0f)
                overObstacleCount -= Time.deltaTime;

            if (!bearer && !throwing)
            {
                //Called from bearer to sync when has bearer, otherwise called here
                UpdateCarryItem();
            }

            //Destroyed
            if (resetOnDeath && !spriteRender.enabled)
            {
                destroyTimer += Time.deltaTime;
                if (destroyTimer > 3f)
                {
                    Reset();
                }
            }
            
        }
        
        public void UpdateCarryItem()
        {
            AdaptOrderInLayer();
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            targetPos = transform.position;
            float targetAngle = 0f;
            targetRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, targetAngle);

            if (bearer)
            {
                Vector3 motion = bearer.GetMove();
                if (bearer)
                {
                    if (motion.magnitude > 0.1f)
                    {
                        lastMotion = motion;
                    }
                }
                
                //Update position of the item
                
                GameObject hand = bearer.hand.gameObject;
                targetPos = hand.transform.position + hand.transform.up * carryOffset.y + hand.transform.right * carryOffset.x * flipX;
                Vector3 rotVectorForw = Quaternion.Euler(0f, 0f, carryAngleDeg * flipX) * hand.transform.forward;
                Vector3 rotVectorUp = Quaternion.Euler(0f, 0f, carryAngleDeg * flipX) * hand.transform.up;
                targetRotation = Quaternion.LookRotation(rotVectorForw, rotVectorUp);
            }

            //Move the object
            transform.position = targetPos;
            transform.rotation = targetRotation;

            //Flip
            transform.localScale = bearer || throwing ? startSize * carrySize : startSize;
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * flipX, transform.localScale.y, transform.localScale.z);

        }

        private void AdaptOrderInLayer()
        {
            autoSort.RefreshSort();

            if (bearer)
            {
                if (!rotateItem)
                {
                    CharacterHoldItem charBearer = bearer.GetComponent<CharacterHoldItem>();
                }
            }
        }

        public bool CanTake(GameObject taker)
        {
            CharacterHoldItem player = taker.GetComponent<CharacterHoldItem>();
            CarryItem currentItem = player.GetHeldItem();
            
            if (currentItem != null && itemType == currentItem.itemType)
                return false;
            
            if (takeTimer >= -0.01f)
            {
                //Avoid taking back an item you just threw
                return (!throwing || lastBearer != taker);
            }

            return false;
        }

        public void Take(CharacterHoldItem bearer)
        {
            this.bearer = bearer;
            lastBearer = bearer;
            collide.isTrigger = true;

            spriteRender.sortingLayerID = bearer.GetComponent<SpriteRenderer>().sortingLayerID;
            autoSort.autoSort = false; //Will be sorted based on character instead
            transform.localScale = startSize * carrySize;
            
            if (onTake != null)
            {
                onTake.Invoke(bearer.gameObject);
            }

            UpdateCarryItem();
        }

        public void Drop()
        {
            lastBearer = this.bearer;
            this.bearer = null;
            collide.isTrigger = throwing ? false : triggerAtStart;
            takeTimer = -0.01f;

            //Reset sorting order/layer
            spriteRender.sortingOrder = startOrder;
            spriteRender.sortingLayerID = startLayer;
            
            if (lastBearer && !throwing)
            {
                //Reset straight floor position
                transform.position = new Vector3(lastBearer.transform.position.x, lastBearer.transform.position.y, initialPos.z);
                transform.localScale = startSize;
                transform.rotation = startRot;
                flipX = 1f;
            }

            //Reset auto sort/rotate
            autoSort.autoSort = startAutoOrder;
            autoSort.RefreshAutoRotate();
            autoSort.RefreshSort();

            if (onDrop != null)
            {
                onDrop.Invoke(lastBearer.gameObject);
            }
        }
        
        public bool IsThrowing()
        {
            return throwing;
        }

        public bool IsDestroyed()
        {
            return destroyed;
        }

        public void UseItem()
        {
            if (bearer)
            {
                //Add code to use
            }
        }
        
        void PlayAudio()
        {
            if (audioSource)
                audioSource.Play();
        }

        public void Destroy()
        {
            if (bearer && bearer.GetComponent<CharacterHoldItem>())
            {
                bearer.GetComponent<CharacterHoldItem>().DropItem();
            }
            destroyed = true;
            collide.enabled = false;
            spriteRender.enabled = false;
            destroyTimer = 0f;
        }

        public void SetStartingPos(Vector3 startPos)
        {
            this.initialPos = startPos;
        }

        public CharacterHoldItem GetBearer()
        {
            return this.bearer;
        }

        public bool HasBearer()
        {
            return (this.bearer != null);
        }

        public Vector3 GetOrientation()
        {
            return lastMotion;
        }

        public float GetFlipX()
        {
            return flipX;
        }

        public bool IsOverObstacle()
        {
            return (overObstacleCount > 0.01f);
        }

        public void Reset()
        {
            if (resetOnDeath)
            {
                destroyed = false;
                collide.enabled = true;
                spriteRender.enabled = true;
                overObstacleCount = 0f;
                transform.position = initialPos;
            }
        }

        public static CarryItem[] GetAll()
        {
            return itemList.ToArray();
        }

        //Triggered on death
        public static void ResetAll()
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                itemList[i].Reset();
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Wall"
                || collision.gameObject.tag == "Door")
            {
                overObstacleCount = 0.2f;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Wall"
                || collision.gameObject.tag == "Door")
            {
                overObstacleCount = 0.2f;
            }
            
        }
        
    }

}
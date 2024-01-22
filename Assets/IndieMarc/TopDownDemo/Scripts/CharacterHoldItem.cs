using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.TopDown
{
    [RequireComponent(typeof(PlayerCharacter))]
    public class CharacterHoldItem : MonoBehaviour
    {
        public Transform hand;

        private PlayerCharacter character;

        private CarryItem heldItem = null;
        private float takeItemTimer = 0f;

        void Awake()
        {
            character = GetComponent<PlayerCharacter>();
        }

        private void Start()
        {
            character.onDeath += DropItem;
        }

        void Update()
        {
            PlayerControls controls = PlayerControls.Get(character.playerID);
            
            takeItemTimer += Time.deltaTime;
            if (heldItem && controls.GetActionDown())
                heldItem.UseItem();
        }

        private void LateUpdate()
        {
            if (heldItem != null)
                heldItem.UpdateCarryItem();
        }

        public void TakeItem(CarryItem item) {

            if (item == heldItem || takeItemTimer < 0f)
                return;

            if (heldItem != null)
                DropItem();

            heldItem = item;
            takeItemTimer = -0.2f;
            item.Take(this);
        }

        public void DropItem()
        {
            if (heldItem != null)
            {
                heldItem.Drop();
                heldItem = null;
            }
        }

        public PlayerCharacter GetCharacter()
        {
            return character;
        }

        public CarryItem GetHeldItem()
        {
            return heldItem;
        }

        public Vector3 GetHandPos()
        {
            if (hand)
                return hand.transform.position;
            return transform.position;
        }

        public Vector2 GetMove()
        {
            return character.GetMove();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<CarryItem>())
                TakeItem(collision.GetComponent<CarryItem>());
        }

        void OnCollisionStay2D(Collision2D coll)
        {
            if (coll.gameObject.GetComponent<Door>() && heldItem && heldItem.GetComponent<Key>())
            {
                heldItem.GetComponent<Key>().TryOpenDoor(coll.gameObject);
            }
        }
    }

}

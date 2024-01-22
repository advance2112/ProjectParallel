using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

/// <summary>
/// Player controls for platformer demo
/// Author: Indie Marc (Marc-Antoine Desbiens)
/// </summary>

namespace IndieMarc.TopDown
{

    public class PlayerControls : MonoBehaviour
    {
        [FormerlySerializedAs("player_id")] public int playerID;
        [FormerlySerializedAs("left_key")] public KeyCode leftKey;
        [FormerlySerializedAs("right_key")] public KeyCode rightKey;
        [FormerlySerializedAs("up_key")] public KeyCode upKey;
        [FormerlySerializedAs("down_key")] public KeyCode downKey;
        [FormerlySerializedAs("action_key")] public KeyCode actionKey;

        private Vector2 move = Vector2.zero;
        private float rotate = 0;
        private bool shoot = false;
        private bool actionPress = false;
        private bool actionHold = false;

        private static Dictionary<int, PlayerControls> controls = new Dictionary<int, PlayerControls>();

        void Awake()
        {
            controls[playerID] = this;
        }

        void OnDestroy()
        {
            controls.Remove(playerID);
        }

        void Update()
        {
            
            float moveLength = Mathf.Min(move.magnitude, 1f);
            move = move.normalized * moveLength;
        }


        //------ These functions should be called from the Update function, not FixedUpdate
        public Vector2 GetMove()
        {
            return move;
        }
        
        public void SetMove(Vector2 move)
        {
            this.move = move;
        }
        
        public float GetRotate()
        {
            return rotate;
        }
        
        public bool GetShoot()
        {
            return shoot;
        }
        
        public void SetRotateAndShoot(Vector2 rotateVector)
        {
            if (rotateVector.magnitude < 0.1f)
            {
                shoot = false;
                return;
            }
            shoot = true;
            float angleInRadians = Mathf.Atan2(rotateVector.y, rotateVector.x);
            float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
            
            rotate = angleInDegrees + 270;
        }

        public void RotateAndShoot(InputAction.CallbackContext context)
        {
            SetRotateAndShoot(context.ReadValue<Vector2>());
        }

        public void Move(InputAction.CallbackContext context)
        {
            SetMove(context.ReadValue<Vector2>());
        }

        public bool GetActionDown()
        {
            return actionPress;
        }

        public bool GetActionHold()
        {
            return actionHold;
        }

        //-----------

        public static PlayerControls Get(int playerID)
        {
            foreach (PlayerControls control in GetAll())
            {
                if (control.playerID == playerID)
                {
                    return control;
                }
            }
            return null;
        }

        public static PlayerControls[] GetAll()
        {
            PlayerControls[] list = new PlayerControls[controls.Count];
            controls.Values.CopyTo(list, 0);
            return list;
        }

    }

}
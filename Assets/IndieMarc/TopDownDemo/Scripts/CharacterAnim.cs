using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieMarc.TopDown
{
    [RequireComponent(typeof(PlayerCharacter))]
    [RequireComponent(typeof(Animator))]
    public class CharacterAnim : MonoBehaviour
    {
        private PlayerCharacter character;
        private CharacterHoldItem characterItem;
        private Animator animator;

        void Awake()
        {
            character = GetComponent<PlayerCharacter>();
            characterItem = GetComponent<CharacterHoldItem>();
            animator = GetComponent<Animator>();
        }

        void Update()
        {
            //Anims
            animator.SetFloat("Speed", character.GetMove().magnitude);
            if(characterItem != null)
                animator.SetBool("Hold", characterItem.GetHeldItem() != null);
        }
        
    }

}
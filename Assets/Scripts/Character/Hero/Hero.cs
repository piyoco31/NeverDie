using UnityEngine;

namespace ND.Hero
{
    public class Hero : MonoBehaviour
    {
        Animator anim;
        E_CharacterState state;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        
        }

        void Start()
        {
            SetHeroState(E_CharacterState.Idle);
        }

        void SetHeroState(E_CharacterState state)
        {
            this.state = state;

            SetAnimByState(state);
        }

        void SetAnimByState(E_CharacterState InState)
        {
            string triggerStr = InState switch
            {
                E_CharacterState.Attack => "Attack",
                E_CharacterState.Death => "Death",
                E_CharacterState.Run => "Run",
                _ => "Idle"
            };

            anim?.SetTrigger(triggerStr);
        }
    }
}



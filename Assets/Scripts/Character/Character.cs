using UnityEngine;
using R3;

namespace ND.Character
{
    public class Character : MonoBehaviour
    {
        ReactiveProperty<E_CharacterState> stateRxProp = new ReactiveProperty<E_CharacterState>(E_CharacterState.Idle);

        public E_CharacterState State 
        { 
            get { return stateRxProp.Value; }
            set { stateRxProp.Value = value; }
        }

        [SerializeField] Animator anim;
        
        private void Awake()
        {
            anim = GetComponent<Animator>();

            stateRxProp.Subscribe(state =>
            {
                SetAnimByState(state);
            });
        }

        void Start()
        {

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

        private void OnDestroy()
        {
            stateRxProp.Dispose();
        }
    }
}

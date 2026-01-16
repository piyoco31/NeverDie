using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using VContainer;

namespace ND.Character.Hero
{
    using Manager;

    public class Supporter : Character
    {
        [Inject] HeroManager heroManager;

        protected override async UniTask AttackToTarget()
        {
            Target = heroManager.GetHealTargetHero();

            if (Target != null)
            {
                Vector3 dir = Target.transform.position - transform.position;

                Quaternion targetRotation = Quaternion.LookRotation(Target == this ? Vector3.right : dir, Vector3.up);

                transform.DORotate(targetRotation.eulerAngles, 0.5f).OnComplete(() =>
                {
                    State = E_CharacterState.Attack;
                });

                await WaitingAttackAnimTime();
                await Target.DamageToCharacter(-Atk);

                State = E_CharacterState.Idle;
                targetRotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                transform.DORotate(targetRotation.eulerAngles, 0.5f);
            }
        }
    }
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace ND.Character.Hero
{
    public class Supporter : Character
    {
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

                isAttackFinish = false;
                await UniTask.WaitUntil(() => isAttackFinish);
                await WaitingAttackAnimTime();

                State = E_CharacterState.Idle;
                targetRotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                transform.DORotate(targetRotation.eulerAngles, 0.5f);
            }
        }

        public async override void OnAnimAttack()
        {
            if (Target)
            {
                await Target.DamageToCharacter(-Atk);
            }

            isAttackFinish = true;
        }
    }
}

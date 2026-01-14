using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using VContainer;

namespace ND.Character.Hero
{
    using Manager;

    public class Attacker : Character
    {
        [Inject] MonsterManager monsterManager;

        protected override async UniTask AttackToTarget()
        {
            if (Target == null || Target.IsDead || Vector3.Distance(Target.transform.position, transform.position) > Range)
            {
                Target = monsterManager.GetTargetMonster(this);
            }

            if (Target != null)
            {
                Vector3 dir = Target.transform.position - transform.position;

                Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);

                transform.DORotate(targetRotation.eulerAngles, 0.5f).OnComplete(() =>
                {
                    State = E_CharacterState.Attack;
                });

                await WaitingAttackAnimTime();
                await Target.DamageToCharacter(Atk);

                State = E_CharacterState.Idle;
                targetRotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                transform.DORotate(targetRotation.eulerAngles, 0.5f);
            }
        }
    }
}

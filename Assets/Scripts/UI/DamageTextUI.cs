using Cysharp.Threading.Tasks;
using ND.Manager;
using TMPro;
using UnityEngine;
using VContainer;

namespace ND.UI
{
    public class DamageTextUI : MonoBehaviour
    {
        [Inject] protected ParticleManager particleManager;

        TMP_Text damageTxt;
        RectTransform rect;
        float posY;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
            damageTxt = GetComponent<TMP_Text>();
        }

        public void Init(float damage, Transform parent)
        {
            transform.SetParent(parent);
            posY = 2;
            rect.sizeDelta = new Vector2(10, 5);
            rect.anchoredPosition3D = new Vector3(0, posY, 0);
            rect.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            if (damage == 0)
            {
                DespawnDamgeText();
            }
            else
            {
                var isPositive = damage < 0;
                damageTxt.text = isPositive ? $"+{Mathf.Abs(damage)}" : $"-{damage}";
                damageTxt.color = isPositive ? Color.green : Color.red;
            }
        }

        async void DespawnDamgeText(float waitTime = 0.0f)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(waitTime));
            await particleManager.DespawnDamageText(this);
        }

        void LateUpdate()
        {
            if (posY < 2.6f)
            {
                posY += Time.deltaTime;

                if (posY >= 2.6f)
                {
                    posY = 2.6f;
                    DespawnDamgeText(1);
                }

                transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(0f, 0f, 0f);
                rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x, posY, rect.anchoredPosition3D.z);
            }
        }
    }
}

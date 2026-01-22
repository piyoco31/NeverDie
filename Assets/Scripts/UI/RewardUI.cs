using Cysharp.Threading.Tasks;
using ND.Manager;
using TMPro;
using UnityEngine;
using VContainer;

namespace ND.UI
{
    public class RewardUI : MonoBehaviour
    {
        [Inject] protected ParticleManager particleManager;

        TMP_Text rewardTxt;
        RectTransform rect;
        float posY;
        float movePosY;

        void Awake()
        {
            rect = GetComponent<RectTransform>();
            rewardTxt = GetComponent<TMP_Text>();
        }

        public void Init(float reward, Vector3 pos)
        {
            rect.sizeDelta = new Vector2(15, 5);
            rect.anchoredPosition3D = pos;
            rect.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            posY = pos.y + 1;
            movePosY = posY + 0.6f;

            rewardTxt.text = $"+${reward}";
        }

        async void DespawnRewardText(float waitTime = 0.0f)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(waitTime));
            await particleManager.DespawnRewardText(this);
        }

        void LateUpdate()
        {
            if (posY < movePosY)
            {
                posY += Time.deltaTime;

                if (posY >= movePosY)
                {
                    posY = movePosY;
                    DespawnRewardText(1);
                }

                transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(0f, 0f, 0f);
                rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x, posY, rect.anchoredPosition3D.z);
            }
        }
    }
}

using UnityEngine;
using R3;

namespace ND.UI
{
    using Character;

    public class MonsterHpBarUI : MonoBehaviour
    {
        Character monster;
        Vector3 localScale;

        protected ReactiveProperty<float> showTimeRxProp = new ReactiveProperty<float>(0);
        public float ShowTime
        {
            private get { return showTimeRxProp.Value; }
            set { showTimeRxProp.Value = value; }
        }

        private void Awake()
        {
            transform.localPosition = new Vector3(0, 1.8f, 0 );
            transform.localScale = new Vector3(0.1f, 0.1f, 0.01f);

            localScale = transform.localScale;
            monster = GetComponentInParent<Character>();
        }

        void Start()
        {
            monster?.CurrentHpRxProp.Subscribe(x =>
            {
                transform.localScale = new Vector3(x / monster.MaxHp * localScale.x, localScale.y, localScale.z);

            }).AddTo(this);

            monster?.MaxHpRxProp.Subscribe(x =>
            {
                transform.localScale = new Vector3(monster.CurrentHp / x * localScale.x, localScale.y, localScale.z);
            }).AddTo(this);

            showTimeRxProp.Subscribe(x =>
            {
                bool isActive = x > 0.0f;

                if (isActive != gameObject.activeInHierarchy)
                    gameObject.SetActive(isActive);

            }).AddTo(this);

            gameObject.SetActive(false);
        }

        public void LateUpdate()
        {
            if (ShowTime > 0)
            {
                ShowTime -= Time.deltaTime;
                transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(-90f, 0f, 0f);

                if (ShowTime <= 0)
                {
                    ShowTime = 0.0f;
                }
            }
        }
    }

}

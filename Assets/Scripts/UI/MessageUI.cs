using TMPro;
using UnityEngine;

namespace ND.UI
{
    public class MessageUI : MonoBehaviour
    {
        [SerializeField] TMP_Text messageTxt;

        float delay = 0.0f;

        private void Start()
        {
            gameObject.SetActive(false);
        }

        public void ShowMessage(string message, float time)
        {
            messageTxt.text = message;
            delay = time;
            gameObject.SetActive(true);
        }

        private void Update() 
        {
            if (delay > 0.0f)
            { 
                delay -= Time.deltaTime;

                if (delay <= 0.0f)
                { 
                    gameObject.SetActive(false);
                    delay = 0.0f;
                }
            }
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;


namespace ND.UI
{
    public class TitleUI : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource;
        [SerializeField] GameObject exitPopup;

        void Start()
        {
            exitPopup.SetActive(false);
            audioSource.Play();
        }

        public void OnClickGameStart()
        {
            audioSource.Stop();
            SceneManager.LoadScene("MainGame");
        }

        public void OnClickGameEnd()
        {
            exitPopup.SetActive(true);
        }

        public void OnClickExitPopupButton(bool isConfirm)
        {
            if (isConfirm)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
            }
            else
            {
                exitPopup.SetActive(false);
            }
        }
    }
}

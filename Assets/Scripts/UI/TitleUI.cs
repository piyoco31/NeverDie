using UnityEngine;
using UnityEngine.SceneManagement;


namespace ND.UI
{
    public class TitleUI : MonoBehaviour
    {
        [SerializeField] GameObject exitPopup;
        // [SerializeField] Button gameStartButton;

        void Start()
        {
            exitPopup.SetActive(false);
            // gameStartButton.interactable = false;
        }

        public void OnClickGameStart()
        {
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

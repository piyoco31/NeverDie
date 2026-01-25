using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace ND.UI
{
    using Character;
    using Data;
    using DG.Tweening;
    using Manager;

    public class InGameUI : MonoBehaviour
    {
        [Inject] IObjectResolver container;
        [Inject] HeroManager heroManager;
        [Inject] UserDataManager userDataManager;
        [Inject] ParticleManager particleManager;

        [SerializeField] MessageUI messageUI;
        [SerializeField] HeroStatUI heroStatUI;
        [SerializeField] GameObject upgradeShop;
        [SerializeField] GameObject exitPopup;
        [SerializeField] GameObject gameOverUI;
        [SerializeField] Transform spawnButtonTr;
        [SerializeField] Transform heroUITr;
        [SerializeField] TMP_Text userMoneyTxt;
        [SerializeField] TMP_Text waveCountTxt;
        [SerializeField] TMP_Text reRollCountTxt;
        [SerializeField] TMP_Text killCountTxt;
        [SerializeField] TMP_Text dimmedTxt;
        [SerializeField] Image dimmedImg;
        [SerializeField] AudioSource audioSource;
        [SerializeField] List<UpgradeShopButtonUI> upgradeButtonList;
        [SerializeField] List<string> clipNameList;

        public Transform HeroUITr { get { return heroUITr; } }
        public bool IsUpgradeShopOpen { get { return upgradeShop.gameObject.activeSelf; } }
        public HeroManager HeroManagerInst { get { return heroManager; }}
        public UserDataManager UserDataManagerInst { get { return userDataManager; } }

        ReactiveProperty<int> reRollCountRxProp = new(1);
        private int ReRollCount { get { return reRollCountRxProp.Value; } set { reRollCountRxProp.Value = value; } }

        Dictionary<string, AudioClip> audioClipDict = new();
        List<HeroSpawnButtonUI> spawnButtonlist = new();
        List<UpgradeData> upgradeDataList;

        private void Start()
        {
            audioSource.loop = false;
            CloseUpgradeShop();
            ActiveExitPopup(false);
        }

        public async UniTask Init(MainGameManager mainGameManager)
        {
            var dataSO = await Addressables.LoadAssetAsync<UpgradeDataSO>("ND_SO_UpgradeDataAsset");

            upgradeDataList = dataSO.upgradeDataList;

            await InitSelectButtonUI();

            userDataManager.MoneyRxProp.Subscribe(x =>
            {
                userMoneyTxt.text = x.ToString();
            }).AddTo(this);

            userDataManager.KillRxProp.Subscribe(x =>
            {
                killCountTxt.text = x.ToString();
            }).AddTo(this);

            mainGameManager.WaveCountRxProp.Subscribe(x =>
            {
                waveCountTxt.text = $"Wave {x + 1}";
            }).AddTo(this);

            reRollCountRxProp.Subscribe(x =>
            {
                reRollCountTxt.text = $"리롤(${x * 100})";
            }).AddTo(this);

            heroManager.IsGameOverRxProp.DistinctUntilChanged().Subscribe(x =>
            {
                if (x)
                {
                    particleManager.PlayBGM("GameOver");
                    AlphaToGameOver();
                }
            }).AddTo(this);

            clipNameList.ForEach(async x =>
            {
                var clip = await Addressables.LoadAssetAsync<AudioClip>($"ND_SFX_{x}");
                audioClipDict.Add(x, clip);
            });

            await AlphaToDimmed();
        }

        async UniTask InitSelectButtonUI()
        {
            var positionDataSO = await Addressables.LoadAssetAsync<HeroPositionDataSO>("ND_SO_HeroPositionDataAsset");
            var poslist = positionDataSO.heroPositionDataList;
            var UIObj = await Addressables.LoadAssetAsync<GameObject>("ND_UI_SpawnButton");

            for (int i = 0; i < 5; i++)
            {
                var spawnButton = Instantiate(UIObj, spawnButtonTr).GetComponent<HeroSpawnButtonUI>();
                container.Inject(spawnButton);
                spawnButton.Init(i, new Vector3(poslist[i].posX, poslist[i].posY, poslist[i].posZ), poslist[i].heroUIPosY);
                spawnButtonlist.Add(spawnButton);
            }
        }

        public void ActiveAllSelectButtons(bool isActive, int currentSelectIdx = 0)
        {
            for (int i = 0; i < spawnButtonlist.Count; i++)
            {
                if (spawnButtonlist[i].IsSpawn) continue;

                spawnButtonlist[i].gameObject.SetActive(isActive ? true : i == currentSelectIdx ? true : false);
            }
        }

        public async UniTask OpenUpgradeShop(bool isWaveEnd = false)
        {
            if (isWaveEnd)
                ReRollCount = 1;

            var itemList = upgradeDataList.FindAll(x => !x.isRare);
            int rareDrop = Random.Range(0, 100);

            if (rareDrop <= 30)
            {
                var rareList = upgradeDataList.FindAll(x => x.isRare);
                itemList.AddRange(rareList);
            }

            var random = new System.Random();
            var list = itemList.OrderBy(x => random.Next()).ToList();

            for (int i = 0; i < upgradeButtonList.Count; i++)
            {
                upgradeButtonList[i].SetUpgradeButton(list[i], OnUpgrade);
            }

            upgradeShop.SetActive(true);

            await UniTask.CompletedTask;
        }

        public void ReRollUpgradeShop()
        {
            if (userDataManager.Money >= ReRollCount * 100)
            {
                userDataManager.Money -= ReRollCount * 100;
                ReRollCount++;
                _ = OpenUpgradeShop();
                ShowMessage("상점을 갱신하였습니다.");
            }
            else
                ShowMessage("소지금이 부족합니다.");
        }

        public void OnClickCheatButton()
        {
            userDataManager.Money += 1000000;
        }

        public void CloseUpgradeShop()
        {
            upgradeShop.SetActive(false);
        }

        public void OnUpgrade(UpgradeShopButtonUI button, int idx)
        {
            var data = upgradeDataList.Find(x => x.idx == idx);

            if (data != null && data.price <= userDataManager.Money)
            {
                PlaySound("UpgradeButton");

                button.IsEnable = false;
                userDataManager.Money -= data.price;
                
                if (data.upgradeTarget == E_UpgradeTargetType.FreeReRoll)
                {
                    _ = OpenUpgradeShop();
                    ShowMessage("상점을 갱신하였습니다.");
                }
                else if (data.upgradeTarget == E_UpgradeTargetType.UserMoney)
                {
                    userDataManager.Money += (int)data.upgradeValue;
                    ShowMessage($"${data.upgradeValue}를 획득하였습니다.");
                }
                else
                {
                    ShowMessage($"'{data.name}'을 구매하였습니다.");
                    heroManager.UpgradeHero(data);
                }
            }
            else
                ShowMessage("소지금이 부족합니다.");
        }

        public void ShowMessage(string message, float time = 3.0f)
        {
            messageUI.ShowMessage(message, time);
        }

        public void SetHeroDetailStat(Character hero)
        {
            heroStatUI.SetHeroDetailStat(hero);
        }

        public void ActiveExitPopup(bool isActive)
        {
            exitPopup.SetActive(isActive);
        }

        async UniTask AlphaToDimmed()
        {
            dimmedTxt.DOFade(0, 2.0f).onComplete = () =>
            {
                dimmedTxt.gameObject.SetActive(false);
            }; 

            dimmedImg.DOFade(0, 2.0f).onComplete = () =>
            {
                dimmedImg.gameObject.SetActive(false);
            };

            await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f));
        }

        public void AlphaToGameOver()
        {
            userDataManager.SaveUserData();
            gameOverUI.SetActive(true);
            var canvas = gameOverUI.GetComponent<CanvasGroup>();
            canvas.DOFade(1, 2.0f);
        }

        public void OnClickMainMenuButton()
        {
            particleManager.StopBGM();
            SceneManager.LoadScene("TitleGame");
        }

        public void PlaySound(string key)
        {
            if (audioClipDict.ContainsKey(key))
            {
                audioSource.Stop();
                var clip = audioClipDict[key];

                if (audioSource.clip != clip)
                    audioSource.clip = clip;

                audioSource.Play();
            }
        }

        public void OnClickExitButton()
        {
            userDataManager.SaveUserData();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}

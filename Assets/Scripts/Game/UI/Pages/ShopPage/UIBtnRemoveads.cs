using Config;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIBtnRemoveads : MonoBehaviour
    {
        public Button btnBuy;
        public int label;
        private void Awake()
        {
            btnBuy.onClick.AddListener(OnBuyRemoveads);
        }

        private void OnEnable()
        {
            if (GameMethod.HasRemoveAD())
            {
                btnBuy.gameObject.SetActive(false);
            }
            else
            {
                btnBuy.gameObject.SetActive(true);
            }
            EventManager.Register(EventKey.PropCountChange, OnPropCountChange);
        }

        private void OnDisable()
        {
            EventManager.Unregister(EventKey.PropCountChange, OnPropCountChange);
        }

        void OnPropCountChange(EventData pEventData)
        {
            if (GameMethod.HasRemoveAD())
            {
                btnBuy.gameObject.SetActive(false);
            }
        }

        void OnBuyRemoveads()
        {
            InputLockManager.Instance.Lock("BuyProduct");
            IAPManager.Instance.BuyProduct(GameConst.ProductID_ADS, OnBuyCallback);
        }

        void OnBuyCallback(bool pIsComplete)
        {
            InputLockManager.Instance.UnLock("BuyProduct");
            btnBuy.gameObject.SetActive(!pIsComplete);
            if (pIsComplete)
            {
                GameVariable.UserBuyFrom = label switch
                {
                    1 => UserBuyFrom.HomePage,
                    2 => UserBuyFrom.DailyPage,
                    3 => UserBuyFrom.NovelPage,
                    0 => GameManager.Instance.CurrentGameModeType switch
                    {
                        GameModeType.Endless => UserBuyFrom.Endless,
                        GameModeType.Daily => UserBuyFrom.Daily,
                        GameModeType.Novel => UserBuyFrom.Novel,
                        _ => string.Empty,
                    },
                    _ => string.Empty,
                };
            }
        }

    }
}
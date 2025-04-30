using Config;
using Game.UI;
using Game.UISystem;
using System;
using System.Runtime.Remoting.Contexts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class MiniFailedPage : PageBase
    {
        [SerializeField] private Button _btnClose;
        [SerializeField] private TextMeshProUGUI _txtLevel;
        [SerializeField] private TextMeshProUGUI _txtDescribe;
        [SerializeField] private RectTransform _failedRoot;
        [SerializeField] private UIBtnPlayOn _btnAbandon;
        [SerializeField] private UIBtnPlayOn _btnPlayProp;
        [SerializeField] private UIBtnPlayOn _btnPlayCoin;

        MiniFailedPageParam mParam;
        MiniGameType mCacheType;
        int mReplayCoin;
        bool mIsUsePropValid;
        protected override void OnInit()
        {
            _btnClose.onClick.AddListener(OnClickBtnClose);
            _btnAbandon.button.onClick.AddListener(OnClickBtnAbandon);
            _btnPlayProp.button.onClick.AddListener(OnClickBtnPlayProp);
            _btnPlayCoin.button.onClick.AddListener(OnClickBtnPlayCoin);
        }

        protected override void OnBeginOpen()
        {
            AudioManager.Instance.PlaySound(SoundID.Tile_Level_Failed);
            //_btnAbandon.txtIcon.text = "-1";
            //_btnAbandon.imgIcon.SetPropIcon(PropID.Energy);  
            _txtLevel.text = $"LEVEL {MiniGameManager.Instance.Level}";
            mReplayCoin = GetReplayCoin();

            mParam = PageParam as MiniFailedPageParam;
            if (mParam == null)
            {
                LogManager.LogError("MiniFailedPage: invalid param");
                return;
            }

            mCacheType = MiniGameManager.Instance.GameType;
            var tUsePropID = mParam.usePropID;
            bool tIsValid = mParam.isValid && tUsePropID != PropID.Invalid;
            mIsUsePropValid = tIsValid && ModuleManager.Prop.HasProp(tUsePropID);
            if (mIsUsePropValid)
            {
                _txtDescribe.text = mCacheType switch
                {
                    MiniGameType.Screw => $"Add 1 more hole and keep playing! ",
                    MiniGameType.Jam3d => $"Set aside some minions to free up space! ",
                    MiniGameType.Tile => $"Set aside 3 tiles to free up space! ",
                    MiniGameType.Bus => $"Get all minions fully loaded on the current buses and keep playing! ",
                    MiniGameType.Triple => GetTripleDescribe(tUsePropID),
                    _ => $"Use an item to continue the game!",
                };

                _btnPlayProp.imgIcon.SetPropIcon(tUsePropID);
                _btnPlayProp.txtIcon.text = "USE 1";
                _btnClose.gameObject.SetActive(true);
                _btnAbandon.gameObject.SetActive(false);
                _btnPlayProp.gameObject.SetActive(true);
                _btnPlayCoin.gameObject.SetActive(false);
            }
            else
            {
                _txtDescribe.text = "Oops! Your backup area is full. \r\nPlease restart the level and have another go!";

                _btnClose.gameObject.SetActive(true);
                _btnAbandon.gameObject.SetActive(false);
                _btnPlayProp.gameObject.SetActive(false);
                _btnPlayCoin.gameObject.SetActive(true);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_failedRoot);
        }

        int GetReplayCoin()
        {
            var tConfig = ModuleManager.MiniGame.GetCurLevelConfig();
            var tRetryCount = ModuleManager.MiniGame.GetReviveCount((int)MiniGameManager.Instance.GameType);
            return tRetryCount switch
            {
                0 => tConfig.ReplayCoin,
                1 => tConfig.ReplayCoin1,
                2 => tConfig.ReplayCoin2,
                3 => tConfig.ReplayCoin3,
                4 => tConfig.ReplayCoin4,
                _ => tConfig.ReplayCoin4,
            };
        }

        string GetTripleDescribe(PropID pPropID)
        {
            if (pPropID == PropID.TripleRevert)
            {
                return $"<color=#dd30f7>Recall 3</color> objects to play on! ";
            }
            else if (pPropID == PropID.TripleAddTime)
            {
                return $"You are so close! Keep playing! ";
            }

            return string.Empty;
        }

        #region UI事件

        void OnClickBtnClose()
        {
            if (!mIsUsePropValid)
            {
                _txtDescribe.text = "Oops! Your backup area is full. \r\nPlease restart the level and have another go!";
            }

            _btnClose.gameObject.SetActive(false);
            _btnAbandon.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_failedRoot);
        }

        void OnClickBtnAbandon()
        {
            MiniGameManager.Instance.DiscardGame();
        }

        void OnClickBtnPlayProp()
        {
            mParam?.reviveAction.Invoke(true);
            MiniGameManager.Instance.TriggerEventGameRevive(mCacheType);
            Close();
        }

        void OnClickBtnPlayCoin()
        {
            if (mParam != null)
            {
                bool tIsf = ModuleManager.Prop.ExpendProp(PropID.Coin, mReplayCoin);
                if (tIsf)
                {
                    Close();
                    MiniGameManager.Instance.RetryGame();
                }
                else
                {
                    PageManager.Instance.OpenPage(PageID.ShopPage, new ShopPageParam(ShopPageParam.ShopGroup.CoinFirst));
                }
            }
            else
            {
                Close();
            }
        }
        #endregion
    }

    public class MiniFailedPageParam
    {
        public PropID usePropID;
        public bool isValid;
        public Action<bool> reviveAction;
        public MiniFailedPageParam(PropID pPropID, bool pIsValid, Action<bool> pAction)
        {
            usePropID = pPropID;
            isValid = pIsValid;
            reviveAction = pAction;
        }
    }
}
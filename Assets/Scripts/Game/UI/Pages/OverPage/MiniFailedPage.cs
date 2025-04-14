using Config;
using Game.UISystem;
using System;
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

            mParam = PageParam as MiniFailedPageParam;
            if (mParam == null)
            {
                LogManager.LogError("MiniFailedPage: invalid param");
                return;
            } 

            var tGameType = MiniGameManager.Instance.GameType;
            var tUsePropID = mParam.usePropID;
            bool tIsValid = mParam.isValid; 
            if (tIsValid && tUsePropID != PropID.Invalid)
            {
                _txtDescribe.text = tGameType switch
                {
                    MiniGameType.Screw => $"Add 1 extra hole to Play on!",
                    MiniGameType.Jam3d => $"Move 3 blocks to the back up area to Play on!",
                    MiniGameType.Tile => $" !",
                    MiniGameType.Bus => $" !",
                    MiniGameType.Triple => $" !",
                    _ => $"Use an item to continue the game!",
                };
                _btnClose.gameObject.SetActive(true);

                var tHasProp = ModuleManager.Prop.HasProp(tUsePropID);
                if (tHasProp)
                {
                    _btnPlayProp.imgIcon.SetPropIcon(tUsePropID);
                    _btnPlayProp.txtIcon.text = "USE 1";
                    _btnAbandon.gameObject.SetActive(false);
                    _btnPlayProp.gameObject.SetActive(true);
                    _btnPlayCoin.gameObject.SetActive(false);
                }
                else
                {
                    _btnPlayCoin.imgIcon.SetPropIcon(PropID.Coin);
                    _btnPlayCoin.txtIcon.text = ModuleManager.MiniGame.GetCurLevelConfig().ReplayCoin.ToString();
                    _btnAbandon.gameObject.SetActive(false);
                    _btnPlayProp.gameObject.SetActive(false);
                    _btnPlayCoin.gameObject.SetActive(true);
                }
            }
            else
            {
                _txtDescribe.text = "Oops! Your backup area is full. \r\nPlease restart the level and have another go!";

                _btnClose.gameObject.SetActive(false);
                _btnAbandon.gameObject.SetActive(true);
                _btnPlayProp.gameObject.SetActive(false);
                _btnPlayCoin.gameObject.SetActive(false);
            }  

            LayoutRebuilder.ForceRebuildLayoutImmediate(_failedRoot);
        }


        #region UI事件

        void OnClickBtnClose()
        {
            _txtDescribe.text = "Oops! Your backup area is full. \r\nPlease restart the level and have another go!";

            _btnClose.gameObject.SetActive(false);
            _btnAbandon.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_failedRoot);
        }

        void OnClickBtnAbandon()
        {
            ModuleManager.Prop.ExpendProp(PropID.Energy);
            PageManager.Instance.OpenPage(PageID.HomePage);
            MiniGameManager.Instance.UnloadCurTypeScene();
        }

        void OnClickBtnPlayProp()
        {
            Close();
            mParam?.reviveAction.Invoke();
        }

        void OnClickBtnPlayCoin()
        {
            Close();
            mParam?.reviveAction.Invoke();
        }
        #endregion
    }

    public class MiniFailedPageParam
    {
        public PropID usePropID;
        public bool isValid;
        public Action reviveAction;
        public MiniFailedPageParam(PropID pPropID, bool pIsValid, Action pAction)
        {
            usePropID = pPropID;
            isValid = pIsValid;
            reviveAction = pAction;
        }
    }
}
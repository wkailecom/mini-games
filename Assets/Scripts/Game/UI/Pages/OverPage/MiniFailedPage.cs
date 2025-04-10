using Config;
using Game.UISystem;
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
        [SerializeField] private Button _btnAbandon;
        [SerializeField] private Button _btnPlayProp;
        [SerializeField] private Button _btnPlayCoin;

        MiniFailedParam mParam;
        protected override void OnInit()
        {
            _btnClose.onClick.AddListener(OnClickBtnClose);
            _btnAbandon.onClick.AddListener(OnClickBtnAbandon);
            _btnPlayProp.onClick.AddListener(OnClickBtnPlayProp);
            _btnPlayCoin.onClick.AddListener(OnClickBtnPlayCoin);
        }

        protected override void OnBeginOpen()
        { 
            AudioManager.Instance.PlaySound(SoundID.Tile_Level_Failed);

            mParam = PageParam as MiniFailedParam;

            //if (mParam == null)
            //{
            //    LogManager.LogError("MiniFailedPage: invalid param");
            //    return;
            //}

            _btnClose.gameObject.SetActive(true);
            _btnAbandon.gameObject.SetActive(false);
            _btnPlayProp.gameObject.SetActive(true);
            _btnPlayCoin.gameObject.SetActive(true);
        }


        #region UI事件

        void OnClickBtnClose()
        { 
            _btnClose.gameObject.SetActive(false);
            _btnAbandon.gameObject.SetActive(true);
            _btnPlayProp.gameObject.SetActive(false);
            _btnPlayCoin.gameObject.SetActive(false);
        }

        void OnClickBtnAbandon()
        {
            ModuleManager.Prop.ExpendProp(PropID.Energy);
            PageManager.Instance.OpenPage(PageID.HomePage);
            MiniGameManager.Instance.UnloadScene(MiniGameManager.Instance.GameType.ToString());
        }
        
        void OnClickBtnPlayProp()
        {

        }

        void OnClickBtnPlayCoin()
        {

        }
        #endregion
    }

    public class MiniFailedParam
    {


    }
}
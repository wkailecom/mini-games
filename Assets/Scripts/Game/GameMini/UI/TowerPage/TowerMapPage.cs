using Config;
using DG.Tweening;
using Game.UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.TileGame
{
    public class TowerMapPage : PageBase
    {
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Button _btnHelp;
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnPlay;

        [SerializeField] private ScrollRect _mapScroll;
        [SerializeField] private UICountDown _timeCountDown;

        protected override void OnInit()
        {
            _btnHelp.onClick.AddListener(OnClickBtnHelp);
            _btnClose.onClick.AddListener(OnClickBtnClose);
            _btnPlay.onClick.AddListener(OnClickBtnPlay);
        }

        protected override void OnBeginOpen()
        {
            base.OnBeginOpen();

        }

        #region UI事件

        void OnClickBtnHelp()
        {
            PageManager.Instance.OpenPage(PageID.TowerRulePage);
        }

        void OnClickBtnClose()
        {
            PageManager.Instance.OpenPage(PageID.MiniMapPage);
        }

        void OnClickBtnPlay()
        {

        }
        #endregion
    }
}

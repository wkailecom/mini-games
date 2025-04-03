using Config;
using Game.UISystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MiniEnterPage : PageBase
    {
        [SerializeField] private Button _btnReturn;
        [SerializeField] private Button _btnLike;
        [SerializeField] private RectTransform _nodeTitle;
        [SerializeField] private RectTransform _nodeIcon;
        [SerializeField] private RectTransform _nodeCommon;

        [SerializeField] private Button _btnLevel;

        private UISwitch _switnLike;

        protected override void OnInit()
        {
            _switnLike = _btnLike.GetComponentInChildren<UISwitch>(true);

            _btnReturn.onClick.AddListener(Close);
            _btnLike.onClick.AddListener(OnClickBtnLike);
            _btnLevel.onClick.AddListener(OnClickBtnLevel);
        }

        protected override void OnBeginOpen()
        {

            _switnLike.SetSwitch(false);
        }


        #region UI事件

        void OnClickBtnLike()
        {
            bool tIsLike = false;

            _switnLike.SetSwitch(tIsLike);
        }

        void OnClickBtnLevel()
        {
             
        }

        #endregion
    }
}

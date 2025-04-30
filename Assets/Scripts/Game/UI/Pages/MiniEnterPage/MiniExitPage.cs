using Config;
using Game.UISystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class MiniExitPage : PageBase
    {
        [SerializeField] private Button _btnLeft;
        [SerializeField] private Button _btnRight;

        [SerializeField] private TextMeshProUGUI _txtLevel;
        [SerializeField] private TextMeshProUGUI _txtDescribe;

        bool mIsClickLeft;
        protected override void OnInit()
        {
            _btnLeft.onClick.AddListener(OnClickBtnLeft);
            _btnRight.onClick.AddListener(OnClickBtnRight);
        }

        protected override void OnBeginOpen()
        {
            mIsClickLeft = false;
            var tCurLevel = MiniGameManager.Instance.Level;
            _txtLevel.text = $"LEVEL {tCurLevel}";
            _txtDescribe.text = "You will lose 1 life!";
        }


        #region UI事件

        void OnClickBtnLeft()
        {
            if (mIsClickLeft)
            {
                ExitGame();
            }
            else
            {
                if (ModuleManager.MiniTower.ActivityValid())
                {
                    _txtDescribe.text = $"You'll lose one balloon once you give up. You have {ModuleManager.MiniTower.CurBalloon} balloon(s) now.";
                    mIsClickLeft = true;
                }
                else
                {
                    ExitGame();
                }
            }
        }

        void ExitGame()
        {
            MiniGameManager.Instance.DiscardGame();
        }

        void OnClickBtnRight()
        {
            Close();
        }

        #endregion
    }

}

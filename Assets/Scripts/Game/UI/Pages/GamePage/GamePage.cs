using Config;
using DG.Tweening;
using Game.UISystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GamePage : PageBase
    {
        [SerializeField] private Button _btnSet;
        [SerializeField] private Button _btnHome;
        [SerializeField] private Button _btnTutorial;
        [SerializeField] private UIBtnHeart _btnHeart;
        [SerializeField] private UIBtnRemoveads _btnAD;
        [SerializeField] private TextMeshProUGUI _textLevel;
        [SerializeField] private GameObject _propButsRoot;


        Sequence _announcerSeq;
        protected override void OnInit()
        {
            _btnSet.onClick.AddListener(OnOpenSettingsPage);
            _btnHome.onClick.AddListener(OnOpenHomePage);
            _btnTutorial.onClick.AddListener(OnOpenTutorialPage);
            _btnHeart.buttonRoot.onClick.AddListener(OnClickUIHeart);
            _btnAD.btnBuy.onClick.AddListener(OnClickUIRemoveads);


        }

        protected override void RegisterEvents()
        {
            EventManager.Register(EventKey.PropCountChange, OnPropCountChange);
            EventManager.Register(EventKey.GameStart, OnGameStart);
            EventManager.Register(EventKey.GameOver, OnGameOver);
        }

        protected override void UnregisterEvents()
        {
            EventManager.Unregister(EventKey.PropCountChange, OnPropCountChange);
            EventManager.Unregister(EventKey.GameStart, OnGameStart);
            EventManager.Unregister(EventKey.GameOver, OnGameOver);
        }

        protected override void OnBeginOpen()
        {
             
        } 

        void OnPropCountChange(EventData pEventData)
        {
            var tEventData = pEventData as PropCountChange;
            if (tEventData.propID == PropID.RemoveAD)
            {
                _btnAD.gameObject.SetActive(!GameMethod.HasRemoveAD());
            }

        } 
         
        void OnGameStart(EventData pEventData)
        {
            var tEventData = pEventData as GameStart;
            var tModeType = tEventData.gameModeType;
            var tLevelID = tEventData.levelID;

          
        }

        void OnGameOver(EventData pEventData)
        {
            var tEventData = pEventData as GameOver;
 
        } 

        #region UI事件

        void OnOpenHomePage()
        {
            GameMethod.TriggerUIAction(UIActionName.ReturnHome, UIPageName.PageGame, UIActionType.Click);
            PageManager.Instance.OpenPage(PageID.HomePage);
        }

        void OnOpenSettingsPage()
        {
            GameMethod.TriggerUIAction(UIActionName.Settings, UIPageName.PageGame, UIActionType.Click);
            PageManager.Instance.OpenPage(PageID.SettingsPage);
        }

        void OnOpenTutorialPage()
        {
            GameMethod.TriggerUIAction(UIActionName.ClickTutorial, UIPageName.PageGame, UIActionType.Click);
            PageManager.Instance.OpenPage(PageID.TutorialPage);
        }

        void OnClickUIHeart()
        {
            GameMethod.TriggerUIAction(UIActionName.AddHearts, UIPageName.PageGame, UIActionType.Click);
        }

        void OnClickUIRemoveads()
        {
            GameMethod.TriggerUIAction(UIActionName.RemoveAds, UIPageName.PageGame, UIActionType.Click);
        }

        #endregion
    }
}

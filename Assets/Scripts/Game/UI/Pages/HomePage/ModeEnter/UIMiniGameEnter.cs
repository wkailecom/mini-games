//using Config;
//using Game;
//using Game.MiniGame;
//using Game.UI;
//using Game.UISystem;
//using System;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//public class UIMiniGameEnter : MonoBehaviour, IHomeEnter
//{
//    public Button BtnPlay;
//    public UICountDown CountDown;
//    public Image[] ImgIcons;

//    private void Awake()
//    {
//        BtnPlay.onClick.AddListener(OnOpenMiniGame);
//    }

//    public void Init(bool pIsUnlock)
//    {
//        if (!pIsUnlock)
//        {
//            gameObject.SetActive(false);
//        }
//        else
//        {
//            var tIsUnderway = ModuleManager.MiniGame.IsUnderway();
//            if (!ModuleManager.MiniGame.IsCompleted && tIsUnderway)
//            {
//                SetContent();
//            }
//            else
//            {
//                CheckNewIssue();
//            }
//        }
//    }

//    void CheckNewIssue()
//    {
//        var tEventsID = ModuleManager.MiniGame.GetNewIssue();
//        if (tEventsID > 0)
//        {
//            ModuleManager.MiniGame.StartNewIssue(tEventsID);
//            SetContent();
//        }
//        else
//        {
//            gameObject.SetActive(false);
//        }
//    }

//    void SetContent()
//    {
//        gameObject.SetActive(true);
//        var tGameType = ModuleManager.MiniGame.GameType;
//        var tEndTime = ModuleManager.MiniGame.EndTime;
//        for (int i = 0; i < ImgIcons.Length; i++)
//        {
//            ImgIcons[i].gameObject.SetActive(i + 1 == (int)tGameType);
//        }
//        CountDown.StartCountDown(tEndTime, "Finished", CheckNewIssue, true);
//        TryShowGuide();
//    }

//    public void TryShowGuide()
//    {
//        //if (!DataTool.GetBool(DataKey.GuideHeart)) return;

//        if (!DataTool.GetBool(MiniGameConst.Guide_Open) && ModuleManager.MiniGame.IsUnderway())
//        {
//            PopTaskManager.Instance.AddTask(PageID.MiniGuidePage, () =>
//            {
//                DataTool.SetBool(MiniGameConst.Guide_Open, true);
//                PageManager.Instance.OpenPage(PageID.MiniGuidePage, MiniGameConst.Guide_Open);
//            });
//        }
//    }

//    void OnOpenMiniGame()
//    {
//        GameMethod.TriggerUIAction(UIActionName.EnterMiniGame, UIPageName.PageHome, UIActionType.Click);
//        PageManager.Instance.OpenPage(PageID.MiniMapPage);
//    }
//}

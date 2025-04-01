using Config;
using Game;
using Game.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameOverPage : PageBase
{
    public GameObject successRoot;
    public Button successBtn;
    public UIRewardItem rewardItem;
    public GameObject describeRoot;
    public ScrollRect scrollRect;

    public GameObject failRoot;
    public Button failBtn;

    List<UIRewardItem> mPropItems = new List<UIRewardItem>();
    MiniGameOverPageParam mParam;

    protected override void OnInit()
    {
        base.OnInit();
        successBtn.onClick.AddListener(OnClickClaimBtn);
        failBtn.onClick.AddListener(OnClickClaimBtn);
    }

    protected override void OnBeginOpen()
    {
        base.OnBeginOpen();
        successRoot.SetActive(false);
        failRoot.SetActive(false);

        mParam = PageParam as MiniGameOverPageParam;
        if (mParam == null)
        {
            Debug.LogError("MiniGameOverPage.OnBeginOpen:mParam is null");
            return;
        }

        MiniGameManager.Instance.UnloadScene(MiniGameManager.Instance.GameType.ToString());
        if (mParam.isSuccess)
        {
            SetSuccessRoot();
            successRoot.SetActive(true);
            AudioManager.Instance.PlaySound(SoundID.Tile_Level_Succeed);
        }
        else
        {
            failRoot.SetActive(true);
            AudioManager.Instance.PlaySound(SoundID.Tile_Level_Failed);
        }
        if (mParam.level >= MiniGameConst.AD_OPEN_LEVEL)
        {
            ADManager.Instance.ShowInterstitial(ADShowReason.Interstitial_MiniGameOver);
        }
    }

    void SetSuccessRoot()
    {
        var tConfig = ModuleManager.MiniGame.GetLevelConfig(mParam.level);
        var tRewards = ModuleManager.MiniGame.GetLevelReward(tConfig.LevelReward);

        if (tRewards != null && tRewards.Count > 0)
        {
            describeRoot.gameObject.SetActive(false);
            scrollRect.gameObject.SetActive(true);

            mPropItems.SetItemsActive(tRewards.Count, rewardItem, scrollRect.content);
            for (int i = 0; i < tRewards.Count; i++)
            {
                mPropItems[i].SetData(tRewards[i]);
            }
            ModuleManager.Prop.AddProps(tRewards, PropSource.MiniGameOver);

            TryShowGuide();
        }
        else
        {
            describeRoot.gameObject.SetActive(true);
            scrollRect.gameObject.SetActive(false);
        }
    }

    private void OnClickClaimBtn()
    {
        if (ModuleManager.MiniGame.IsUnderway())
        {
            PageManager.Instance.OpenPage(PageID.MiniMapPage);
        }
        else
        {
            PageManager.Instance.OpenPage(PageID.HomePage);
        }
    }

    private void TryShowGuide()
    {
        if (!DataTool.GetBool(MiniGameConst.Guide_OverRewards))
        {
            DataTool.SetBool(MiniGameConst.Guide_OverRewards, true);
            PageManager.Instance.OpenPage(PageID.MiniGuidePage, MiniGameConst.Guide_OverRewards);
        }
    }
}

public class MiniGameOverPageParam
{
    public int level;
    public bool isSuccess;

    public MiniGameOverPageParam(int pLevel, bool pIsSuccess)
    {
        level = pLevel;
        isSuccess = pIsSuccess;
    }


}
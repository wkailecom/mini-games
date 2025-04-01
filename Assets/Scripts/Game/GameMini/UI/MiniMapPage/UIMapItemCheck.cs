using Config;
using Game;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MiniGame
{
    public class UIMapItemCheck : UILevelItem
    {
        //    public GameObject giftLock;

        //    public GameObject giftUnlock;

        //    public GameObject giftOpened;

        //    public GameObject giftOpen;

        //    public Transform MaskImage;

        //    public UITileMapGiftProp uiGiftProp;

        //    private List<PropData> rewards;

        //    // Start is called before the first frame update
        //    void Start()
        //    {

        //    }

        //    public void InitRewards(Transform transform)
        //    {
        //        //初始化奖励id
        //        rewards = ModuleManager.TileGame.GetLevelReward(levelConfig.CheckpointReward);
        //        uiGiftProp.Init(rewards);
        //    }

        //    public void SetGiftState(GiftState state)
        //    {
        //        if (itemType != ItemType.Check)
        //        {
        //            return;
        //        }
        //        uiGiftProp.SetActive(false);
        //        giftUnlock.SetActive(false);
        //        giftLock.SetActive(false);
        //        giftOpened.SetActive(false);
        //        giftOpen.SetActive(false);
        //        switch (state)
        //        {
        //            case GiftState.Unlock:
        //                giftUnlock.SetActive(true);
        //                break;
        //            case GiftState.Lock:
        //                giftLock.SetActive(true);
        //                break;
        //            case GiftState.Opened:
        //                giftOpened.SetActive(true);
        //                break;
        //        }
        //    }

        //    public void PreviewRewards()
        //    {
        //        uiGiftProp.SetActive(!uiGiftProp.gameObject.activeSelf);
        //    }

        //    public void OpenGift()
        //    {
        //        giftUnlock.SetActive(false);
        //        giftLock.SetActive(false);
        //        giftOpened.SetActive(false);
        //        giftOpen.SetActive(true);
        //        GetGameReward();
        //        //AudioManager.Instance.PlaySound(SoundID.Tile_GiftBox_Open);
        //    }

        //    private void GetGameReward()
        //    {
        //        ////奖励获取界面
        //        //var tPageParam = new RewardPageParam(rewards, PropSource.TileNodalReward);
        //        //tPageParam.SetAutoClaim();

        //        //PageManager.Instance.OpenPage(PageID.RewardPage, tPageParam);

    }


    //    public enum GiftState
    //    {
    //        Lock,
    //        Unlock,
    //        Opened,
    //    }
    //}
}
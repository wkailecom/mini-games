using Config;
using Game.UISystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class MiniSucceedPage : PageBase
    {
        [SerializeField] private TextMeshProUGUI _txtLevel;

        [SerializeField] private Button _btnReturn;
        [SerializeField] private Button _btnNext;
        [SerializeField] private UIRewardItem _rewardItem;

        [SerializeField] private RectTransform _successRoot;
        [SerializeField] private RectTransform _nodeBalloon;
        [SerializeField] private RectTransform _nodeReward;
        [SerializeField] private RectTransform _levelReward;

        List<UIRewardItem> mNodeItems;
        List<UIRewardItem> mLevelItems;

        List<PropData> mNodeRewards;
        List<PropData> mLevelRewards;

        MiniSucceedPageParam mParam;
        protected override void OnInit()
        {
            mNodeItems = new List<UIRewardItem>();
            mLevelItems = new List<UIRewardItem>();

            _btnReturn.onClick.AddListener(OnClickBtnReturn);
            _btnNext.onClick.AddListener(OnClickBtnNext);
        }

        protected override void OnBeginOpen()
        {
            mParam = PageParam as MiniSucceedPageParam;
            AudioManager.Instance.PlaySound(SoundID.Tile_Level_Succeed);
            _txtLevel.text = $"LEVEL {ModuleManager.MiniGame.GetCurLevel((int)MiniGameManager.Instance.GameType)}";

            if (mParam == null)
            {
                LogManager.LogError("MiniSucceedPage: invalid param");
                return;
            }

            var tConfig = ModuleManager.MiniGame.GetCurLevelConfig();
            var tRewards = ModuleManager.MiniGame.GetLevelReward(tConfig.LevelReward);
            mNodeRewards = tRewards;
            mLevelRewards = tRewards;

            if (mNodeRewards.Count > 0)
            {
                mNodeItems.SetItemsActive(mNodeRewards.Count, _rewardItem, _nodeReward);
                for (int i = 0; i < mNodeRewards.Count; i++)
                {
                    mNodeItems[i].SetData(mNodeRewards[i]);
                }

                _nodeBalloon.gameObject.SetActive(false);
                _nodeReward.gameObject.SetActive(true);
            }
            else
            {
                _nodeBalloon.gameObject.SetActive(true);
                _nodeReward.gameObject.SetActive(false);
            }

            if (mLevelRewards.Count > 0)
            {
                mLevelItems.SetItemsActive(mLevelRewards.Count, _rewardItem, _levelReward);
                for (int i = 0; i < mLevelRewards.Count; i++)
                {
                    mLevelItems[i].SetData(mLevelRewards[i]);
                }
                _levelReward.gameObject.SetActive(true);
            }
            else
            {
                _levelReward.gameObject.SetActive(false);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(_successRoot);
        }


        #region UI事件


        void OnClickBtnReturn()
        {
            MiniGameManager.Instance.UnloadCurTypeScene();
            PageManager.Instance.OpenPage(PageID.HomePage);
        }

        void OnClickBtnNext()
        {
            var tGameType = MiniGameManager.Instance.GameType;
            var tCurLevel = ModuleManager.MiniGame.GetCurLevel((int)tGameType);
            MiniGameManager.Instance.StartGame(tGameType, tCurLevel);
        }

        void OnClickSavePhoto()
        {
            ////应用平台判断，路径选择
            //if (Application.platform == RuntimePlatform.Android)
            //{
            //    string destination = "/mnt/sdcard/DCIM/";
            //    if (!Directory.Exists(destination))
            //    {
            //        Directory.CreateDirectory(destination);
            //    }
            //    destination = destination + "/" + "TestUnityScreenCapture";
            //    File.WriteAllBytes(destination, screenCapture.sprite.texture.GetRawTextureData());
            //}
        }

        #endregion
    }

    public class MiniSucceedPageParam
    {


    }
}
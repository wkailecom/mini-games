using Config;
using Game.UISystem;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MiniSucceedPage : PageBase
    {
        [SerializeField] private TextMeshProUGUI _txtLevel;

        [SerializeField] private Button _btnReturn;
        [SerializeField] private Button _btnNext;
        [SerializeField] private UIRewardItem _rewardItem;

        [SerializeField] private RectTransform _nodeBalloon;
        [SerializeField] private RectTransform _nodeReward;
        [SerializeField] private RectTransform _levelReward;

        List<UIRewardItem> mNodeItems;
        List<UIRewardItem> mLevelItems;

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

            if (mParam == null)
            {
                LogManager.LogError("MiniSucceedPage: invalid param");
                return;
            }

            if (mParam.nodeRewards.Count > 0)
            {
                mNodeItems.SetItemsActive(mParam.nodeRewards.Count, _rewardItem, _nodeReward);
                for (int i = 0; i < mParam.nodeRewards.Count; i++)
                {
                    mNodeItems[i].SetData(mParam.nodeRewards[i]);
                }

                _nodeBalloon.gameObject.SetActive(false);
                _nodeReward.gameObject.SetActive(true);
            }
            else
            {
                _nodeBalloon.gameObject.SetActive(true);
                _nodeReward.gameObject.SetActive(false);
            }

            if (mParam.levelRewards.Count > 0)
            {
                mLevelItems.SetItemsActive(mParam.levelRewards.Count, _rewardItem, _levelReward);
                for (int i = 0; i < mParam.levelRewards.Count; i++)
                {
                    mNodeItems[i].SetData(mParam.levelRewards[i]);
                }
                _levelReward.gameObject.SetActive(true);
            }
            else
            {
                _levelReward.gameObject.SetActive(false);
            }
        }


        #region UI事件


        void OnClickBtnReturn()
        {
            PageManager.Instance.OpenPage(PageID.HomePage);
        }

        void OnClickBtnNext()
        {
            // PageManager.Instance.OpenPage(PageID.MiniEnterPage);
            //MiniGameManager.Instance.StartGame(mGameType, mCurLevel);
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
        public List<PropData> levelRewards = new List<PropData>();
        public List<PropData> nodeRewards = new List<PropData>();

    }
}
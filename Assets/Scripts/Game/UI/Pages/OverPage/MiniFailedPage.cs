using Game.UISystem;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MiniFailedPage : PageBase
    {
        [SerializeField] private TextMeshProUGUI _txtLevel;

        [SerializeField] private Button _btnReturn;
        [SerializeField] private Button _btnNext;
        [SerializeField] private UIRewardItem _rewardItem;

        [SerializeField] private RectTransform _nodeBalloon;
        [SerializeField] private RectTransform _nodeReward;
        [SerializeField] private RectTransform _levelReward;

        List<UIRewardItem> mPropItems;

        MiniFailedParam mParam;
        protected override void OnInit()
        {
            mPropItems = new List<UIRewardItem>();

            _btnReturn.onClick.AddListener(OnClickBtnReturn);
            _btnNext.onClick.AddListener(OnClickBtnNext);
        }

        protected override void OnBeginOpen()
        {
            mParam = PageParam as MiniFailedParam;

            if (mParam == null)
            {
                LogManager.LogError("MiniFailedPage: invalid param");
                return;
            }

            //mPropItems.SetItemsActive(mParam.rewards.Count, rewardItem, contentRoot);
        }


        #region UI事件

        void OnClickBtnReturn()
        {

        }

        void OnClickBtnNext()
        {

        }

        #endregion
    }

    public class MiniFailedParam
    {


    }
}
using Config;
using Game;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class RewardPage : PageBase
    {
        public Text title;
        public Text tips;
        public UIRewardItem rewardItem;
        public RectTransform contentRoot;
        public Button claimButton;
        public Button doubleButton;


        List<UIRewardItem> mPropItems = new List<UIRewardItem>();
        bool mDoubleRewardVideoWatched;
        Dictionary<int, int> rewards = new Dictionary<int, int>();
        RewardPageParam mParam;
        protected override void OnInit()
        {
            base.OnInit();
            claimButton.onClick.AddListener(OnClickClaimButton);
            doubleButton.onClick.AddListener(OnClickDoubleButton);

            rewardItem.gameObject.SetActive(false);
        }

        protected override void OnBeginOpen()
        {
            mParam = PageParam as RewardPageParam;
            if (mParam == null)
            {
                LogManager.LogError("RewardPage.OnBeginOpen: PageParam is Invalid");
                return;
            }

            title.text = mParam.title;
            SetProps();
            SetButton();
            if (mParam.needGet && mParam.autoClaim)
            {
                ModuleManager.Prop.AddProps(mParam.rewards, mParam.source);
            }
            if (mParam.autoClose)
            {
                StartCoroutine(AutoCloseTask());
            }
        }

        void SetProps()
        {
            mPropItems.SetItemsActive(mParam.rewards.Count, rewardItem, contentRoot);

            for (int i = 0; i < mParam.rewards.Count; i++)
            {
                mPropItems[i].SetData(mParam.rewards[i]);
            }
        }

        void SetButton()
        {
            //claimButton.gameObject.SetActive(!mParam.autoClaim);
            doubleButton.gameObject.SetActive(!mParam.autoClaim && mParam.canDouble);
        }

        IEnumerator AutoCloseTask(float pWaitTime = 1)
        {
            yield return new WaitForSeconds(pWaitTime);
            Close();
        }

        void ClaimProps()
        {
            if (mParam == null)
            {
                Close();
                return;
            }

            if (mParam.needGet && !mParam.autoClaim)
            {
                ModuleManager.Prop.AddProps(mParam.rewards, mParam.source);
            }
            Close();
        }

        IEnumerator ClaimMultipleProps(int pMultiple = 2)
        {
            if (mParam == null)
            {
                Close();
                yield break;
            }

            foreach (var tProp in mParam.rewards)
            {
                tProp.AddCount(tProp.Count * (pMultiple - 1));
            }
            SetProps();

            yield return new WaitForSeconds(0.5f);
            ClaimProps();
        }

        void OnClickClaimButton()
        {
            ClaimProps();

            if (mParam == null) return;
            mParam.ConfirmAction?.Invoke();
        }

        void OnClickDoubleButton()
        {
            if (ADManager.Instance.IsRewardVideoReady)
            {
                ADManager.Instance.ShowRewardVideo(ADShowReason.Video_RewardDoubled);
            }
            else
            {
                MessageHelp.Instance.ShowMessage("no internet connection");
            }
        }

        void OnVideoADRewarded(EventData pEventData)
        {
            StartCoroutine(ClaimMultipleProps(2));
        }

    }

}
public class RewardPageParam
{
    public List<PropData> rewards { get; private set; }
    public PropSource source { get; private set; }
    public string title { get; private set; }
    public bool needGet { get; private set; }
    public bool canDouble { get; private set; }
    public bool autoClaim { get; private set; }
    public bool autoClose { get; private set; }

    public UnityAction ConfirmAction;

    public RewardPageParam(PropID pRewardId, int pRewardCount, PropSource pSource)
        : this(new PropData(pRewardId, pRewardCount), pSource) { }

    public RewardPageParam(PropData pReward, PropSource pSource)
        : this(new List<PropData>() { pReward }, pSource) { }

    public RewardPageParam(List<PropData> pRewards, PropSource pSource)
    {
        title = "SPLENDID!";
        rewards = pRewards;
        source = pSource;
        needGet = false;
    }

    public void SetTitle(string pTextID)
    {
        //title = TextTool.GetText(pTextID);
    }

    public void SetCanDouble()
    {
        needGet = true;
        canDouble = true;
        autoClaim = false;
        autoClose = false;
    }

    public void SetAutoClaim()
    {
        needGet = true;
        canDouble = false;
        autoClaim = true;
    }

    public void SetAutoClose()
    {
        canDouble = false;
        autoClaim = true;
        autoClose = true;
    }
}
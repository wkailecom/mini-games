using LLFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AdEvents : MonoSingleton<AdEvents>
{
    private const string Param_Platform = "platform";
    private const string Param_AdId = "id";
    private const string Param_SubId = "sub_id";
    private const string Param_Country = "country";
    private const string Param_Revenue = "revenue";
    private const string Param_AdType = "type";
    public const string Param_AdTypeBanner = "banner";
    public const string Param_AdTypeReward = "reward_video";
    public const string Param_AdTypeInterstitial = "interstitial";

    #region Event Action
    private static event Action<string, string, string, string> _onAdLoadedEvent;
    private static event Action<string, string, string, string> _onAdDisplayedEvent; // banner点击展示以后会调用
    private static event Action<string, string, string, string> _onAdClosedEvent;
    private static event Action<string, string, string, string> _onAdDisplayFailedEvent;
    private static event Action<string, string, string, string> _onVideoAdRewardedEvent;
    //private static event Action<string, string, string, string> _onBanerRevenueEvent; // banner刷新调用
    private static event Action<string, string, string, string, string, string> _onAdForecastRevenueEvent; //广告预估价值

    // 广告加载完毕
    public static event Action<string, string, string, string> OnAdLoadedEvent
    {
        add
        {
            if (_onAdLoadedEvent == null || !_onAdLoadedEvent.GetInvocationList().Contains(value))
            {
                _onAdLoadedEvent += value;
            }
        }

        remove
        {
            if (_onAdLoadedEvent != null && _onAdLoadedEvent.GetInvocationList().Contains(value))
            {
                _onAdLoadedEvent -= value;
            }
        }
    }

    // 广告打开
    // 参数顺序: type platform id sub_id
    // type用Param_AdTypeBanner Param_AdTypeReward Param_AdTypeInterstitial来判断是哪个类型
    public static event Action<string, string, string, string> OnAdDisplayedEvent
    {
        add
        {
            if (_onAdDisplayedEvent == null || !_onAdDisplayedEvent.GetInvocationList().Contains(value))
            {
                _onAdDisplayedEvent += value;
            }
        }

        remove
        {
            if (_onAdDisplayedEvent != null && _onAdDisplayedEvent.GetInvocationList().Contains(value))
            {
                _onAdDisplayedEvent -= value;
            }
        }
    }

    // 广告关闭
    // 参数顺序: type platform id sub_id
    public static event Action<string, string, string, string> OnAdClosedEvent
    {
        add
        {
            if (_onAdClosedEvent == null || !_onAdClosedEvent.GetInvocationList().Contains(value))
            {
                _onAdClosedEvent += value;
            }
        }

        remove
        {
            if (_onAdClosedEvent != null && _onAdClosedEvent.GetInvocationList().Contains(value))
            {
                _onAdClosedEvent -= value;
            }
        }
    }

    // 广告打开失败
    // 参数顺序: type platform id sub_id
    public static event Action<string, string, string, string> OnAdDisplayFailedEvent
    {
        add
        {
            if (_onAdDisplayFailedEvent == null || !_onAdDisplayFailedEvent.GetInvocationList().Contains(value))
            {
                _onAdDisplayFailedEvent += value;
            }
        }

        remove
        {
            if (_onAdDisplayFailedEvent != null && _onAdDisplayFailedEvent.GetInvocationList().Contains(value))
            {
                _onAdDisplayFailedEvent -= value;
            }
        }
    }

    // 获得激励视频奖励
    // 参数顺序: type platform id sub_id
    public static event Action<string, string, string, string> OnVideoAdRewardedEvent
    {
        add
        {
            if (_onVideoAdRewardedEvent == null || !_onVideoAdRewardedEvent.GetInvocationList().Contains(value))
            {
                _onVideoAdRewardedEvent += value;
            }
        }

        remove
        {
            if (_onVideoAdRewardedEvent != null && _onVideoAdRewardedEvent.GetInvocationList().Contains(value))
            {
                _onVideoAdRewardedEvent -= value;
            }
        }
    }

    //// banner刷新 
    //// 参数顺序: type platform id sub_id
    //public static event Action<string, string, string, string> OnBanerRevenueEvent
    //{
    //    add
    //    {
    //        if (_onBanerRevenueEvent == null || !_onBanerRevenueEvent.GetInvocationList().Contains(value))
    //        {
    //            _onBanerRevenueEvent += value;
    //        }
    //    }

    //    remove
    //    {
    //        if (_onBanerRevenueEvent != null && _onBanerRevenueEvent.GetInvocationList().Contains(value))
    //        {
    //            _onBanerRevenueEvent -= value;
    //        }
    //    }
    //}

    // 获取广告预估价值
    // 参数顺序: type platform id sub_id
    public static event Action<string, string, string, string, string, string> OnAdForecastRevenueEvent
    {
        add
        {
            if (_onAdForecastRevenueEvent == null || !_onAdForecastRevenueEvent.GetInvocationList().Contains(value))
            {
                _onAdForecastRevenueEvent += value;
            }
        }

        remove
        {
            if (_onAdForecastRevenueEvent != null && _onAdForecastRevenueEvent.GetInvocationList().Contains(value))
            {
                _onAdForecastRevenueEvent -= value;
            }
        }
    }

    #endregion

    // ******************************* Helper methods *******************************	
    public Param ParseParam(string paramJson)
    {
        Param param = new Param();
        if (!string.IsNullOrEmpty(paramJson))
        {
            Dictionary<string, string> dic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(paramJson);
            if (dic != null)
            {
                if (dic.ContainsKey(Param_Platform)) param.platform = dic[Param_Platform];
                if (dic.ContainsKey(Param_AdType)) param.adType = dic[Param_AdType];
                if (dic.ContainsKey(Param_AdId)) param.id = dic[Param_AdId];
                if (dic.ContainsKey(Param_SubId)) param.subId = dic[Param_SubId];
                if (dic.ContainsKey(Param_Country)) param.country = dic[Param_Country];
                if (dic.ContainsKey(Param_Revenue)) param.revenue = dic[Param_Revenue];
            }
        }

        return param;
    }


    // ******************************* ADMOB *******************************	

    #region ADMOB

    /**
     * 广告展示成功的回调Action
     * */
    public void onAdLoaded(string paramJson)
    {
        Param param = ParseParam(paramJson);
        Debug.Log("untiy_original_ad - onAdDisplayed: " + param.adType + " " + param.platform + " " + param.id + " " + param.subId);
        if (_onAdLoadedEvent != null)
        {
            _onAdLoadedEvent(param.adType, param.platform, param.id, param.subId);
        }
    }

    /**
     * 广告展示成功的回调Action
     * */
    public void onAdDisplayed(string paramJson)
    {
        Param param = ParseParam(paramJson);
        Debug.Log("untiy_original_ad - onAdDisplayed: " + param.adType + " " + param.platform + " " + param.id + " " + param.subId);
        if (_onAdDisplayedEvent != null)
        {
            _onAdDisplayedEvent(param.adType, param.platform, param.id, param.subId);
        }
    }

    public void onAdClosed(string paramJson)
    {
        Param param = ParseParam(paramJson);
        Debug.Log("untiy_original_ad - onAdClosed: " + param.adType + " " + param.platform + " " + param.id + " " + param.subId);
        if (param != null)
        {
            if (_onAdClosedEvent != null)
            {
                _onAdClosedEvent(param.adType, param.platform, param.id, param.subId);
            }
        }
    }

    /**
     * 广告展示失败的回调Action
     * */
    public void onAdFailedToDisplay(string paramJson)
    {
        Param param = ParseParam(paramJson);
        Debug.Log("untiy_original_ad - onAdFailedToDisplay: " + param.adType + " " + param.platform + " " + param.id + " " + param.subId);
        if (_onAdDisplayFailedEvent != null)
        {
            _onAdDisplayFailedEvent(param.adType, param.platform, param.id, param.subId);
        }
    }


    /**
     *激励视频获得奖励的回调
     * */
    public void onUserEarnedReward(string paramJson)
    {
        Param param = ParseParam(paramJson);
        Debug.Log("untiy_original_ad - onUserEarnedReward: " + param.adType + " " + param.platform + " " + param.id + " " + param.subId);
        if (_onVideoAdRewardedEvent != null)
        {
            _onVideoAdRewardedEvent(param.adType, param.platform, param.id, param.subId);
        }
    }

    /**
     * 广告预估价值的回调
     * */
    public void didPayRevenueForAd(string paramJson)
    {
        Param param = ParseParam(paramJson);
        //Debug.Log("untiy_original_ad - didPayRevenueForAd: " + Newtonsoft.Json.JsonConvert.SerializeObject(param));
        if (_onAdForecastRevenueEvent != null)
        {
            _onAdForecastRevenueEvent(param.adType, param.platform, param.id, param.subId, param.country, param.revenue);
        }
    }

    #endregion

    public class Param
    {
        public string platform = ""; // 广告来源渠道 admob facebook等
        public string adType = ""; // 广告类型 banner interstitial reward_video
        public string id = ""; // 当前播放广告的id
        public string subId = ""; // 当前广告的子id，一般没用
        public string country = ""; // 当前国家
        public string revenue = ""; // 广告价值
    }
}

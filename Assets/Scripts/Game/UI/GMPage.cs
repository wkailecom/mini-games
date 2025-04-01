using Config;
using Game;
using Game.UISystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GMPage : PageBase
{
    public Transform outputParent;
    public Transform commandParent;
    public Text outputAsset;
    public GameObject commandAsset;

#if GM_MODE
    Dictionary<Text, (string title, Func<string> content)> mOutputs = new Dictionary<Text, (string title, Func<string> content)>();
    Dictionary<GameObject, InputField> mCommands = new Dictionary<GameObject, InputField>();

    protected override void OnInit()
    {
        base.OnInit();

        outputAsset.gameObject.SetActive(false);
        commandAsset.SetActive(false);

        #region Output
        AddOutput("广告", () => "\n    是否购买去广告项了： " + (GameMethod.HasRemoveAD() ? "是" : "否") +
                                "\n    视频是否准备好：" + (ADManager.Instance.IsRewardVideoReady ? "是" : "否") +
                                "\n    插屏是否准备好：" + (ADManager.Instance.IsInterstitialReady ? "是" : "否") +
                                "\n    Banner是否准备好：" + (ADManager.Instance.IsBannerReady ? "是" : "否"));
        AddOutput("支付", () => "\n    是否跳过支付验单： " + (GameVariable.IsSkipPurchasValidate ? "是" : "否") +
                                "\n    是否测试设备：" + (GameVariable.IsTestDevice ? "是" : "否"));
        AddOutput("用户ID", () => AppInfoManager.Instance.UserID);
        AddOutput("用户分组", () => AppInfoManager.Instance.UserGroupName);
        AddOutput("ATT状态", () => NativeUtil.GetATTState().ToString());
        AddOutput("当前国家", () => AppInfoManager.Instance.DeviceCountry);
        AddOutput("安装时间", () => ModuleManager.UserInfo.FirstLoginTime.ToString());
        AddOutput("安装天数", () => ModuleManager.UserInfo.InstallDaysCount.ToString());
        AddOutput("首次安装版本", () => AppInfoManager.Instance.FirstVersion);

        #endregion

        #region Command
        //AddCommand("复制FCM令牌", (pParam) =>
        //{
        //    GUIUtility.systemCopyBuffer = FirebaseManager.Instance.FCMToken;
        //});
        AddCommand("切换AB用户", (pParam) =>
        {
            AppInfoManager.Instance.SwitchUserGroup();
            Refresh();
        });
        AddCommand("切换国家", (pParam) =>
        {
            var cultures = System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures);
            // 用于存储国家缩写的列表
            List<string> countryCodes = new List<string>();
            //遍历每个CultureInfo，获取国家缩写
            foreach (System.Globalization.CultureInfo culture in cultures)
            {
                var region = new System.Globalization.RegionInfo(culture.Name);
                if (!countryCodes.Contains(region.TwoLetterISORegionName))
                {
                    countryCodes.Add(region.TwoLetterISORegionName);
                }
            }
            var tCountry = pParam.ToUpper();
            if (countryCodes.Contains(tCountry))
            {
                AppInfoManager.Instance.SwitchCountry(tCountry);
                Refresh();
            }
            else
            {
                LogManager.LogError("国家不存在，请输入正确国家缩写");
            }
        });

        AddCommand("一键过关", (pParam) =>
        {
            if (GameManager.Instance.GameStart)
            {
                Close();
                GameManager.Instance.FinishGame(true);
            }
        });
        AddCommand("增加体力", (pParam) =>
        {
            if (!int.TryParse(pParam, out int tCount))
            {
                tCount = 1;
            }
            ModuleManager.Prop.AddProp(PropID.Health, tCount, PropSource.Unknown);
        });
        AddCommand("增加提示道具", (pParam) =>
        {
            if (!int.TryParse(pParam, out int tCount))
            {
                tCount = 1;
            }
            ModuleManager.Prop.AddProp(PropID.Hint, tCount, PropSource.Unknown);
        });

        AddCommand("小游戏跳关", (pParam) =>
        {
            if (int.TryParse(pParam, out int tCount))
            {
                tCount = tCount >= ModuleManager.MiniGame.MaxLevel ? ModuleManager.MiniGame.MaxLevel : tCount;
                ModuleManager.MiniGame.InfoData.CurrentLevel = tCount;
                ModuleManager.MiniGame.SyncLevel();
            }
        });


        AddCommand("移除去广告", (pParam) =>
        {
            ModuleManager.Prop.ExpendProp(PropID.RemoveAD);
            Refresh();
        });
        AddCommand("获得去广告", (pParam) =>
        {
            ModuleManager.Prop.AddProp(PropID.RemoveAD, 1, PropSource.Unknown);
            Refresh();
        });
        AddCommand("展示激励", (pParam) => { ADManager.Instance.ShowRewardVideo(ADShowReason.Video_GMCommand); });
        AddCommand("展示插屏", (pParam) => { ADManager.Instance.ShowInterstitial(ADShowReason.Interstitial_GMCommand); });
        AddCommand("展示Banner", (pParam) => { ADManager.Instance.ShowBanner(); });
        AddCommand("隐藏Banner", (pParam) => { ADManager.Instance.HideBanner(); });
        AddCommand("打开广告调试", (pParam) => { NativeUtil.ShowTestTool(); });
        //AddCommand("打开日志输出", (pParam) =>
        //{
        //    LunarConsolePlugin.LunarConsole.Show();
        //});
        AddCommand("是否跳过支付验单", (pParam) =>
        {
            GameVariable.IsSkipPurchasValidate = !GameVariable.IsSkipPurchasValidate;
            Refresh();
        }); 
        AddCommand("ECPM多次打点", (pParam) =>
        {
            if (int.TryParse(pParam, out int tCount))
            {
                if (tCount > 0)
                {
                    FirebaseReportManager.GMLogECPMEvent(ADType.Invalid, tCount);
                }
            }

        });


        #endregion
    }

    protected override void OnBeginOpen()
    {
        base.OnBeginOpen();
        Refresh();
    }

    void AddOutput(string pTitle, Func<string> pOutput)
    {
        var tText = Instantiate(outputAsset, outputParent);
        tText.gameObject.SetActive(true);

        mOutputs.Add(tText, (pTitle, pOutput));
    }

    void AddCommand(string pTitle, Action<string> pCommand)
    {
        GameObject tCommandObject = Instantiate(commandAsset, commandParent);
        tCommandObject.SetActive(true);

        var tInputField = tCommandObject.transform.Find("InputField").GetComponent<InputField>();

        var tButton = tCommandObject.transform.Find("Button").GetComponent<Button>();
        tButton.onClick.AddListener(() => { pCommand.Invoke(tInputField.text); });

        var tButtonText = tButton.transform.Find("Text").GetComponent<Text>();
        tButtonText.text = pTitle;

        mCommands.Add(tCommandObject, tInputField);
    }

    void Refresh()
    {
        foreach (var tOutput in mOutputs)
        {
            tOutput.Key.text = tOutput.Value.title + ": " + tOutput.Value.content.Invoke();
            LayoutRebuilder.ForceRebuildLayoutImmediate(tOutput.Key.rectTransform);
        }
    }

    public void OnClickBtnRefresh()
    {
        Refresh();
    }
#endif
}

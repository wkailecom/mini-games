using Config;
using DG.Tweening;
using Game.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SettingsPage : PageBase
    {
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnHome;

        [SerializeField] private Button _btnMusic;
        [SerializeField] private Button _btnSound;
        //[SerializeField] private Button _btnVibration;
        [SerializeField] private Button _btnRestore;

        [Space]
        [SerializeField] private TextMeshProUGUI _txtVersion;
        [SerializeField] private Button _btnGm;

        private UISwitch _switnMusic;
        private UISwitch _switnSound;
        private UISwitch _switnVibration;

        Color _fontDefaultColor = new Color32(173, 192, 214, 255);
        Color _fontSelectColor = new Color32(87, 106, 143, 255);

        bool mIsCouncil;
        float mFontSize = 1;
        string mCurPage;
        protected override void OnInit()
        {
            _switnMusic = _btnMusic.GetComponentInChildren<UISwitch>(true);
            _switnSound = _btnSound.GetComponentInChildren<UISwitch>(true);
            //_switnVibration = _btnVibration.GetComponentInChildren<UISwitch>(true);

            _btnClose.onClick.AddListener(Close);
            _btnHome.onClick.AddListener(OnClickHome);

            _btnMusic.onClick.AddListener(OnClickMusic);
            _btnSound.onClick.AddListener(OnClickSound);
            //_btnVibration.onClick.AddListener(OnClickVibration);
            _btnRestore.onClick.AddListener(OnClickRestore);

            _txtVersion.GetComponent<Button>().onClick.AddListener(OnClickVersion);
        }

        protected override void OnBeginOpen()
        {
            _txtVersion.text = AppInfoManager.Instance.AppVersion;
            mIsCouncil = PageManager.Instance.IsOpen(PageID.GamePage);
            mCurPage = mIsCouncil ? UIPageName.PopupSettingsIngame : UIPageName.PopupSettings;

            _switnMusic.SetSwitch(AudioManager.Instance.MusicSwitch);
            _switnSound.SetSwitch(AudioManager.Instance.SoundSwitch);
            //_switnVibration.SetSwitch(AudioManager.Instance.VibrateSwitch);

        }

        string AnroidEmailSubject => Application.productName + "_Android_" + Application.version;
        string IosEmailSubject => Application.productName + "_Ios_" + Application.version;

        void SendEmail()
        {
            string[] tMailTo = new string[] { APPDefine.email };
#if UNITY_EDITOR
            Debug.LogError("真机测试！！");
#elif UNITY_ANDROID
            AndroidJavaClass tIntentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject tIntentObject = new AndroidJavaObject("android.content.Intent");
            tIntentObject.Call<AndroidJavaObject>("setAction", tIntentClass.GetStatic<string>("ACTION_SEND"));
            tIntentObject.Call<AndroidJavaObject>("putExtra", tIntentClass.GetStatic<string>("EXTRA_EMAIL"), tMailTo);
            tIntentObject.Call<AndroidJavaObject>("putExtra", tIntentClass.GetStatic<string>("EXTRA_SUBJECT"), AnroidEmailSubject);
            tIntentObject.Call<AndroidJavaObject>("putExtra", tIntentClass.GetStatic<string>("EXTRA_TEXT"), "Default Content");
            tIntentObject.Call<AndroidJavaObject>("setType", "plain/text");

            AndroidJavaClass tUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject tCurrentActivity = tUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            tCurrentActivity.Call("startActivity", tIntentObject);
#elif UNITY_IOS
            Uri uri = new Uri(string.Format("mailto:{0}?subject={1}", APPDefine.email, IosEmailSubject));
            Application.OpenURL(uri.AbsoluteUri);
#endif
        }


        #region UI事件

        void OnClickContact()
        {
            GameMethod.TriggerUIAction(UIActionName.Contact, mCurPage, UIActionType.Click);
            SendEmail();
        }

        void OnClickTermsOfService()
        {
            Application.OpenURL(APPDefine.termsOfServiceURL);
        }

        void OnClickPrivacyPolicy()
        {
            Application.OpenURL(APPDefine.pricacyPolicyURL);
        }

        void OnClickRestore()
        {
            GameMethod.TriggerUIAction(UIActionName.RestoreBuy, mCurPage, UIActionType.Click);
            IAPManager.Instance.RestorePurchases();
        }

        void OnClickMusic()
        {
            AudioManager.Instance.MusicSwitch = !AudioManager.Instance.MusicSwitch;
            _switnMusic.SetSwitch(AudioManager.Instance.MusicSwitch);
            GameMethod.TriggerUIAction(UIActionName.Sound, mCurPage, AudioManager.Instance.MusicSwitch ? UIActionType.Show : UIActionType.Close);
        }

        void OnClickSound()
        {
            AudioManager.Instance.SoundSwitch = !AudioManager.Instance.SoundSwitch;
            _switnSound.SetSwitch(AudioManager.Instance.SoundSwitch);
            GameMethod.TriggerUIAction(UIActionName.Sound, mCurPage, AudioManager.Instance.SoundSwitch ? UIActionType.Show : UIActionType.Close);
        }

        void OnClickVibration()
        {
            AudioManager.Instance.VibrateSwitch = !AudioManager.Instance.VibrateSwitch;
            _switnVibration.SetSwitch(AudioManager.Instance.VibrateSwitch);
            GameMethod.TriggerUIAction(UIActionName.Vibration, mCurPage, AudioManager.Instance.VibrateSwitch ? UIActionType.Show : UIActionType.Close);
        }

        void OnClickHome()
        {
            GameMethod.TriggerUIAction(UIActionName.ReturnHome, mCurPage, UIActionType.Click, ADType.Interstitial);
            ADManager.Instance.PlayInterstitial(ADShowReason.Interstitial_ReturnHome);
            PageManager.Instance.OpenPage(PageID.HomePage);
        }


        public void OnClickVersion()
        {
            GUIUtility.systemCopyBuffer = AppInfoManager.Instance.UserID;
#if GM_MODE
            Close();
            Instantiate(_btnGm, UIRoot.Instance.transform);
#endif
        }

        #endregion
    }
}

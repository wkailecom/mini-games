using Config;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.U2D; 
using LLFramework;
using Game;

public enum Language
{
    English,
    Portuguese_Brazil,
    Chinese,
    German,
    French,
    Japanese,
}

public class LanguageManager : Singleton<LanguageManager>
{
    const string LANGUAGE_TYPE_KEY = "LanguageType";
    const string KeyNotFound = "[{0}]";

    public static Language CurrentLanguages
    {
        get => (Language)PlayerPrefs.GetInt(LANGUAGE_TYPE_KEY, 0);
        set => PlayerPrefs.SetInt(LANGUAGE_TYPE_KEY, (int)value);
    }

    private Language selectedLanguage;
    public Language SelectedLanguage
    {
        get
        {
            return selectedLanguage;
        }
        set
        {
            if (value != selectedLanguage)
            {
                selectedLanguage = value;
                EventManager.Trigger(EventKey.ChangeLanguage);
                InvokeOnLocalize();
            }
        }
    }

    public void SaveLanguage() => CurrentLanguages = selectedLanguage;

    public void Init()
    {
        if (PlayerPrefs.GetInt(LANGUAGE_TYPE_KEY, -1) == -1)
        {
            var tSystemLanguage = Application.systemLanguage;
            var value = tSystemLanguage switch
            {
                SystemLanguage.English => Language.English,
                SystemLanguage.Portuguese => Language.Portuguese_Brazil,
                SystemLanguage.Chinese => Language.Chinese,
                SystemLanguage.ChineseSimplified => Language.Chinese,
                SystemLanguage.ChineseTraditional => Language.Chinese,
                SystemLanguage.German => Language.German,
                SystemLanguage.French => Language.French,
                SystemLanguage.Japanese => Language.Japanese,
                _ => Language.English
            };
#if UNITY_EDITOR
            value= Language.English;
#endif
            PlayerPrefs.SetInt(LANGUAGE_TYPE_KEY, (int)value);
        }

        selectedLanguage = CurrentLanguages;
    }


    [Header("更改语言时调用的事件")]
    [Tooltip("每次更改所选语言时都会调用此事件。")]
    public UnityEvent Localize = new UnityEvent();

    public void InvokeOnLocalize()
    {
        Localize?.Invoke();
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            //  var localized = FindObjectsOfType<LocalizedText>();
            //foreach (var local in localized)
            //{
            //    local.OnLocalize();
            //}
        }
#endif
    }

    public void AddOnLocalizeEvent(ILocalize localize)
    {
        Localize.RemoveListener(localize.OnLocalize);
        Localize.AddListener(localize.OnLocalize);
        localize.OnLocalize();
    }

    public void RemoveOnLocalizeEvent(ILocalize localize)
    {
        Localize.RemoveListener(localize.OnLocalize);
    }



    public TextsConfig GetTextConfig(string key)
    {
        if (ConfigData.textsConfig.DataList == null)
        {
            LogManager.Log("编辑器初始化text表"); 
            ConfigData.textsConfig.LoadData(GameConst.CONFIG_ROOT_PATH);
        }

        return ConfigData.textsConfig.GetByPrimary(key);
    }

    public static string GetText(string key)
    {
        return GetText(key, Instance.selectedLanguage);
    }

    public static string GetText(string key, Language language)
    {
        if (string.IsNullOrEmpty(key))
        {
            LogManager.LogError("key param is null!");
            return string.Empty;
        }

        var tTextConfig = Instance.GetTextConfig(key);
        if (tTextConfig == null)
        {
            return string.Format(KeyNotFound, key);
        }

        return GetLanguagesString(tTextConfig, language);
    }

    public static string GetFormatText(string key, params object[] arguments)
    {
        if (string.IsNullOrEmpty(key) || arguments == null || arguments.Length == 0)
        {
            return GetText(key);
        }

        return string.Format(GetText(key), arguments);
    }

    static string GetLanguagesString(TextsConfig pTextConfig, Language pLanguage)
    {
        return pLanguage switch
        {
            Language.English => pTextConfig.EN_US,
            Language.Chinese => pTextConfig.ZH,
            Language.Portuguese_Brazil => pTextConfig.PT_BR,
            Language.German => pTextConfig.DE_DE,
            Language.French => pTextConfig.FR,
            Language.Japanese => pTextConfig.JA,
            _ => pTextConfig.EN_US,
        };

    }

    public string GetLanguageAbbr(Language pLanguage)
    {
        return pLanguage switch
        {
            Language.English => "En",
            Language.Chinese => "Zh",
            Language.Portuguese_Brazil => "Pt",
            Language.German => "De",
            Language.French => "Fr",
            Language.Japanese => "Ja",
            _ => "En",
        };
    }

    SpriteAtlas LanguageTextAtlas;
    public Sprite GetSprite(string pSpriteName)
    {
        LanguageTextAtlas ??= ResTool.Load<SpriteAtlas>("Atlas/LanguageText");
        var tLanguageNmae = GetLanguageAbbr(Instance.selectedLanguage);
        return LanguageTextAtlas.GetSprite($"{tLanguageNmae}_{pSpriteName}");
    }

}


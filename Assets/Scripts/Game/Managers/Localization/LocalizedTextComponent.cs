using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public abstract class LocalizedTextComponent<T> : MonoBehaviour, ILocalize where T : Component
{
    private T text;

    [Tooltip("本地化的Key")]
    [SerializeField]
    private string key;
    public string Key
    {
        get { return key; }
        set
        {
            key = value;
            OnLocalize();
        }
    }

    public List<object> Parameters => parameters;

    private readonly List<object> parameters = new List<object>();

    public void Awake()
    {
        text = GetComponent<T>();
    }

    //[UsedImplicitly]
    //public void Reset()
    //{
    //    text = GetComponent<T>();
    //}

    public void OnEnable()
    {
        LanguageManager.Instance.AddOnLocalizeEvent(this);
    }

    public void OnDisable()
    {
        LanguageManager.Instance.RemoveOnLocalizeEvent(this);
    }

    protected abstract void SetText(T component, string value);


    public void OnLocalize()
    {
#if UNITY_EDITOR
        var flags = text != null ? text.hideFlags : HideFlags.None;
        if (text != null) text.hideFlags = HideFlags.DontSave;
#endif
        if (text == null)
        {
            Debug.LogWarning("Missing Text Component on " + gameObject.name, gameObject);
            return;
        }

        if (parameters != null && parameters.Count > 0)
        {
            SetText(text, LanguageManager.GetFormatText(key, parameters.ToArray()));
        }
        else
        {
            SetText(text, LanguageManager.GetText(key));
        }

#if UNITY_EDITOR
        if (text != null) text.hideFlags = flags;
#endif
    }


    public void ClearParameters()
    {
        parameters.Clear();
    }

    public void AddParameter(object parameter)
    {
        parameters.Add(parameter);
        OnLocalize();
    }
    public void AddParameter(int parameter)
    {
        AddParameter((object)parameter);
    }
    public void AddParameter(float parameter)
    {
        AddParameter((object)parameter);
    }
    public void AddParameter(string parameter)
    {
        AddParameter((object)parameter);
    }
    public void SetParameters(params object[] parameters)
    {
        ClearParameters();
        this.parameters.AddRange(parameters);
        OnLocalize();
    }
}
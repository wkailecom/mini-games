using UnityEngine;
using UnityEngine.UI;


[AddComponentMenu("UI/Localized Image", 11)]
[RequireComponent(typeof(Image))]
public class LocalizedImage : MonoBehaviour, ILocalize
{
    Image image;
    string imageName;

    void Awake()
    {
        image = GetComponent<Image>();
        var tName = image.sprite.name.Split('_');
        if (tName.Length > 1)
        {
            imageName = tName[1];
        }
    }

    public void OnEnable()
    {
        LanguageManager.Instance.AddOnLocalizeEvent(this);
    }

    public void OnDisable()
    {
        LanguageManager.Instance.RemoveOnLocalizeEvent(this);
    }

    public void OnLocalize()
    {
        var tSprite = LanguageManager.Instance.GetSprite(imageName);
        if (tSprite != null)
        {
            image.sprite = tSprite;
        }
    }



}
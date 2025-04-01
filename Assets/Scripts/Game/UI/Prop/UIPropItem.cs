using Config;
using UnityEngine;
using UnityEngine.UI;

public class UIPropItem : MonoBehaviour
{
    public Image propIcon;
    public Text propName;
    public Text propCount;
    public string propCountFormat;

    protected PropData mData;
    protected PropConfig mConfig;

    public void SetData(PropData pData)
    {
        mData = pData;
        if (mData == null)
        {
            LogManager.LogError("UIProp.SetData: param is null!");
            return;
        }

        mConfig = ConfigData.propConfig.GetByPrimary((int)mData.ID);
        if (mConfig == null)
        {
            LogManager.LogError($"UIProp.SetData: get config return null! ID - {(int)mData.ID}");
            return;
        }

        SetIcon();
        SetPropName();
        SetPropCount();
    }

    void SetIcon()
    {
        if (propIcon != null)
        {
            propIcon.SetPropIcon(mConfig.icon, false);
        }
    }

    void SetPropName()
    {
        if (propName != null)
        {
            propName.text = mConfig.propID;
        }
    }

    void SetPropCount()
    {
        if (propCount == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(propCountFormat))
        {
            propCount.text = mData.Count.ToString();
        }
        else
        {
            propCount.text = string.Format(propCountFormat, mData.Count.ToString());
        }
    }
}
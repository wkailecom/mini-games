using Config;
using Game;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

interface IHomeEnter
{
    void Init(bool pIsUnlock);

}

public class UIDailyEnter : MonoBehaviour, IHomeEnter
{
    public GameObject Unlocked;
    public GameObject Locked;
    public Button BtnDaily;
    public TextMeshProUGUI TxtLocked;

    private void Awake()
    {
        BtnDaily.onClick.AddListener(OnOpenDaily);
    }

    public void Init(bool pIsUnlock)
    {
         
        Unlocked.SetActive(pIsUnlock);
        Locked.SetActive(!pIsUnlock);
        gameObject.SetActive(true);
    }

    void OnOpenDaily()
    {
        GameMethod.TriggerUIAction(UIActionName.EnterDaily, UIPageName.PageHome, UIActionType.Click);
        PageManager.Instance.OpenPage(PageID.DailyPage);
    }
}

using Config;
using Game;
using Game.MiniGame;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMiniHeart : MonoBehaviour
{
    public Button heartBtn;
    public UICountDown countDown;
    public GameObject addRoot;

    private void Awake()
    {
        heartBtn.onClick.AddListener(OpenGetADHealth);
    }

    private void OnEnable()
    {
        EventManager.Register(EventKey.PropCountChange, OnPropCountChange);

        CheckCountDown();
    }

    private void OnDisable()
    {
        EventManager.Unregister(EventKey.PropCountChange, OnPropCountChange);

    }

    void OnPropCountChange(EventData pEventData)
    {
        var tEventData = pEventData as PropCountChange;
        if (tEventData.propID == PropID.Health && tEventData.changedCount > 0)
        {
            CheckCountDown();
        }
    }

    void CheckCountDown()
    {
        var tIsFull = GameMethod.IsFullHealth();
        if (tIsFull)
        {
            addRoot.SetActive(false);
            countDown.StopCountDown(false, "FULL");
        }
        else
        {
            addRoot.SetActive(true);
            countDown.StartCountDown(ModuleManager.UserInfo.HealthHarvestTime, "Full");
        }
    }

    public void OpenGetADHealth()
    {
        //if (GameMethod.IsFullHealth())
        //{
        //    MessageHelp.Instance.ShowMessage("Your lives are full.");
        //}
        //else
        //{
            PageManager.Instance.OpenPage(PageID.AdsPropPopup, new AdsPropPageParam(PropID.Health, null));
        //}
    }

}

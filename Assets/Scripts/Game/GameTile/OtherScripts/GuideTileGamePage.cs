using DG.Tweening;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GuideTileGamePage : PageBase
{
    public GuideMask GuideMask;
    public Transform GuideTextRoot;
    public Text GuideText;
    public Transform GuideArrow;
    public Button MaskBut;

    GuideTileGamePageParam mParam;
    protected override void OnInit()
    {
        GuideMask.Init();
    }

    protected override void RegisterEvents()
    {
        //EventManager.Register(EventDataType.TileStartGame, OnTileStartGame);
        //EventManager.Register(EventDataType.TileCellClick, OnTileCellClick);
    }

    protected override void UnregisterEvents()
    {
        //EventManager.Unregister(EventDataType.TileStartGame, OnTileStartGame);
        //EventManager.Unregister(EventDataType.TileCellClick, OnTileCellClick);
    }

    protected override void OnBeginOpen()
    {
        SetGuide();
    }

    protected override void OnReopen()
    {
        SetGuide();
    }

    void OnTileCellClick(EventData pEventData)
    {
        //var tEventData = pEventData as TileCellClick;
        //mParam?.action?.Invoke();
    }

    void SetGuide()
    {
        mParam = PageParam as GuideTileGamePageParam;
        if (mParam == null)
        {
            Close();
            return;
        }

        GuideText.text = GetGuideText(mParam.guideID);

        if (mParam.targetNode == null)
        {
            GuideMask.gameObject.SetActive(false);
            GuideArrow.gameObject.SetActive(false);
            MaskBut.gameObject.SetActive(true);
            MaskBut.onClick.RemoveAllListeners();
            MaskBut.onClick.AddListener(() => { mParam?.action?.Invoke(); });
            GuideTextRoot.localPosition = new Vector3(0, -240f, 0);
            return;
        }

        GuideMask.gameObject.SetActive(true);
        GuideArrow.gameObject.SetActive(true);
        MaskBut.gameObject.SetActive(false);

        //if (mParam.guideID == 1 || (mParam.guideID >= 2 && mParam.guideID <= 10))
        //{
        //    MaskBut.gameObject.SetActive(true);
        //    MaskBut.onClick.RemoveAllListeners();
        //    MaskBut.onClick.AddListener(() =>
        //    {
        //        AudioManager.Instance.PlaySound(SoundID.Tile_Click);
        //        mParam?.action?.Invoke();
        //    });
        //}
        //var tTargetRect = GuideMask._targetArea;
        //GuideMask.Play(mParam.targetNode);
        //GuideArrow.localPosition = tTargetRect.localPosition;
        //GuideTextRoot.localPosition = new Vector3(0, tTargetRect.localPosition.y + 250f, 0);

        //var tTargetScale = 1;
        //if (mParam.guideID != 1)
        //{
        //    tTargetRect.sizeDelta = Vector2.one * 100;
        //    tTargetScale = 2;
        //}
        //tTargetRect.localScale = Vector3.one * (tTargetScale + 1);
        //tTargetRect.DOScale(tTargetScale, 0.5f);

    }

    string GetGuideText(int pGuideID)
    {
        return pGuideID switch
        {
            1 => "Welcome, Adventurer!\r\nLet's start your <color=#dd30f7>Tile Crush Journey</color>!",

            2 => "Now let's connect three cards together!<color=#dd30f7>Tap the card</color>!",
            3 => "Well done! <color=#dd30f7>Tap this card again</color>!",
            4 => "Finally,<color=#dd30f7>tap this card again</color>!",
            5 => "Awesome! Three cards are cleared! Now try to clear all cards.",

            6 => "Congratulations! You get three free items!",
            7 => "This is <color=#dd30f7>Recall</color>.\r\nIt recalls your latest step.",
            8 => "This is <color=#dd30f7>Magnet</color>.\r\nIt clears a set of tiles.",
            9 => "This is <color=#dd30f7>Shuffle</color>.\r\nIt rearranges the board.",
            10 => "Hope you enjoy the Adventure! Remember to use items when you get stuck.",
            _ => string.Empty
        };
    }
}

public class GuideTileGamePageParam
{
    public RectTransform targetNode;
    public int guideID;
    public UnityAction action;
}
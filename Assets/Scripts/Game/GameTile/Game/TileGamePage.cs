using Config;
using DG.Tweening;
using Game.UISystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TileGamePage : PageBase
{
    public List<Sprite> iconSprites = new List<Sprite>();

    public RectTransform AdapterPanel;
    public RectTransform FloorViewRoot;
    public RectTransform SlotViewRoot;
    public Transform SlotPoolRoot;

    public Text LevelText;
    //public UICountDown TimeCountDown;
    //public UICountDown HeartTimeCountDown;
    public Button ReturnBut;
    public Button ShopBut;
   // public UIMiniHeart HeartBut;

    public Button RecallBut;
    public Button MagnetBut;
    public Button ShuffleBut;
    public Button TileShopBut;

   // public TileCell CellTile;
    public TileCellSlot CellSlot;
    public RectTransform FloorRoot1;
    public RectTransform FloorRoot2;

    public Vector3 CellAnimCenter { get; private set; }

    bool mIsMoving;
    bool mIsHinting;
    bool mIsShuffleing;
    float mSlotCellSize = 0.85f;
    HorizontalLayoutGroup mSlotHGroup;
    float floorViewScale = 1;
    Vector2 floorViewRectDefaultPos;
    Vector2 tileCellDefaultScale = new Vector2(140, 145);

    MonoPool<TileCellSlot> SlotPool;
    List<FloorRootItem> mFloorRootList = new List<FloorRootItem>();
    List<TileCellSlot> mSlotItemList = new List<TileCellSlot>();
    List<GridLayoutGroup> mGridLayoutGroups = new List<GridLayoutGroup>();

    protected override void OnInit()
    {
        base.OnInit();

        SlotPool = new MonoPool<TileCellSlot>(CellSlot, transform, SlotPoolRoot);
        mSlotHGroup = SlotViewRoot.GetComponent<HorizontalLayoutGroup>();
        ReturnBut.onClick.AddListener(OnclickReturnButton);
        ShopBut.onClick.AddListener(OnclickShopBut);

        RecallBut.onClick.AddListener(OnclickRecallPropBut);
        MagnetBut.onClick.AddListener(OnclickMagnetPropBut);
        ShuffleBut.onClick.AddListener(OnclickShufflePropBut);
        TileShopBut.onClick.AddListener(OnclickTileShopBut);

        floorViewRectDefaultPos = FloorViewRoot.anchoredPosition;
        CellAnimCenter = FloorViewRoot.position;
    }

    protected override void RegisterEvents()
    {
        base.RegisterEvents();

        //EventManager.Register(EventDataType.TileCellClick, OnTileCellClick);
        //EventManager.Register(EventDataType.TileStartGame, OnTileStartGame);
        //EventManager.Register(EventDataType.PurchaseSuccess, OnPurchaseSuccess);
        //EventManager.Register(EventDataType.PropCountChange, OnPropCountChange);
        //EventManager.Register(EventDataType.VideoADRewarded, OnVideoADRewarded);
    }

    protected override void UnregisterEvents()
    {
        base.UnregisterEvents();

        //EventManager.Unregister(EventDataType.TileCellClick, OnTileCellClick);
        //EventManager.Unregister(EventDataType.TileStartGame, OnTileStartGame);
        //EventManager.Unregister(EventDataType.PurchaseSuccess, OnPurchaseSuccess);
        //EventManager.Unregister(EventDataType.PropCountChange, OnPropCountChange);
        //EventManager.Unregister(EventDataType.VideoADRewarded, OnVideoADRewarded);
    }

    protected override void OnBeginOpen()
    {
        RefreshAdapter();
        //TimeCountDown.Show(ModuleManager.TileGame.GetEndTime(), endStr: "Finished");
        InitFloorView();
        InitSlotView();
        RefreshPropRoot();
    }

    protected override void OnReopen()
    {
        RefreshState();
    }

    void RefreshState()
    {
        mIsMoving = false;
        mIsHinting = false;
        mIsShuffleing = false;
    }

    void OnPurchaseSuccess(EventData pEventData)
    {
        //var tEventData = pEventData as PurchaseSuccess;
        //var tConfig = tEventData.productConfig;
        //if (tConfig.category == (int)ShopItemCategory.RemoveAD)
        //{
        //    RefreshAdapter();
        //}
    }

    void OnPropCountChange(EventData pEventData)
    {
        var tEventData = pEventData as PropCountChange;
        RefreshPropRoot();
    }

    void OnVideoADRewarded(EventData pEventData)
    {
        //if (!IsOpen) return;

        //var tEventData = pEventData as ADEvent;
        //if (tEventData.ADType == ADType.RewardVideo)
        //{
        //    PropID tPropID = tEventData.showReason switch
        //    {
        //        ADShowReason.Video_TileRecall => PropID.TileRecall,
        //        ADShowReason.Video_TileMagnet => PropID.TileMagnet,
        //        ADShowReason.Video_TileShuffle => PropID.TileShuffle,
        //        _ => PropID.Invalid,
        //    };
        //    if (tPropID != PropID.Invalid)
        //    {
        //        var tPageParam = new RewardPageParam(tPropID, 1, PropSource.Rewarded);
        //        tPageParam.SetAutoClaim();
        //        tPageParam.ConfirmAction = () => { RefreshPropRoot(); };
        //        PageManager.Instance.OpenPage(PageID.RewardPage, tPageParam);
        //    }
        //}
    }

    void OnTileStartGame(EventData pEventData)
    {
        //var tEventData = pEventData as TileStartGame;
        //LevelText.text = TextTool.GetText("Com_Level", ModuleManager.TileGame.CurrentLevel);
        //AudioManager.Instance.PlaySound(SoundID.Tile_Level_Begin);

        //RefreshFloorView();
        //RefreshSlotView();
        //RefreshState();
        //RefreshFloorViewLayout();
        //CalculateFloorViewScale();
    }

    void OnTileCellClick(EventData pEventData)
    {
        //var tEventData = pEventData as TileCellClick;

        //PlaceCell(tEventData.cellData, tEventData.startPos);
    }

    void RefreshAdapter()
    {
        //float tOffsetY;
        //if (CommonMethod.HasRemoveAD())
        //{
        //    tOffsetY = UIRoot.SafeOffset.bottom;
        //}
        //else
        //{
        //    tOffsetY = UIRoot.BANNER_OFFSET_Y + UIRoot.SafeOffset.bottom;
        //}
        //AdapterPanel.offsetMin = new Vector2(AdapterPanel.offsetMin.x, tOffsetY);
        //AdapterPanel.offsetMax = new Vector2(AdapterPanel.offsetMax.x, -UIRoot.SafeOffset.top);
    }

    void PlaceCell(TileCellData pCellData, Vector3 pStartPos)
    {
        //var tInsertIndex = TileGameManager.Instance.SlotViewData.AddSlotData(pCellData);
        //if (tInsertIndex < 0) return;
        //pCellData?.SetState(TileCellState.Hide);
        //TileGameManager.Instance.FloorViewData.RefreshFloorsData();

        //var tSlotItem = AddSlotItem(tInsertIndex, pCellData.IconIndex);
        //tSlotItem.group.alpha = 0;

        //var tFlyItem = GetSlotItem();
        //tFlyItem.icon.sprite = iconSprites[pCellData.IconIndex];
        //tFlyItem.group.alpha = 1;
        //tFlyItem.rect.SetParent(transform);
        //tFlyItem.rect.position = pStartPos;
        //tFlyItem.rect.localScale = Vector3.one;

        ////float tPosX = 36 + (tInsertIndex - 3) * (36.5f + 100);
        ////var targetPos = new Vector3(tPosX, SlotViewRoot.position.y, SlotViewRoot.position.z);

        //AudioManager.Instance.PlaySound(SoundID.Tile_Brick_Click);
        //mIsMoving = true;
        //var seq = DOTween.Sequence();
        //seq.Insert(0, tFlyItem.transform.DOScale(mSlotCellSize, 0.2f));
        //seq.Insert(0, tFlyItem.transform.DOMove(tSlotItem.rect.position, 0.2f));
        //seq.OnComplete(() =>
        //{
        //    ReturnSlotItem(tFlyItem);
        //    tSlotItem.group.alpha = 1;
        //    var tIndexList = TileGameManager.Instance.SlotViewData.EraseSlotData();
        //    EraseSlotItem(tIndexList);
        //    TileGameManager.Instance.CheckGameStatus();
        //    mIsMoving = false;
        //});

    }

    public void InitFloorView()
    {

    }

    public void InitSlotView()
    {


    }

    public void RefreshPropRoot()
    {

        //HeartBut.addRoot.SetActive(!CommonMethod.IsFullTileHeart());

        //bool tHasRecall = ModuleManager.Prop.HasProp(PropID.TileRecall);
        //bool tHasMagnet = ModuleManager.Prop.HasProp(PropID.TileMagnet);
        //bool tHasShuffle = ModuleManager.Prop.HasProp(PropID.TileShuffle);

        //RecallBut.transform.GetChild(0).gameObject.SetActive(!tHasRecall);
        //RecallBut.transform.GetChild(1).gameObject.SetActive(tHasRecall);

        //MagnetBut.transform.GetChild(0).gameObject.SetActive(!tHasMagnet);
        //MagnetBut.transform.GetChild(1).gameObject.SetActive(tHasMagnet);

        //ShuffleBut.transform.GetChild(0).gameObject.SetActive(!tHasShuffle);
        //ShuffleBut.transform.GetChild(1).gameObject.SetActive(tHasShuffle);

    }

    public void RefreshFloorView()
    {
        //var tFloorViewData = TileGameManager.Instance.FloorViewData.floorsData;
        //int tDiffCount = tFloorViewData.Count - mFloorRootList.Count;
        //if (tDiffCount <= 0)
        //{
        //    for (int i = 0; i < mFloorRootList.Count; i++)
        //    {
        //        if (i < tFloorViewData.Count)
        //        {
        //            SetFloorRoot(i, tFloorViewData[i]);
        //            mFloorRootList[i].floorRoot.gameObject.SetActive(true);
        //        }
        //        else
        //        {
        //            mFloorRootList[i].floorRoot.gameObject.SetActive(false);
        //        }
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < mFloorRootList.Count; i++)
        //    {
        //        SetFloorRoot(i, tFloorViewData[i]);
        //        mFloorRootList[i].floorRoot.gameObject.SetActive(true);
        //    }

        //    var tRootCount = mFloorRootList.Count;
        //    for (int i = 0; i < tDiffCount; i++)
        //    {
        //        AddFloorRoot(tFloorViewData[tRootCount + i]);
        //    }
        //}

        //StartCoroutine(LicensingAnim());
    }

    //刷新FloorView的layout布局
    private void RefreshFloorViewLayout()
    {
        FloorViewRoot.localScale = Vector3.one;
        FloorViewRoot.anchoredPosition = floorViewRectDefaultPos;
        for (int i = 0; i < mGridLayoutGroups.Count; i++)
        {
            var tGroup = mGridLayoutGroups[i];
            if (tGroup == null) continue;
            tGroup.CalculateLayoutInputHorizontal();
            tGroup.CalculateLayoutInputVertical();
            tGroup.SetLayoutHorizontal();
            tGroup.SetLayoutVertical();
        }
    }

    //计算floorView缩放
    private void CalculateFloorViewScale()
    {
        //var dataIndex = ModuleManager.TileGame.GetOriginalDataIndex(ModuleManager.TileGame.CurrentLevel);
        Vector2 maxXY = new Vector2(float.MinValue, float.MinValue);
        Vector2 minXY = new Vector2(float.MaxValue, float.MaxValue);

        //for (int i = 0; i < dataIndex.Count; i++)
        //{
        //    var tShow = mFloorRootList[i].cellList;
        //    for (int j = 0; j < tShow.Count; j++)
        //    {
        //        if (!dataIndex[i].Contains(tShow[j].GetIndex())) continue;
        //        var tPos = tShow[j].transform.position;
        //        var tX = tPos.x;
        //        var tY = tPos.y;
        //        if (tX > maxXY.x)
        //            maxXY.x = tX;
        //        if (tY > maxXY.y)
        //            maxXY.y = tY;
        //        if (tX < minXY.x)
        //            minXY.x = tX;
        //        if (tY < minXY.y)
        //            minXY.y = tY;
        //    }
        //}

        Vector2 tCenterPoint = (minXY + maxXY) / 2;
        Vector2 tPosOffset = (Vector2)FloorViewRoot.position - tCenterPoint;

        Vector2 tCurSize = new Vector2(maxXY.x - minXY.x, maxXY.y - minXY.y);
        Vector3[] tCorners = new Vector3[4];
        FloorViewRoot.GetWorldCorners(tCorners);//从左下开始，顺时针
        Vector2 tViewSize = new Vector2(tCorners[2].x - tCorners[0].x, tCorners[1].y - tCorners[0].y);
        tCurSize += Vector2.one * 250;//预留洗牌空隙

        var tScale1 = tViewSize.x / tCurSize.x;
        var tScale2 = tViewSize.y / tCurSize.y;
        floorViewScale = Mathf.Min(tScale1, tScale2, 1.2f);

        FloorViewRoot.position += floorViewScale * (Vector3)tPosOffset;
        FloorViewRoot.localScale *= floorViewScale;

    }

    public void RefreshSlotView()
    {
        //foreach (var tSlot in mSlotItemList)
        //{
        //    ReturnSlotItem(tSlot);
        //}
        //mSlotItemList.Clear();
        //foreach (var tSlotData in TileGameManager.Instance.SlotViewData.slotsData)
        //{
        //    AddSlotItem(tSlotData);
        //}
        //LayoutRebuilder.ForceRebuildLayoutImmediate(SlotViewRoot);
    }

    public void EraseSlotItem(List<int> pEraseIndexList)
    {
        //int tindex = 0;
        //for (int i = pEraseIndexList.Count - 1; i >= 0; i--)
        //{
        //    mIsMoving = true;
        //    var tSlotItem = mSlotItemList[pEraseIndexList[i]];
        //    //tSlotItem.transform.DOScale(0, 0.5f).SetDelay(tindex * 0.05f).OnComplete(() =>
        //    //{
        //    //    AudioManager.Instance.PlaySound(SoundID.Tile_Brick_Eliminate);
        //    //    ReturnSlotItem(tSlotItem.gameObject);
        //    //    LayoutRebuilder.ForceRebuildLayoutImmediate(SlotViewRoot);
        //    //    mIsMoving = false;
        //    //});
        //    var targetPos = mSlotItemList[pEraseIndexList[0]].transform.position;
        //    if (i > 0)
        //    {
        //        targetPos = mSlotItemList[pEraseIndexList[i - 1]].transform.position;
        //    }
        //    tSlotItem.transform.DOMove(targetPos, 0.1f).SetDelay(tindex * 0.1f).OnComplete(() =>
        //    {
        //        AudioManager.Instance.PlaySound(SoundID.Tile_Brick_Eliminate);
        //        ReturnSlotItem(tSlotItem);
        //        LayoutRebuilder.ForceRebuildLayoutImmediate(SlotViewRoot);
        //        mIsMoving = false;
        //    });
        //    mSlotItemList.Remove(tSlotItem);
        //    ++tindex;
        //}
    }

    void AddFloorRoot(FloorData pFloorData)
    {
        int pFloorIndex = mFloorRootList.Count;
        var tFloorRoot = pFloorIndex % 2 != 0 ? FloorRoot1 : FloorRoot2;
        tFloorRoot = Instantiate(tFloorRoot, FloorViewRoot);

        var gridGroup = tFloorRoot.GetComponent<GridLayoutGroup>();
        mGridLayoutGroups.Add(gridGroup);

        //List<TileCell> tTileCellList = new List<TileCell>();
        //for (int i = 0; i < TileGameDefine.SHOW_ITEM_MAX_COUNT; i++)
        //{
        //    var tCellTile = Instantiate(CellTile, tFloorRoot);
        //    tCellTile.Init(pFloorData.floorData[i]);
        //    tTileCellList.Add(tCellTile);
        //}

        FloorRootItem tFloorRootItem = new FloorRootItem();
        tFloorRootItem.floor = pFloorIndex;
        tFloorRootItem.floorRoot = tFloorRoot;
        //tFloorRootItem.cellList = tTileCellList;
        mFloorRootList.Add(tFloorRootItem);
    }

    void SetFloorRoot(int pFloor, FloorData pFloorData)
    {
        //for (int i = 0; i < pFloorData.floorData.Count; i++)
        //{
        //    mFloorRootList[pFloor].cellList[i].Init(pFloorData.floorData[i]);
        //}
    }

    void AddSlotItem(TileSlotData pSlotItem)
    {
        var tSlotItem = GetSlotItem();
        tSlotItem.icon.sprite = iconSprites[pSlotItem.IconIndex];
        tSlotItem.rect.localScale = Vector3.one * mSlotCellSize;
        tSlotItem.rect.SetParent(SlotViewRoot);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(SlotViewRoot);
        mSlotItemList.Add(tSlotItem);
    }

    TileCellSlot AddSlotItem(int pInsertIndex, int pIconIndex)
    {
        var tSlotItem = GetSlotItem();
        tSlotItem.icon.sprite = iconSprites[pIconIndex];
        tSlotItem.rect.localScale = Vector3.one * mSlotCellSize;
        tSlotItem.rect.SetParent(SlotViewRoot);
        tSlotItem.rect.SetSiblingIndex(pInsertIndex);
        LayoutRebuilder.ForceRebuildLayoutImmediate(SlotViewRoot);
        mSlotItemList.Insert(pInsertIndex, tSlotItem);
        return tSlotItem;
    }

    IEnumerator LicensingAnim()
    {
        yield return new WaitForSeconds(0.02f);
        //InputLockManager.Instance.Lock("LicensingAnim");
        //for (int j = 0; j < mFloorRootList.Count; j++)
        //{
        //    for (int i = 0; i < mFloorRootList[j].cellList.Count; i++)
        //    {
        //        mFloorRootList[j].cellList[i].PlayLicensingAnim();
        //    }
        //}
        //yield return new WaitForSeconds(0.5f);
        //foreach (var item in mFloorRootList)
        //{
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(item.floorRoot);
        //}
        //InputLockManager.Instance.UnLock("LicensingAnim");

        //if (ModuleManager.TileGame.IsGuideClick)
        //{
        //    ModuleManager.TileGame.IsGuideStep = 3;
        //    SetGuide(2);
        //}
        //if (ModuleManager.TileGame.IsGuideProp)
        //{
        //    ModuleManager.Prop.AddProp(PropID.TileRecall, 1, PropSource.Unknown);
        //    ModuleManager.Prop.AddProp(PropID.TileMagnet, 1, PropSource.Unknown);
        //    ModuleManager.Prop.AddProp(PropID.TileShuffle, 1, PropSource.Unknown);
        //    ModuleManager.TileGame.IsGuideStep = 4;
        //    SetGuide(6);
        //}
    }

    public void SetGuide(int pGuideId)
    {
        //RectTransform tTargetNode = null;
        //UnityAction tAction = null;
        //if (pGuideId == 2)
        //{
        //    var tCell = mFloorRootList[2].cellList[28];
        //    tTargetNode = tCell.GetComponent<RectTransform>();
        //    tAction = () =>
        //    {
        //        tCell.OnPointerClick(null);
        //        SetGuide(++pGuideId);
        //    };
        //}
        //else if (pGuideId == 3)
        //{
        //    var tCell = mFloorRootList[2].cellList[33];
        //    tTargetNode = tCell.GetComponent<RectTransform>();
        //    tAction = () =>
        //    {
        //        tCell.OnPointerClick(null);
        //        SetGuide(++pGuideId);
        //    };
        //}
        //else if (pGuideId == 4)
        //{
        //    var tCell = mFloorRootList[2].cellList[38];
        //    tTargetNode = tCell.GetComponent<RectTransform>();
        //    tAction = () =>
        //    {
        //        tCell.OnPointerClick(null);
        //        SetGuide(++pGuideId);
        //    };
        //}
        //else if (pGuideId == 5)
        //{
        //    tTargetNode = null;
        //    //tAction = () => { PageManager.Instance.ClosePage(PageID.GuideTileGamePage); };
        //}
        //else if (pGuideId == 6)
        //{
        //    tTargetNode = null;
        //    tAction = () => { SetGuide(++pGuideId); };
        //}
        //else if (pGuideId == 7)
        //{
        //    tTargetNode = RecallBut.GetComponent<RectTransform>();
        //    tAction = () => { SetGuide(++pGuideId); };
        //}
        //else if (pGuideId == 8)
        //{
        //    tTargetNode = MagnetBut.GetComponent<RectTransform>();
        //    tAction = () => { SetGuide(++pGuideId); };
        //}
        //else if (pGuideId == 9)
        //{
        //    tTargetNode = ShuffleBut.GetComponent<RectTransform>();
        //    tAction = () => { SetGuide(++pGuideId); };
        //}
        //else if (pGuideId == 10)
        //{
        //    tTargetNode = null;
        //   // tAction = () => { PageManager.Instance.ClosePage(PageID.GuideTileGamePage); };
        //}

        //GuideTileGamePageParam tParam = new GuideTileGamePageParam()
        //{
        //    targetNode = tTargetNode,
        //    guideID = pGuideId,
        //    action = tAction
        //};
        //PageManager.Instance.OpenPage(PageID.GuideTileGamePage, tParam);



        //if (ModuleManager.TileGame.IsGuideClick)
        //{
        //    GuideTileGamePageParam tParam = new GuideTileGamePageParam()
        //    {
        //        targetNode = mFloorRootList[0].cellList[33].Icon.rectTransform,
        //        guideText = "Well done! Tap another one!"
        //    };
        //    PageManager.Instance.OpenPage(PageID.GuideTileGamePage, tParam);
        //}

        //if (ModuleManager.TileGame.IsGuideClick)
        //{
        //    GuideTileGamePageParam tParam = new GuideTileGamePageParam()
        //    {
        //        targetNode = mFloorRootList[0].cellList[38].Icon.rectTransform,
        //        guideText = "This time, the <color=#dd30f7>3rd</color> one!"
        //    };
        //    PageManager.Instance.OpenPage(PageID.GuideTileGamePage, tParam);
        //}

        //GuideMask.gameObject.SetActive(true);
        //GuideMask.Init();
        //GuideMask.Play(mFloorRootList[2].cellList[28].Icon.rectTransform);
        //GuideMask._targetArea.localScale *= floorViewScale;
        //var tTargetArea = GuideMask._targetArea;

        //var tTargetPos = tTargetArea.localPosition;
        //tTargetPos.y += 10;
        //tTargetArea.localPosition = tTargetPos;
        //tTargetArea.localScale = Vector3.one * 2;
        //var sq = DOTween.Sequence();
        //sq.Insert(0.5f, GuideMask.DOFade(0, 0));
        //sq.Insert(1, GuideMask.DOFade(0.5f, 0.5f));
        //sq.Insert(1, tTargetArea.DOScale(floorViewScale, 0.5f));

        //GuideMask.Play(mFloorRootList[2].cellList[33].transform as RectTransform);
        //GuideMask.Play(mFloorRootList[2].cellList[38].transform as RectTransform);

        //if (ModuleManager.TileGame.CurrIssueNum == 1 && ModuleManager.TileGame.CurrentLevel == 1 && tEventData.isNewGame)
        //{


        //}
        //else
        //{
        //    GuideMask.gameObject.SetActive(false);
        //}
    }


    public TileCellSlot GetSlotItem()
    {
        return SlotPool.GetOne();
    }

    public void ReturnSlotItem(TileCellSlot pReturnItem)
    {
        SlotPool.FreeOne(pReturnItem);
    }

    public struct FloorRootItem
    {
        public int floor;
        public RectTransform floorRoot;
        //public List<TileCell> cellList;
    }

    #region 道具

    void PropRecall(Action<bool> pCallBack = null)
    {
        //var tData = TileGameManager.Instance.SlotViewData.GetRecallIndex();
        //if (tData.Index < 0 || tData.SlotData == null)
        //{
        //    pCallBack?.Invoke(false);
        //    return;
        //}
        //pCallBack?.Invoke(true);
        //AudioManager.Instance.PlaySound(SoundID.Tile_Prop_Recall);

        //var tFlyItem = mSlotItemList[tData.Index];
        //tFlyItem.rect.SetParent(transform);

        //var tSlotData = (TileSlotData)tData.SlotData;

        //var tCellItem = mFloorRootList[tSlotData.Floor].cellList[tSlotData.Index];
        //var tCellData = TileGameManager.Instance.FloorViewData.floorsData[tSlotData.Floor].floorData[tSlotData.Index];

        //mIsMoving = true;
        //var seq = DOTween.Sequence();
        //seq.Insert(0, tFlyItem.transform.DOScale(0.7f, 0.2f));
        //seq.Insert(0, tFlyItem.transform.DOMove(tCellItem.transform.position, 0.2f));
        //seq.OnComplete(() =>
        //{
        //    TileGameManager.Instance.SlotViewData.RemoveSlotData(tData.Index);
        //    tCellData.SetState(TileCellState.Show);
        //    TileGameManager.Instance.FloorViewData.RefreshFloorsData();
        //    mSlotItemList.Remove(tFlyItem);
        //    ReturnSlotItem(tFlyItem);
        //    mIsMoving = false;
        //});
    }

    IEnumerator PropMagnet(Action<bool> pCallBack = null)
    {
        //mIsHinting = true;

        //var tFloorsData = TileGameManager.Instance.FloorViewData.floorsData;
        //var tSlotsData = TileGameManager.Instance.SlotViewData.slotsData;

        //int tHintCount = 3;
        //int tHintIconIndex = -1;
        //if (tSlotsData.Count > 0)
        //{
        //    var tSlotGroupData = new Dictionary<int, int>();//<iconIndex,数量>
        //    foreach (var tData in tSlotsData)
        //    {
        //        int iconIndex = tData.IconIndex;
        //        if (tSlotGroupData.ContainsKey(iconIndex))
        //        {
        //            tSlotGroupData[iconIndex]++;
        //        }
        //        else
        //        {
        //            tSlotGroupData.Add(iconIndex, 1);
        //        }
        //    }

        //    bool tCanTwo = (TileGameDefine.SLOT_MAX_COUNT - tSlotsData.Count) >= 2;
        //    if (tCanTwo)
        //    {
        //        tHintIconIndex = tSlotsData[0].IconIndex;
        //        tHintCount -= tSlotGroupData[tHintIconIndex];
        //    }
        //    else
        //    {
        //        foreach (var tGroup in tSlotGroupData)
        //        {
        //            if (tGroup.Value == 2)
        //            {
        //                tHintIconIndex = tGroup.Key;
        //                tHintCount -= tGroup.Value;
        //                break;
        //            }
        //        }

        //        if (tHintIconIndex == -1)
        //        {
        //            pCallBack?.Invoke(false);
        //            mIsHinting = false;
        //            yield break;
        //        }
        //    }
        //}
        //pCallBack?.Invoke(true);

        //var tHintData = new List<(TileCellData Data, Vector3 Pos)>();
        //for (int i = tFloorsData.Count - 1; i >= 0; i--)
        //{
        //    foreach (var tCellData in tFloorsData[i].floorData)
        //    {
        //        if (tHintData.Count >= tHintCount) break;

        //        if (tCellData.State != TileCellState.Hide)
        //        {
        //            if (tHintIconIndex == -1)
        //            {
        //                tHintIconIndex = tCellData.IconIndex;
        //            }

        //            if (tCellData.IconIndex == tHintIconIndex)
        //            {
        //                var tPos = mFloorRootList[tCellData.Floor].cellList[tCellData.Index].transform.position;
        //                tHintData.Add((tCellData, tPos));
        //            }
        //        }
        //    }
        //}

        //AudioManager.Instance.PlaySound(SoundID.Tile_Prop_Magnet);
        //foreach (var tData in tHintData)
        //{
        //    PlaceCell(tData.Data, tData.Pos);

        //    yield return new WaitForSeconds(0.2f);
        //}
        yield return new WaitForSeconds(0.5f);
        mIsHinting = false;
    }

    IEnumerator PropShuffle(Action<bool> pCallBack = null)
    {
        mIsShuffleing = true;
        //pCallBack?.Invoke(true);
        //foreach (var item in mFloorRootList)
        //{
        //    for (int i = 0; i < item.cellList.Count; i++)
        //    {
        //        item.cellList[i].PlayShuffleAnim(i % 2 == 0, 0.01f * i);
        //    }
        //}
        //AudioManager.Instance.PlaySound(SoundID.Tile_Prop_Shuffle);
         yield return new WaitForSeconds(1f);

        //FrontAssign();
        //TileGameManager.Instance.FloorViewData.ShuffleFloorData();
        //TileGameManager.Instance.SlotViewData.ClearQueueData();

        //foreach (var item in mFloorRootList)
        //{
        //    for (int i = 0; i < item.cellList.Count; i++)
        //    {
        //        item.cellList[i].StopShuffleAnim();
        //    }
        //}
        //yield return new WaitForSeconds(0.2f);
        //foreach (var item in mFloorRootList)
        //{
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(item.floorRoot);
        //}
        mIsShuffleing = false;
    }

    void FrontAssign()
    {
        //var tFloorsData = TileGameManager.Instance.FloorViewData.floorsData;
        //var tSlotsData = TileGameManager.Instance.SlotViewData.slotsData;

        //if (tSlotsData.Count <= 0) return;

        //var tSlotGroupData = new Dictionary<int, int>();//<iconIndex,数量>
        //foreach (var tData in tSlotsData)
        //{
        //    int iconIndex = tData.IconIndex;
        //    if (tSlotGroupData.ContainsKey(iconIndex))
        //    {
        //        tSlotGroupData[iconIndex]++;
        //    }
        //    else
        //    {
        //        tSlotGroupData.Add(iconIndex, 1);
        //    }
        //}
        //tSlotGroupData = tSlotGroupData.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, o => o.Value);

        //Dictionary<int, int> tFrontDic = new Dictionary<int, int>();
        //foreach (var item in tSlotGroupData)
        //{
        //    tFrontDic.Add(item.Key, 3 - item.Value);
        //    break;
        //}

        //List<TileCellData> tFrontDataList = new List<TileCellData>();
        //for (int i = tFloorsData.Count - 2; i >= 0; i--)
        //{
        //    foreach (var tCellData in tFloorsData[i].floorData)
        //    {
        //        if (tCellData.State != TileCellState.Hide)
        //        {
        //            if (tFrontDic.Count == 0) break;

        //            if (tFrontDic.ContainsKey(tCellData.IconIndex))
        //            {
        //                if ((--tFrontDic[tCellData.IconIndex]) <= 0)
        //                {
        //                    tFrontDic.Remove(tCellData.IconIndex);
        //                }
        //                tFrontDataList.Add(tCellData);
        //            }
        //        }
        //    }
        //}

        //if (tFrontDataList.Count <= 0) return;

        ////限定区域内
        //int4 tLimit = ModuleManager.TileGame.GetCurrentLimitData();
        //int[] tIndexes = new int[tFrontDataList.Count];
        //for (int i = 0; i < tFrontDataList.Count; i++)
        //{
        //    int tX = UnityEngine.Random.Range(tLimit.w, tLimit.y + 1);
        //    int tY = UnityEngine.Random.Range(tLimit.x, tLimit.z + 1);
        //    tIndexes[i] = TileCellData.GetIndex(tX, tY);
        //}
        ////var tIndexes = CommonMethod.NoRepeatRandom(0, TileGameDefine.SHOW_ITEM_MAX_COUNT - 1, tFrontDataList.Count);
        //var tEndFloorData = tFloorsData[tFloorsData.Count - 1].floorData;
        //int tIconIndex;
        //TileCellState tCellState;
        //for (int i = 0; i < tIndexes.Length; i++)
        //{
        //    tIconIndex = tEndFloorData[tIndexes[i]].IconIndex;
        //    tCellState = tEndFloorData[tIndexes[i]].State;
        //    tEndFloorData[tIndexes[i]].SetState(tFrontDataList[i].State, tFrontDataList[i].IconIndex);
        //    tFrontDataList[i].SetState(tCellState, tIconIndex);
        //}
    }

    #endregion

    #region UI事件

    public void OnclickReturnButton()
    {
        //if (ModuleManager.TileGame.IsUnderway())
        //{
        //    PageManager.Instance.OpenPage(PageID.TileLevelPage);
        //}
        //else
        //{
        //    PageManager.Instance.OpenPage(PageID.HomePage);
        //}
    }

    //撤销道具
    public void OnclickRecallPropBut()
    {
        //if (mIsMoving || mIsHinting || mIsShuffleing) return;
        //LogManager.Log("羊羊道具===>撤销");

        //bool tHasRecall = ModuleManager.Prop.HasProp(PropID.TileRecall);
        //if (tHasRecall)
        //{
        //    PropRecall((isf) =>
        //    {
        //        if (isf)
        //        {
        //            ModuleManager.Prop.ExpendProp(PropID.TileRecall);
        //            RefreshPropRoot();
        //        }
        //        else
        //        {
        //            LogManager.Log("羊羊道具===>撤销  无撤销数据");
        //            HintHelp.Instance.ShowHint("Recall is not available.");
        //        }
        //    });
        //}
        //else
        //{
        //    var tPageParam = new MiniShopSinglePageParam(PropID.TileRecall, PropSource.Shop);
        //    PageManager.Instance.OpenPage(PageID.MiniShopSinglePage, tPageParam);
        //}
    }
    //提示道具
    public void OnclickMagnetPropBut()
    {
        //if (mIsMoving || mIsHinting || mIsShuffleing) return;
        //LogManager.Log("羊羊道具===>提示");

        //bool tHasMagnet = ModuleManager.Prop.HasProp(PropID.TileMagnet);
        //if (tHasMagnet)
        //{
        //    StartCoroutine(PropMagnet((isf) =>
        //    {
        //        if (isf)
        //        {
        //            ModuleManager.Prop.ExpendProp(PropID.TileMagnet);
        //            RefreshPropRoot();
        //        }
        //        else
        //        {
        //            LogManager.Log("羊羊道具===>提示  无提示数据");
        //            HintHelp.Instance.ShowHint("Magnet is not available.");
        //        }
        //    }));

        //}
        //else
        //{
        //    var tPageParam = new MiniShopSinglePageParam(PropID.TileMagnet, PropSource.Shop);
        //    PageManager.Instance.OpenPage(PageID.MiniShopSinglePage, tPageParam);
        //}
    }
    //洗牌道具
    public void OnclickShufflePropBut()
    {
        //if (mIsMoving || mIsHinting || mIsShuffleing) return;
        //LogManager.Log("羊羊道具===>洗牌");

        //bool tHasShuffle = ModuleManager.Prop.HasProp(PropID.TileShuffle);
        //if (tHasShuffle)
        //{
        //    StartCoroutine(PropShuffle((isf) =>
        //    {
        //        if (isf)
        //        {
        //            ModuleManager.Prop.ExpendProp(PropID.TileShuffle);
        //            RefreshPropRoot();
        //        }
        //        else
        //        {
        //            LogManager.Log("羊羊道具===>洗牌  无洗牌数据");
        //            HintHelp.Instance.ShowHint("Shuffle is not available.");
        //        }
        //    }));
        //}
        //else
        //{
        //    var tPageParam = new MiniShopSinglePageParam(PropID.TileShuffle, PropSource.Shop);
        //    PageManager.Instance.OpenPage(PageID.MiniShopSinglePage, tPageParam);
        //}
    }

    public void OnclickTileShopBut()
    {
        //if (mIsMoving || mIsHinting || mIsShuffleing) return;
        //PageManager.Instance.OpenPage(PageID.MiniShopPage);
    }
    public void OnclickShopBut()
    {
        //if (mIsMoving || mIsHinting || mIsShuffleing) return;
        //PageManager.Instance.OpenPage(PageID.MiniShopGoldPage);
    }

    public void OnclickHeartBut()
    {
        //if (CommonMethod.IsFullTileHeart())
        //{
        //    HintHelp.Instance.ShowHint("Your lives are full.");
        //}
        //else
        //{
        //    PageManager.Instance.OpenPage(PageID.TileStaminaPage);
        //}
    }

    #endregion
}





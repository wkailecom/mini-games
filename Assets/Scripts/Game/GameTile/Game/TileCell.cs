//using DG.Tweening;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class TileCell : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
//{
//    public Image IconBg;
//    public Image Icon;
//    public Image Freeze;
//    public Material mat;

//    [SerializeField]
//    private float animTime = 0.1f;

//    float defaultScale = 1;
//    float pressedScale = 1.2f;

//    TileGamePage mTileGamePage;
//    TileCellData mCellData;

//    public void Init(TileCellData pCellData)
//    {
//        mCellData = pCellData;
//        mCellData.OnStateChange += RefreshState;
//        mTileGamePage = GetComponentInParent<TileGamePage>();

//        RefreshState();
//    }


//    void RefreshState()
//    {
//        IconBg.gameObject.SetActive(mCellData.State != TileCellState.Hide);
//        Icon.gameObject.SetActive(mCellData.State != TileCellState.Hide);
//        Icon.raycastTarget = mCellData.State == TileCellState.Show;
//        Freeze.gameObject.SetActive(mCellData.State == TileCellState.Freeze);
//        if (mCellData.State != TileCellState.Hide)
//        {
//            SetIcon(mCellData.IconIndex);
//        }
//    }


//    public void SetIcon(int pIconIndex)
//    {
//        if (pIconIndex >= 0 && pIconIndex < mTileGamePage.iconSprites.Count)
//        {
//            Icon.sprite = mTileGamePage.iconSprites[pIconIndex];
//        }
//    }

//    public int GetIndex()
//    {
//        return mCellData.Index;
//    }

//    #region 动画

//    Tweener tweener;
//    Vector3 startPos;
//    public void PlayShuffleAnim(bool pIsForward, float pDelayedTime)
//    {
//        startPos = transform.position;

//        float radius = 200f;
//        float duration = 0.5f;

//        float angle = 0f;
//        float tPoxZ = transform.position.z;
//        Vector3 center = new Vector3(mTileGamePage.CellAnimCenter.x, mTileGamePage.CellAnimCenter.y, tPoxZ);

//        float targetAngle = pIsForward ? 360f : -360f;
//        // 创建 Tween，从起始角度 0 到 360，持续时间为 duration 秒
//        tweener = DOTween.To(() => angle, x => angle = x, targetAngle, duration)
//            .SetDelay(pDelayedTime)
//            .SetEase(Ease.Linear)   // 使用线性 Easing
//            .SetLoops(-1, LoopType.Restart)  // 循环播放
//            .OnUpdate(UpdatePosition);  // 在 Tween 更新时更新位置


//        void UpdatePosition()
//        {
//            // 根据当前角度计算位置
//            float x = center.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
//            float y = center.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
//            transform.position = new Vector3(x, y, tPoxZ);
//        }
//    }

//    public void StopShuffleAnim()
//    {
//        tweener.Kill();

//        transform.DOMove(startPos, 0.2f);
//    }

//    public void PlayLicensingAnim(float pDelayedTime = 0)
//    {
//        var targetPos = transform.position;
//        transform.position = new Vector3(0, 0, transform.position.z);
//        tweener = transform.DOMove(targetPos, 0.3f).SetDelay(pDelayedTime);
//    }

//    #endregion

//    void TriggerTileCellClickEvent()
//    {
//        var tEventData = EventManager.GetEventData(EventDataType.TileCellClick) as TileCellClick;
//        tEventData.cellData = mCellData;
//        tEventData.startPos = transform.position;
//        EventManager.TriggerEvent(tEventData);
//    }

//    public void OnPointerClick(PointerEventData eventData)
//    {
//        //Debug.Log("========> OnPointerClick");
//        if (mCellData.State == TileCellState.Freeze) return;

//        TriggerTileCellClickEvent();
//        transform.DOKill(true);
//    }

//    public void OnPointerDown(PointerEventData eventData)
//    {
//        if (mCellData.State == TileCellState.Freeze) return;

//        //Debug.Log("========> OnPointerDown");

//        transform.DOKill();
//        transform.DOScale(pressedScale, animTime);
//        Icon.material = mat;
//        IconBg.material = mat;
//    }

//    public void OnPointerUp(PointerEventData eventData)
//    {
//        if (mCellData.State == TileCellState.Freeze) return;

//        //Debug.Log("========> OnPointerUp");

//        transform.DOKill();
//        transform.DOScale(defaultScale, animTime);
//        Icon.material = null;
//        IconBg.material = null;
//    }
//}

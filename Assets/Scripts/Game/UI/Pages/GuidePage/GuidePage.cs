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
    public class GuidePage : PageBase
    {
        [SerializeField] private Image _imgMask;
        [SerializeField] private Button _btnNext;
        [SerializeField] private GuideMask _guideMask;
        [SerializeField] private RectTransform _fingerTran;

        [SerializeField] private CanvasGroup _explainRoot;
        [SerializeField] private TextMeshProUGUI _explainTxt;
        [SerializeField] private TextMeshProUGUI _explainTapTxt;
        [SerializeField] private GameObject _explainItemCell;
        [SerializeField] private TextMeshProUGUI _explainItemCellTxt;
        [SerializeField] private GameObject _explainItemSingleLock;
        [SerializeField] private GameObject _explainItemDoubleLock;

        [SerializeField] private Image _maskBg;
        [SerializeField] private RectTransform _maskItemRoot;
        [SerializeField] private RectTransform _maskItem;

        Image mTargetArea;
        PassEventBtn mPassEvent;
        Action mNextAction;
        bool mIsRemoveCanvas;

        Color mMaskColor = new Color32(255, 250, 243, 178);
        Color mMaskColor2 = new Color32(60, 60, 60, 178);
        List<RectTransform> mMaskItems = new();
        HomePage mHomePage;
        GamePage mGamePage;
        protected override void OnInit()
        {
            _guideMask.Init();
            mTargetArea = _guideMask.TargetArea.GetComponent<Image>();
            mPassEvent = _btnNext.GetComponent<PassEventBtn>();
            _btnNext.onClick.AddListener(OnClickBtnNext);

        }

        protected override void OnBeginOpen()
        {
            _imgMask.gameObject.SetActive(false);
            _btnNext.gameObject.SetActive(false);
            _fingerTran.gameObject.SetActive(false);
            _explainRoot.gameObject.SetActive(false);
            _guideMask.gameObject.SetActive(false);
            mNextAction = null;
            mIsRemoveCanvas = false;

            _maskBg.gameObject.SetActive(false);
            _maskItem.gameObject.SetActive(false);

            if (PageManager.Instance.IsOpen(PageID.HomePage))
            {
                mHomePage = PageManager.Instance.GetHomePage(); 
                if ((string)PageParam == "IQGudie")
                {
                }
            }
        }

        protected override void RegisterEvents()
        {
        }

        protected override void UnregisterEvents()
        {

        }

        protected override void OnClosed()
        {
          
        }

        Queue<IEnumerator> mSteps = new();
        delegate void GuideStep();

        void SetFingerPos(Transform pReferTran, Vector2 pOffset = default)
        {
            _fingerTran.transform.localPosition = GameMethod.OtherWorldToSelfLocalPos(pReferTran, _fingerTran.transform) + pOffset;
            _fingerTran.gameObject.SetActive(true);
        }

        void SetExplainTxt(string pStr, bool pIsTap)
        {
            _explainTxt.text = pStr;
            _explainTapTxt.gameObject.SetActive(pIsTap);
            _explainItemCell.gameObject.SetActive(false);
            _explainItemSingleLock.gameObject.SetActive(false);
            _explainItemDoubleLock.gameObject.SetActive(false);
            _explainRoot.gameObject.SetActive(true);
            _explainRoot.alpha = 0f;
            _explainRoot.DOFade(1, 0.5f);
        }

        void SetExplainPos(Transform pReferTran, Vector2 pOffset)
        {
            _explainRoot.transform.localPosition = GameMethod.OtherWorldToSelfLocalPos(pReferTran, _explainRoot.transform) + pOffset;
        }

        void SetImgMask(Image pMask, Color pMaskColor)
        {
            pMask.color = Color.clear;
            pMask.gameObject.SetActive(true);
            pMask.DOColor(pMaskColor, 0.5f);
        }

        void SetClickPos(Transform pTarget, Color32 pMaskColor, bool pIsFinger = false)
        {
            _guideMask.color = pMaskColor;
            mTargetArea.color = pMaskColor;
            _guideMask.Play(pTarget as RectTransform);
            _guideMask.gameObject.SetActive(true);

            if (pIsFinger)
            {
                SetFingerPos(pTarget);
            }
            else
            {
                _fingerTran.gameObject.SetActive(false);
            }
        }

        void CopySizeAndPos(RectTransform pItemTran, RectTransform pTarget)
        {
            pItemTran.sizeDelta = new Vector2(pTarget.rect.width, pTarget.rect.height);
            pItemTran.position = pTarget.position;
        }

        void AddCanvas(GameObject pGo)
        {
            var tCanvas = pGo?.AddComponent<Canvas>();
            if (tCanvas == null) return;
            tCanvas.overrideSorting = true;
            tCanvas.sortingOrder = GetComponent<Canvas>().sortingOrder + 1;
            tCanvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 |
                                               AdditionalCanvasShaderChannels.TexCoord2 |
                                               AdditionalCanvasShaderChannels.TexCoord3 |
                                               AdditionalCanvasShaderChannels.Normal |
                                               AdditionalCanvasShaderChannels.Tangent;
            pGo?.AddComponent<GraphicRaycaster>();
        }

        void RemoveCanvas(GameObject pGo)
        {
            Destroy(pGo.GetComponent<GraphicRaycaster>());
            Destroy(pGo.GetComponent<Canvas>());
        }


        #region UI事件

        void OnClickBtnNext()
        {
            mNextAction?.Invoke();
        }


        #endregion
    }
}
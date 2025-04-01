using Config;
using DG.Tweening;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UISystem
{
    public class PageBase : AnimationUI
    {
        public int ID => mPageConfig.pageID;
        public bool IsFullPage { get { return mPageConfig.isFullPage; } }
        public bool SaveToHistory { get { return mPageConfig.saveToHistory; } }
        public bool NeedCache { get { return mPageConfig.needCache; } }
        public bool IsOpen { get; private set; }
        public object PageParam { get; private set; }

        PageConfig mPageConfig;
        Canvas[] mCanvases;
        Renderer[] mRenderers;
        RawImage mPageMask;

        public void Init(PageConfig pConfig)
        {
            mPageConfig = pConfig;
            name = Path.GetFileNameWithoutExtension(mPageConfig.prefabPath);

            OnInit();
            InitCanvasesAndRenderers();
            InitPageMask();
        }

        void InitCanvasesAndRenderers()
        {
            var tPageCanvas = gameObject.AddComponent<Canvas>();
            tPageCanvas.overrideSorting = true;
            tPageCanvas.sortingOrder = 0;
            gameObject.AddComponent<GraphicRaycaster>();

            mCanvases = GetComponentsInChildren<Canvas>(true);
            mRenderers = GetComponentsInChildren<Renderer>(true);
        }

        void InitPageMask()
        {
            if (mPageConfig.maskAlpha < 0)
            {
                return;
            }

            mPageMask = gameObject.AddComponent<RawImage>();
            if (mPageMask == null)
            {
                LogManager.LogError("Can't add RawImage to " + name);
                return;
            }

            mPageMask.GetComponent<CanvasRenderer>().cullTransparentMesh = false;
            if (!string.IsNullOrEmpty(mPageConfig.maskColor) && ColorUtility.TryParseHtmlString(mPageConfig.maskColor, out Color newColor))
            {
                newColor.a = 0;
                mPageMask.color = newColor;
            }
            else
            {
                mPageMask.color = Color.clear;
            }
        }

        public IEnumerator OpenPage(object pPageParam, bool pPlayAnimation, int pBaseSortingOrder)
        {
            foreach (var tCanvas in mCanvases)
            {
                tCanvas.sortingOrder = tCanvas.sortingOrder % PageManager.PAGE_CANVAS_INTERVAL + pBaseSortingOrder;
            }
            foreach (var tRender in mRenderers)
            {
                tRender.sortingOrder = tRender.sortingOrder % PageManager.PAGE_CANVAS_INTERVAL + pBaseSortingOrder;
            }

            IsOpen = true;
            PageParam = pPageParam;
            gameObject.SetActive(true);

            RegisterEvents();

            OnBeginOpen();
            var tEventData = EventManager.GetEventData<PageOperation>(EventKey.PageBeginOpen);
            tEventData.pageID = (PageID)ID;
            EventManager.Trigger(tEventData);

            yield return Show(pPlayAnimation);

            OnOpened();
            tEventData = EventManager.GetEventData<PageOperation>(EventKey.PageOpened);
            tEventData.pageID = (PageID)ID;
            EventManager.Trigger(tEventData);
        }

        public IEnumerator ClosePage(bool pPlayAnimation)
        {
            IsOpen = false;

            UnregisterEvents();

            OnBeginClose();
            yield return Hide(pPlayAnimation);
            OnClosed();

            var tEventData = EventManager.GetEventData<PageOperation>(EventKey.PageClosed);
            tEventData.pageID = (PageID)ID;
            EventManager.Trigger(tEventData);

            gameObject.SetActive(false);
        }

        protected virtual void OnInit() { }

        protected virtual void OnBeginOpen() { }

        protected virtual void OnOpened() { }

        protected virtual void OnBeginClose() { }

        protected virtual void OnClosed() { }

        protected virtual void OnReopen() { }

        protected virtual void RegisterEvents() { }

        protected virtual void UnregisterEvents() { }

        protected override void OnBeginShow(bool pPlayAnimation)
        {
            base.OnBeginShow(pPlayAnimation);

            MoveToFront();
            SetMask(true, pPlayAnimation);
        }

        protected override void OnBeginHide(bool pPlayAnimation)
        {
            base.OnBeginHide(pPlayAnimation);

            SetMask(false, pPlayAnimation);
        }

        void MoveToFront()
        {
            transform.SetAsLastSibling();
        }

        public virtual void OnUpdate(float pDeltaTime) { }

        public void Reopen(object pPageParam)
        {
            PageParam = pPageParam;

            MoveToFront();
            OnReopen();
        }

        public virtual void OnCoveredByOtherPage()
        {
            LogManager.Log(name + " is covered by other page!");
        }

        public virtual void OnCoverPageRemove()
        {
            LogManager.Log(name + " cover is removed!");
        }

        public void Close()
        {
            PageManager.Instance.ClosePage(this);
        }

        void SetMask(bool pShow, bool pPlayAnimation)
        {
            if (mPageMask == null)
            {
                return;
            }

            if (pShow)
            {
                mPageMask.DOFade(mPageConfig.maskAlpha, pPlayAnimation ? GetAnimationTime(true) : 0);
            }
            else
            {
                mPageMask.DOFade(0, pPlayAnimation ? GetAnimationTime(false) : 0);
            }
        }
    }
}
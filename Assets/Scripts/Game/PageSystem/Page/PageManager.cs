using Config;
using Game;
using Game.UI;
using LLFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UISystem
{
    public class PageManager : Singleton<PageManager>
    {
        public const int PAGE_CANVAS_INTERVAL = 10;

        string AssetPath(string pAssetPath) => "UI/Pages/" + pAssetPath;

        bool mInitialized;
        PageBase mCurrentPage;
        List<PageBase> mActivePages;
        Dictionary<int, PageBase> mCachedPages;
        List<PageHistory> mPageHistory;

        bool mSwitchLock;
        bool SwitchLock
        {
            get { return mSwitchLock; }
            set
            {
                mSwitchLock = value;
                if (value)
                {
                    InputLockManager.Instance.Lock("PageSwitch");
                }
                else
                {
                    InputLockManager.Instance.UnLock("PageSwitch");
                }
            }
        }

        //执行加锁的任务
        void StartLockedTask(IEnumerator pLockedTask)
        {
            TaskManager.Instance.StartTask(ExecuteLockedTask(pLockedTask));
        }

        IEnumerator ExecuteLockedTask(IEnumerator pLockedTask)
        {
            WaitForEndOfFrame tWaitForEndOfFrame = new WaitForEndOfFrame();
            while (SwitchLock)
            {
                yield return tWaitForEndOfFrame;
            }

            SwitchLock = true;
            yield return pLockedTask;
            SwitchLock = false;
        }

        public void Init()
        {
            if (mInitialized)
            {
                LogManager.LogError("PageManager.Init: Already initialized!");
                return;
            }

            mCurrentPage = null;
            mActivePages = new List<PageBase>();
            mCachedPages = new Dictionary<int, PageBase>();
            mPageHistory = new List<PageHistory>();

            mInitialized = true;
        }

        public void Uninit()
        {
            StartLockedTask(UninitTask());
        }

        IEnumerator UninitTask()
        {
            if (!mInitialized)
            {
                LogManager.LogError("PageManager.Uninit: not initizlie!");
                yield break;
            }

            yield return CloseActivePagesAndSaveHistory(false);
            yield return ClearCachedPagesTask();
            mPageHistory.Clear();

            mCurrentPage = null;
            mActivePages = null;
            mCachedPages = null;
            mPageHistory = null;

            mInitialized = false;
        }

        public void OpenPage(PageID pPageID, object pPageParam = null)
        {
            StartLockedTask(OpenPageTask(pPageID, pPageParam));
        }

        IEnumerator OpenPageTask(PageID pPageID, object pPageParam = null)
        {
            PageBase tPageToOpen = GetPage((int)pPageID);
            if (tPageToOpen == null)
            {
                yield break;
            }

            if (tPageToOpen.IsOpen)
            {
                DoReopenPage(tPageToOpen, pPageParam);
                yield break;
            }

            if (tPageToOpen.IsFullPage)
            {
                yield return MultiTask.DoMultiTask(CloseActivePagesAndSaveHistory(true), DoOpenPage(tPageToOpen, pPageParam, true));
            }
            else
            {
                mCurrentPage?.OnCoveredByOtherPage();
                yield return DoOpenPage(tPageToOpen, pPageParam, true);
            }
        }
        public void ClosePage(PageID pPageID)
        {
            ClosePage(GetActivePage((int)pPageID));
        }

        public void ClosePage(PageBase pPageToClose)
        {
            StartLockedTask(ClosePageTask(pPageToClose));
        }

        IEnumerator ClosePageTask(PageBase pPageToClose)
        {
            if (pPageToClose == null) yield break;
            if (!pPageToClose.IsOpen)
            {
                LogManager.LogError($"pageToClose: {pPageToClose.name} is not open page");
                yield break;
            }

            List<IEnumerator> tMultiTasks = new List<IEnumerator>();
            if (pPageToClose != mCurrentPage)
            {
                for (int i = mActivePages.Count - 1; i >= 0; i--)
                {
                    if (pPageToClose == mActivePages[i])
                    {
                        break;
                    }
                    tMultiTasks.Add(DoClosePage(mActivePages[i], true));
                }

                mCurrentPage = pPageToClose;
            }
            tMultiTasks.Add(CloseCurrentPage());

            yield return MultiTask.DoMultiTask(tMultiTasks.ToArray());
        }

        IEnumerator DoOpenPage(PageBase pPage, object pPageParam, bool pPlayAnimation)
        {
            if (!mActivePages.Contains(pPage))
            {
                mActivePages.Add(pPage);
            }
            mCurrentPage = pPage;

            yield return pPage.OpenPage(pPageParam, pPlayAnimation, mActivePages.Count * PAGE_CANVAS_INTERVAL);
        }

        IEnumerator DoClosePage(PageBase pPage, bool pPlayAnimation)
        {
            mActivePages.Remove(pPage);

            yield return pPage.ClosePage(pPlayAnimation);
            TryDestroyPage(pPage);
        }

        IEnumerator CloseActivePagesAndSaveHistory(bool pPlayAnimation = true)
        {
            if (mActivePages.Count == 0)
            {
                yield break;
            }

            IEnumerator[] tClosePageTasks = new IEnumerator[mActivePages.Count];
            for (int i = mActivePages.Count - 1; i >= 0; i--)
            {
                PageBase tActivePage = mActivePages[i];
                if (tActivePage.SaveToHistory)
                {
                    mPageHistory.Add(new PageHistory(tActivePage));
                }
                tClosePageTasks[i] = DoClosePage(tActivePage, pPlayAnimation);
            }

            yield return MultiTask.DoMultiTask(tClosePageTasks);
        }

        void DoReopenPage(PageBase pPage, object pPageParam)
        {
            if (pPage.IsFullPage)
            {
                StartLockedTask(CloseCoverPages());
            }
            else
            {
                // 把要Reopen的Page移动到ActivePages的最后
                mActivePages.Remove(pPage);
                mActivePages.Add(pPage);
            }

            mCurrentPage = pPage;
            pPage.Reopen(pPageParam);
        }

        IEnumerator CloseCoverPages()
        {
            List<IEnumerator> tTasks = new List<IEnumerator>();
            for (int i = mActivePages.Count - 1; i >= 0; i--)
            {
                if (!mActivePages[i].IsFullPage)
                {
                    tTasks.Add(DoClosePage(mActivePages[i], true));
                }
            }

            if (tTasks.Count > 0)
            {
                yield return MultiTask.DoMultiTask(tTasks.ToArray());
            }
        }

        IEnumerator CloseCurrentPage()
        {
            if (mCurrentPage.IsFullPage)
            {
                yield return CloseCurrentPageFull();
            }
            else
            {
                yield return CloseCurrentPageCover();
            }
        }

        IEnumerator CloseCurrentPageFull()
        {
            PageHistory tLastPageHistory = PopPageHistory();
            if (tLastPageHistory == null || !tLastPageHistory.isFullPage)
            {
                LogManager.LogError("History wrong! Prepage is not full page?");
                yield return BackToHomePageTask();
                yield break;
            }
            PageBase tLastFullPage = GetPage(tLastPageHistory.pageID);

            yield return MultiTask.DoMultiTask(DoClosePage(mCurrentPage, true), DoOpenPage(tLastFullPage, tLastPageHistory.pageParam, true));
            yield return RecoverCoverPages();
        }

        IEnumerator RecoverCoverPages()
        {
            AsyncRequestResult tResult = new AsyncRequestResult();

            List<IEnumerator> tTasks = new List<IEnumerator>();
            for (int i = mPageHistory.Count - 1; i >= 0; i--)
            {
                PageHistory tPageHistory = mPageHistory[i];
                if (tPageHistory.isFullPage)
                {
                    break;
                }

                mPageHistory.RemoveAt(i);
                tTasks.Add(DoOpenPage(GetPage(tPageHistory.pageID), tPageHistory.pageParam, true));
            }

            if (tTasks.Count > 0)
            {
                yield return MultiTask.DoMultiTask(tTasks.ToArray());
            }
        }

        IEnumerator CloseCurrentPageCover()
        {
            yield return DoClosePage(mCurrentPage, true);
            mCurrentPage = null;

            if (mActivePages.Count == 0)
            {
                LogManager.LogError("关闭Cover界面后，无ActivePage");
                yield return BackToHomePageTask();
                yield break;
            }

            PageBase tPrePage = mActivePages[mActivePages.Count - 1];
            tPrePage.OnCoverPageRemove();
            mCurrentPage = tPrePage;
        }

        IEnumerator ClearCachedPagesTask()
        {
            foreach (var tPage in mCachedPages.Values)
            {
                if (tPage.IsOpen)
                {
                    yield return DoClosePage(tPage, false);
                }

                if (tPage != null)
                {
                    DestroyPage(tPage);
                }
            }
            mCachedPages.Clear();
        }

        void TryDestroyPage(PageBase pPage)
        {
            if (!pPage.NeedCache)
            {
                DestroyPage(pPage);
            }
        }

        void DestroyPage(PageBase pPage)
        {
            Object.Destroy(pPage.gameObject);
        }

        PageBase GetPage(int pPageID)
        {
            PageBase tPage = GetActivePage(pPageID);
            if (tPage != null)
            {
                return tPage;
            }

            if (mCachedPages.TryGetValue(pPageID, out tPage))
            {
                return tPage;
            }

            tPage = CreatePage(pPageID);
            if (tPage == null)
            {
                return null;
            }
            //tPage.gameObject.SetActive(false);

            if (tPage.NeedCache)
            {
                mCachedPages[pPageID] = tPage;
            }

            return tPage;
        }

        PageBase GetActivePage(int pPageID)
        {
            for (int i = 0; i < mActivePages.Count; i++)
            {
                if (mActivePages[i].ID == pPageID)
                {
                    return mActivePages[i];
                }
            }

            return null;
        }

        PageBase CreatePage(int pPageID)
        {
            PageConfig tPageConfig = ConfigData.pageConfig.GetByPrimary(pPageID);
            if (tPageConfig == null)
            {
                return null;
            }

            PageBase tPageBase = ResTool.CreatePrefab<PageBase>(AssetPath(tPageConfig.prefabPath), UIRoot.Instance.PagesPanel);
            if (tPageBase == null)
            {
                return null;
            }

            tPageBase.Init(tPageConfig);
            tPageBase.gameObject.SetActive(false);
            return tPageBase;
        }

        PageHistory PopPageHistory()
        {
            if (mPageHistory.Count == 0)
            {
                LogManager.LogError("History is empty!");
                return null;
            }

            PageHistory tLastPageHistory = mPageHistory[mPageHistory.Count - 1];
            mPageHistory.RemoveAt(mPageHistory.Count - 1);
            return tLastPageHistory;
        }

        public bool HasCreate(PageID pPageID)
        {
            PageBase tPage = GetActivePage((int)pPageID);
            if (tPage != null)
            {
                return true;
            }

            return mCachedPages.ContainsKey((int)pPageID);
        }

        public bool IsOpen(PageID pPageID)
        {
            PageBase tPage = GetActivePage((int)pPageID);
            if (tPage != null)
            {
                return tPage.IsOpen;
            }

            if (mCachedPages.TryGetValue((int)pPageID, out tPage))
            {
                return tPage.IsOpen;
            }
            return false;
        }

        public void BackToHomePage()
        {
            StartLockedTask(BackToHomePageTask());
        }
        public GamePage GetGamePage()
        {
            return GetPage((int)PageID.GamePage) as GamePage;
        }

        public HomePage GetHomePage()
        {
            return GetPage((int)PageID.HomePage) as HomePage;
        }

        public T GetPage<T>(PageID pPageID) where T : PageBase
        {
            return GetPage((int)pPageID) as T;
        }

        IEnumerator BackToHomePageTask()
        {
            if (!mCachedPages.ContainsKey((int)PageID.HomePage))
            {
                yield break;
            }
            yield return OpenPageTask(PageID.HomePage);
            mPageHistory.Clear();
        }

        public void SetUIVisible(bool pVisible, bool pPlayAnimation = true)
        {
            IEnumerator[] tTasks = new IEnumerator[mActivePages.Count];
            for (int i = 0; i < mActivePages.Count; i++)
            {
                tTasks[i] = pVisible ? mActivePages[i].Show(pPlayAnimation) : mActivePages[i].Hide(pPlayAnimation);
            }
            StartLockedTask(MultiTask.DoMultiTask(tTasks));
        }

        public void OnUpdate(float pDeltaTime)
        {
            if (mActivePages == null) return;

            foreach (var tActivePage in mActivePages)
            {
                tActivePage.OnUpdate(pDeltaTime);
            }
        }
    }
}


namespace Game.UISystem
{
    public class PageHistory
    {
        public int pageID;
        public bool isFullPage;
        public object pageParam;

        public PageHistory(PageBase pPage)
        {
            pageID = pPage.ID;
            isFullPage = pPage.IsFullPage;
            pageParam = pPage.PageParam;
        }
    }
}
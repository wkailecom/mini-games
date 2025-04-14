using Config;
using Game.UI;
using Game.UISystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class MiniEnterPage : PageBase
    {
        [SerializeField] private Button _btnReturn;
        [SerializeField] private Button _btnLike;
        [SerializeField] private Button _btnEnter;
        [SerializeField] private RectTransform _titleRoot;
        [SerializeField] private RectTransform _iconRoot;
        [SerializeField] private Image _imgIcon;

        [SerializeField] private Button _btnLevel;
        [SerializeField] private TextMeshProUGUI _txtLevel;

        UISwitch _switnLike;
        MiniEnterPageParam mParam;
        MiniTypeConfig mTypeConfig;
        MiniGameType mGameType;
        int mCurLevel;
        protected override void OnInit()
        {
            _switnLike = _btnLike.GetComponentInChildren<UISwitch>(true);

            _btnReturn.onClick.AddListener(Close);
            _btnLike.onClick.AddListener(OnClickBtnLike);
            _btnEnter.onClick.AddListener(OnClickBtnEnter);
            _btnLevel.onClick.AddListener(OnClickBtnLevel);
        }

        protected override void OnBeginOpen()
        {
            base.OnBeginOpen();
            RefreshUI();
        }

        protected override void OnReopen()
        {
            base.OnReopen();
            RefreshUI();
        }

        void RefreshUI()
        {
            mParam = PageParam as MiniEnterPageParam;
            if (mParam == null)
            {
                LogManager.LogError("MiniEnterPage: invalid param");
                return;
            }

            mTypeConfig = mParam.typeConfig;
            mGameType = (MiniGameType)mTypeConfig.ID;
            mCurLevel = ModuleManager.MiniGame.GetCurLevel((int)mGameType);

            bool tIsLike = ModuleManager.MiniFavor.IsFavor((int)mGameType);
            _switnLike.SetSwitch(tIsLike);
            _titleRoot.ClearChild();
            _iconRoot.ClearChild();

            ResTool.CreatePrefab<Transform>(mTypeConfig.animTitle, GameConst.PREFAB_MINI_EVENT_PATH, _titleRoot.transform);
            ResTool.CreatePrefab<Transform>(mTypeConfig.animIcon, GameConst.PREFAB_MINI_EVENT_PATH, _iconRoot.transform);
            _txtLevel.text = $"LEVEL {mCurLevel}";

            var tCurType = MiniGameManager.Instance.GameType;
            if (tCurType == MiniGameType.Invalid || tCurType == mGameType)
            {
                _btnEnter.gameObject.SetActive(false);
            }
            else
            {
                var tCurConfig = ModuleManager.MiniGame.GetTypeConfig((int)tCurType);
                _imgIcon.sprite = ResTool.LoadIcon(tCurConfig.enterIcon, GameConst.ATLAS_MINI_EVENT_PATH);
                _btnEnter.gameObject.SetActive(true);
            }
        }


        #region UI事件

        void OnClickBtnLike()
        {
            ModuleManager.MiniFavor.SetFavor(mTypeConfig.ID, !_switnLike.isOn);
            _switnLike.SetSwitch(!_switnLike.isOn);
        }

        void OnClickBtnEnter()
        { 
            var tCurConfig = ModuleManager.MiniGame.GetTypeConfig((int)MiniGameManager.Instance.GameType);
            PageManager.Instance.OpenPage(PageID.MiniEnterPage, new MiniEnterPageParam(tCurConfig)); 
        }

        void OnClickBtnLevel()
        {
            if (ModuleManager.Prop.HasProp(PropID.Energy))
            {
                MiniGameManager.Instance.StartGame(mGameType, mCurLevel);
            }
            else
            {
                //PageManager.Instance.OpenPage(PageID.AdsPropPopup, new AdsPropPageParam(PropID.Energy, null));
                PageManager.Instance.OpenPage(PageID.SwapEnergyPage);
            }
        }

        #endregion
    }

    public class MiniEnterPageParam
    {
        public MiniTypeConfig typeConfig { get; private set; }

        public MiniEnterPageParam(MiniTypeConfig pTypeConfig)
        {
            typeConfig = pTypeConfig;
        }

    }
}

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
        [SerializeField] private RectTransform _titleRoot;
        [SerializeField] private RectTransform _iconRoot;
        [SerializeField] private RectTransform _enterRoot;
        [SerializeField] private Image _imgIcon;

        [SerializeField] private Button _btnLevel;
        [SerializeField] private TextMeshProUGUI _txtLevel;

        UISwitch _switnLike;
        MiniEnterPageParam mParam;
        MiniGameType mGameType;
        int mCurLevel;
        protected override void OnInit()
        {
            _switnLike = _btnLike.GetComponentInChildren<UISwitch>(true);

            _btnReturn.onClick.AddListener(Close);
            _btnLike.onClick.AddListener(OnClickBtnLike);
            _btnLevel.onClick.AddListener(OnClickBtnLevel);
        }

        protected override void OnBeginOpen()
        {
            mParam = PageParam as MiniEnterPageParam;
            if (mParam == null)
            {
                LogManager.LogError("MiniEnterPage: invalid param");
                return;
            }

            mCurLevel = ModuleManager.MiniGame.CurLevel;
            mGameType = (MiniGameType)mParam.typeConfig.ID;

            bool tIsLike = ModuleManager.MiniFavor.IsFavor(mParam.typeConfig.ID);
            _switnLike.SetSwitch(tIsLike);
            _titleRoot.ClearChild();
            _iconRoot.ClearChild();

            ResTool.CreatePrefab<Transform>(mParam.typeConfig.animTitle, GameConst.PREFAB_MINI_EVENT_PATH, _titleRoot.transform);
            ResTool.CreatePrefab<Transform>(mParam.typeConfig.animIcon, GameConst.PREFAB_MINI_EVENT_PATH, _iconRoot.transform);
            _imgIcon.sprite = ResTool.LoadIcon(mParam.typeConfig.enterIcon, GameConst.ATLAS_MINI_EVENT_PATH);
            _txtLevel.text = $"LEVEL {mCurLevel}";
        }


        #region UI事件

        void OnClickBtnLike()
        {
            ModuleManager.MiniFavor.SetFavor(mParam.typeConfig.ID, !_switnLike.isOn);
            _switnLike.SetSwitch(!_switnLike.isOn);
        }

        void OnClickBtnLevel()
        {
            if (ModuleManager.Prop.HasProp(PropID.Energy))
            {
                MiniGameManager.Instance.StartGame(mGameType, mCurLevel);
            }
            else
            {
                PageManager.Instance.OpenPage(PageID.AdsPropPopup, new AdsPropPageParam(PropID.Energy, null));
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

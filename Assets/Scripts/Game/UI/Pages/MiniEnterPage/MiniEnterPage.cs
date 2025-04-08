using Config;
using Game.UISystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
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

        UISwitch _switnLike;
        MiniEnterPageParam mParam;
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

            _switnLike.SetSwitch(false);
            _titleRoot.ClearChild();
            _iconRoot.ClearChild();

            ResTool.CreatePrefab<Transform>(mParam.typeConfig.animTitle, GameConst.PREFAB_MINI_EVENT_PATH, _titleRoot.transform);
            ResTool.CreatePrefab<Transform>(mParam.typeConfig.animIcon, GameConst.PREFAB_MINI_EVENT_PATH, _iconRoot.transform);
            _imgIcon.sprite = ResTool.LoadIcon(mParam.typeConfig.enterIcon, GameConst.ATLAS_MINI_EVENT_PATH);
        }


        #region UI事件

        void OnClickBtnLike()
        {
            bool tIsLike = false;

            _switnLike.SetSwitch(tIsLike);
        }

        void OnClickBtnLevel()
        {

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

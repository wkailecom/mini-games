using Config;
using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class MiniRevivePopup : PageBase
    {
        [SerializeField] private RectTransform _tranPanel;
        [SerializeField] private TextMeshProUGUI _txtTitle;
        [SerializeField] private TextMeshProUGUI _txtLevel;
        [SerializeField] private TextMeshProUGUI _txtDescribe;
        [SerializeField] private GameObject _nodeRoot;
        [SerializeField] private UIPopupBtn _btnLeft;
        [SerializeField] private UIPopupBtn _btnRight;

        [SerializeField] private Sprite[] sprites;

        PropID mPropID;
        bool mIsAD;
        ProgressNode[] nodes;
        MiniRevivePopupParam mParam;

        protected override void OnInit()
        {
            base.OnInit();

            _btnLeft.button.onClick.AddListener(OnClickLeft);
            _btnRight.button.onClick.AddListener(OnClickRight);

            nodes = _nodeRoot.GetComponentsInChildren<ProgressNode>(true);
        }

        protected override void OnBeginOpen()
        {
            base.OnBeginOpen();

            mParam = PageParam as MiniRevivePopupParam;
            if (mParam == null)
            {
                LogManager.LogError("MiniRevivePopup.OnBeginOpen PageParam is null!!!");
                return;
            }

            if (mParam.isReturn)
            {
                _txtTitle.text = "Quit Level?";
                _txtLevel.text = $"Current Level：{mParam.level}";
                _txtDescribe.text = $"You will lose 1 life!";

                _btnLeft.btnText.text = "Exit";
                _btnRight.btnText.text = "Continue";

                _btnLeft.btnImage.sprite = sprites[1];
                _btnRight.btnImage.sprite = sprites[1];
                _btnLeft.effRoot.SetActive(false);
                _btnRight.effRoot.SetActive(true);
                _btnRight.itemRoot.SetActive(false);
                _btnRight.button.interactable = true;
            }
            else
            {
                _txtTitle.text = "No more space!";
                _txtLevel.text = $"Current Level：{mParam.level}";

                _btnLeft.btnText.text = "Try Again";
                _btnRight.btnText.text = "Play on";

                var tGameType = ModuleManager.MiniGame.GameType;
                bool tIsValid = false;
                var tPropId = PropID.Invalid;
                if (tGameType == MiniGameType.Screw)
                {
                    tPropId = PropID.ScrewExtraSlot;
                    tIsValid = ScrewJam.EventManager.Instance.CheckCanExtraSlotUse.Invoke();
                    if (tIsValid)
                    {
                        _txtDescribe.text = $"Not feeling like to lose 1 heart and start over?\r\nAdd 1 extra hole to Play on!";
                    }
                    else
                    {
                        _txtDescribe.text = "Oops! Your backup area is full. \r\nPlease restart the level and have another go!";
                    }
                }
                else if (tGameType == MiniGameType.Jam3d)
                {
                    tPropId = PropID.Jam3DReplace;
                    tIsValid = GameLogic.JamManager.GetSingleton().Board.CanReplace();
                    if (tIsValid)
                    {
                        _txtDescribe.text = $"Not feeling like to lose 1 heart and start over?\r\nMove 3 blocks to the back up area to Play on!";
                    }
                    else
                    {
                        _txtDescribe.text = "Oops! Your backup area is full. \r\nPlease restart the level and have another go!";
                    }

                }
                else if (tGameType == MiniGameType.Tile)
                {
                    tPropId = PropID.TileRecall;

                }
                else
                {
                    _txtDescribe.text = "Oops! Your backup area is full. \r\nPlease restart the level and have another go!";
                }

                mPropID = tPropId;
                if (tIsValid && tPropId != PropID.Invalid)
                {
                    _btnRight.btnImage.sprite = sprites[2];
                    _btnRight.button.interactable = true;

                    var mConfig = ConfigData.propConfig.GetByPrimary((int)tPropId);
                    if (mConfig != null)
                    {
                        _btnRight.propIcon.SetPropIcon(mConfig.icon, false);
                        _btnRight.itemRoot.SetActive(true);
                        _btnRight.effRoot.SetActive(true);
                    }
                    else
                    {
                        _btnRight.itemRoot.SetActive(false);
                        _btnRight.effRoot.SetActive(false);
                    }
                }
                else
                {
                    _btnRight.btnImage.sprite = sprites[0];
                    _btnRight.button.interactable = false;
                    _btnRight.effRoot.SetActive(false);
                }
            }

            SetNode(mParam.level, ModuleManager.MiniGame.MaxLevel);

            mIsAD = ModuleManager.MiniGame.IssueNum != 1 || ModuleManager.MiniGame.CurLevel >= 3;
        }

        void SetNode(int curLevel, int endLevel)
        {
            int t = curLevel - 2;
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i].SetNodeIndex(t);
                bool isFill = t <= curLevel;
                if (t <= 0 || t > endLevel)
                {
                    nodes[i].Hide();
                    t++;
                    continue;
                }

                nodes[i].Show();
                if (t == 1)
                    nodes[i].IsFirst(isFill);
                else if (t == endLevel)
                    nodes[i].IsEnd(isFill);
                else
                    nodes[i].IsMid(isFill);

                if (t == curLevel)
                    nodes[i].SetIsCurrentNode();

                t++;
            }
        }

        void OnClickLeft()
        {
            Close();
            mParam?.clickAction?.Invoke(false);

            if (mParam.isReturn)
            {
                ModuleManager.Prop.ExpendProp(PropID.Energy);
                if (ModuleManager.MiniGame.IsUnderway())
                {
                    if (mParam.level >= MiniGameConst.AD_OPEN_LEVEL)
                    {
                        ADManager.Instance.PlayInterstitial(ADShowReason.Interstitial_MiniGameReturn);
                    }
                    PageManager.Instance.OpenPage(PageID.MiniMapPage);
                }
                else
                {
                    PageManager.Instance.OpenPage(PageID.HomePage);
                }
            }
            MiniGameManager.Instance.UnloadScene(MiniGameManager.Instance.GameType.ToString());
        }

        void OnClickRight()
        {
            if (mParam.isReturn)
            {
                Close();
            }
            else
            {
                if (mPropID == PropID.Invalid || ModuleManager.Prop.HasProp(mPropID))
                {
                    Close();
                }
                mParam?.clickAction?.Invoke(true);
            }

        }

    }

    public class MiniRevivePopupParam
    {
        public int level;
        public bool isReturn;
        public UnityAction<bool> clickAction;
    }
}
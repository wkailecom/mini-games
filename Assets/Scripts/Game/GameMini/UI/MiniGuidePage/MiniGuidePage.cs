using Config;
using DG.Tweening;
using Game.MiniGame;
using Game.UISystem;
using GameLogic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    public class MiniGuidePage : PageBase
    {
        [SerializeField] private Image _imgMask;
        [SerializeField] private Button _btnNext;
        [SerializeField] private GuideMask _guideMask;
        [SerializeField] private RectTransform _fingerTran;

        [SerializeField] private CanvasGroup _explainRoot;
        [SerializeField] private TextMeshProUGUI _explainTxt;
        [SerializeField] private TextMeshProUGUI _explainTapTxt;

        Camera mSceneCamera = null;
        Camera mUICamera = null;
        Image mTargetArea;
        PassEventBtn mPassEvent;
        Action mNextAction;

        string mGuideKey;
        protected override void OnInit()
        {
            _guideMask.Init();
            mTargetArea = _guideMask.TargetArea.GetComponent<Image>();
            mPassEvent = _btnNext.GetComponent<PassEventBtn>();
            _btnNext.onClick.AddListener(OnClickBtnNext);

        }

        protected override void OnBeginOpen()
        {
            _btnNext.gameObject.SetActive(false);
            _fingerTran.gameObject.SetActive(false);
            _explainRoot.gameObject.SetActive(false);
            _guideMask.gameObject.SetActive(false);
            mNextAction = null;

            mGuideKey = PageParam as string;
            if (string.IsNullOrEmpty(mGuideKey))
            {
                Debug.LogError("MiniGuidePage.OnBeginOpen:mParam is null");
                Close();
                return;
            }

            if (PageManager.Instance.IsOpen(PageID.HomePage))
            {
                var tHomePage = PageManager.Instance.GetHomePage();

                //var tMiniGameEnter = tHomePage.MiniGameEnter.transform;
                //SetClickPos(tMiniGameEnter, true, true, true);
                //SetExplainPos(tHomePage.BtnLevel.transform, new Vector2(0, 350));
                //SetExplainTxt("Let’s take a break from the main game and try out this fun mini-game!", false);
            }
            else if (PageManager.Instance.IsOpen(PageID.MiniMapPage))
            {
                var tMiniMapPage = PageManager.Instance.GetPage<MiniMapPage>(PageID.MiniMapPage);

                var tBtnGame = tMiniMapPage.BtnGame.transform.GetChild(0);
                SetClickPos(tBtnGame, false, false, true);
                SetExplainPos(tBtnGame, new Vector2(0, 350));

                var tName = ModuleManager.MiniGame.GameType switch
                {
                    MiniGameType.Screw => "Screw Crush Mission",
                    MiniGameType.Jam3d => "Jam Crush Mission",
                    MiniGameType.Tile => "Tile Crush Mission",
                    _ => "Crush Mission",
                };

                var tExplainTxt = mGuideKey switch
                {
                    MiniGameConst.Guide_ScrewStart1 => $"Welcome.Commander!\n Click here to start your <#dd2dfa><b>{tName}</b></color>!",
                    MiniGameConst.Guide_JamStart1 => $"Welcome.Commander!\n Click here to start your <#dd2dfa><b>{tName}</b></color>!",
                    MiniGameConst.Guide_ScrewStart2 => $"Congrats!\r\nYou've completed the <#dd2dfa><b>{tName}</b></color>!\r\nMore levels to be discovered...",
                    MiniGameConst.Guide_JamStart2 => $"Congrats!\r\nYou've completed the <#dd2dfa><b>{tName}</b></color>!\r\nMore levels to be discovered...",
                    _ => "Welcome.Commander!\n Click here to start",
                };

                SetExplainTxt(tExplainTxt, false);
            }
            else if (PageManager.Instance.IsOpen(PageID.ScrewGamePage))
            {
                var tScrewPage = PageManager.Instance.GetPage<ScrewGamePage>(PageID.ScrewGamePage);

                if (mGuideKey.Equals(MiniGameConst.Guide_ScreRules))
                {
                    ScrewJam.EventManager.Instance.OnChangeClickState(false);
                    var screws = ScrewJam.EventManager.Instance.GetScrews.Invoke();
                    var list = screws.Keys.ToList();

                    SetClickPos2(screws[list[0]].transform);
                    SetExplainPos(tScrewPage.transform, new Vector2(0, -450));
                    SetExplainTxt($"Fill the boxes with matching screws!", false);

                    _btnNext.gameObject.SetActive(true);
                    mPassEvent.enabled = false;
                    mNextAction = () =>
                    {
                        ScrewJam.EventManager.Instance.EliminateScrewDirectly(list[0]);
                        SetClickPos2(screws[list[1]].transform);

                        mNextAction = () =>
                        {
                            ScrewJam.EventManager.Instance.EliminateScrewDirectly(list[1]);
                            SetClickPos2(screws[list[2]].transform);

                            mNextAction = () =>
                            {
                                ScrewJam.EventManager.Instance.EliminateScrewDirectly(list[2]);
                                ScrewJam.EventManager.Instance.OnChangeClickState(true);
                                Close();
                            };
                        };
                    };

                }
                else if (mGuideKey.Equals(MiniGameConst.Guide_ScreProps))
                {
                    //给玩家道具
                    ModuleManager.Prop.AddProps(new int[] { (int)PropID.ScrewExtraSlot, (int)PropID.ScrewHammer, (int)PropID.ScrewExtraBox }, new int[] { 2, 2, 2 }, PropSource.Guide);

                    SetExplainPos(tScrewPage.BtnProp3.transform, new Vector2(0, 300));
                    SetClickPos(tScrewPage.BtnProp1.transform, true, true, true);
                    SetExplainTxt($"Add a <color=#dd2dfa>new hole</color>! (Up to 2 holes per level)", false);

                    _btnNext.gameObject.SetActive(true);
                    mPassEvent.enabled = false;
                    mNextAction = () =>
                    {
                        SetClickPos(tScrewPage.BtnProp2.transform, true, true, true);
                        SetExplainTxt($"<color=#dd2dfa>Break a glass</color> to release more screws!", false);

                        mNextAction = () =>
                        {
                            SetClickPos(tScrewPage.BtnProp3.transform, true, true, true);
                            SetExplainTxt($"Activate the <color=#dd2dfa>next toolbox</color>! More options!", false);

                            mNextAction = () =>
                            {
                                Close();
                            };
                        };
                    };
                }
                else
                {
                    Close();
                }
            }
            else if (PageManager.Instance.IsOpen(PageID.Jam3DGamePage))
            {
                var tJamPage = PageManager.Instance.GetPage<Jam3DGamePage>(PageID.Jam3DGamePage);

                if (mGuideKey.Equals(MiniGameConst.Guide_JamRules))
                {
                    TileItem tileItem = null;
                    var t = JamManager.GetSingleton().JamGridMono.walkableTiles;
                    var virusItemList = (from tile in t.Values
                                         where (tile && (tileItem = JamManager.GetSingleton().GetTileItem(tile.tileItem)) && (tileItem.VirusState is VirusState.Idle || tileItem.VirusState is VirusState.Born) && tileItem is VirusItem)
                                         select tile.tileItem).ToList();

                    List<TileItem> tileItems = new List<TileItem>();
                    int tColor = 0;
                    for (int i = virusItemList.Count - 1; i >= 0; i--)
                    {
                        var tTileItem = JamManager.GetSingleton().GetTileItem(virusItemList[i]);
                        if (i == virusItemList.Count - 1)
                        {
                            tColor = tTileItem.virusColor;
                            tileItems.Add(tTileItem);
                        }
                        else if (tTileItem.virusColor == tColor)
                        {
                            tileItems.Add(tTileItem);
                        }
                    }

                    SetClickPos2(tileItems[0].transform);
                    SetExplainPos(tJamPage.transform, new Vector2(0, -450));
                    SetExplainTxt($"Match <color=#dd30f7>3 blocks</color> with the <color=#dd30f7>same color</color>. \nNow tap the 1st one!", false);

                    _btnNext.gameObject.SetActive(true);
                    mPassEvent.enabled = false;
                    mNextAction = () =>
                    {
                        tileItems[0].GetComponent<EventTrigger>().OnPointerClick(new PointerEventData(EventSystem.current));
                        SetClickPos2(tileItems[1].transform);
                        SetExplainTxt($"Well done! Tap another one!", false);

                        mNextAction = () =>
                        {
                            tileItems[1].GetComponent<EventTrigger>().OnPointerClick(new PointerEventData(EventSystem.current));
                            SetClickPos2(tileItems[2].transform);
                            SetExplainTxt($"This time, the <color=#dd30f7>3rd</color> one!", false);

                            mNextAction = () =>
                            {
                                tileItems[2].GetComponent<EventTrigger>().OnPointerClick(new PointerEventData(EventSystem.current));
                                SetExplainTxt($"Awesome! 3 blocks are cleared! \r\nNow try <color=#dd30f7>clear the rest</color>. :-)", false);
                                _fingerTran.gameObject.SetActive(false);
                                _guideMask.gameObject.SetActive(false);

                                mNextAction = () =>
                                {
                                    Close();
                                };
                            };
                        };
                    };

                }
                else if (mGuideKey.Equals(MiniGameConst.Guide_JamProps))
                {
                    //给玩家道具
                    ModuleManager.Prop.AddProps(new int[] { (int)PropID.Jam3DShuffle, (int)PropID.Jam3DReplace, (int)PropID.Jam3DRevert }, new int[] { 2, 2, 2 }, PropSource.Guide);

                    SetExplainPos(tJamPage.BtnProp3.transform, new Vector2(0, 300));
                    SetExplainTxt($"Hooray! \nYou get <color=#dd30f7>3 free hints</color>!", false);

                    _btnNext.gameObject.SetActive(true);
                    mPassEvent.enabled = false;
                    mNextAction = () =>
                    {
                        SetClickPos(tJamPage.BtnProp1.transform, true, true, true);
                        SetExplainTxt($"This is <color=#dd30f7>Shuffle</color>. \nIt rearranges the board.", false);

                        mNextAction = () =>
                        {
                            SetClickPos(tJamPage.BtnProp2.transform, true, true, true);
                            SetExplainTxt($"This is <color=#dd30f7>Undo</color>. \nIt reverses your latest step.", false);

                            mNextAction = () =>
                            {
                                SetClickPos(tJamPage.BtnProp3.transform, true, true, true);
                                SetExplainTxt($"This is <color=#dd30f7>Recall</color>. \nIt helps you to release 3 blocks to the backup area.", false);

                                mNextAction = () =>
                                {
                                    _fingerTran.gameObject.SetActive(false);
                                    _guideMask.gameObject.SetActive(false);
                                    SetExplainTxt($"That's it! \nRemember to <color=#dd30f7>use these items</color> when you get stuck.", false);

                                    mNextAction = () =>
                                    {
                                        Close();
                                    };
                                };
                            };
                        };
                    };
                }
                else
                {
                    Close();
                }
            }
            else if (PageManager.Instance.IsOpen(PageID.MiniGameOverPage))
            {
                var tOverPage = PageManager.Instance.GetPage<MiniGameOverPage>(PageID.MiniGameOverPage);

                SetExplainPos(tOverPage.rewardItem.transform, new Vector2(0, -350));
                SetExplainTxt($"There're <color=#dd30f7>JUICY REWARDS</color> after <color=#dd30f7>EACH level</color>!", false);

                _btnNext.gameObject.SetActive(true);
                mPassEvent.enabled = true;
                mNextAction = () =>
                {
                    Close();
                };
            }
        }

        //void GetCamera()
        //{
        //    foreach (var tCams in Camera.allCameras)
        //    {
        //        if (tCams.name.Equals("Tile3dMainCamera") || tCams.name.Equals("Jam3DCamera") || tCams.name.Equals("ScrewJamCamera"))
        //            mSceneCamera = tCams;
        //        if (tCams.name.Equals("UICamera"))
        //            mUICamera = tCams;
        //    }
        //    mSceneCamera ??= Camera.current; 
        //}

        void SetClickPos2(Transform pTarget)
        {
            Camera mSceneCamera = null;
            Camera mUICamera = null;
            foreach (var tCams in Camera.allCameras)
            {
                if (tCams.name.Equals("Tile3dMainCamera") || tCams.name.Equals("Jam3DCamera") || tCams.name.Equals("ScrewJamCamera"))
                    mSceneCamera = tCams;
                if (tCams.name.Equals("UICamera"))
                    mUICamera = tCams;
            }
            var localPoint = GameMethod.OtherWorldToSelfLocalPos(pTarget, GetComponent<RectTransform>(), mSceneCamera, mUICamera);

            _guideMask.Play(pTarget, new Vector2(200, 200));
            _guideMask.TargetArea.anchoredPosition = localPoint;

            mTargetArea.type = Image.Type.Simple;
            mTargetArea.transform.localScale = Vector3.one * 8;
            mTargetArea.transform.DOScale(1.1f, 0.6f);

            _fingerTran.localEulerAngles = new Vector3(0, 0, 90);
            _fingerTran.localScale = new Vector3(-1, 1, 1);
            _fingerTran.anchoredPosition = localPoint;
            _fingerTran.gameObject.SetActive(true);
        }

        void SetClickPos(Transform pTarget, bool pIsCircle = false, bool pIsAnim = false, bool pIsFinger = true)
        {
            mTargetArea.type = pIsCircle ? Image.Type.Simple : Image.Type.Sliced;
            _guideMask.Play(pTarget as RectTransform);

            if (pIsAnim)
            {
                mTargetArea.transform.localScale = Vector3.one * 8;
                mTargetArea.transform.DOScale(1.1f, 0.6f);
            }
            else
            {
                mTargetArea.transform.localScale = Vector3.one * 1.1f;
            }
            if (pIsFinger)
            {
                SetFingerPos(pTarget);
            }
            else
            {
                _fingerTran.gameObject.SetActive(false);
            }
        }

        void SetFingerPos(Transform pReferTran, Vector2 pOffset = default)
        {
            _fingerTran.transform.localPosition = GameMethod.OtherWorldToSelfLocalPos(pReferTran, _fingerTran.transform) + pOffset;
            _fingerTran.gameObject.SetActive(true);
        }

        void SetExplainTxt(string pStr, bool pIsTap)
        {
            _explainTxt.text = pStr;
            _explainTapTxt.gameObject.SetActive(pIsTap);
            _explainRoot.gameObject.SetActive(true);
            _explainRoot.alpha = 0f;
            _explainRoot.DOFade(1, 0.5f);
        }

        void SetExplainPos(Transform pReferTran, Vector2 pOffset)
        {
            _explainRoot.transform.localPosition = GameMethod.OtherWorldToSelfLocalPos(pReferTran, _explainRoot.transform) + pOffset;
        }

        void OnClickBtnNext()
        {
            mNextAction?.Invoke();
        }
    }

    public class MiniGuidePageParam
    {

    }
}
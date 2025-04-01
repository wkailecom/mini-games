using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace ScrewJam
{
    public class GameView : MonoBehaviour
    {
        private Color[] screwColors = new Color[] { Color.red, Color.blue, Color.green, Color.gray };

        public Camera cam;

        public Transform boxRoot;

        public Transform layerRoot;

        public HoleSlotView HoleSlot;

        public Button addHole;

        public Button hammer;

        public Button toolbox;

        private Dictionary<int, Transform> layers = new Dictionary<int, Transform>();

        private Transform[] boards;

        private Transform[] screws;

        private int curBoxIndex;

        private GameObject activeBox;

        private GameObject nextBox;

        private Vector3 nextBoxPosition = new Vector3(-4, 0, 0);

        private Vector3 nextBoxActivePosition = new Vector3(-1.4f, 0, 0);

        private Vector3 currentBoxActivePosition = new Vector3(1.4f, 0, 0);

        private int[] physicsLayer;

        private bool usingHammer;

        private RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[5];

        private List<int> noParentScrewIndex = new List<int>();

        private SpriteAtlas shapeAtlas;

        private bool enableRaycast = true;

        private LevelData levelData;

        private void Awake()
        {
            //
            var tPos = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
            nextBoxPosition = new Vector3(tPos.x, nextBoxPosition.y, nextBoxPosition.z);
        }

        void Start()
        {

        }

        public void Init(LevelData levelData, HoleInfo[] holeInfos)
        {
            this.levelData = levelData;
            physicsLayer = new int[]
            {
                LayerMask.NameToLayer("Layer0"),
                LayerMask.NameToLayer("Layer1"),
                LayerMask.NameToLayer("Layer2"),
                LayerMask.NameToLayer("Layer3"),
                LayerMask.NameToLayer("Layer4"),
                LayerMask.NameToLayer("Layer5"),
                LayerMask.NameToLayer("Layer6"),
                LayerMask.NameToLayer("Layer7"),
            };

            var config = ResourcesManager.LoadAsset<GameConfig>(Constant.CONFIG_PATH);
            screwColors = config.screwColor.ToArray();

            boards = new Transform[levelData.boards.Length];
            List<Transform> screwList = new List<Transform>();

            var tBoards = levelData.boards;

            for (int i = 0; i < tBoards.Length; i++)
            {
                Transform root;
                if (!layers.TryGetValue(tBoards[i].layer, out root))
                {
                    root = new GameObject("layer" + tBoards[i].layer).transform;
                    root.parent = layerRoot;
                    root.localPosition = new Vector3(0, 0, -tBoards[i].layer);
                    layers.Add(tBoards[i].layer, root);
                }
                //创建板子
                var boardObj = CreateBoard(tBoards[i], root);
                var boardView = boardObj.GetComponent<BoardView>();
                ArraySegment<ScrewData> screwSlice = new ArraySegment<ScrewData>();
                if (tBoards[i].screwIndex.Length > 0)
                {
                    screwSlice = new ArraySegment<ScrewData>(levelData.screws, tBoards[i].screwIndex[0], tBoards[i].screwIndex.Length);
                    boardView.SetData(screwSlice, tBoards[i].layer, tBoards[i].screwIndex[0], tBoards[i].screwIndex.Length);
                }
                boardView.SetColor(config.boardColor[tBoards[i].layer]);

                boards[i] = boardObj.transform;
                boardObj.GetComponent<BoardView>().boardInfoIndex = i;

                //创建螺丝
                for (int j = 0; j < tBoards[i].screwIndex.Length; j++)
                {
                    var screwObj = CreateScrew(boardObj.transform);
                    int tIndex = tBoards[i].screwIndex[j];
                    var screwData = levelData.screws[tIndex];
                    screwObj.transform.localPosition = screwData.position;
                    screwObj.GetComponent<ScrewView>().SetColor(screwColors[screwData.colorIndex]);
                    screwList.Add(screwObj.transform);
                }
            }
            screws = screwList.ToArray();
            for (int i = 0; i < tBoards.Length; i++)
            {
                if (tBoards[i].screwIndex.Length == 1)
                {
                    SetJointState(true, i);
                }
                else if (tBoards[i].screwIndex.Length == 0)
                {
                    var boardView = boards[i].GetComponent<BoardView>();
                    boardView.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
                }
            }

            activeBox = CreateBox(levelData.boxes[curBoxIndex]);
            if (levelData.boxes.Length > 1)
            {
                nextBox = CreateBox(levelData.boxes[curBoxIndex + 1]);
                nextBox.transform.localPosition = nextBoxPosition;
            }

            HoleSlot.Init(holeInfos);

            RegisterEvents();
        }

        private void OnClickAddHoleSlot()
        {
            HoleSlot.AddHoleSlot();
            EventManager.Instance.AddHoleSlot();
        }

        private void OnClickHammer()
        {
            usingHammer = !usingHammer;
            hammer.GetComponent<Image>().color = usingHammer ? Color.gray : Color.white;
        }

        private void OnClickToolbox()
        {
            if (nextBox == null)
                return;
            activeBox.transform.DOLocalMoveX(currentBoxActivePosition.x, 0.5f);
            nextBox.transform.DOLocalMoveX(nextBoxActivePosition.x, 0.5f).OnComplete(() =>
            {
                EventManager.Instance.BoxEntryAnimationDone();
            });
            EventManager.Instance.AddBox();
        }

        private void OnChangeClickState(bool pState)
        {
            enableRaycast = pState;
        }

        public void RefreshBox(int curIndex, int nextIndex)
        {
            var removeBox = activeBox;
            var seq = DOTween.Sequence();
            seq.Insert(0, removeBox.transform.DOMoveX(6, 0.5f).OnComplete(() =>
            {
                GameObject.Destroy(removeBox);
            }));
            if (nextBox != null)
            {
                if (Vector3.Distance(nextBox.transform.localPosition, nextBoxPosition) > 0.01f)//两个箱子都要移除
                {
                    //需要移除
                    var removeBox2 = nextBox;
                    seq.Insert(0, removeBox2.transform.DOMoveX(6, 0.5f).OnComplete(() =>
                    {
                        GameObject.Destroy(removeBox2);
                    }));
                    activeBox = CreateBox(levelData.boxes[curIndex]);
                    activeBox.transform.localPosition = nextBoxPosition - new Vector3(1, 0, 0);
                    seq.Insert(0, activeBox.transform.DOMoveX(0, 0.5f));
                }
                else
                {
                    activeBox = nextBox;
                    seq.Insert(0, activeBox.transform.DOMoveX(0, 0.5f));
                }
            }
            if (nextIndex >= 0)
            {
                nextBox = CreateBox(levelData.boxes[nextIndex]);
                nextBox.transform.localPosition = nextBoxPosition - new Vector3(1, 0, 0);
                seq.Insert(0, nextBox.transform.DOMoveX(nextBoxPosition.x, 0.5f));
            }
            seq.OnComplete(() =>
            {
                EventManager.Instance.BoxEntryAnimationDone();
            });
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0) && enableRaycast)
            {
                Vector2 mousePos2D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var hitNum = Physics2D.RaycastNonAlloc(mousePos2D, Vector2.zero, raycastHit2Ds);
                if (hitNum == 0)
                    return;
                var result = raycastHit2Ds.Take(hitNum).Where(t => t.collider.GetComponent<SpriteRenderer>().enabled).OrderBy(t => t.distance).FirstOrDefault();
                if (result.collider != null)
                {
                    var boardView = result.collider.GetComponent<BoardView>();
                    if (usingHammer)
                    {
                        OnBreakBoard(boardView);
                        CreateBreakEffect(layerRoot).transform.position = mousePos2D;
                        EventManager.Instance.triggerEvents.HammerComplete?.Invoke();
                        usingHammer = false;
                        hammer.GetComponent<Image>().color = usingHammer ? Color.gray : Color.white;
                    }
                    else
                    {
                        var index1 = OnClickBoard(boardView, mousePos2D);
                        var indexNoParent = GetNearestScrewFromNoParent(mousePos2D);
                        if (index1 != -1 && indexNoParent != -1)
                        {
                            //查找最近
                            var distance1 = Vector2.Distance(screws[index1].position, mousePos2D);
                            var distance2 = Vector2.Distance(screws[indexNoParent].position, mousePos2D);
                            var nearestIndex = distance1 < distance2 ? index1 : indexNoParent;
                            EliminateScrew(nearestIndex, nearestIndex == index1 ? boardView.boardInfoIndex : -1);
                        }
                        else
                        {
                            var validIndex = index1 != -1 ? index1 : indexNoParent;
                            if (validIndex != -1)
                            {
                                EliminateScrew(validIndex, validIndex == index1 ? boardView.boardInfoIndex : -1);
                            }
                        }
                    }
                }
                else
                {
                    //查找孤立的螺丝
                    var indexNoParent = GetNearestScrewFromNoParent(mousePos2D);
                    if (indexNoParent != -1)
                    {
                        EliminateScrew(indexNoParent, -1);
                    }
                }
            }
        }

        /// <summary>
        /// 使用锤子破坏板子
        /// </summary>
        /// <param name="hitInfo"></param>
        private void OnBreakBoard(BoardView view)
        {
            view.GetComponent<SpriteRenderer>().enabled = false;
            view.GetComponent<Collider2D>().isTrigger = true;
            view.HideHole();
            var rigid2d = view.GetComponent<Rigidbody2D>();
            if (rigid2d.bodyType == RigidbodyType2D.Dynamic)
                rigid2d.bodyType = RigidbodyType2D.Static;
            for (int i = view.startIndex; i <= view.endIndex; i++)
            {
                noParentScrewIndex.Add(i);
            }
            EventManager.Instance.triggerEvents.PlaySound?.Invoke("Panel_Break2");
        }

        /// <summary>
        /// 点击板子
        /// </summary>
        /// <param name="hitInfo"></param>
        /// <returns></returns>
        private int OnClickBoard(BoardView view, Vector3 worldPos)
        {
            var screwIndex = GetNearestScrew(view, worldPos);
            if (screwIndex == -1 || !CheckScrewValid(screwIndex, view.layerIndex))
                return -1;
            return screwIndex;
        }

        private List<Collider2D> checkValidList = new List<Collider2D>();
        /// <summary>
        /// 检查螺丝是否可点击
        /// </summary>
        /// <param name="screwIndex"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        private bool CheckScrewValid(int screwIndex, int layerIndex)
        {
            var p = screws[screwIndex].position;
            if (layerIndex + 1 == physicsLayer.Length)//最上层，可操作
                return true;
            int targetLayer = 0;
            for (int i = layerIndex + 1; i < physicsLayer.Length; i++)
            {
                targetLayer |= 1 << physicsLayer[i];
            }

            ContactFilter2D contact = new ContactFilter2D();
            contact.useLayerMask = true;
            contact.useTriggers = false;
            contact.layerMask = targetLayer;
            var collider = Physics2D.OverlapCircle(p, Constant.SCREW_RADIUS, contact, checkValidList);
            return collider <= 0;
        }

        /// <summary>
        /// 获取板子上距离点最近的螺丝
        /// </summary>
        /// <param name="boardTrans"></param>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        private int GetNearestScrew(BoardView boardView/*Transform boardTrans*/, Vector3 worldPos)
        {
            int result = -1;
            for (int i = boardView.startIndex; i <= boardView.endIndex; i++)
            {
                if (!screws[i].gameObject.activeSelf)
                    continue;
                var distance = Vector2.Distance(screws[i].position, worldPos);
                if (distance < Constant.SCREW_RADIUS)
                    return i;
            }
            return result;
        }

        private int GetNearestScrewFromNoParent(Vector3 worldPos)
        {
            int result = -1;
            if (noParentScrewIndex.Count == 0)
                return result;
            for (int i = 0; i < noParentScrewIndex.Count; i++)
            {
                if (!screws[noParentScrewIndex[i]].gameObject.activeSelf)
                    continue;
                var distance = Vector2.Distance(screws[noParentScrewIndex[i]].position, worldPos);
                if (distance < Constant.SCREW_RADIUS)
                    return noParentScrewIndex[i];
            }
            return result;
        }

        /// <summary>
        /// 消除螺丝
        /// </summary>
        /// <param name="screwIndex"></param>
        /// <param name="boardIndex"></param>
        private void EliminateScrew(int screwIndex, int boardIndex)
        {
            screws[screwIndex].gameObject.SetActive(false);
            if (EventManager.Instance.EliminateScrew(screwIndex, boardIndex))
            {
                //screws[screwIndex].gameObject.SetActive(false);
                EventManager.Instance.triggerEvents.PlaySound?.Invoke("Screw_Clicked_Success");
            }
            else
            {
                screws[screwIndex].gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 设置关节的状态
        /// </summary>
        /// <param name="board"></param>
        public void SetJointState(bool isEnabled, int boardIndex)
        {
            var boardView = boards[boardIndex].GetComponent<BoardView>();
            if (isEnabled)
            {
                var joint = boardView.gameObject.AddComponent<HingeJoint2D>();
                boardView.gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

                for (int i = boardView.startIndex; i <= boardView.endIndex; i++)
                {
                    if (screws[i].gameObject.activeSelf)
                    {
                        joint.anchor = screws[i].localPosition;
                        break;
                    }
                }
            }
            else
            {
                boardView.GetComponent<HingeJoint2D>().enabled = false;
            }
        }

        int boxPlayAnimationCount;

        public void BoxAddScrew(SourceLocation source, int boxIndex, bool isExtra)
        {
            var tBox = isExtra ? nextBox : activeBox;
            Vector3 sourcePos = source.screwIndex != -1 ? screws[source.screwIndex].position : HoleSlot.GetSlotPos(source.slotIndex);
            var boxView = tBox.GetComponent<BoxView>();
            Vector3 targetPos = boxView.GetEmptyHolePos(source.boxHoleIndex);
            var tColor = source.screwIndex != -1 ? screws[source.screwIndex].GetComponent<ScrewView>().Color : HoleSlot.GetSlotColor(source.slotIndex);
            var flyItem = CreateFlyItem(tColor);
            EventManager.Instance.triggerEvents.PlaySound?.Invoke("Screw_Clicked_Success");
            flyItem.transform.position = sourcePos;
            if (source.isFull)
                boxPlayAnimationCount += 1;
            var seq = DOTween.Sequence();
            seq.AppendCallback(() =>
                {
                    flyItem.AnimUp();
                }).AppendInterval(0.15f)
                .Append(flyItem.transform.DOMove(targetPos, 0.5f))
                .AppendCallback(() =>
                {
                    flyItem.AnimDown();
                }).AppendInterval(0.15f)
                .AppendCallback(() =>
                {
                    boxView.AddScrew();
                    GameObject.Destroy(flyItem.gameObject);
                });
            if (source.isFull)
            {
                seq.AppendCallback(() =>
                {
                    boxView.PlayAnimation();
                }).AppendInterval(0.5f).AppendCallback(() =>
                {
                    boxPlayAnimationCount -= 1;
                    boxPlayAnimationCount = Mathf.Max(0, boxPlayAnimationCount);
                    if (boxPlayAnimationCount == 0)
                        EventManager.Instance.BoxAnimationDone();
                });
            }
        }

        public void HoleSlotAddScrew(int screwIndex, int slotIndex, int colorIndex)
        {
            var tColor = screws[screwIndex].GetComponent<ScrewView>().Color;
            Vector3 sourcePos = screws[screwIndex].position;
            Vector3 targetPos = HoleSlot.GetSlotPos(slotIndex);
            var flyItem = CreateFlyItem(tColor);
            flyItem.transform.position = sourcePos;
            var seq = DOTween.Sequence();
            seq.AppendCallback(() =>
            {
                flyItem.AnimUp();
            }).AppendInterval(0.15f)
            .Append(flyItem.transform.DOMove(targetPos, 0.5f))
            .AppendCallback(() =>
            {
                flyItem.AnimDown();
            }).AppendInterval(0.15f)
            .AppendCallback(() =>
            {
                EventManager.Instance.HoleSlotAnimationDone(slotIndex);
                HoleSlot.AddScrew(slotIndex, screwColors[colorIndex]);
                GameObject.Destroy(flyItem.gameObject);
            });
            EventManager.Instance.triggerEvents.PlaySound?.Invoke("Screw_Clicked_Success");
        }

        public void RemoveSlot(int slotIndex)
        {
            HoleSlot.RemoveScrew(slotIndex);
        }

        private GameObject CreateBox(BoxData boxData)
        {
            var tBox = ResourcesManager.LoadAsset<GameObject>(Constant.BOX_PATH);
            var boxObj = Instantiate(tBox, boxRoot);
            var boxView = boxObj.GetComponent<BoxView>();
            boxView.SetBoxColor(screwColors[boxData.colorIndex]);
            boxView.SetBoxWidth(boxData.count);
            return boxObj;
        }

        private GameObject CreateBoard(BoardData boardData, Transform parent)
        {
            var tBoard = ResourcesManager.LoadAsset<GameObject>(Constant.BOARD_TEMPLETE_PATH);
            if (shapeAtlas == null)
            {
                shapeAtlas = ResourcesManager.LoadAsset<SpriteAtlas>(Constant.SHAPE_ATLAS);
            }
            var sp = shapeAtlas.GetSprite(boardData.boardName);
            //var tex = ResourcesManager.LoadAsset<Texture2D>(Constant.SHAPE_PATH + boardData.boardName);
            //var sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            var renderer = tBoard.GetComponent<SpriteRenderer>();
            renderer.sprite = sp;
            var obj = Instantiate(tBoard, parent);
            obj.name = boardData.boardName;
            obj.AddComponent<PolygonCollider2D>();
            obj.transform.localPosition = boardData.position;
            obj.transform.localEulerAngles = boardData.eulerAngle;
            obj.layer = physicsLayer[boardData.layer];
            return obj;
        }

        private GameObject CreateScrew(Transform parent)
        {
            var tScrew = ResourcesManager.LoadAsset<GameObject>(Constant.SCREW_PATH);
            var obj = Instantiate(tScrew, parent);
            return obj;
        }

        private GameObject CreateBreakEffect(Transform parent)
        {
            var tEffect = ResourcesManager.LoadAsset<GameObject>(Constant.BOARD_BREAK_EFFECT);
            var obj = Instantiate(tEffect, parent);
            return obj;
        }

        private FlyItem CreateFlyItem(Color color)
        {
            var tItem = ResourcesManager.LoadAsset<GameObject>(Constant.SCREW_FLYITEM_PATH);
            var obj = Instantiate(tItem);
            SceneManager.MoveGameObjectToScene(obj, gameObject.scene);
            var item = obj.GetComponent<FlyItem>();
            item.SetColor(color);
            return item;
        }

        private void RegisterEvents()
        {
            EventManager.Instance.OnHoleSlotAddScrew += HoleSlotAddScrew;
            EventManager.Instance.OnHoleSlotRemove += RemoveSlot;
            EventManager.Instance.OnBoxAddScrew += BoxAddScrew;
            EventManager.Instance.OnBoardJointEnabled += SetJointState;
            EventManager.Instance.OnBoxRefresh += RefreshBox;

            EventManager.Instance.OnClickAddHoleSlot += OnClickAddHoleSlot;
            EventManager.Instance.OnClickHammer += OnClickHammer;
            EventManager.Instance.OnClickToolbox += OnClickToolbox;
            EventManager.Instance.OnChangeClickState += OnChangeClickState;
            EventManager.Instance.GetScrews += GetScrews;
            Action<int> ta = EliminateScrewDirectly;
            ta += EventManager.Instance.EliminateScrewDirectly;
            EventManager.Instance.EliminateScrewDirectly = ta;
        }

        private void UnregisterEvents()
        {
            EventManager.Instance.OnHoleSlotAddScrew -= HoleSlotAddScrew;
            EventManager.Instance.OnHoleSlotRemove -= RemoveSlot;
            EventManager.Instance.OnBoxAddScrew -= BoxAddScrew;
            EventManager.Instance.OnBoardJointEnabled -= SetJointState;
            EventManager.Instance.OnBoxRefresh -= RefreshBox;

            EventManager.Instance.OnClickAddHoleSlot -= OnClickAddHoleSlot;
            EventManager.Instance.OnClickHammer -= OnClickHammer;
            EventManager.Instance.OnClickToolbox -= OnClickToolbox;
            EventManager.Instance.OnChangeClickState -= OnChangeClickState;
            EventManager.Instance.GetScrews -= GetScrews;
            EventManager.Instance.EliminateScrewDirectly -= EliminateScrewDirectly;
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        #region Guide
        private void EliminateScrewDirectly(int screwIndex)
        {
            screws[screwIndex].gameObject.SetActive(false);
        }

        private Dictionary<int, Transform> GetScrews()
        {
            //直接取三个
            var result = new Dictionary<int, Transform>();
            for (int i = 0; i < 3; i++)
            {
                result.Add(i, screws[i]);
            }
            return result;
        }
        #endregion
    }

}

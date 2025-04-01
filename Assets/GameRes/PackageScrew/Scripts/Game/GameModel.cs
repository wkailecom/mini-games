using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScrewJam
{
    public class GameModel : MonoSingleton<GameModel>
    {
        //public int level;

        public LevelData levelData { get; private set; }

        public BoardInfo[] boardInfos { get; private set; }

        public ScrewInfo[] screwInfos { get; private set; }

        public HoleInfo[] holeSlotInfos { get; private set; }

        public BoxInfo[] boxInfos { get; private set; }

        public int curBoxIndex { get; private set; }

        public bool extraBoxEnabled { get; private set; }

        private bool readyToRefreshBox = false;

        private bool boxAnimationDone = true;

        private bool boxEntryAnimationDone = true;

        private bool[] holeSlotAnimationDone;

        private bool isFailed = false;

        private bool isSuccess = false;

        void Start()
        {

        }

        public void StartLevel(int levelId)
        {
            Init(levelId);
            GetComponent<GameView>().Init(levelData, holeSlotInfos);
        }

        public void Init(int levelId)
        {
            Debug.Log("GameModel Init called");
            List<BoardData> boardDataList = new List<BoardData>();
            List<BoxData> boxList = new List<BoxData>();
            List<ScrewData> screwList = new List<ScrewData>();
            Debug.Log(("GameModel List is null: " + boardDataList == null));
            var levelStr = ResourcesManager.LoadAsset<TextAsset>(Constant.LEVELDATA_PATH + levelId.ToString());
            Debug.Log("GameModel levelstr content: " + levelStr.text);
            levelData = JsonConvert.DeserializeObject<LevelData>(levelStr.text);
            Debug.Log("GameModel screws length: "+levelData.screws.Length); 
            Debug.Log("GameModel boards length: "+levelData.boards.Length);
            var levelDataType = typeof(LevelData);
            Debug.Log($"levelDataType:{levelDataType.Name},namespace:{levelDataType.Namespace},assembly:{levelDataType.Assembly}");
            var boardDataType = typeof(BoardData);
            Debug.Log($"boardDataType:{boardDataType.Name}");
            var boardListType = typeof(List<BoardData>);
            Debug.Log($"boardDataListType:{boardListType.Name}");
            
            BoardData[] tBoards = levelData.boards;
            screwInfos = new ScrewInfo[levelData.screws.Length];
            boardInfos = new BoardInfo[levelData.boards.Length];

            holeSlotInfos = new HoleInfo[7];
            holeSlotAnimationDone = new bool[7];
            for (int i = 0; i < holeSlotInfos.Length; i++)
            {
                holeSlotInfos[i].colorIndex = -1;
                bool flag = (i == 0 || i == holeSlotInfos.Length - 1);
                holeSlotInfos[i].isEnabled = !flag;
                holeSlotAnimationDone[i] = true;
            }

            boxInfos = new BoxInfo[levelData.boxes.Length];
            for (int i = 0; i < levelData.boxes.Length; i++)
            {
                boxInfos[i].colorIndex = levelData.boxes[i].colorIndex;
                boxInfos[i].holeNum = levelData.boxes[i].count;
            }

            for (int i = 0; i < tBoards.Length; i++)
            {
                BoardInfo boardInfo = default(BoardInfo);
                try
                {
                    boardInfo = new BoardInfo
                    {
                        layer = tBoards[i].layer,
                        screwStartIndex = tBoards[i].screwIndex.Length > 0 ? tBoards[i].screwIndex[0]:-1,
                        screwLength = tBoards[i].screwIndex.Length,
                        remain = tBoards[i].screwIndex.Length
                    };
                }
                catch (Exception e)
                {
                    Debug.Log("=====");
                    throw;
                }
                boardInfos[i] = boardInfo;

                for (int j = 0; j < tBoards[i].screwIndex.Length; j++)
                {
                    int tIndex = tBoards[i].screwIndex[j];
                    var screwData = levelData.screws[tIndex];
                    screwInfos[tIndex] = new ScrewInfo
                    {
                        colorIndex = screwData.colorIndex,
                    };
                }
            }

            EventManager.Instance.OnEliminateScrew += ElimiateScrew;
            EventManager.Instance.OnAddHoleSlot += AddHoleSlot;
            EventManager.Instance.OnAddBox += AddBox;
            EventManager.Instance.OnBoxAnimationDone += SetBoxAnimationDone;
            EventManager.Instance.OnHoleSlotAnimationDone += SetHoleSlotAnimationDone;
            EventManager.Instance.OnBoxEntryAnimationDone += BoxEntryAnimationDone;
            EventManager.Instance.OnClickToolbox += OnClickToolbox;
            EventManager.Instance.OnTriggerReplay += OnTriggerReplay;
            EventManager.Instance.CheckCanExtraSlotUse += CheckCanExtraSlotUse;
            EventManager.Instance.EliminateScrewDirectly += EliminateScrewDirectly;
            EventManager.Instance.OnChangeClickState += OnChangeClickState;
            
            Debug.Log("GameModel Init finish");
        }

        private void OnTriggerReplay()
        {
            isFailed = false;
        }

        private bool CheckCanExtraSlotUse()
        {
            for (int i = 0; i < holeSlotInfos.Length; i++)
            {
                if (!holeSlotInfos[i].isEnabled)
                    return true;
            }
            return false;
        }

        public bool ElimiateScrew(int screwIndex, int boardIndex)
        {
            int colorIndex = screwInfos[screwIndex].colorIndex;
            if (CheckBoxColorAndStatus(colorIndex, out int matchBoxIndex))
            {
                ChangeState(boardIndex);
                SourceLocation source = new SourceLocation
                {
                    screwIndex = screwIndex,
                    slotIndex = -1
                };
                MoveToBox(source, matchBoxIndex);
                return true;
            }
            else if (!CheckSlotIsFull())
            {
                ChangeState(boardIndex);
                MoveToSlot(screwIndex, colorIndex);
                return true;
            }
            return false;
        }

        private void ChangeState(int boardIndex)
        {
            if (boardIndex != -1)
            {
                boardInfos[boardIndex].remain -= 1;
                if (boardInfos[boardIndex].remain == 1)
                {
                    EventManager.Instance.SetJointState(true, boardIndex);
                }
                else if (boardInfos[boardIndex].remain == 0)
                {
                    EventManager.Instance.SetJointState(false, boardIndex);
                }
            }
        }

        public bool CheckBoxColorAndStatus(int colorIndex, out int boxIndex)
        {
            boxIndex = -1;
            bool curBoxMatch = boxInfos[curBoxIndex].colorIndex == colorIndex && !boxInfos[curBoxIndex].IsFull && boxEntryAnimationDone;
            if (curBoxMatch)
            {
                boxIndex = curBoxIndex;
                return true;
            }
            if (extraBoxEnabled)
            {
                bool nextBoxMatch = boxInfos[curBoxIndex + 1].colorIndex == colorIndex && !boxInfos[curBoxIndex + 1].IsFull;
                if (nextBoxMatch)
                {
                    boxIndex = curBoxIndex + 1;
                    return true;
                }
            }
            return false;
        }

        public void Update()
        {
            if (!updateEnable)
                return;
            if (isFailed)
                return;
            if (boxEntryAnimationDone)
            {
                UpdateSlot();
            }
            if (readyToRefreshBox && boxAnimationDone)
            {
                RefreshNextBox();
                readyToRefreshBox = false;
            }
        }

        bool updateEnable = true;
        private void OnChangeClickState(bool enabled)
        {
            updateEnable = enabled;
        }

        public void SetBoxAnimationDone()
        {
            boxAnimationDone = true;
            if (isSuccess)
                EventManager.Instance.triggerEvents.OnGameSuccess?.Invoke();
        }

        public void SetHoleSlotAnimationDone(int slotIndex)
        {
            holeSlotAnimationDone[slotIndex] = true;
        }

        public void BoxEntryAnimationDone()
        {
            boxEntryAnimationDone = true;
        }

        public void OnClickToolbox()
        {
            boxEntryAnimationDone = false;
        }

        private void UpdateSlot()
        {
            if (holeSlotInfos == null)
            {
                Debug.Log("GameModel holeSlotInfos is null");
            }

            if (holeSlotAnimationDone == null)
            {
                Debug.Log("GameModel holeSlotAnimationDone is null");
            }
            for (int i = 0; i < holeSlotInfos.Length; i++)
            {
                //Debug.Log($"GameModel UpdateSlot i:{i}");
                if (!holeSlotAnimationDone[i])
                    continue;
                if (CheckBoxColorAndStatus(holeSlotInfos[i].colorIndex, out int extraIndex))
                {
                    RemoveSlot(i);
                    SourceLocation source = new SourceLocation
                    {
                        screwIndex = -1,
                        slotIndex = i
                    };
                    MoveToBox(source, extraIndex);
                }
            }
            //Debug.Log("GameModel UpdateSlot finish");
            if (CheckIsFailed())
            {
                isFailed = true;
                Debug.Log("GameModel Failure");
                EventManager.Instance.triggerEvents.OnGameFailed.Invoke();
            }
        }

        private bool AddHoleSlot()
        {
            bool result = false;
            for (int i = holeSlotInfos.Length - 1; i >= 0; i--)
            {
                if (holeSlotInfos[i].isEnabled)
                    continue;
                holeSlotInfos[i].isEnabled = true;
                result = true;
                EventManager.Instance.triggerEvents.ExtraSlotComplete?.Invoke();
                break;
            }
            return result;
        }

        private void RemoveSlot(int slotIndex)
        {
            holeSlotInfos[slotIndex].colorIndex = -1;
            EventManager.Instance.RemoveSlot(slotIndex);
        }

        private void MoveToBox(SourceLocation source, int boxIndex)
        {
            boxAnimationDone = false;
            var tInfo = boxInfos[boxIndex];
            source.boxHoleIndex = tInfo.curNum;
            tInfo.curNum += 1;
            boxInfos[boxIndex] = tInfo;
            if (tInfo.IsFull)
            {
                source.isFull = true;
                if (CheckSuccess())
                {
                    EventManager.Instance.BoxAddScrew(source, boxIndex, boxIndex != curBoxIndex);
                    Debug.Log("success");
                    EventManager.Instance.triggerEvents.ReadyToSuccess.Invoke();
                    isSuccess = true;
                }
                else
                {
                    readyToRefreshBox = true;
                    EventManager.Instance.BoxAddScrew(source, boxIndex, boxIndex != curBoxIndex);
                }
            }
            else
            {
                EventManager.Instance.BoxAddScrew(source, boxIndex, boxIndex != curBoxIndex);
            }
        }

        private void RefreshNextBox()
        {
            if (extraBoxEnabled)
            {
                //检查两个箱子都满了
                if (boxInfos[curBoxIndex].IsFull && boxInfos[curBoxIndex + 1].IsFull)
                {
                    curBoxIndex += 2;
                    boxEntryAnimationDone = false;
                    EventManager.Instance.RefreshBox(curBoxIndex, curBoxIndex + 1 < boxInfos.Length ? curBoxIndex + 1 : -1);
                    extraBoxEnabled = false;
                }
            }
            else
            {
                //刷新下个箱子
                curBoxIndex += 1;
                boxEntryAnimationDone = false;
                EventManager.Instance.RefreshBox(curBoxIndex, curBoxIndex + 1 < boxInfos.Length ? curBoxIndex + 1 : -1);
            }

        }

        private void MoveToSlot(int screwIndex, int colorIndex)
        {
            var slotIndex = Array.FindIndex(holeSlotInfos, t => t.colorIndex == -1 && t.isEnabled);
            if (slotIndex != -1)
            {
                holeSlotAnimationDone[slotIndex] = false;
                holeSlotInfos[slotIndex].colorIndex = colorIndex;
                EventManager.Instance.HoleSlotAddScrew(screwIndex, slotIndex, colorIndex);
            }
        }

        private void AddBox()
        {
            if (!extraBoxEnabled)
            {
                extraBoxEnabled = true;
                EventManager.Instance.triggerEvents.ExtraBoxComplete?.Invoke();
            }
        }

        private bool CheckSuccess()
        {
            var flag1 = holeSlotInfos.Count(t => t.colorIndex != -1);
            if (flag1 > 0)
                return false;
            var flag2 = boxInfos.Count(t => !t.IsFull);
            if (flag2 > 0)
                return false;
            return true;
        }

        /// <summary>
        /// 待放区是否满了
        /// </summary>
        /// <returns></returns>
        private bool CheckSlotIsFull()
        {
            var count = holeSlotInfos.Count(t => t.isEnabled && t.colorIndex == -1);
            return count == 0;
        }

        private bool CheckIsFailed()
        {
            //待放区满，并且没有螺丝钉将被消除
            bool isSlotFull = CheckSlotIsFull();

            for (int i = 0; i < holeSlotInfos.Length; i++)
            {
                if (holeSlotInfos[i].colorIndex == -1)
                    continue;
                if (holeSlotInfos[i].colorIndex == boxInfos[curBoxIndex].colorIndex && !boxInfos[curBoxIndex].IsFull)
                {
                    return false;
                }
            }

            //当前的箱子未满
            if (!boxInfos[curBoxIndex].IsFull)
                return isSlotFull;

            if (curBoxIndex + 1 >= boxInfos.Length)
                return isSlotFull;

            bool isBoxFull = true;
            for (int i = 0; i < holeSlotInfos.Length; i++)
            {
                if (holeSlotInfos[i].colorIndex != -1
                    && holeSlotInfos[i].colorIndex == boxInfos[curBoxIndex + 1].colorIndex)
                {
                    isBoxFull = false;
                    break;
                }
            }
            return isSlotFull && isBoxFull;
        }

        #region Guide
        private void EliminateScrewDirectly(int screwIndex)
        {
            int boardIndex = -1;
            for (int i = 0; i < boardInfos.Length; i++)
            {
                var info = boardInfos[i];
                if (info.screwStartIndex <= screwIndex
                    && (info.screwStartIndex + info.screwLength) > screwIndex)
                {
                    boardIndex = i;
                    break;
                }
            }
            ElimiateScrew(screwIndex, boardIndex);
        }

        #endregion

        private void OnDestroy()
        {
            EventManager.Instance.OnEliminateScrew -= ElimiateScrew;
            EventManager.Instance.OnAddHoleSlot -= AddHoleSlot;
            EventManager.Instance.OnAddBox -= AddBox;
            EventManager.Instance.OnBoxAnimationDone -= SetBoxAnimationDone;
            EventManager.Instance.OnHoleSlotAnimationDone -= SetHoleSlotAnimationDone;
            EventManager.Instance.OnBoxEntryAnimationDone -= BoxEntryAnimationDone;
            EventManager.Instance.OnClickToolbox -= OnClickToolbox;
            EventManager.Instance.OnTriggerReplay -= OnTriggerReplay;
            EventManager.Instance.CheckCanExtraSlotUse -= CheckCanExtraSlotUse;
            EventManager.Instance.EliminateScrewDirectly -= EliminateScrewDirectly;
            EventManager.Instance.OnChangeClickState -= OnChangeClickState;
        }
    }


    public struct ScrewInfo
    {
        public int colorIndex;
        public Vector3 position;
    }

    public struct HoleInfo
    {
        public int colorIndex;
        public bool isEnabled;
    }

    public struct BoxInfo
    {
        public int colorIndex;
        public int holeNum;
        public int curNum;
        public bool IsFull { get => curNum == holeNum; }
    }

    public struct BoardInfo
    {
        public int layer;
        public int screwStartIndex;
        public int screwLength;
        public int remain;//剩余螺丝数

        public bool IsEnable { get => remain > 0; }
        public int screwEndIndex { get => screwStartIndex + screwLength; }
        public bool IsEmpty { get => remain == 0; }
    }

    public struct SourceLocation
    {
        public int screwIndex;
        public int slotIndex;
        public bool isFull;
        public int boxHoleIndex;
    }
}

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ScrewJam.ScrewEditor
{
    public class Generator
    {
        /// <summary>
        /// 随机分配颜色
        /// </summary>
        /// <param name="totalColor"></param>
        /// <param name="screws"></param>
        /// <param name="boxes"></param>
        public static void RandomDistributeColorIndex(int totalColor, List<ScrewData> screws, List<BoxData> boxes)
        {
            //获取总螺丝数
            int totalScrew = screws.Count;
            //一共需要几组
            var targetGroup = totalScrew / 3;
            List<int> colorList = new List<int>();
            List<int> boxList = new List<int>();
            int colorIndex = 0;
            for (int i = 0; i < targetGroup; i++)
            {
                boxList.Add(colorIndex);
                for (int j = 0; j < 3; j++)
                {
                    colorList.Add(colorIndex);
                }
                colorIndex++;
                if (colorIndex == totalColor)
                {
                    colorIndex = 0;
                }
            }

            //打乱
            Shuffle(colorList);
            for (int i = 0; i < screws.Count; i++)
            {
                var t = screws[i];
                t.colorIndex = colorList[i];
                screws[i] = t;
            }

            Shuffle(boxList);
            for (int i = 0; i < boxes.Count; i++)
            {
                var t = boxes[i];
                t.colorIndex = boxList[i];
                boxes[i] = t;
            }
        }

        /// <summary>
        /// 根据生成的螺丝遮挡信息随机分配颜色
        /// </summary>
        /// <param name="needColorNum"></param>
        /// <param name="screws"></param>
        /// <param name="boxes"></param>
        /// <param name="screwEditorData"></param>
        public static void DistributeColorByEditorData(int needColorNum, int maxColorNum, List<ScrewData> screws, List<BoxData> boxes, IEnumerable<ScrewEditorData> screwEditorData, int hardLevel)
        {
            //构造数据
            var editorDataList = screwEditorData.ToList();
            //螺丝索引列表，前置数量少的排在前面
            List<int> screwIndexList = new List<int>();
            //以依赖数量为阶段，每个阶段各存在多少螺丝
            List<int> preStageNumber = new List<int>();
            int[] number = new int[editorDataList.Count];
            //按照前置螺丝数量分组
            var groupedData = screwEditorData.GroupBy(t => Mathf.Max(0, t.preIndexArray.Length - hardLevel)).OrderBy(t => t.Key).ToList();
            foreach (var editorDatas in groupedData)
            {
                preStageNumber.Add(editorDatas.Count());
                number[editorDatas.Key] = editorDatas.Count();
                screwIndexList.AddRange(editorDatas.Select(t => editorDataList.IndexOf(t)));
            }
            var s = string.Join(",", screwIndexList);
            Debug.Log("分组顺序排列的螺丝Index: " + s);

            //提供每个区间结束位置的索引，使用已分配的螺丝数量获取
            int[] endIndexList = new int[screwIndexList.Count];
            int temp = 0;
            for (int i = 0; i < screwIndexList.Count; i++)
            {
                temp += number[i];
                endIndexList[i] = temp - 1;
            }

            //随机
            List<BoxData> boxList = new List<BoxData>();
            var targetGroup = screws.Count / 3;
            var extra = screws.Count % 3;
            if (extra == 2)
            {
                targetGroup += 1;
            }
            int c = 0;
            for (int i = 0; i < targetGroup; i++)
            {
                BoxData boxData = new BoxData();
                boxData.colorIndex = c;
                if (extra == 1 && i == targetGroup - 1)
                {
                    boxData.count = 4;
                }
                else if (extra == 2 && i == targetGroup - 1)
                {
                    boxData.count = 2;
                }
                else
                {
                    boxData.count = 3;
                }
                boxList.Add(boxData);
                c++;
                if (c == needColorNum)
                    c = 0;
            }
            Shuffle(boxList);

            //根据现有的盒子颜色，随机螺丝颜色
            int curColorIndex = 0;
            int colorCounter = 0;
            System.Random rand = new System.Random();
            int startIndex = 0;
            for (int i = 0; i < screwIndexList.Count; i++)
            {
                int endIndex = endIndexList[i];
                int tIndex = rand.Next(startIndex, endIndex);
                var sc = screws[screwIndexList[tIndex]];
                try
                {
                    //Debug.Log($"curColorIndex:{curColorIndex}");
                    sc.colorIndex = boxList[curColorIndex].colorIndex;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    throw;
                }
                screws[screwIndexList[tIndex]] = sc;

                screwIndexList[tIndex] = screwIndexList[startIndex];
                startIndex += 1;

                if (colorCounter >= boxList[curColorIndex].count - 1)
                {
                    curColorIndex++;
                    colorCounter = 0;
                }
                else
                {
                    colorCounter += 1;
                }
            }

            //颜色映射
            List<int> numbers = new List<int>();
            for (int i = 0; i < maxColorNum; i++)
            {
                numbers.Add(i);
            }
            Dictionary<int, int> assignedNumbers = new Dictionary<int, int>();
            for (int i = 0; i < needColorNum; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, numbers.Count);
                int randomNumber = numbers[randomIndex];
                assignedNumbers.Add(i, randomNumber);
                numbers.RemoveAt(randomIndex);
            }
            for (int i = 0; i < screws.Count; i++)
            {
                var tData = screws[i];
                tData.colorIndex = assignedNumbers[tData.colorIndex];
                screws[i] = tData;
            }
            for (int i = 0; i < boxList.Count; i++)
            {
                var tData = boxList[i];
                tData.colorIndex = assignedNumbers[tData.colorIndex];
                boxList[i] = tData;
            }


            boxes.Clear();
            for (int i = 0; i < boxList.Count; i++)
            {
                BoxData boxData = new BoxData();
                boxData.colorIndex = boxList[i].colorIndex;
                boxData.count = boxList[i].count;
                boxes.Add(boxData);
            }

            var s3 = string.Join(",", preStageNumber);
            Debug.Log("每阶段螺丝数: " + s3);

            var s4 = string.Join(",", number);
            Debug.Log("螺丝数和螺丝依赖数的关系: " + s4);

            var s5 = string.Join(",", endIndexList);
            Debug.Log("点击螺丝数量和取值范围的关系" + s5);

            var s6 = string.Join(",", boxList);
            Debug.Log("盒子颜色:" + s6);

            var s7 = string.Join(",", screws.Select(t => t.colorIndex));
            Debug.Log(s7);
        }

        public static void Shuffle<T>(List<T> list)
        {
            System.Random random = new System.Random();
            int count = list.Count;
            for (int i = count - 1; i > 0; i--)
            {
                int j = random.Next(0, count - 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        public static void NewColoring(int needColor, int totalColor, List<ScrewData> screws, List<BoxData> boxes, IEnumerable<ScrewEditorData> screwEditorData, int hardLevel)
        {
            int screwNumber = screws.Count;

            //构造数据
            var editorDataList = screwEditorData.ToList();
            var s = string.Join(",", editorDataList.Select(t => t.preCount));
            Debug.Log($"前置数量{s}");

            //存储螺丝Index和前置Index
            Dictionary<int, List<int>> screwIndexPreDic = new Dictionary<int, List<int>>();
            for (int i = 0; i < screwNumber; i++)
            {
                screwIndexPreDic.Add(editorDataList[i].selfIndex, new List<int>(editorDataList[i].preIndexArray));
            }

            int count = 0;

            int curGroupID = 1;
            int groupCounter = 0;
            List<int> waitList = new List<int>();
            //当前待放数量
            int waitNumber = 0;
            //箱子分组id
            List<int> boxGroupID = new List<int>();
            while (count < 200)
            {
                count++;
                //存在等待消除的
                if (waitList.Count > 0)
                {
                    bool resultExist = false;
                    for (int i = waitList.Count - 1; i >= 0; i--)
                    {
                        //选择没有前置的
                        if (screwIndexPreDic[waitList[i]].Count == 0)
                        {
                            PickScrew(waitList[i], curGroupID, screwIndexPreDic, screws);
                            waitNumber += 1;
                            groupCounter += 1;
                            if (groupCounter > 2)
                            {
                                if (waitNumber > 4)
                                {
                                    boxGroupID.Insert(Mathf.Max(0, boxGroupID.Count - 2), curGroupID);
                                }
                                else
                                {
                                    boxGroupID.Add(curGroupID);
                                }
                                curGroupID += 1;
                                groupCounter = 0;
                            }
                            waitList.RemoveAt(i);
                            resultExist = true;
                        }
                    }
                    if (resultExist)
                        continue;
                }


                var targetScrewIndex = GetScrewLessThan(hardLevel + 1, screwIndexPreDic, screws);
                //未找到小于目标值的螺丝
                if (targetScrewIndex == -1)
                {
                    targetScrewIndex = GetScrewWithoutPre(screwIndexPreDic, screws);
                    if (targetScrewIndex != -1)
                        PickScrew(targetScrewIndex, curGroupID, screwIndexPreDic, screws);

                }
                else
                {
                    PickScrew(targetScrewIndex, curGroupID, screwIndexPreDic, screws);
                    var targetScrewPreNumber = screwIndexPreDic[targetScrewIndex].Count;
                    //把前置都加入到待消除列表
                    for (int i = 0; i < targetScrewPreNumber; i++)
                    {
                        if (!waitList.Contains(screwIndexPreDic[targetScrewIndex][i]))
                        {
                            waitList.Add(screwIndexPreDic[targetScrewIndex][i]);
                        }
                    }
                }

                if (CheckNeedBreak(screws))
                {
                    if (!boxGroupID.Contains(curGroupID))
                        boxGroupID.Add(curGroupID);
                    break;
                }

                waitNumber = 0;
                groupCounter += 1;
                if (groupCounter > 2)
                {
                    boxGroupID.Add(curGroupID);
                    curGroupID += 1;
                    groupCounter = 0;
                }
            }

            //使用groupID分配颜色
            int maxGroupID = int.MinValue;
            for (int i = 0; i < screws.Count; i++)
            {
                if (maxGroupID < screws[i].groupID)
                {
                    maxGroupID = screws[i].groupID;
                }
            }
            //给组分配颜色
            Dictionary<int, int> groupIDMap = new Dictionary<int, int>();
            int curIndex = 0;
            for (int i = 1; i <= maxGroupID; i++)
            {
                groupIDMap.Add(i, curIndex);
                if (curIndex == needColor - 1)
                {
                    curIndex = 0;
                }
                else
                {
                    curIndex += 1;
                }
            }

            //颜色映射
            List<int> numbers = new List<int>();
            for (int i = 0; i < totalColor; i++)
            {
                numbers.Add(i);
            }
            Dictionary<int, int> assignedNumbers = new Dictionary<int, int>();
            for (int i = 0; i < needColor; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, numbers.Count);
                int randomNumber = numbers[randomIndex];
                assignedNumbers.Add(i, randomNumber);
                numbers.RemoveAt(randomIndex);
            }

            for (int i = 1; i <= groupIDMap.Count; i++)
            {
                groupIDMap[i] = assignedNumbers[groupIDMap[i]];
            }

            //若余数为1
            if (screws.Count % 3 == 1)
            {
                boxGroupID[boxGroupID.Count - 1] = boxGroupID[boxGroupID.Count - 2];
                var boxData = boxes[boxes.Count - 1];
                boxData.count = 4;
                boxes[boxes.Count - 1] = boxData;
                groupIDMap[boxGroupID.Count] = groupIDMap[boxGroupID.Count - 1];
            }
            else if (screws.Count % 3 == 2)
            {
                boxes.Add(new BoxData
                {
                    colorIndex = -1,
                    count = 2,
                });
                boxGroupID.Add(curGroupID); 
            }

            for (int i = 0; i < screws.Count; i++)
            {
                var screwData = screws[i];
                screwData.colorIndex = groupIDMap[screwData.groupID];
                screws[i] = screwData;
            }
            for (int i = 0; i < boxes.Count; i++)
            {
                var boxData = boxes[i];
                boxData.colorIndex = groupIDMap[boxGroupID[i]];
                boxes[i] = boxData;
            }



            Debug.Log($"分组情况：{string.Join(",", screws.Select(t => t.groupID))}");

            var s1 = screwIndexPreDic.Values.Select(t => t.Count);
            Debug.Log("查看剩余螺丝的前置情况:" + string.Join(",", s1));

            var s7 = string.Join(",", screws.Select(t => t.colorIndex));
            Debug.Log(s7);
        }

        //拿取指定的螺丝
        private static void PickScrew(int pIndex, int groupID, Dictionary<int, List<int>> screwIndexPreDic, List<ScrewData> screws)
        {
            try
            {
                //设置group
                var screwData = screws[pIndex];
                screwData.groupID = groupID;
                screws[pIndex] = screwData;
                //刷新前置列表
                for (int i = 0; i < screwIndexPreDic.Count; i++)
                {
                    screwIndexPreDic[i].Remove(pIndex);
                }
            }
            catch (Exception e)
            {
                Debug.Log("===");
                throw;
            }
        }

        //获取前置不为0且小于x的螺丝index
        private static int GetScrewLessThan(int x, Dictionary<int, List<int>> screwIndexPreDic, List<ScrewData> screws)
        {
            for (int i = 0; i < screwIndexPreDic.Count; i++)
            {
                if (screws[i].groupID != 0)
                    continue;
                if (screwIndexPreDic[i].Count != 0 && screwIndexPreDic[i].Count < x)
                    return i;
            }
            return -1;
        }

        //是否全部分配完毕
        //private static bool CheckNeedBreak(List<ScrewEditorData> screwEditorDatas)
        //{
        //    var result = screwEditorDatas.Where(t => t.groupID == 0).Count();
        //    return result == 0;
        //}

        private static bool CheckNeedBreak(List<ScrewData> screwDatas)
        {
            var result = screwDatas.Where(t => t.groupID == 0).Count();
            return result == 0;
        }

        //获取未分配且无前置的
        private static int GetScrewWithoutPre(Dictionary<int, List<int>> screwIndexPreDic, List<ScrewData> screwDatas)
        {
            for (int i = 0; i < screwIndexPreDic.Count; i++)
            {
                if (screwDatas[i].groupID == 0 && screwIndexPreDic[i].Count == 0)
                    return i;
            }
            return -1;
        }
    }
}
#endif

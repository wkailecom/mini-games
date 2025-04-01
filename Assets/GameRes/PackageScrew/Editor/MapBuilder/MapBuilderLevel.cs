#if UNITY_EDITOR 
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace ScrewJam.ScrewEditor
{
    public partial class MapBuilder : MonoBehaviour
    {
        bool isShowExportPanel = false;
        //int colorNumber2 = 5;
        //int hardLevel2 = 0;
        int batchStartLevel;
        int batchEndLevel;
        float exportPanelWidth = 300;
        string exportLevelStr;

        private void DrawExportButton()
        {
            if (!isShowExportPanel)
            {
                if (GUILayout.Button("显示导出面板", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    isShowExportPanel = true;
                }
            }
            else
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("隐藏导出面板", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    isShowExportPanel = false;
                }
                GUI.backgroundColor = normalBackgroundColor;
            }
        }

        private void DrawExportPanel(int id)
        {
            if (!isShowExportPanel)
                return;
            GUILayout.Label($"x:{exportPanelPosition.x}, y:{exportPanelPosition.y}, width:{exportPanelWidth}");
            GUILayout.Label("填写导出关卡");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("颜色", GUILayout.Width(30));
            colorNumber = EditorGUILayout.IntField(colorNumber, GUILayout.Width(60));
            GUILayout.Label("难度", GUILayout.Width(30));
            hardLevel = EditorGUILayout.IntField(hardLevel, GUILayout.Width(60));
            EditorGUILayout.EndHorizontal();

            //连续批量
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Label("开始", GUILayout.Width(30));
            batchStartLevel = EditorGUILayout.IntField(batchStartLevel, GUILayout.Width(60));
            GUILayout.Label("结束", GUILayout.Width(30));
            batchEndLevel = EditorGUILayout.IntField(batchEndLevel, GUILayout.Width(60));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("批量导出", GUILayout.Width(150), GUILayout.Height(30)))
            {
                //BatchLevelHandle
                EditorCoroutineUtility.StartCoroutine(BatchLevelHandle(batchStartLevel, batchEndLevel), this);
            }

            //批量
            GUILayout.Space(20);
            GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea);
            textAreaStyle.wordWrap = true; // 启用自动换行
            exportLevelStr = EditorGUILayout.TextArea(exportLevelStr, textAreaStyle, GUILayout.Height(200), GUILayout.Width(exportPanelWidth - 20));
            if (GUILayout.Button("导出填写的关卡", GUILayout.Width(150), GUILayout.Height(30)))
            {
                //EditorCoroutineUtility.StartCoroutine(BatchLevelHandle(batchStartLevel, batchEndLevel), this);
                EditorCoroutineUtility.StartCoroutine(BatchLevelHandle(exportLevelStr), this);
            }

            GUI.DragWindow();
        }

        private void GetAllData(out List<BoardData> tBoardData, out List<ScrewData> tScrewData, out List<Transform> tScrewTrans)
        {
            Transform[] layers = mapRoot.Cast<Transform>().ToArray();

            //板子数据
            tBoardData = new List<BoardData>();
            //螺丝数据
            tScrewData = new List<ScrewData>();
            tScrewTrans = new List<Transform>();

            for (int i = 0; i < layers.Length; i++)
            {
                var tBoards = layers[i].Cast<Transform>().ToArray();
                for (int j = 0; j < tBoards.Length; j++)
                {
                    var tScrewsPos = tBoards[j].Cast<Transform>().ToArray();
                    List<int> tIndexList = new List<int>();
                    for (int k = 0; k < tScrewsPos.Length; k++)
                    {
                        ScrewData screw = new ScrewData
                        {
                            position = tScrewsPos[k].localPosition,
                        };

                        tScrewData.Add(screw);
                        tScrewTrans.Add(tScrewsPos[k]);
                        tIndexList.Add(tScrewData.Count - 1/*tScrewData.IndexOf(screw)*/);
                    }
                    BoardData b = new BoardData
                    {
                        boardName = tBoards[j].name,
                        layer = i,
                        position = tBoards[j].localPosition,
                        eulerAngle = tBoards[j].localEulerAngles,
                        screwIndex = tIndexList.ToArray()
                    };
                    tBoardData.Add(b);
                }
            }
        }

        /// <summary>
        /// 导出关卡
        /// </summary>
        IEnumerator ExportLevelData()
        {
            if (globalBoards == null || globalBoxes == null || globalScrews == null)
            {
                Debug.Log("未分配颜色");
                yield break;
            }

            LevelData levelData = new LevelData
            {
                boards = globalBoards.ToArray(),
                screws = globalScrews.ToArray(),
                boxes = globalBoxes.ToArray(),
            };
            var exportPath = EditorUtility.SaveFilePanel("保存", "Assets/Resources" + Constant.LEVELDATA_PATH, "", "txt");
            if (exportPath != null && !string.IsNullOrEmpty(exportPath))
            {
                Debug.Log("导出路径:" + exportPath);
                string p = Directory.GetCurrentDirectory().Replace("\\", "/") + "/";
                exportPath = exportPath.Replace(p, "");
                var setting = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                File.WriteAllText(exportPath, JsonConvert.SerializeObject(levelData, Formatting.None, setting));
                AssetDatabase.Refresh();
            }
            yield return null;
        }

        /// <summary>
        /// 导入关卡
        /// </summary>
        IEnumerator LoadLevelData()
        {
            yield return null;
            //string dir = Directory.GetCurrentDirectory().Replace("\\", "/") + "/Assets/Bundles/ScrewPackage/BundleResources/" + Constant.LEVELDATA_PATH;
            //Debug.Log(dir);
            string levelPath = EditorUtility.OpenFilePanelWithFilters("导入", Directory.GetCurrentDirectory().Replace("\\", "/") + "/Assets/Bundles/ScrewPackage/BundleResources/LevelDatas/", new string[] { "Txt", "txt" });
            var sp = levelPath.Split('/');
            currentLevel = sp[sp.Length - 1];
            if (levelPath != null && !string.IsNullOrEmpty(levelPath))
            {
                Debug.Log("导入关卡:" + levelPath);
                levelPath = levelPath.Replace(Directory.GetCurrentDirectory().Replace("\\", "/") + "/", "");
                LoadAsset(levelPath);
            }
        }

        /// <summary>
        /// 导入导出指定的关卡
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        IEnumerator BatchLevelHandle(string levels)
        {
            yield return null;
            var levelList = levels.Split(',');
            for (int i = 0; i < levelList.Length; i++)
            {
                if (string.IsNullOrEmpty(levelList[i]))
                    continue;
                var tLevelIndex = int.Parse(levelList[i]);
                yield return LoadLevelWithIndex(tLevelIndex);
                Debug.Log($"输出关卡{tLevelIndex}");
            }
        }

        /// <summary>
        /// 批量导入导出
        /// </summary>
        /// <param name="startLevel"></param>
        /// <param name="endLevel"></param>
        /// <returns></returns>
        IEnumerator BatchLevelHandle(int startLevel, int endLevel)
        {
            yield return null;
            int start = startLevel;
            int end = endLevel;
            for (int i = start; i <= end; i++)
            {
                string path = $"Assets/Bundles/ScrewPackage/BundleResources/LevelDatas/{i}.txt";
                if (!File.Exists(path))
                    continue;
                ResetDatas();
                yield return LoadLevelDataWithoutDialog(i);
                int totalScrew = 0;
                foreach (var item in layers)
                {
                    for (int j = 0; j < item.Value.childCount; j++)
                    {
                        totalScrew += item.Value.GetChild(j).Cast<Transform>().Count();
                    }
                }
                CheckScrewRelated();
                Coloring2();
                yield return ExportLevelDataWithoutDialog(i);
                yield return null;
            }
            Debug.Log("finish");
        }

        IEnumerator LoadLevelWithIndex(int levelIndex)
        {
            string path = $"Assets/Bundles/ScrewPackage/BundleResources/LevelDatas/{levelIndex}.txt";
            if (!File.Exists(path))
                yield break;
            ResetDatas();
            yield return LoadLevelDataWithoutDialog(levelIndex);
            int totalScrew = 0;
            foreach (var item in layers)
            {
                for (int j = 0; j < item.Value.childCount; j++)
                {
                    totalScrew += item.Value.GetChild(j).Cast<Transform>().Count();
                }
            }
            CheckScrewRelated();
            Coloring2();
            yield return ExportLevelDataWithoutDialog(levelIndex);
            yield return null;
        }

        /// <summary>
        /// 读取固定路径的关卡
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        IEnumerator LoadLevelDataWithoutDialog(int level)
        {
            yield return null;
            currentLevel = level.ToString();
            string path = $"Assets/Bundles/ScrewPackage/BundleResources/LevelDatas/{level}.txt";
            Debug.Log($"导入关卡:{path}");
            LoadAsset(path);
            yield return null;
        }

        /// <summary>
        /// 关卡导出到固定路径
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        IEnumerator ExportLevelDataWithoutDialog(int level)
        {
            if (globalBoards == null || globalBoxes == null || globalScrews == null)
            {
                Debug.Log("未分配颜色");
                yield break;
            }

            LevelData levelData = new LevelData
            {
                boards = globalBoards.ToArray(),
                screws = globalScrews.ToArray(),
                boxes = globalBoxes.ToArray(),
            };
            string path = $"Assets/Bundles/ScrewPackage/BundleResources/LevelDatas/{level}.txt";
            Debug.Log($"导出路径:{path}");
            var setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            File.WriteAllText(path, JsonConvert.SerializeObject(levelData, Formatting.None, setting));
            AssetDatabase.Refresh();
            yield return null;
        }

        private void LoadAsset(string levelPath)
        {
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(levelPath);
            var setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            LevelData levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text, setting);
            for (int i = 0; i < levelData.boards.Length; i++)
            {
                var tBoardData = levelData.boards[i];
                Transform resultLayer;
                if (!layers.TryGetValue(tBoardData.layer, out resultLayer))
                {
                    resultLayer = GetTargetLayer(tBoardData.layer);
                }
                var tex = allBoards[tBoardData.boardName] as Texture2D;
                var tBoardObj = CreateBoardByTex2D(tex);
                tBoardObj.transform.parent = resultLayer;
                tBoardObj.transform.localPosition = tBoardData.position;
                tBoardObj.transform.localEulerAngles = tBoardData.eulerAngle;
                //创建螺丝
                for (int j = 0; j < tBoardData.screwIndex.Length; j++)
                {
                    var tScrewObj = CreateScrew(tBoardObj.transform);
                    var editorData = tScrewObj.GetComponent<ScrewEditorData>();
                    editorData.selfIndex = tBoardData.screwIndex[j];
                    var localPos = levelData.screws[tBoardData.screwIndex[j]].position;
                    tScrewObj.transform.localPosition = localPos;
                    try
                    {

                        SetScrewColor(tScrewObj.transform, config.screwColor[levelData.screws[tBoardData.screwIndex[j]].colorIndex]);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("============");
                        throw;
                    }
                }
            }
            curMaxLayer = layers.Count;
        }

        /// <summary>
        /// 输出关卡信息
        /// </summary>
        /// <param name="beginLevel"></param>
        /// <param name="endLevel"></param>
        /// <returns></returns>
        IEnumerator OutputLevelInfo(int beginLevel, int endLevel)
        {
            yield return null;
            int begin = beginLevel;
            int end = endLevel;
            StringBuilder str = new StringBuilder();
            str.Append($"Level,Layer,Screw\n");
            for (int i = begin; i <= end; i++)
            {
                string path = $"Assets/Bundles/ScrewPackage/BundleResources/LevelDatas/{i}.txt";
                if (!File.Exists(path))
                    continue;
                ResetDatas();
                currentLevel = i.ToString();
                TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                var setting = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                LevelData levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text, setting);
                List<int> layers = new List<int>();
                for (int j = 0; j < levelData.boards.Length; j++)
                {
                    if (!layers.Contains(levelData.boards[j].layer))
                        layers.Add(levelData.boards[j].layer);
                }
                str.Append($"{i},{layers.Count},{levelData.screws.Length}\n");
            }
            Debug.Log(str.ToString());
        }

    }
}


#endif
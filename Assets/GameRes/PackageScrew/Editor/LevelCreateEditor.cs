using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using System.Linq;
using NUnit.Framework;
using System.IO;
using UnityEngine.SceneManagement;
using Unity.Plastic.Newtonsoft.Json;

namespace ScrewJam.ScrewEditor
{
    /// <summary>
    /// 将screw的场景关卡导出为json
    /// </summary>
    public class LevelCreateEditor : EditorWindow
    {
        public const string LEVEL_PATH = "Assets/Other/Level/Level 1.unity";

        public int levelStart = 1;

        public int levelEnd = 632;

        [MenuItem("Tools/GenerateLevel")]
        public static void Init()
        {
            var t = GetWindow<LevelCreateEditor>();
            t.Show();
        }

        public void OnGUI()
        {
            levelStart = EditorGUILayout.IntField("开始", levelStart);
            levelEnd = EditorGUILayout.IntField("结束", levelEnd);
            if (GUILayout.Button("导出scene关卡"))
            {
                EditorCoroutineUtility.StartCoroutine(LoadSceneRange(levelStart, levelEnd), this);
            }
        }

        private IEnumerator LoadSceneRange(int start, int end)
        {
            for (int i = start; i <= end; i++)
            {
                EditorUtility.DisplayProgressBar("title", "info", i / (float)(end - start));
                yield return GetScene(i);
            }
            EditorUtility.ClearProgressBar();
        }

        private IEnumerator GetScene(int sceneIndex)
        {
            yield return null;
            Scene scene;
            try
            {
                scene = EditorSceneManager.OpenScene($"Assets/Other/Level/Level {sceneIndex}.unity", OpenSceneMode.Single);
            }
            catch (System.Exception e)
            {
                Debug.Log($"scene not find, index:{sceneIndex}, {e}");
                yield break;
            }
            yield return new WaitUntil(() => scene.isLoaded);
            AssetDatabase.Refresh();
            Debug.Log($"scene name:{scene.name}, scene status:{scene.isLoaded}, isValid:{scene.IsValid()}");
            GameObject[] rootObjs = scene.GetRootGameObjects();
            var layers = rootObjs[0].transform.GetChild(1).Cast<Transform>();

            List<BoardData> localBoards = new List<BoardData>();
            List<ScrewData> localScrews = new List<ScrewData>();
            int layerIndex = 0;
            foreach (var layerTr in layers)
            {
                var panels = layerTr.GetChild(1).Cast<Transform>().Where(t => t.name.StartsWith("Panel"));
                foreach (var panelTr in panels)
                {
                    var holes = panelTr.Cast<Transform>().Where(t => t.name.StartsWith("Hole")).Select(t => t.localPosition / 2f);
                    List<int> holeIndexList = new List<int>();
                    foreach (var holePos in holes)
                    {
                        ScrewData screwData = new ScrewData
                        {
                            position = holePos
                        };
                        localScrews.Add(screwData);
                        holeIndexList.Add(localScrews.Count - 1/*localScrews.IndexOf(screwData)*/);
                    }

                    BoardData boardData = new BoardData
                    {
                        boardName = panelTr.GetChild(0).GetComponent<SpriteRenderer>().sprite.name,
                        layer = layerIndex,
                        position = panelTr.localPosition / 2f,
                        eulerAngle = panelTr.localEulerAngles,
                        screwIndex = holeIndexList.ToArray()
                    };
                    localBoards.Add(boardData);
                }
                layerIndex++;
            }

            LevelData levelData = new LevelData
            {
                boards = localBoards.ToArray(),
                screws = localScrews.ToArray(),
                boxes = new BoxData[0]
            };

            var setting = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            try
            {
                string exportPath = $"Assets/Resources/LevelExport/{sceneIndex}.txt";
                string p = Directory.GetCurrentDirectory().Replace("\\", "/") + "/";
                exportPath = exportPath.Replace(p, "");
                File.WriteAllText(exportPath, JsonConvert.SerializeObject(levelData, setting));
                AssetDatabase.Refresh();
            }
            catch (System.Exception e)
            {
                Debug.Log($"导出scene{sceneIndex}错误, {e}");
            }
            AssetDatabase.Refresh();
        }
    }
}



#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace ScrewJam.ScrewEditor
{
    [ExecuteInEditMode]
    public partial class MapBuilder : MonoBehaviour
    {
        public const float SCALE = 1f;

        private Transform mapRoot;

        private Dictionary<int, Transform> layers = new Dictionary<int, Transform>();

        private int curMaxLayer;

        private int curSelectLayer;

        private string focusLayerName = "FocusLayer";

        private string ignoreLayerName = "IgnoreLayer";

        private Dictionary<string, Texture> allBoards = new Dictionary<string, Texture>();

        private bool placeScrew;

        private Color normalBackgroundColor;

        private int[] boxColor;

        //private Color[] colors;//= new Color[] { Color.red, Color.blue, Color.green, Color.gray, Color.yellow };

        private List<ScrewData> globalScrews;

        private List<BoardData> globalBoards;

        private List<BoxData> globalBoxes;

        Rect detailInfoPosition = new Rect(10, 50, 0, 0);

        Rect leftAreaPosition = new Rect(10, 200, 0, 0);

        Rect selectPanelPosition = new Rect(180, 200, 0, 0);

        Rect exportPanelPosition = new Rect(180, 200, 0, 0);

        Vector2 leftPos = new Vector2(10, 200);

        float selectPanelWidth = 300;

        string currentLevel;

        bool gmToolsFold = false;

        int colorNumber = 5;

        int hardLevel = 0;

        GameConfig config;

        private void Awake()
        {
            LoadColorConfig();
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        [DidReloadScripts]
        private static void Reload()
        {
            var builder = FindObjectOfType<MapBuilder>();
            if (builder != null)
            {
                builder.Awake();
            }
        }

        private void OnEnable()
        {
            mapRoot = GameObject.Find("MapRoot").transform;
            curMaxLayer = 0;
            layers.Clear();

            LoadBoard();

            curMaxLayer = mapRoot.childCount;
            curSelectLayer = curMaxLayer - 1;
            mapRoot.Cast<Transform>().ToDictionary(kvp => kvp.GetSiblingIndex(), kvp => kvp);
            normalBackgroundColor = GUI.backgroundColor;
        }

        /// <summary>
        /// 加载所有图形
        /// </summary>
        private void LoadBoard()
        {
            //Assets/Bundles/ScrewPackage/Art/Shape/
            allBoards = new Dictionary<string, Texture>();
            string[] files = Directory.GetFiles("Assets/Bundles/ScrewPackage/Art/Shape/");
            foreach (string file in files)
            {
                if (file.Contains("meta"))
                    continue;
                string fileName = Path.GetFileNameWithoutExtension(file);
                Texture texture = AssetDatabase.LoadAssetAtPath<Texture>(file);
                if (texture != null)
                {
                    allBoards[fileName] = texture;
                }
            }
        }

        private void LoadColorConfig()
        {
            config = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/Bundles/ScrewPackage/BundleResources/ScrewGameConfig.asset");
        }

        private Rect cacheSceneView;
        private void OnSceneGUI(SceneView sceneView)
        {
            cacheSceneView = sceneView.position;
            Handles.BeginGUI();
            HandleEvent(sceneView);
            detailInfoPosition = GUILayout.Window(1, detailInfoPosition, ShowDetailInfo, "Info", GUILayout.Width(150));
            var left = GUILayout.Window(2, new Rect(leftPos.x, leftPos.y, 0, 0), LeftArea, "Tools", GUILayout.Width(150));
            leftPos = new Vector2(left.x, left.y);
            RightArea(sceneView);
            if (isShowSelectPanel)
            {
                selectPanelWidth = cacheSceneView.width - 200;
                selectPanelPosition = GUILayout.Window(3, selectPanelPosition, DrawSelectPanel, "Board_Select", GUILayout.Width(selectPanelWidth), GUILayout.Height(cacheSceneView.height * 0.6f));
            }
            if (isShowExportPanel)
            {
                exportPanelWidth = cacheSceneView.width - 200;
                exportPanelPosition = GUILayout.Window(4, exportPanelPosition, DrawExportPanel, "ExportLevel", GUILayout.Width(exportPanelWidth), GUILayout.Height(cacheSceneView.height * 0.6f));
            }
            Handles.EndGUI();
        }

        private void LeftArea(int id)
        {
            GUILayout.Label($"x:{leftAreaPosition.x}, y:{leftAreaPosition.y}");
            if (GUILayout.Button("增加一层", GUILayout.Width(150), GUILayout.Height(30)))
            {
                GetTargetLayer(curMaxLayer);
                SetCurrentEnabledLayer(curMaxLayer);
                curMaxLayer += 1;
                curSelectLayer = curMaxLayer - 1;
            }

            if (GUILayout.Button("删除最顶层", GUILayout.Width(150), GUILayout.Height(30)))
            {
                DelateTopLayer();
            }
            GUILayout.Space(10);

            if (!isShowSelectPanel)
            {
                if (GUILayout.Button("显示选择面板", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    isShowSelectPanel = true;
                }
            }
            else
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("隐藏选择面板", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    isShowSelectPanel = false;
                }
                GUI.backgroundColor = normalBackgroundColor;
            }
            if (!placeScrew)
            {
                GUI.backgroundColor = Color.green;
                if (GUILayout.Button("放置钉子", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    placeScrew = true;
                }
                GUI.backgroundColor = normalBackgroundColor;
            }
            else
            {
                GUI.backgroundColor = Color.red;
                if (GUILayout.Button("取消放置", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    placeScrew = false;
                }
                GUI.backgroundColor = normalBackgroundColor;
            }
            GUILayout.Space(10);
            if (GUILayout.Button("分配颜色", GUILayout.Width(150), GUILayout.Height(30)))
            {
                //Coloring();
                CheckScrewRelated();
                Coloring2();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("颜色数量");
            colorNumber = EditorGUILayout.IntField(colorNumber);//GUILayout.TextArea(colorNumber);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("难度");
            hardLevel = EditorGUILayout.IntField(hardLevel);//GUILayout.TextArea(hardLevel);
            GUILayout.EndHorizontal();

            GUILayout.Space(30);
            if (GUILayout.Button("导入关卡数据", GUILayout.Width(150), GUILayout.Height(30)))
            {
                ResetDatas();
                EditorCoroutineUtility.StartCoroutine(LoadLevelData(), this);
            }
            if (GUILayout.Button("导出关卡数据", GUILayout.Width(150), GUILayout.Height(30)))
            {
                EditorCoroutineUtility.StartCoroutine(ExportLevelData(), this);
            }
            GUILayout.Space(30);
            DrawExportButton();

            

            GUILayout.Space(10);
            gmToolsFold = EditorGUILayout.Foldout(gmToolsFold, "dev工具");
            if (gmToolsFold)
            {
                if (GUILayout.Button("输出关卡信息", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    EditorCoroutineUtility.StartCoroutine(OutputLevelInfo(batchStartLevel, batchEndLevel), this);
                }
                if (GUILayout.Button("清除颜色显示", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    ClearScrewColor();
                }
                if (GUILayout.Button("检测板子重叠", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    CheckLayersBoardOverlap();
                }
                if (GUILayout.Button("显示被盖压的螺丝", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    ShowLayersScrewOverlap();
                }
                if (GUILayout.Button("螺丝重叠数据", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    CheckScrewRelated();
                }
                if (GUILayout.Button("清除场景", GUILayout.Width(150), GUILayout.Height(30)))
                {
                    ResetDatas();
                    int count = mapRoot.childCount;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        GameObject.Destroy(mapRoot.GetChild(i).gameObject);
                    }
                }
            }
            GUI.DragWindow();
        }

        private void RightArea(SceneView sceneView)
        {
            var rect = sceneView.position;
            Rect scrollViewRect = new Rect(rect.width - 200, 25, 200, rect.height / 2f);
            GUILayout.BeginArea(scrollViewRect);
            {
                GUILayout.Label("层级选择");
                DrawLayersScroll(sceneView);
            }
            GUILayout.EndArea();
        }

        /// <summary>
        /// 处理点击事件
        /// </summary>
        /// <param name="sceneView"></param>
        private void HandleEvent(SceneView sceneView)
        {
            Event e = Event.current;
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                if (placeScrew)
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    RaycastHit2D hit = Physics2D.Raycast(ray.origin, Vector2.zero, 1000, LayerMask.GetMask(focusLayerName));
                    if (hit.collider != null)
                    {
                        CreateScrewHoleByClick(hit);
                    }
                }
            }
        }

        /// <summary>
        /// 显示信息
        /// </summary>
        private void ShowDetailInfo(int id)
        {
            GUILayout.Label($"x:{detailInfoPosition.x}, y:{detailInfoPosition.y}");
            GUILayout.Label($"当前关卡:{currentLevel}");
            GUILayout.Label("当前最大层级:" + curMaxLayer);
            GUILayout.Label("当前选择层级:" + curSelectLayer);
            int totalScrew = 0;
            int totalBoard = 0;
            foreach (var item in layers)
            {
                totalBoard += item.Value.childCount;
                for (int i = 0; i < item.Value.childCount; i++)
                {
                    totalScrew += item.Value.GetChild(i).Cast<Transform>().Count();
                }
            }
            GUILayout.Label("当前板子数量:" + totalBoard);
            GUILayout.Label("当前螺丝数量:" + totalScrew);

            if (boxColor != null && boxColor.Length > 0)
            {
                StringBuilder str = new StringBuilder();
                for (int i = 0; i < boxColor.Length; i++)
                {
                    str.Append(boxColor[i] + ",");
                }
                GUILayout.Label("盒子颜色:" + str.ToString());
            }
            GUI.DragWindow();
        }

        /// <summary>
        /// 获取指定层级，若不存在则创建
        /// </summary>
        /// <param name="layerIndex">指定层级index</param>
        /// <returns></returns>
        private Transform GetTargetLayer(int layerIndex)
        {
            Transform resultLayer;
            if (!layers.TryGetValue(layerIndex, out resultLayer))
            {
                var obj = new GameObject("Layer" + layerIndex);
                obj.transform.SetParent(mapRoot);
                obj.transform.localPosition = new Vector3(0, 0, -layerIndex * 1);
                obj.transform.localScale = Vector3.one * SCALE;
                layers.Add(layerIndex, obj.transform);
                resultLayer = obj.transform;
            }
            return resultLayer;
        }

        /// <summary>
        /// 设置当前激活的层级
        /// </summary>
        /// <param name="layerIndex"></param>
        private void SetCurrentEnabledLayer(int layerIndex)
        {
            foreach (var item in layers)
            {
                if (item.Key < layerIndex)
                {
                    SetLayerStatus(item.Value, false);
                }
                else if (item.Key > layerIndex)
                {
                    item.Value.gameObject.SetActive(false);
                }
                else
                {
                    SetLayerStatus(item.Value, true);
                    curSelectLayer = layerIndex;
                }
            }
        }

        /// <summary>
        /// 删除最顶层
        /// </summary>
        private void DelateTopLayer()
        {
            Transform resultLayer;
            if (layers.TryGetValue(curMaxLayer - 1, out resultLayer))
            {
                layers.Remove(curMaxLayer - 1);
                GameObject.DestroyImmediate(resultLayer.gameObject);
                curMaxLayer -= 1;
                curSelectLayer = curMaxLayer;

                if (curMaxLayer > 0)
                {
                    for (int i = 0; i < curMaxLayer; i++)
                    {
                        if (i == curMaxLayer - 1)
                        {
                            SetLayerStatus(layers[i], true);
                        }
                        else
                        {
                            SetLayerStatus(layers[i], false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 创建螺丝孔
        /// </summary>
        /// <param name="hit"></param>
        private void CreateScrewHoleByClick(RaycastHit2D hit)
        {
            var obj = CreateScrew(hit.collider.transform);
            obj.transform.position = new Vector3(hit.point.x, hit.point.y, hit.collider.transform.position.z - 0.1f);
        }

        private GameObject CreateScrew(Transform parent)
        {
            //var screwObj = Resources.Load<GameObject>(Constant.SCREW_PATH);
            var screwObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Bundles/ScrewPackage/BundleResources/Prefabs/Screw.prefab");
            var obj = GameObject.Instantiate(screwObj, parent);
            obj.AddComponent<ScrewEditorData>();
            return obj;
        }

        private float iconSize = 50;
        private bool isShowSelectPanel = false;
        private Vector2 scrollViewRoot;
        /// <summary>
        /// 绘制选择界面
        /// </summary>
        /// <param name="sceneView"></param>
        private void DrawSelectPanel(int id)
        {
            if (!isShowSelectPanel)
                return;
            if (allBoards == null || allBoards.Count == 0)
                return;
            GUILayout.Label($"x:{selectPanelPosition.x}, y:{selectPanelPosition.y}");
            if (GUILayout.Button("隐藏选择面板", GUILayout.Height(50)))
            {
                isShowSelectPanel = false;
            }

            int column = Mathf.FloorToInt(selectPanelWidth / iconSize) - 1;

            GUILayout.BeginVertical();
            {
                scrollViewRoot = GUILayout.BeginScrollView(scrollViewRoot);
                var boards = allBoards.Values.ToArray();
                for (int i = 0; i < boards.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < column; j++)
                    {
                        var index = i * column + j;
                        if (index < boards.Length)
                        {
                            if (GUILayout.Button(boards[index], GUILayout.Width(iconSize), GUILayout.Height(iconSize)))
                            {
                                CreateBoardInLayer(boards[index] as Texture2D);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        Vector2 layersScrollPos;
        /// <summary>
        /// 层级选择按钮
        /// </summary>
        /// <param name="sceneView"></param>
        private void DrawLayersScroll(SceneView sceneView)
        {
            layersScrollPos = GUILayout.BeginScrollView(layersScrollPos);
            {
                for (int i = 0; i < layers.Count; i++)
                {
                    if (GUILayout.Button("Layer" + i, GUILayout.Width(150), GUILayout.Height(30)))
                    {
                        Selection.activeGameObject = layers[i].gameObject;
                        SetCurrentEnabledLayer(i);
                    }
                    GUILayout.Space(10);
                }
            }
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// 在当前选中层创建一个板子
        /// </summary>
        /// <param name="texture2D"></param>
        private void CreateBoardInLayer(Texture2D texture2D)
        {
            var tex2dObj = CreateBoardByTex2D(texture2D);
            tex2dObj.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            tex2dObj.transform.parent = layers[curSelectLayer];
            tex2dObj.layer = LayerMask.NameToLayer(focusLayerName);
            tex2dObj.transform.localPosition = Vector3.zero;
            tex2dObj.transform.localScale = Vector3.one * SCALE;
            Selection.activeGameObject = tex2dObj;
        }

        private GameObject CreateBoardByTex2D(Texture2D texture2D)
        {
            Debug.Log("texture2D name:" + texture2D.name);
            var tex2dObj = new GameObject(texture2D.name);
            var renderer = tex2dObj.AddComponent<SpriteRenderer>();
            var sp = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
            sp.name = texture2D.name;
            renderer.sprite = sp;
            tex2dObj.AddComponent<PolygonCollider2D>();
            return tex2dObj;
        }

        /// <summary>
        /// 设置节点下物理层级
        /// </summary>
        /// <param name="layerRoot"></param>
        /// <param name="isActive"></param>
        private void SetLayerStatus(Transform layerRoot, bool isActive)
        {
            layerRoot.gameObject.SetActive(true);
            var tLayersObj = layerRoot.Cast<Transform>().Select(t => t.gameObject).ToArray();
            for (int i = 0; i < tLayersObj.Length; i++)
            {
                tLayersObj[i].layer = isActive ? LayerMask.NameToLayer(focusLayerName) : LayerMask.NameToLayer(ignoreLayerName);
                var renderer = tLayersObj[i].GetComponent<SpriteRenderer>();
                var defaultColor = renderer.color;
                renderer.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, isActive ? 0.5f : 1f);
            }
        }

        private void Coloring()
        {
            GetAllData(out List<BoardData> tBoardData, out List<ScrewData> tScrewData, out List<Transform> tScrewTrans);
            List<BoxData> tBoxData = new List<BoxData>();
            int boxCount = tScrewData.Count / 3;
            for (int i = 0; i < boxCount; i++)
            {
                tBoxData.Add(new BoxData() { count = 3 });
            }
            Generator.RandomDistributeColorIndex(5, tScrewData, tBoxData);
            boxColor = tBoxData.Select(t => t.colorIndex).ToArray();

            for (int i = 0; i < tScrewData.Count; i++)
            {
                try
                {
                    SetScrewColor(tScrewTrans[i], config.screwColor[tScrewData[i].colorIndex]);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    //throw;
                }
            }

            globalBoards = tBoardData;
            globalScrews = tScrewData;
            globalBoxes = tBoxData;
        }

        private void Coloring2()
        {
            ClearConsole();

            GetAllData(out List<BoardData> tBoardData, out List<ScrewData> tScrewData, out List<Transform> tScrewTrans);
            var datas = mapRoot.GetComponentsInChildren<ScrewEditorData>();
            List<BoxData> tBoxData = new List<BoxData>();
            int boxCount = tScrewData.Count / 3;
            for (int i = 0; i < boxCount; i++)
            {
                tBoxData.Add(new BoxData() { count = 3 });
            }
            Generator.NewColoring(colorNumber, config.screwColor.Length, tScrewData, tBoxData, datas, hardLevel);
            //return;
            boxColor = tBoxData.Select(t => t.colorIndex).ToArray();
            for (int i = 0; i < tScrewData.Count; i++)
            {
                try
                {
                    SetScrewColor(tScrewTrans[i], config.screwColor[tScrewData[i].colorIndex]);
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                    //throw;
                }
            }

            globalBoards = tBoardData;
            globalScrews = tScrewData;
            globalBoxes = tBoxData;
        }

        /// <summary>
        /// 清理所有螺丝颜色
        /// </summary>
        private void ClearScrewColor()
        {
            GetAllData(out List<BoardData> tBoardData, out List<ScrewData> tScrewData, out List<Transform> tScrewTrans);
            for (int i = 0; i < tScrewTrans.Count; i++)
            {
                SetScrewColor(tScrewTrans[i], Color.white);
            }
        }

        /// <summary>
        /// 设置某一螺丝颜色
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="color"></param>
        private void SetScrewColor(Transform trans, Color color)
        {
            var renderers = trans.GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].color = color;
            }
        }

        private void ResetDatas()
        {
            int childCount = mapRoot.childCount;
            if (childCount > 0)
            {
                for (int i = childCount - 1; i >= 0; i--)
                {
                    GameObject.DestroyImmediate(mapRoot.GetChild(i).gameObject);
                }
            }
            curMaxLayer = 0;
            curSelectLayer = 0;
            layers?.Clear();
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void ClearConsole()
        {
            return;
            //清除console
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.SceneView));
            System.Type logEntries = assembly.GetType("UnityEditor.LogEntries");
            System.Reflection.MethodInfo clearConsoleMethod = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            clearConsoleMethod.Invoke(new object(), null);
        }
    }
}


#endif
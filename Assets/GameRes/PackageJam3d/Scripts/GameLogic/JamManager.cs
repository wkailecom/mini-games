using System.Collections.Generic;
using System.Resources;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;
using Jam3d;
using System;

namespace GameLogic
{
    public enum GameState
    {
        Start,
        Playing,
        InTween,
        End
    }

    public class JamManager : Jam3d.Singleton<JamManager>, ISingleton
    {
        public JamTriggerEvents jamTriggerEvents;

        private JamManager()
        {
        }

        public Camera _mainCamera;

        public int LevelIndex
        {
            get { return _levelIndex; }
        }

        private List<Material> materials;
        private Material lower;
        private Material upper;
        private int _levelIndex = 1;
        private int[] _levelMaps;

        private static readonly int ColorProperty = Shader.PropertyToID("_Color");

        public JamGrid JamGridMono { get; private set; }
        public Board Board { get; set; }

        private GameObject _boardPrefab;

        private Transform _buildParent;
        private Transform _gameEntityParent;
        private Transform _levelParent;

        private Transform _floor;
        private Material _meshMaterial;

        private int _beforeColorNum;
        private int _beforeHandCardNum;

        private GameState GameState { get; set; }

        public void OnInit()
        {
            Physics.autoSyncTransforms = true;

            materials = new List<Material>();

            Color[] Colors =
            {
                new Color(0f, 0.71f, 0.82f), new Color(0.77f, 0.45f, 0.02f), new Color(0.83f, 0.14f, 0.14f),
                new Color(0f, 0.77f, 0f), new Color(0.3f, 0.55f, 0.5f), new Color(0.74f, 0.21f, 0.87f),
                new Color(0.52f, 0.22f, 0.22f), new Color(0.33f, 0.27f, 0.90f), new Color(0.55f, 0.55f, 0.55f),
                new Color(0.65f, 0.62f, 0f)
            };

            //var asyncOperationHandle =
            //    Addressables.LoadAssetAsync<Material>("Assets/TilePalette/Materials/Terrain/block_lower.mat");
            //asyncOperationHandle.Completed += operationHandle => { lower = operationHandle.Result; };

            //var upAsyncOperationHandle =
            //    Addressables.LoadAssetAsync<Material>("Assets/TilePalette/Materials/Terrain/block_upper.mat");
            //upAsyncOperationHandle.Completed += operationHandle => { upper = operationHandle.Result; };
        }

        public Material GetMaterial(int index)
        {
            if (materials.Count <= 0)
            {
                for (int i = 1; i <= 10; i++)
                {
                    var m = jamTriggerEvents.JamLoadResourceByRelativePathInJamPackage?.Invoke(
                        $"BundleResources/Materials/virus{i}") as Material;
                    materials.Add(m);
                }
            }

            return materials[index];
        }

        public void SetTransform(GameObject board, Transform levelParent, Transform buildParent, Transform gameParent)
        {
            _boardPrefab = board;
            _buildParent = buildParent;
            _gameEntityParent = gameParent;
            _levelParent = levelParent;
            _mainCamera = GameObject.FindWithTag("JamCamera").GetComponent<Camera>();
        }

        private Color ParseColor(string color)
        {
            var r = Convert.ToInt32(color.Substring(0, 2), 16);
            var g = Convert.ToInt32(color.Substring(2, 2), 16);
            var b = Convert.ToInt32(color.Substring(4, 2), 16);

            return new Color(r / 255f, g / 255f, b / 255f);
        }

        int lastLevelId = -1;
        int lastLevelIdPlayTimes;
        int exchangeIndex;
        string curExchangeConfStr;

        public void UpdateStepsExchangeIndex(int levelId, int threshold)
        {
            if (lastLevelId == levelId)
            {
                if (++lastLevelIdPlayTimes >= threshold)
                {
                    exchangeIndex++;
                }
            }
            else
            {
                lastLevelIdPlayTimes = 1;
                exchangeIndex = 0;
            }

            lastLevelId = levelId;
        }

        void RefreshCurStepExchangeConfig(string stepsExchange)
        {
            var swapSetting = CommonMethod.TryParseListString(stepsExchange, ';');
            var index = Mathf.Clamp(exchangeIndex, 0, swapSetting.Count - 1);
            curExchangeConfStr = swapSetting.Count > 0 ? swapSetting[index] : string.Empty;
        }

        public void RegisterJamTriggerEvents(JamTriggerEvents jamTriggerEvents)
        {
            this.jamTriggerEvents = jamTriggerEvents;
        }

        public void StartGame(int[] levels)
        {
            //MainComponent.Max.ShowBanner();

            if (_floor == null)
            {
                _floor = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
                _floor.SetParent(_gameEntityParent);

                var meshRenderer = _floor.GetComponent<MeshRenderer>();
                meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                meshRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                meshRenderer.receiveShadows = true;
                _meshMaterial =
                    jamTriggerEvents.JamLoadResourceByRelativePathInJamPackage?.Invoke(
                        $"BundleResources/Materials/level_plane") as Material;
                meshRenderer.material = _meshMaterial;
            }

            _floor.gameObject.SetLayerRecursively("Jam3d");

            if (Board == null)
            {
                Board = Object.Instantiate(_boardPrefab, _gameEntityParent).GetComponent<Board>();
            }

            Board.gameObject.SetLayerRecursively("Jam3d");

            Board.Clear();
            _gameEntityParent.gameObject.SetActive(true);
            _buildParent.gameObject.SetActive(false);

            _levelParent.localPosition = Vector3.zero;
            _levelMaps = levels;
            _levelIndex = 0;
            LoadLevel(levels[_levelIndex]);
            //EventDispatcher.TriggerEvent(EventKey.E_JamAfterStartGame);
            jamTriggerEvents.JamAfterStartGame?.Invoke();
        }


        private const float FieldOfView = 70f;
        private const float GameFiledOfView = 10.5f;

        public void ExitGame()
        {
            Pool.ObjectPool.Instance.HideObjects("grass_block");
            Pool.ObjectPool.Instance.HideObjects("dirt_block");


            var childCount = _levelParent.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                var go = _levelParent.GetChild(i).gameObject;
                Object.Destroy(go);
                // var success =  Addressables.ReleaseInstance(go);
                //if (!success)
                //{
                //    Debug.LogWarning("[AssetsLoader] release instance error :" + go);
                //}
            }

            var transform = _mainCamera.transform;
            transform.position = new Vector3(0, 0, -10);
            transform.rotation = Quaternion.identity;


            _mainCamera.fieldOfView = FieldOfView;

            _gameEntityParent.gameObject.SetActive(false);
            _buildParent.gameObject.SetActive(true);
            _levelIndex = 0;
        }

        /// <summary>
        /// on scene loaded
        /// </summary>
        private void LoadLevel(int levelNum)
        {
            GameState = GameState.Start;

            //step-1: load base prefab and get grid view
            var levelGo = jamTriggerEvents.JamLoadLevelFunc?.Invoke(levelNum, _levelParent);
//#if UNITY_EDITOR
//            var go = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/Bundles/Jam3dPackage/BundleResources/Prefabs/Gen/{name}.prefab");
//            var levelGo = GameObject.Instantiate(go, _levelParent);
//#else
//            var levelGo = ResourcesManager.Instance.Instantiate($"Jam3dPackage/BundleResources/Prefabs/Gen/{name}", _levelParent);
//#endif
            levelGo.SetLayerRecursively("Jam3d");
            JamGridMono = levelGo.GetComponent<JamGrid>();
            var exchangeStepConfStr = jamTriggerEvents.JamStepExchangeStringFunc?.Invoke(levelNum);
            //step-2: set swap info 
            RefreshCurStepExchangeConfig(exchangeStepConfStr);
            if (!string.IsNullOrEmpty(curExchangeConfStr))
            {
                JamGridMono.ParseSwapSetting(curExchangeConfStr);
            }

            //step-3: create terrain with config
            TextAsset textAsset =
                jamTriggerEvents.JamLoadResourceFunc(levelNum, JamTriggerEvents.ResType.TerrainConfig) as TextAsset;

            var setting = textAsset.text;
            var config = JsonUtility.FromJson<GridConfig>(setting);
            JamGridMono.Construct(config);

            //step-4: terrain color
            //if (!string.IsNullOrEmpty(colors))
            //{
            //    var color = colors.Split(',');
            //    lower.SetColor(ColorProperty, ParseColor(color[1]));
            //    upper.SetColor(ColorProperty, ParseColor(color[2]));
            //}

            //step-5: camera and level position
            var size = JamGridMono.gridSize;
            _floor.transform.localScale = new Vector3(size, 1f, size / 3f);
            levelGo.transform.localPosition = new Vector3(size * _levelIndex, 0, 0);
            if (!_mainCamera) return;
            var x = JamGridMono.GetOffsetX();
            var z = JamGridMono.GetRowCount();
            z = (z - z * 0.6f) * 0.5f;
            _mainCamera.transform.position = new Vector3(x, 105, -53 - z);
            Board.transform.position = new Vector3(x, 0.1f, -z + 0.5f);
            _mainCamera.transform.rotation = Quaternion.Euler(65f, 0, 0);
            var referenceResolution = new Vector2(1080, 2160);
            var screenResolution = new Vector2(Screen.width, Screen.height);
            var ratio = screenResolution.y / screenResolution.x / (referenceResolution.y / referenceResolution.x);
            if (ratio > 1)
            {
                _mainCamera.fieldOfView = GameFiledOfView * ratio;
            }
            else
            {
                _mainCamera.fieldOfView = GameFiledOfView;
            }

            if (_levelIndex != 0)
            {
                _levelParent.DOLocalMove(new Vector3(-size * _levelIndex, 0, 0), 0.5f).OnComplete(delegate
                {
                    Board.Clear();
                    var childCount = _levelParent.childCount;
                    if (childCount > 1)
                    {
                        var hideLevel = _levelParent.GetChild(childCount - 2);
                        if (hideLevel)
                        {
                            hideLevel.gameObject.SetActive(false);
                        }
                    }

                    GameState = GameState.Playing;
                });
            }
            else
            {
                GameState = GameState.Playing;
            }
        }

        public static void HideTagPool(string tag)
        {
            Pool.ObjectPool.Instance.HideSearchPattern(tag);
        }

        public static void PutBack(GameObject obj)
        {
            Pool.ObjectPool.Instance.PutBack(obj);
        }

        private void IsFinished()
        {
            if (!JamGridMono.AllBlocksRemoved()) return;
            EndGame(true);
        }

        /// <summary>
        /// add item to waiting place
        /// </summary>
        /// <param name="virusItem"></param>
        public void PlaceItem(VirusItem virusItem)
        {
            if (GameState != GameState.Playing)
                return;
            Board.PlaceColor(virusItem);
        }

        public void DetectGameState()
        {
            IsFinished();
        }

        public void DetectMatch()
        {
            if (Board.DetectSlot())
                EndGame(false);
        }

        public List<int> StartNavigation(int tileIndex)
        {
            if (GameState == GameState.End || JamGridMono.IsBlocker(tileIndex))
                return null;
            return JamGridMono.FindPath(tileIndex);
        }

        /// <summary>
        /// only undo once
        /// </summary>
        public bool Undo()
        {
            if (GameState != GameState.Playing)
                return false;
            _beforeHandCardNum = GetHandCardCount();
            _beforeColorNum = GetHandCardColorCount();
            var index = PutGridItemsBackInGridSpots();
            return index != -1;
        }

        private int PutGridItemsBackInGridSpots()
        {
            var item = Board.Undo();
            if (item == null)
            {
                return -1;
            }

            var tile = JamGridMono.GetGridSpot(item.index);
            if (item.sourceType == SourceType.Spawn && tile.affectItem != 0)
            {
                var ai = JamGridMono.GetTileItem(tile.affectItem);
                ai.Undo();
            }
            else
            {
                item.Undo();
                tile.AssignChildGridItem();
            }

            return 1;
        }


        public bool Shuffle()
        {
            _beforeHandCardNum = GetHandCardCount();
            _beforeColorNum = GetHandCardColorCount();
            return GameState == GameState.Playing && JamGridMono.Shuffle();
        }

        public bool Replace()
        {
            _beforeHandCardNum = GetHandCardCount();
            _beforeColorNum = GetHandCardColorCount();
            return GameState == GameState.Playing && Board.Replace();
        }

        public bool Pull(GameObject Effect_Magic)
        {
            if (GameState != GameState.Playing)
            {
                return false;
            }

            _beforeHandCardNum = GetHandCardCount();
            _beforeColorNum = GetHandCardColorCount();

            if (Board.IsFull())
                return false;
            var color = Board.GetPullColor();
            var slot = Board.GetPullPosition(color);
            if (slot == null)
            {
                return false;
            }

            var item = JamGridMono.PullColorItem(color);
            //var effect = ResourcesManager.Instance.Instantiate("BundleResources/Prefabs/Effect/Effect_Magic");
            var effect = GameObject.Instantiate(Effect_Magic);
            effect.transform.position = new Vector3(3.5f, 1f, -20f);
            var pos = item.transform.position;
            pos.y += 1;
            effect.transform.DOMove(pos, 0.5f).OnComplete(delegate
            {
                Object.Destroy(effect);

                var isUp = item.DoPull();
                DOTween.To(() => 1, value => { }, 1, isUp ? 0 : 0.4f).OnComplete(delegate { slot.ItemDoPlace(); });
            });
            slot.SetItem(item);
            //XLuaKit.CallLua("ReceiveGameBehaviour", (int)BehaviourType.click_props, (int)PropsType.Pull,
            //    item.index);
            return true;
        }

        public void DoTwinsBlocker(int index, SourceType sourceType)
        {
            if (sourceType == SourceType.Spawn)
            {
                var terrainTile = JamGridMono.GetGridSpot(index);
                var spawnItem = terrainTile.GetAdjacentSpawn();
                if (spawnItem)
                {
                    var blocker = JamGridMono.GetBlocker(spawnItem.index);
                    if (!blocker || !(blocker is TwinsBlocker twinsBlocker)) return;
                    DOTween.To(() => 1, _ => { }, 1, 0.2f).OnComplete(delegate { twinsBlocker.OtherDoPath(); });
                }
            }
            else
            {
                var blocker = JamGridMono.GetBlocker(index);
                if (!blocker || !(blocker is TwinsBlocker twinsBlocker)) return;
                DOTween.To(() => 1, _ => { }, 1, 0.2f).OnComplete(delegate { twinsBlocker.OtherDoPath(); });
            }
        }

        public bool HideTwinsBlocker(int index)
        {
            var isAffect = false;
            var blocker = JamGridMono.GetBlocker(index);
            if (blocker)
            {
                isAffect = blocker.ClearBlockerEffect();
            }

            return isAffect;
        }

        public void DoIceBlocker()
        {
            var blocker = JamGridMono.GetBlockersByType(BlockerType.Ice);
            foreach (var t in blocker)
            {
                var tile = GetTerrainTile(t.index);
                if (tile.tileItem == 0) continue;
                var item = JamGridMono.GetTileItem(tile.tileItem);
                if (item.VirusState == VirusState.Idle)
                    t.ClearBlockerEffect();
            }
        }

        public void Replay()
        {
            GameState = GameState.Playing;
            DOTween.KillAll();
            JamGridMono.ReBron();
            Board.Clear();
        }

        public void StartNewGame()
        {
            var childCount = _levelParent.childCount;
            for (var i = childCount - 1; i >= 0; i--)
            {
                Object.Destroy(_levelParent.GetChild(i).gameObject);
            }

            _levelIndex = 0;
            DOTween.KillAll();
            Board.Clear();

            _levelParent.localPosition = Vector3.zero;

            Pool.ObjectPool.Instance.HideObjects("grass_block");
            Pool.ObjectPool.Instance.HideObjects("dirt_block");

            LoadLevel(_levelMaps[_levelIndex]);
            //MainComponent.XLua.CallLua("PassLevel", _levelIndex);
        }

        public Vector3 TileIndexToPosition(int index)
        {
            return JamGridMono.TileIndexToPosition(index);
        }

        public TerrainTile GetTerrainTile(int index)
        {
            return JamGridMono.GetGridSpot(index);
        }

        public TileItem GetTileItem(int index)
        {
            return JamGridMono.GetTileItem(index);
        }

        public TerrainTile GetNeighborTerrainTile(int index, Direction direction)
        {
            if (GameState == GameState.End)
                return null;
            return direction switch
            {
                Direction.Left => JamGridMono.GetLeftGridSpot(index),
                Direction.Up => JamGridMono.GetTopGridSpot(index),
                Direction.Right => JamGridMono.GetRightGridSpot(index),
                Direction.Down => JamGridMono.GetBottomGridSpot(index),
                _ => null
            };
        }

        public void EndGame(bool success)
        {
            Debug.Log("end game");
            if (GameState != GameState.Playing)
                return;
            GameState = GameState.End;
            //DOTween.KillAll();

            if (!success)
            {
                // MainComponent.Max.HideBanner();
                // MainComponent.XLua.CallLua("OnGameEnd", false);
                Debug.LogError("game over");
                //EventDispatcher.TriggerEvent(EventKey.E_JamLevelFailed, _levelIndex);
                jamTriggerEvents.JamLevelFailedEvent?.Invoke(_levelIndex);
            }
            else
            {
                _levelIndex++;
                if (_levelIndex < _levelMaps.Length)
                {
                    LoadLevel(_levelMaps[_levelIndex]);
                    //MainComponent.XLua.CallLua("PassLevel", _levelIndex);
                    //EventDispatcher.TriggerEvent(EventKey.E_JamSubLevelSuccess, _levelIndex);
                    jamTriggerEvents.JamSubLevelSuccessEvent?.Invoke(_levelIndex);
                }
                else
                {
                    // MainComponent.Max.HideBanner();
                    // MainComponent.XLua.CallLua("OnGameEnd", true);
                    //EventDispatcher.TriggerEvent(EventKey.E_JamLevelSuccess, _levelIndex);
                    jamTriggerEvents.JamLevelSuccessEvent?.Invoke(_levelIndex);
                }
            }
        }

        public List<int> BoardPlaceColorInfo()
        {
            return Board.GetPlaceColorInfo();
        }

        public bool CanReplace()
        {
            return Board.CanReplace();
        }

        public bool ContinueGame()
        {
            if (!Board.CanReplace()) return false;
            GameState = GameState.Playing;
            Board.playing = true;
            Replace();
            return true;
        }

        public bool InGame()
        {
            return GameState == GameState.Playing;
        }

        public string GetGridElementInfo()
        {
            return JamGridMono.GetLevelElementInfo();
        }

        public int GetGridItemCount()
        {
            return JamGridMono.GridItemCount;
        }

        public int GetGridGroupCount()
        {
            return JamGridMono.GridGroupCount;
        }


        public int GetPurgeCount()
        {
            return Board.PurgeCount;
        }

        public int GetHandCardCount()
        {
            return Board.HandCount();
        }

        public int GetBeforeHandCardCount()
        {
            return _beforeHandCardNum;
        }

        public int GetReplaceCount()
        {
            return Board.ReplaceCount();
        }

        public int GetHandCardColorCount()
        {
            return Board.HandColorCount();
        }

        public int GetBeforeHandCardColorCount()
        {
            return _beforeColorNum;
        }

        public void PlaySoundAction(string sound)
        {
            //MainComponent.Audio.PlaySound(sound);
            jamTriggerEvents.JamPlaySoundAction?.Invoke(sound);
        }

        public void UnloadScene()
        {
            jamTriggerEvents.JamUnloadScene?.Invoke();
        }
    }

    public class JamTriggerEvents
    {
        public enum ResType
        {
            TerrainConfig,
        }

        public Action JamAfterStartGame { get; private set; }
        public Action<int> JamLevelSuccessEvent { get; private set; }
        public Action<int> JamSubLevelSuccessEvent { get; private set; }
        public Action<int> JamLevelFailedEvent { get; private set; }
        public Action<string> JamPlaySoundAction { get; private set; }
        public Action JamUnloadScene { get; private set; }
        public Func<int, Transform, GameObject> JamLoadLevelFunc { get; private set; }

        public Func<int, ResType, Object> JamLoadResourceFunc { get; private set; }

        public Func<string, Object> JamLoadResourceByRelativePathInJamPackage { get; private set; }

        public Func<int, string> JamStepExchangeStringFunc { get; private set; }

        public JamTriggerEvents(Action jamAfterStartGame, Action<int> jamLevelSuccessEvent,
            Action<int> jamSubLevelSuccessEvent, Action<int> jamLevelFailedEvent, Action<string> jamPlaySoundAction,
            Action jamUnloadScene, Func<int, Transform, GameObject> jamLoadLevelFunc)
        {
            JamAfterStartGame = jamAfterStartGame;
            JamLevelSuccessEvent = jamLevelSuccessEvent;
            JamSubLevelSuccessEvent = jamSubLevelSuccessEvent;
            JamLevelFailedEvent = jamLevelFailedEvent;
            JamPlaySoundAction = jamPlaySoundAction;
            JamUnloadScene = jamUnloadScene;
            JamLoadLevelFunc = jamLoadLevelFunc;
        }

        public JamTriggerEvents(Action jamAfterStartGame, Action<int> jamLevelSuccessEvent,
            Action<int> jamSubLevelSuccessEvent, Action<int> jamLevelFailedEvent,
            Action<string> jamPlaySoundAction, Action jamUnloadScene, Func<int, Transform, GameObject> jamLoadLevelFunc,
            Func<int, string> jamStepExchangeStringFunc,
            Func<int, ResType, Object> jamLoadAssetFunc, Func<string, Object> jamLoadResourceByRelativePathInJamPackage)
            : this(jamAfterStartGame, jamLevelSuccessEvent, jamSubLevelSuccessEvent, jamLevelFailedEvent,
                jamPlaySoundAction, jamUnloadScene, jamLoadLevelFunc)
        {
            JamStepExchangeStringFunc = jamStepExchangeStringFunc;
            JamLoadResourceFunc = jamLoadAssetFunc;
            JamLoadResourceByRelativePathInJamPackage = jamLoadResourceByRelativePathInJamPackage;
        }
    }
}
using Jam3d;
using System.Linq;
using TMPro;
using UnityEngine;

namespace GameLogic
{
    public class SpawnItem : TileItem
    {
        [HideInInspector]public Direction direction;
        
        public TextMeshPro tmp;
        
        private bool _spawning;
        private bool _isUndoing;

        private void Awake()
        {
            tmp.color = new Color(0.32f, 0.32f, 0.81f);
        }

        private int CanSpawnCount
        {
            get
            {
                return _virusItems.Count(t => t.VirusState == VirusState.Hide);
            }
        }

        public int CurrentIndex
        {
            get
            {
                var ret = 0;
                for (var i = 0; i < _virusItems.Length; i++)
                {
                    if (_virusItems[i].VirusState != VirusState.Idle) continue;
                    ret = i;
                    break;
                }
                return _virusItems.Length - ret;
            }
        }
        
        private int NextIndex
        {
            get
            {
                var ret = 0;
                for (var i = _virusItems.Length - 1; i >= 0 ; i--)
                {
                    if (_virusItems[i].VirusState != VirusState.Hide) continue;
                    ret= i;
                    break;
                }
                return ret;
            }
        }
        
        private TerrainTile _affectTile;
        
        private VirusItem[] _virusItems;

        private VirusItem _current;
        public VirusItem Current => _current;
        public VirusItem Next => _virusItems[NextIndex];
        
        public GameObject itemPrefab;
        
        /// <summary>
        /// color inspector
        /// </summary>
        public int[] virusColors;

        public VirusItem DONext(bool hasPath = true,bool violent = false)
        {
            if (_current && _current.VirusState == VirusState.Born)
            {
                _current.VirusState = VirusState.Idle;
                return _current;
            }

            if (_current && _current.VirusState == VirusState.Idle)
            {
                return _current;
            }
            
            if (CanSpawnCount == 0)
            {
                VirusState = VirusState.Dead;
                return null;
            }
            _current = _virusItems[NextIndex];
            // if (CanSpawnCount == 0)
            // {
            //      var pathList = JamManager.GetSingleton().StartNavigation(_current.index);
            //     _current.VirusState = pathList != null ? VirusState.Idle : VirusState.Born;
            // }
            // else
            {
                _current.VirusState = hasPath ? VirusState.Idle : VirusState.Born;
            }
            tmp.text = CanSpawnCount.ToString();
            return _current;
        }

        protected override void OnStateChanged(VirusState value)
        {
            switch (value)
            {
                case VirusState.Reborn when _virusItems == null:
                    Debug.LogError("virus array not initialization!!!");
                    return;
                case VirusState.Reborn:
                {
                    foreach (var t in _virusItems)
                    {
                        t.VirusState = VirusState.Hide;
                    }
                    //Next();
                    _current = null;
                    tmp.text = capacity.ToString();
                    break;
                }
                case VirusState.Born:
                    if(_virusItems != null)
                        return;
                    var size = JamManager.GetSingleton().JamGridMono.gridSize;
                    var y = direction switch
                    {
                        Direction.Left => 90,
                        Direction.Up => 180,
                        Direction.Right => -90,
                        Direction.Down => 0,
                        _ => 0
                    };
                    _affectTile = direction switch
                    {
                        Direction.Left => JamManager.GetSingleton().GetTerrainTile(index - 1),
                        Direction.Up => JamManager.GetSingleton().GetTerrainTile(index - size),
                        Direction.Right => JamManager.GetSingleton().GetTerrainTile(index + 1),
                        Direction.Down => JamManager.GetSingleton().GetTerrainTile(index + size),
                        _ => null
                    };
                    if (_affectTile)
                    {
                        _affectTile.affectItem = index;
                        
                        _virusItems = new VirusItem[capacity];
                        virusColors = new int[capacity];
                        
                        for (var i = 0; i < capacity; i++)
                        {
                            var virusGo = Instantiate(itemPrefab, transform.parent);
                            var vs = virusGo.GetComponent<VirusItem>();
                            vs.sourceType = SourceType.Spawn;
                            vs.VirusState = VirusState.Hide;
                            vs.index = _affectTile.index;
                            _virusItems[i] = vs;
                            virusGo.transform.rotation = Quaternion.Euler(0,y,0);
                            CommonMethod.SetLayerRecursively(virusGo, "Jam3d");
                        }
                        _affectTile.walkable = false;
                    }
                    else
                    {
                        Debug.LogError("spawn item direction error " + index);
                    }     
                    _current = null;
                    tmp.text = CanSpawnCount.ToString();
                    break;
                // case VirusState.Idle:
                //     Next();
                //     break;
            }
        }

        public override void AssignColor(int color,int order)
        {
            if (_virusItems == null || _virusItems.Length == 0)
            {
                if (virusColors == null || virusColors.Length == 0)
                {
                    virusColors = new int[capacity];
                }
                virusColors[order] = color;
                Debug.LogWarning("spawn item is not born");
                return;
            }
           
            if (order >= capacity)
            {
                Debug.LogError("spawn item is full");
                return;
            }

            virusColors[order] = color;
            var spawnItem = _virusItems[order];
            spawnItem.AssignColor(color, 0);
        }

        public override void DoPath()
        {
            
        }


        public override void Unlock()
        {
            // if(VirusState == VirusState.Dead)
            //     return;
            //Next();
            //_current.VirusState = VirusState.Idle;
        }
        
        public override void Undo()
        {
            if (_current.VirusState == VirusState.Place)
            {
                _current.Undo();
            }
            else
            {
                VirusItem item = null;
                foreach (var virus in _virusItems)
                {
                    if (virus != _current && virus.VirusState == VirusState.Place)
                    {
                        _current.VirusState = VirusState.Hide;
                        virus.Undo();
                        item = _current = virus;
                        break;
                    }
                }

                if (item == null)
                {
                    _current.VirusState = VirusState.Hide;
                }
            }

            _affectTile.isBlock = true;
            _affectTile.walkable = false;
            _affectTile.AdjacentSpotClose();
            tmp.text = CanSpawnCount.ToString();
        }

        public override bool IsUndoing()
        {
            return _isUndoing;
        }
    }
}
using System;
using DG.Tweening;
using UnityEngine;

namespace GameLogic
{
    public class PinItem : TileItem
    {
        private Direction _direction;
        public int affectRange = 2;

        public Transform pole;
        private void Awake()
        {
            if (Math.Abs(transform.eulerAngles.y - 90) < 1)
            {
                _direction = Direction.Down;
            }
            else if (Math.Abs(transform.eulerAngles.y - 270) < 1)
            {
                _direction = Direction.Up;
            }
            else if (Math.Abs(transform.eulerAngles.y  - 180f) < 1)
            {
                _direction = Direction.Left;
            }
            else
            {
                _direction = Direction.Right;
            }
            
            var scale = pole.localScale;
            var sx = -(affectRange / 2f);
            scale.x = sx;
            pole.localScale = scale;
            var pos = pole.localPosition;
            pos.x = 0.324f - sx;
            pole.localPosition = pos;
        }
        
        
        protected override void OnStateChanged(VirusState value)
        {
            if (value != VirusState.Born && value != VirusState.Reborn) return;
            // if(!JamManager.GetSingleton().JamGrid)
            //     return;
            var size = JamManager.GetSingleton().JamGridMono.gridSize;
            var affect = _direction switch
            {
                Direction.Left => JamManager.GetSingleton().GetTerrainTile(index - affectRange),
                Direction.Up => JamManager.GetSingleton().GetTerrainTile(index - affectRange * size),
                Direction.Right => JamManager.GetSingleton().GetTerrainTile(index + affectRange),
                Direction.Down => JamManager.GetSingleton().GetTerrainTile(index + affectRange * size),
                _ => null
            };

            if (!affect)
            {
                Debug.LogError($"{index} pin affect range {affectRange} error.");
                return;
            }

            //animator.Play("Default");
            gameObject.SetActive(true);
            affect.tileItem = index;
            affect.walkable = false;
        }

        public override void AssignColor(int color, int order)
        {
            //Debug.LogError("assign color");
        }

        public override void DoPath()
        {
            
        }

        public override void Unlock()
        {
            var size = JamManager.GetSingleton().JamGridMono.gridSize;
            if (VirusState == VirusState.Dead)
            {
                var a = _direction switch
                {
                    Direction.Left => JamManager.GetSingleton().GetTerrainTile(index - affectRange),
                    Direction.Up => JamManager.GetSingleton().GetTerrainTile(index - affectRange * size),
                    Direction.Right => JamManager.GetSingleton().GetTerrainTile(index + affectRange),
                    Direction.Down => JamManager.GetSingleton().GetTerrainTile(index + affectRange * size),
                    _ => null
                };
                if (a != null) a.ClearGridItem();
                return;
            }

            var affectTile = _direction switch
            {
                Direction.Left => JamManager.GetSingleton().GetTerrainTile(index + 1),
                Direction.Up => JamManager.GetSingleton().GetTerrainTile(index + size),
                Direction.Right => JamManager.GetSingleton().GetTerrainTile(index - 1),
                Direction.Down => JamManager.GetSingleton().GetTerrainTile(index - size),
                _ => null
            };
            if (affectTile == null || !affectTile.walkable)
            {
                return;
            }

            var affect = _direction switch
            {
                Direction.Left => JamManager.GetSingleton().GetTerrainTile(index - affectRange),
                Direction.Up => JamManager.GetSingleton().GetTerrainTile(index - affectRange * size),
                Direction.Right => JamManager.GetSingleton().GetTerrainTile(index + affectRange),
                Direction.Down => JamManager.GetSingleton().GetTerrainTile(index + affectRange * size),
                _ => null
            };

            if (!affect)
            {
                Debug.LogError($"{index} pin affect range {affectRange} error.");
                return;
            }

            JamManager.GetSingleton().JamGridMono.Unblock(index);
            var pathList = JamManager.GetSingleton().StartNavigation(affect.index);
            if (pathList != null)
            {
                affect.ClearGridItem();
            }
            else
            {
                affect.OnPreUnlock(false);
            }
            DoDisappear();
        }

        private void DoDisappear()
        {
            //Debug.LogError("you saw the pin disappear.");
            VirusState = VirusState.Dead;
            var pos = transform.localPosition;
            switch (_direction)
            {
                case Direction.Left:
                    pos.x += 1;
                    break;
                case Direction.Up:
                    pos.z -= 1;
                    break;
                case Direction.Right:
                    pos.x -= 1;
                    break;
                case Direction.Down:
                    pos.z += 1;
                    break;
            }
            
            transform.DOScale(Vector3.one * 0.5f, 0.5f);
            transform.DOShakePosition(0.5f, new Vector3(0, 0.3f, 0));
            transform.DOLocalMove(pos, 0.5f).OnComplete(delegate
            {
                gameObject.SetActive(false);
            });
        }

        public override void Undo()
        {
        }

        public override bool IsUndoing()
        {
            return false;
        }
    }
}
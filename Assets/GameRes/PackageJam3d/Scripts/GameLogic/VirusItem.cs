using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameLogic
{
    public class VirusItem : TileItem
    {
        private static int _globalIntroduction;
        private List<int> _pathList = new List<int>();

        public float speed = 1;
        public ParticleSystem moveEffect;

        private Tween _tween;
        private Vector3? _moveTarget;

        private bool _isUndoing;
        private bool _onShuffle;

        private int _cacheIndex;

        protected Animator Animator;
        private static readonly int RunHash = Animator.StringToHash("RunInt");

        private Transform _transform;

        private Quaternion _left;
        private Quaternion _right;
        private Quaternion _top;
        private Quaternion _bottom;

        public int introduction;

        // private const float OutLineValue = 0.05f;
        // private const float SpecularScale = 0.01f;

        public SkinnedMeshRenderer skinnedMeshRenderer;
        public MeshRenderer downRenderer;


        private MaterialPropertyBlock _propertyBlock;
        
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        // private static readonly int Outline = Shader.PropertyToID("_Outline");
        // private static readonly int Scale = Shader.PropertyToID("_SpecularScale");

        [HideInInspector]public Transform TwinsBone;

        protected virtual void Awake()
        {
            var eventTrigger = GetComponent<EventTrigger>();
            var clickEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick,
                callback = new EventTrigger.TriggerEvent()
            };
            clickEntry.callback.AddListener(OnItemClick);
            eventTrigger.triggers.Add(clickEntry);

            Animator = GetComponentInChildren<Animator>();

            _transform = transform;
            _left = Quaternion.Euler(0, 90, 0);
            _right = Quaternion.Euler(0, -90, 0);
            _top = Quaternion.Euler(0, 180, 0);
            _bottom = Quaternion.Euler(0, 0, 0);

            sourceType = SourceType.Independence;

            TwinsBone = transform.Find("mesh/Point001/Bone001");
        }

        private void OnItemClick(BaseEventData baseEventData)
        {
            //AudioManager.Instance.PlaySound(AudioName.Tile_Brick_Click);
            JamManager.GetSingleton().PlaySoundAction("Tile_Brick_Click");
            switch (VirusState)
            {
                case VirusState.Replace:
                    VirusState = VirusState.WaitToPlace;
                    JamManager.GetSingleton().PlaceItem(this);
                    return;
                case VirusState.Born:
                    Animator.Play("No");
                    return;
            }

            if (VirusState != VirusState.Idle)
            {
                Debug.LogWarning("click no stand up item : " + index + " state :" + VirusState);
                return;
            }

            DoPath();
            JamManager.GetSingleton().DoTwinsBlocker(index, sourceType);
        }

        public override void DoPath()
        {
            _pathList = JamManager.GetSingleton().StartNavigation(index);
            if (_pathList == null)
                return;

            VirusState = VirusState.Moving;
            JamManager.GetSingleton().JamGridMono.Unblock(index);
            introduction = _globalIntroduction++;
            Animator.SetInteger(RunHash, 1);
            moveEffect.Play();
            JamManager.GetSingleton().PlaceItem(this);
        }


        public override void AssignColor(int color, int order)
        {
            virusColor = color;
            ChangeColor();
        }

        /// <summary>
        /// change material
        /// </summary>
        private void ChangeColor()
        {
            var m= JamManager.GetSingleton().GetMaterial(virusColor);
            skinnedMeshRenderer.material = m;
            downRenderer.material = m;
        }

        private void Update()
        {
#if GM_MODE
            ChangeColor();
#endif
            if (VirusState != VirusState.Moving) return;
            if (_pathList.Count > 0 && _moveTarget == null)
            {
                var pos = JamManager.GetSingleton().TileIndexToPosition(_pathList[0]);
                _moveTarget = pos;
                if (_moveTarget != null)
                {
                    _pathList.RemoveAt(0);
                    if (_moveTarget?.x < _transform.position.x)
                    {
                        _transform.localRotation = _left;
                    }

                    if (_moveTarget?.x > _transform.position.x)
                    {
                        _transform.localRotation = _right;
                    }

                    if (_moveTarget?.z > _transform.position.z)
                    {
                        _transform.localRotation = _top;
                    }

                    if (_moveTarget?.z < _transform.position.z)
                    {
                        _transform.localRotation = _bottom;
                    }
                }
            }

            if (_pathList.Count == 0 && _moveTarget == null)
            {
                Animator.SetInteger(RunHash, 0);
                VirusState = VirusState.WaitToPlace;
                return;
            }

            if (_moveTarget == null) return;
            {
                var position = _transform.position;
                var pos = (Vector3)_moveTarget;
                pos.y = position.y;
                position = Vector3.MoveTowards(position, pos, Time.deltaTime * speed);
                _transform.position = position;
                if (Vector3.Distance(position, pos) < 0.01f)
                {
                    _moveTarget = null;
                }
            }
        }

        protected override void OnStateChanged(VirusState value)
        {
            _moveTarget = null;

            switch (value)
            {
                case VirusState.Hide:
                    DoHide();
                    break;
                case VirusState.Fake:
                    break;
                case VirusState.Born:
                    DoBorn();
                    break;
                case VirusState.Idle:
                    DoIdle();
                    break;
                case VirusState.Moving:
                    break;
                case VirusState.Disappear:
                    DoDisappear();
                    break;
                case VirusState.Shuffle:
                    DoShuffle();
                    break;
                case VirusState.Place:
                case VirusState.Dead:
                case VirusState.WaitToPlace:
                    break;
                case VirusState.Reborn:
                    DoReborn();
                    break;
            }
        }

        /// <summary>
        /// spawn queue virus 
        /// </summary>
        private void DoHide()
        {
            _transform.localScale = Vector3.zero;
            StandUp = false;
        }

        /// <summary>
        /// start state
        /// </summary>
        private void DoBorn()
        {
            if (!Animator) return;
            var pos = JamManager.GetSingleton().TileIndexToPosition(index);
            _transform.localPosition = pos;
            _transform.localScale = Vector3.one;
            _transform.localRotation = Quaternion.identity;

            //play born animation
            Animator.SetInteger(RunHash, 0);
            Animator.Play("Idle_Short");

            StandUp = false;
            // var materials = SkinnedMeshRenderer.materials;
            // var colorMaterial = materials[0];
            // colorMaterial.SetFloat(Outline, 0);
            // colorMaterial.SetFloat(Scale, 0);
        }


        protected bool StandUp;

        /// <summary>
        /// on hide fake or born state to idle
        /// </summary>
        private void DoIdle()
        {
            if (!Animator) return;
            var pos = JamManager.GetSingleton().TileIndexToPosition(index);
            _transform.localPosition = pos;
            if (_transform.localScale != Vector3.one)
            {
                _transform.DOScale(Vector3.one, 0.3f);
            }

            if (_transform.localRotation != Quaternion.identity)
            {
                _transform.DOLocalRotate(Vector3.zero, 0.3f);
            }


            if (!StandUp)
            {
                //play idle animation
                var transformLocalRotation = Animator.transform.localRotation;
                transformLocalRotation.eulerAngles = new Vector3(0, 180, 0);
                Animator.transform.localRotation = transformLocalRotation;
                Animator.Play("StandUp");

                StandUp = true;
            }

            Animator.SetInteger(RunHash, 0);

            // var materials = SkinnedMeshRenderer.materials;
            // var colorMaterial = materials[0];
            // colorMaterial.SetFloat(Outline, OutLineValue);
            // colorMaterial.SetFloat(Scale, SpecularScale);
        }

        /// <summary>
        /// first time move to slot
        /// </summary>
        /// <param name="slot"></param>
        public void DoPlace(Slot slot)
        {
            _tween?.Kill();
            var position = slot.transform.position;
            _tween = _transform.DOMove(position, 0.5f);
            Animator.SetInteger(RunHash, 1);

            var targetDir = _transform.position - position;
            var targetRotation = Quaternion.LookRotation(targetDir, Vector3.up);
            _transform.rotation = targetRotation;

            _tween.OnComplete(delegate
            {
                _tween = null;
                JamManager.GetSingleton().DetectMatch();
                var isAffect = JamManager.GetSingleton().HideTwinsBlocker(index);
                if (!isAffect)
                {
                    JamManager.GetSingleton().DoIceBlocker();
                }

                Animator.SetInteger(RunHash, 0);
                moveEffect.Stop();
                _transform.localRotation = Quaternion.Euler(0, 0, 0);            
            });
            VirusState = VirusState.Place;
        }

        /// <summary>
        /// replace by prop
        /// </summary>
        /// <param name="slot"></param>
        public void DoReplace(Slot slot)
        {
            _tween?.Kill();
            _tween = null;

            Animator.SetInteger(RunHash, 0);
            moveEffect.Stop();

            var targetPos = slot.transform.position;
            targetPos.z -= 1.2f;
            _transform.position = targetPos;
            _transform.localRotation = Quaternion.Euler(0, 0, 0);

            VirusState = VirusState.Replace;
        }

        private void DoDisappear()
        {
            var localY = _transform.localPosition.y;

            _transform.DOLocalMoveY(localY + 0.5f, 0.3f).OnComplete(delegate
            {
                VirusState = VirusState.Dead;
                _transform.DOScale(Vector3.zero, 0.3f).OnComplete(delegate
                {
                    JamManager.GetSingleton().DetectGameState();
                });
            });
        }

        private void DoReborn()
        {
            _moveTarget = null;


            Animator.SetInteger(RunHash, 0);

            var pos = JamManager.GetSingleton().TileIndexToPosition(index);
            pos.y = 0.5f;
            _transform.position = pos;
            _pathList = JamManager.GetSingleton().StartNavigation(index);
            VirusState = _pathList != null ? VirusState.Idle : VirusState.Born;
        }

        private void DoShuffle()
        {
            if (_onShuffle)
                return;

            _onShuffle = true;
            var pos = _cacheIndex == index ? _transform.position : JamManager.GetSingleton().TileIndexToPosition(index);
            PlayShuffleTween(pos);
        }

        private void PlayShuffleTween(Vector3 toPos)
        {
            var upTween = _transform.DOLocalMoveY(5, 0.5f);
            upTween.OnComplete(delegate
            {
                toPos.y = 5;
                //transform.LookAt(toPos);
                //Animator.Play("StandUp");
                //Animator.SetInteger(RunHash, 1);
                var moveTween = _transform.DOMove(toPos, 0.5f);
                moveTween.OnComplete(delegate
                {
                    var downTween = _transform.DOLocalMoveY(1, 0.5f);
                    downTween.OnComplete(delegate
                    {
                        _cacheIndex = index;
                        _onShuffle = false;
                        //VirusState = VirusState.Idle;
                        _pathList = JamManager.GetSingleton().StartNavigation(index);
                        VirusState = _pathList != null ? VirusState.Idle : VirusState.Born;
                    });
                });
            });
        }


        public override void Unlock()
        {
            if (VirusState != VirusState.Born)
                return;
            VirusState = VirusState.Idle;
        }

        public override void Undo()
        {
            var tilePosition = JamManager.GetSingleton().TileIndexToPosition(index);
            _isUndoing = true;
            _tween?.Kill();
            var move = _transform.DOMove(tilePosition, 0.2f);
            move.OnComplete(delegate
            {
                _isUndoing = false; 
                VirusState = VirusState.Idle;
            });
            
            _moveTarget = null;
        }

        public bool DoPull()
        {
            if (!StandUp)
            {
                //play idle animation
                Animator.Play("StandUp");
                StandUp = true;
                Animator.SetInteger(RunHash, 0);
                return true;
            }

            Animator.SetInteger(RunHash, 0);
            return false;
        }

        public override bool IsUndoing()
        {
            return _isUndoing;
        }

        [ContextMenu("PrintState")]
        public void PrintState()
        {
            Debug.Log(VirusState + " : " + introduction);
        }
    }
}
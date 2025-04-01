using DG.Tweening;
using Game.UISystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.TileGame
{
    public class TowerRulePage : PageBase
    {
        public Button closeBtn;

        public List<Transform> objs = new List<Transform>();

        public Text rule3;

        private void Awake()
        {
            closeBtn.onClick.AddListener(() => { Close(); });
        }

        protected override void OnBeginOpen()
        {
            base.OnBeginOpen();
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].localScale = Vector3.zero;
                objs[i].DOScale(1, 0.25f).SetEase(Ease.OutBack).SetDelay(i * 0.2f + 0.5f);
            }
        }
    }
}

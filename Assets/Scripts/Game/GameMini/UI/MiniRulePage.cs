using Game.UISystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.MiniGame
{
    public class MiniRulePage : PageBase
    {
        [SerializeField] private Button _btnClose;
        [SerializeField] private Text _txtTitle;
        [SerializeField] private Text _txtDescribe;
        [SerializeField] private Transform _tranContent;

        protected override void OnInit()
        {
            _btnClose.onClick.AddListener(Close);

        }

        protected override void OnBeginOpen()
        {
            int tType = (int)ModuleManager.MiniGame.GameType;
            for (int i = 0; i < _tranContent.childCount; i++)
            {
                _tranContent.GetChild(i).gameObject.SetActive(i + 1 == tType);
            }

            if (tType == 1)
            {
                _txtTitle.text = "SCREWJAM RULES";
                _txtDescribe.text = "Fill the toolboxes with <color=#dd2dfa>matching screws</color>. \r\nClear the board by <color=#dd2dfa>completing all the toolboxes</color>.";
            }
            else if (tType == 2)
            {
                _txtTitle.text = "Jam Crush Rules";
                _txtDescribe.text = "Clean the board by matching \r\n<color=#dd30f7>3 blocks with the same color</color>.\r\n\r\nThe <color=#dd30f7>more levels</color> you win, \r\nthe <color=#dd30f7>more rewards</color> to unlock!";
            }
            else if (tType == 3)
            {
                _txtTitle.text = "Triple Crush Rules";
                _txtDescribe.text = "Clean the board by matching \r\n<color=#dd30f7>3 Items with the same pattern</color>.\r\n\r\nThe <color=#dd30f7>more levels</color> you win, \r\nthe <color=#dd30f7>more rewards</color> to unlock!";
            }

        }
    }

}
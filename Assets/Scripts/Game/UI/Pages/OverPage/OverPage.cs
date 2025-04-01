using Game.UISystem; 
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class OverPage : PageBase
    {
        [SerializeField] private TextMeshProUGUI _txtLevel;

        [SerializeField] private Button _btnHome; 

         
        OverPageParam mParam; 
        protected override void OnInit()
        { 
            _btnHome.onClick.AddListener(OnClickBtnHome); 
        }

        protected override void OnBeginOpen()
        {
            mParam = PageParam as OverPageParam;
             
            if (mParam == null)
            {
                LogManager.LogError(" OverPage: invalid param");
                return;
            }

            
        }


        #region UI事件

        void OnClickBtnHome()
        {

        }

        #endregion
    }

    public class OverPageParam
    {
         

    }
}
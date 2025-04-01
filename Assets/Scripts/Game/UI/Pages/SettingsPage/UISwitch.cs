using UnityEngine;

namespace Game.UI
{
    public class UISwitch : MonoBehaviour
    {
        [SerializeField] private Transform _onRoot;
        [SerializeField] private Transform _offRoot;

        public void SetSwitch(bool pIsOn)
        {
            _onRoot.gameObject.SetActive(pIsOn);
            _offRoot.gameObject.SetActive(!pIsOn);
        }
    }
}
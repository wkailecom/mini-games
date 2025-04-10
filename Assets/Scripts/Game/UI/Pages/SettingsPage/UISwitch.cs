using UnityEngine;

namespace Game.UI
{
    public class UISwitch : MonoBehaviour
    {
        [SerializeField] private Transform _onRoot;
        [SerializeField] private Transform _offRoot;
        [HideInInspector] public bool isOn;
        public void SetSwitch(bool pIsOn)
        {
            isOn = pIsOn;
            _onRoot.gameObject.SetActive(pIsOn);
            _offRoot.gameObject.SetActive(!pIsOn);
        }
    }
}
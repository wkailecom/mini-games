using System;
using UnityEngine;

namespace ScrewJam
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Tools/CreateGameConfig", order = 1)]
    [Serializable]
    public class GameConfig : ScriptableObject
    {
        [SerializeField]
        public Color[] boardColor;
        [SerializeField]
        public Color[] screwColor;
    }
}
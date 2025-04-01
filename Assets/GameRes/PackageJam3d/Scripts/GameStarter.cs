using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;
using Jam3d;

namespace Jam3d
{
    [Serializable]
    public class LevelConfig
    {
        public int[] maps;
    }

    public class GameStarter : MonoBehaviour
    {
        public LevelConfig[] levels;

        public int currentLevel = 1;

        public Transform gameEntityParent;
        public Transform buildEntityParent;
        public Transform levelParent;

        public GameObject boardPrefab;
        private void Awake()
        {
            JamManager.GetSingleton().SetTransform(boardPrefab, levelParent, buildEntityParent, gameEntityParent);
        }

        // Start is called before the first frame update
        void Start()
        {
            //JamManager.GetSingleton().StartGame(levels[currentLevel - 1].maps);
        }
    }
}
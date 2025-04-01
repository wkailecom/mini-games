using System;
using UnityEngine;
using UnityEngine.Scripting;

[assembly:Preserve]
namespace ScrewJam
{
    [Serializable]
    public struct LevelData
    {
        public BoardData[] boards;
        public BoxData[] boxes;
        public ScrewData[] screws;
    }

    [Serializable]
    public struct BoardData
    {
        public string boardName;
        public int layer;
        public Vector3 position;//local
        public Vector3 eulerAngle;//local
        public int[] screwIndex;
    }

    [Serializable]
    public struct BoxData
    {
        public int colorIndex;
        public int count;
    }

    [Serializable]
    public struct ScrewData
    {
        public int colorIndex;
        public Vector3 position;
        public int groupID;
    }
}
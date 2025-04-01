#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using System.Text;

namespace ScrewJam.ScrewEditor
{
    [ExecuteInEditMode]
    public class ScrewEditorData : MonoBehaviour
    {
        //在螺丝数组中的index
        public int selfIndex;

        //前置的螺丝数组
        public int[] preIndexArray = new int[0];

        //前置的螺丝transform，用来显示debug
        [HideInInspector]
        public Transform[] preTrArray = new Transform[0];

        private GUIStyle style = new GUIStyle();

        //前置螺丝数量
        [HideInInspector]
        public int preCount;

        //[HideInInspector]
        public int groupID = 0;

        private void Awake()
        {
            style.normal.textColor = Color.black;
            style.fontStyle = FontStyle.Bold;
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        [DidReloadScripts]
        private static void Reload()
        {
            var builder = FindObjectOfType<ScrewEditorData>();
            if (builder != null)
            {
                builder.Awake();
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (!gameObject.activeInHierarchy)
                return;
            var pos = transform.position;
            StringBuilder str = new StringBuilder();
            if (preIndexArray != null && preIndexArray.Length > 0)
            {
                str.Append($"{preIndexArray.Length}");
            }
            //if (groupID != 0)
            //{
            //    str.Append($"group:{groupID}");
            //}
            Handles.Label(pos, str.ToString(), style);
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }
    }
}


#endif
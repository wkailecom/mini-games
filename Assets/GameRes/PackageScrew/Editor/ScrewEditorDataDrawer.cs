using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScrewJam.ScrewEditor
{
    [CustomEditor(typeof(ScrewEditorData))]
    public class ScrewEditorDataDrawer : Editor
    {
        private void OnSceneGUI()
        {
            var t = target as ScrewEditorData;
            Handles.color = Color.green;
            Handles.DrawWireCube(t.transform.position, Vector3.one * 0.5f);
            if(t.preTrArray != null && t.preTrArray.Length > 0)
            {
                for (int i = 0; i < t.preTrArray.Length; i++)
                {
                    Handles.color = Color.red;
                    Handles.DrawLine(t.transform.position, t.preTrArray[i].position);
                    Handles.DrawWireCube(t.preTrArray[i].position, Vector3.one * 0.5f);
                }
            }
        }
    }
}


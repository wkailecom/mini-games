using UnityEditor;

namespace ScrewJam.ScrewEditor
{
    [CustomEditor(typeof(MapBuilder))]
    public class MapBuilderEditor : Editor
    {
        private void OnSceneGUI()
        {
            var builder = target as MapBuilder;
        }
    }
}


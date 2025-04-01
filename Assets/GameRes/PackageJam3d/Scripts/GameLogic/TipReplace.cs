using GameLogic;
using UnityEngine;
using UnityEngine.Assertions;

public class TipReplace : MonoBehaviour
{
    
    private static readonly int Color1 = Shader.PropertyToID("_BaseColor");

    public SkinnedMeshRenderer[] meshes;
    
    
    public void Start()
    {
        var colors = JamManager.GetSingleton().BoardPlaceColorInfo();
        if (colors.Count != meshes.Length)
        {
            colors.Clear();
            colors.Add(2);
            colors.Add(2);
            colors.Add(4);
            colors.Add(4);
            colors.Add(1);
            colors.Add(1);
            colors.Add(3);
        }
        
        Assert.IsTrue(colors.Count == meshes.Length);
        for (var i = 0; i < meshes.Length; i++)
        {
            meshes[i].material = JamManager.GetSingleton().GetMaterial(colors[i]);
        }
    }
}

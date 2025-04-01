
using System.Collections.Generic;
using UnityEngine;

namespace Jam3d
{
    public static partial class CommonMethod
    {
        public static void SetLayerRecursively(this GameObject obj, string layerName)
        {
            obj.layer = LayerMask.NameToLayer(layerName);

            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursively(layerName);
            }
        }

        public static List<string> TryParseListString(string pString, char pSplit = '&')
        {
            if (string.IsNullOrEmpty(pString))
            {
                return new List<string>();
            }
            string[] splitString = pString.Split(pSplit);
            List<string> tResult = new List<string>(splitString.Length);
            for (int i = 0; i < splitString.Length; i++)
            {
                tResult.Add(splitString[i]);
            }
            return tResult;
        }
    }
}
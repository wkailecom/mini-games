using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AddressableTool
{
    [CreateAssetMenu(menuName = "Addressables/自定义分组", fileName = "GroupRulesConfig", order = 0)]
    public class GroupRulesConfig : ScriptableObject
    {
        [SerializeField]
        public List<AddressableGroupData> groupData;
    }


    [Serializable]
    public class AddressableGroupData
    {
        public DefaultAsset searchAsset;
        public GroupType groupType;
        public string searchPattern;
        public SearchOption searchOption;
        public string groupName;

        public AddressableGroupData()
        {
            searchAsset = null;
            groupType = GroupType.withDirectory;
            searchPattern = "*";
            searchOption = SearchOption.TopDirectoryOnly;
            groupName = string.Empty;
        }
    }


    public enum GroupType
    {
        withDirectory,
        withFileName,
    }

}
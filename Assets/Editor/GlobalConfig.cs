using UnityEditor;

[InitializeOnLoad]
public class GlobalConfig
{
    static GlobalConfig()
    {
        PlayerSettings.Android.keystorePass = "zcxadsfg65ads65f";
        PlayerSettings.Android.keyaliasName = "cryptogram";
        PlayerSettings.Android.keyaliasPass = "adsfabcxzvbadsfgaf";
    }
}
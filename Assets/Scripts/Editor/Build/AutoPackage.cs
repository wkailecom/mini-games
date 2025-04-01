//using AppleAuth.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AutoPackage : Editor
{
    private const string SKADNETWORKITEMS = "SKAdNetworkItems";
    private const string SKADNETWORKID = "SKAdNetworkIdentifier";
    private static readonly string[] AD_IDs = new string[] {
"275upjj5gd.skadnetwork",
"294l99pt4k.skadnetwork",
"2fnua5tdw4.skadnetwork",
"2u9pt9hc89.skadnetwork",
"3rd42ekr43.skadnetwork",
"4468km3ulz.skadnetwork",
"44jx6755aq.skadnetwork",
"44n7hlldy6.skadnetwork",
"4fzdc2evr5.skadnetwork",
"4pfyvq9l8r.skadnetwork",
"523jb4fst2.skadnetwork",
"578prtvx9j.skadnetwork",
"5l3tpt7t6e.skadnetwork",
"5lm9lj6jb7.skadnetwork",
"6964rsfnh4.skadnetwork",
"6g9af3uyq4.skadnetwork",
"74b6s63p6l.skadnetwork",
"7rz58n8ntl.skadnetwork",
"7ug5zh24hu.skadnetwork",
"84993kbrcf.skadnetwork",
"8s468mfl3y.skadnetwork",
"9nlqeag3gk.skadnetwork",
"9rd848q2bz.skadnetwork",
"9t245vhmpl.skadnetwork",
"a7xqa6mtl2.skadnetwork",
"c3frkrj4fj.skadnetwork",
"c6k4g5qg8m.skadnetwork",
"cg4yq2srnc.skadnetwork",
"cj5566h2ga.skadnetwork",
"e5fvkxwrpn.skadnetwork",
"ejvt5qm6ak.skadnetwork",
"g28c52eehv.skadnetwork",
"g2y4y55b64.skadnetwork",
"gta9lk7p23.skadnetwork",
"hs6bdukanm.skadnetwork",
"kbd757ywx3.skadnetwork",
"kbmxgpxpgc.skadnetwork",
"klf5c3l5u5.skadnetwork",
"m8dbw4sv7c.skadnetwork",
"mlmmfzh3r3.skadnetwork",
"mtkv5xtk9e.skadnetwork",
"n6fk4nfna4.skadnetwork",
"n9x2a789qt.skadnetwork",
"ppxm28t8ap.skadnetwork",
"prcb7njmu6.skadnetwork",
"pwa73g5rt2.skadnetwork",
"pwdxu55a5a.skadnetwork",
"qqp299437r.skadnetwork",
"r45fhb6rf7.skadnetwork",
"rx5hdcabgc.skadnetwork",
"t38b2kh725.skadnetwork",
"tl55sbb4fm.skadnetwork",
"u679fj5vs4.skadnetwork",
"uw77j35x4d.skadnetwork",
"v72qych5uu.skadnetwork",
"wg4vff78zm.skadnetwork",
"wzmmz9fp6w.skadnetwork",
"yclnxrl5pm.skadnetwork",
"ydx93a7ass.skadnetwork",
"3qcr597p9d.skadnetwork",
"3qy4746246.skadnetwork",
"3sh42y64q3.skadnetwork",
"424m5254lk.skadnetwork",
"4dzt52r2t5.skadnetwork",
"5a6flpkh64.skadnetwork",
"8c4e2ghe7u.skadnetwork",
"av6w8kgt66.skadnetwork",
"cstr6suwn9.skadnetwork",
"f38h382jlk.skadnetwork",
"p78axxw29g.skadnetwork",
"s39g8k73mm.skadnetwork",
"v4nxqhlyqp.skadnetwork",
"zq492l623r.skadnetwork",
"22mmun2rn5.skadnetwork",
"24t9a8vw3c.skadnetwork",
"32z4fx6l9h.skadnetwork",
"3l6bd9hu43.skadnetwork",
"52fl2v3hgk.skadnetwork",
"54nzkqm89y.skadnetwork",
"5tjdwbrq8w.skadnetwork",
"6xzpu9s2p8.skadnetwork",
"79pbpufp6p.skadnetwork",
"9b89h5y424.skadnetwork",
"9yg77x724h.skadnetwork",
"a8cz6cu7e5.skadnetwork",
"dkc879ngq3.skadnetwork",
"feyaarzu9v.skadnetwork",
"ggvn48r87g.skadnetwork",
"glqzh8vgby.skadnetwork",
"k674qkevps.skadnetwork",
"ludvb6z3bs.skadnetwork",
"m5mvw97r93.skadnetwork",
"n66cz3y3bx.skadnetwork",
"nzq8sh4pbs.skadnetwork",
"rvh3l7un93.skadnetwork",
"vcra2ehyfk.skadnetwork",
"x44k69ngh6.skadnetwork",
"x5l83yy675.skadnetwork",
"x8jxxk4ff5.skadnetwork",
"x8uqf25wch.skadnetwork",
"xy9t38ct57.skadnetwork",
"zmvfpc5aq8.skadnetwork",
"4w7y6s5ca2.skadnetwork",
"6p4ks3rnbw.skadnetwork",
"97r2b46745.skadnetwork",
"a2p9lx4jpn.skadnetwork",
"b9bk5wbcq9.skadnetwork",
"bxvub5ada5.skadnetwork",
"dzg6xy7pwj.skadnetwork",
"f73kdq92p3.skadnetwork",
"hdw39hrw9y.skadnetwork",
"krvm3zuq6h.skadnetwork",
"lr83yxwka7.skadnetwork",
"mls7yz5dvl.skadnetwork",
"mp6xlyr22a.skadnetwork",
"s69wq72ugq.skadnetwork",
"su67r6k2v3.skadnetwork",
"w9q455wk68.skadnetwork",
"y45688jllp.skadnetwork",
"n38lu8286q.skadnetwork",
"v9wttpbfk9.skadnetwork",
"252b5q8x7y.skadnetwork",
"9g2aggbj52.skadnetwork",
"488r3q3dtq.skadnetwork",
"6v7lgmsu45.skadnetwork",
"89z7zv988g.skadnetwork",
"8m87ys6875.skadnetwork",
"hb56zgv37p.skadnetwork",
"m297p6643m.skadnetwork",
"238da6jt44.skadnetwork",
"ecpz2srf59.skadnetwork",
"gvmwg8q7h5.skadnetwork",
"pu4na253f3.skadnetwork",
"v79kvwwj4g.skadnetwork",
"yrqqpx2mcb.skadnetwork",
"z4gj7hsk7h.skadnetwork",
"f7s53z58qe.skadnetwork",
"7953jerfzd.skadnetwork",
"7fmhfwg9en.skadnetwork",
"qu637u8glc.skadnetwork",
"x2jnk7ly8j.skadnetwork",
"737z793b9f.skadnetwork",
"bvpn9ufa9b.skadnetwork",
"hjevpa356n.skadnetwork",
"y5ghdn5j9k.skadnetwork",
"24zw6aqk47.skadnetwork",
"47vhws6wlr.skadnetwork",
"4mn522wn87.skadnetwork",
"8r8llnkz5a.skadnetwork",
"9vvzujtq5s.skadnetwork",
"cp8zw746q7.skadnetwork",
"cs644xg564.skadnetwork",
"dbu4b84rxf.skadnetwork",
"eh6m2bh4zr.skadnetwork",
"gta8lk7p23.skadnetwork",
"t6d3zquu66.skadnetwork",
"vutu7akeur.skadnetwork",
"ln5gz23vtd.skadnetwork",
"z959bm4gru.skadnetwork",
"55644vm79v.skadnetwork",
"577p5t736z.skadnetwork",
"6rd35atwn8.skadnetwork",
"6yxyv74ff7.skadnetwork",
"7bxrt786m8.skadnetwork",
"7fbxrn65az.skadnetwork",
"ce8ybjwass.skadnetwork",
"dt3cjx1a9i.skadnetwork",
"fz2k2k5tej.skadnetwork",
"g6gcrrvk4p.skadnetwork",
"h65wbv5k3f.skadnetwork",
"jk2fsx2rgz.skadnetwork",
"k6y4y55b64.skadnetwork",
"mqn7fxpca7.skadnetwork",
"r8lj5b58b5.skadnetwork",
"tmhh9296z4.skadnetwork",
"tvvz7th9br.skadnetwork",
"vhf287vqwu.skadnetwork",
"xga6mpmplv.skadnetwork",
"z24wtl6j62.skadnetwork",
    };

    // Podfile 加海岐代码
    [PostProcessBuild(45)] // between 40 and 50 to ensure that it's not overriden by Podfile generation (40) and that it's added before "pod install" (50)
    public static void OnPostprocessBuild_pod(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
#if UNITY_IOS
            using (StreamWriter sw = File.AppendText(path + "/Podfile"))
            {
                string s = @"post_install do |installer|
  installer.generated_projects.each do |project|
    project.targets.each do |target|
        target.build_configurations.each do |config|
            config.build_settings[""DEVELOPMENT_TEAM""] = ""UTC9MVNF9V""
            config.build_settings[""IPHONEOS_DEPLOYMENT_TARGET""] = ""12.0""
         end
    end
  end
end";
                sw.WriteLine("\n" + s);
            }
#endif
        }
    }

    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
#if UNITY_IOS
            // 海岐的framework
            using (StreamWriter sw = File.AppendText(path + "/Podfile"))
            {
                //in this example I'm adding an app extension
                sw.WriteLine("\ntarget 'UnityFramework' do\n  pod 'WWADFramework', :git => 'git@github.com:WangGeBing/WWADFramework.git', :branch => 'cryptogram'\nend");
            }

            //获得proj文件
            string projectPath = PBXProject.GetPBXProjectPath(path);
            PBXProject xcodeProject = new PBXProject();
            xcodeProject.ReadFromString(File.ReadAllText(projectPath));

            var projectGuid = xcodeProject.ProjectGuid();
            var xcodeTargetGuid = xcodeProject.GetUnityMainTargetGuid();
            var unityTargetGuid = xcodeProject.GetUnityFrameworkTargetGuid();

            //设置Always Embed Swift Standard Libraries
            xcodeProject.SetBuildProperty(projectGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            xcodeProject.SetBuildProperty(xcodeTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            xcodeProject.SetBuildProperty(unityTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            //添加依赖库
            //xcodeProject.AddFrameworkToProject(xcodeTargetGuid, "libz.dylib", true);
            //xcodeProject.AddFrameworkToProject(xcodeTargetGuid, "MessageUI.framework", true);
            xcodeProject.AddFrameworkToProject(xcodeTargetGuid, "UserNotifications.framework", true);
            //xcodeProject.AddFrameworkToProject(xcodeTargetGuid, "OMSDK_Pubmatic.xcframework", true);
            //Add_OMSDK_Pubmatic(xcodeProject);
            xcodeProject.SetBuildProperty(unityTargetGuid, "CLANG_ENABLE_MODULES", "YES");
            //设置关闭bitcode
            xcodeProject.SetBuildProperty(unityTargetGuid, "ENABLE_BITCODE", "NO");
            xcodeProject.SetBuildProperty(xcodeTargetGuid, "ENABLE_BITCODE", "NO");
            //添加苹果自带功能
            // //xcodeProject.AddCapability(xcodeTargetGuid, PBXCapabilityType.GameCenter);
            // xcodeProject.AddCapability(xcodeTargetGuid, PBXCapabilityType.PushNotifications);
            // xcodeProject.AddCapability(xcodeTargetGuid, PBXCapabilityType.BackgroundModes);
            AddCapability(xcodeProject, path);
            //添加苹果登录
            //var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, xcodeTargetGuid);
            //manager.AddSignInWithAppleWithCompatibility(unityTargetGuid);
            //manager.WriteToFile();

            //保存工程
            xcodeProject.WriteToFile(projectPath);

            //修改plist
            var plistPath = Path.Combine(path, "Info.plist");
            PlistDocument plist = new PlistDocument();
            plist.ReadFromString(File.ReadAllText(plistPath));
            var rootDict = plist.root;

            PlistElementArray urlArray = null;
            //推送 Add BackgroundModes 
            if (!rootDict.values.ContainsKey("UIBackgroundModes"))
                urlArray = rootDict.CreateArray("UIBackgroundModes");
            else
                urlArray = rootDict.values["UIBackgroundModes"].AsArray();
            urlArray.values.Clear();
            urlArray.AddString("remote-notification");

            // 语音权限
            //rootDict.SetString("NSMicrophoneUsageDescription", "是否允许此游戏使用麦克风？");
            // 分享
            rootDict.CreateArray("LSApplicationQueriesSchemes");
            rootDict["LSApplicationQueriesSchemes"].AsArray().AddString("fb");
            rootDict["LSApplicationQueriesSchemes"].AsArray().AddString("instagram");
            rootDict["LSApplicationQueriesSchemes"].AsArray().AddString("tumblr");
            rootDict["LSApplicationQueriesSchemes"].AsArray().AddString("twitter");
            // 网络
            rootDict.CreateDict("NSAppTransportSecurity");
            rootDict["NSAppTransportSecurity"].AsDict().SetBoolean("NSAllowsArbitraryLoads", true);
            rootDict["NSAppTransportSecurity"].AsDict().CreateDict("NSExceptionDomains").CreateDict("localhost").SetBoolean("NSExceptionAllowsInsecureHTTPLoads", true);
            // admob sdk 应用id
            rootDict.SetString("GADApplicationIdentifier", "ca-app-pub-7540938745394762~2741880963");
            // max sdk 应用 key
            rootDict.SetString("AppLovinSdkKey", "hBXivm5wUowe1VgTGK0ycdpVQzliyRmIlKTiMt2o4ST8zfgSEgqDDayTVadc1Cvrf8UlImzRuBDQcSahO8e7Tx");

            // Att 授权 文字说明
            rootDict.SetString("NSUserTrackingUsageDescription", "This will only be used to serve more relevant ads");
            // Send SKAN postback copies
            rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://appsflyer-skadnetwork.com/");
            // 广告
            if (!plist.root.values.ContainsKey(SKADNETWORKITEMS))
            {
                plist.root.CreateArray(SKADNETWORKITEMS);
            }

            var skadnetworkItems = plist.root[SKADNETWORKITEMS] as UnityEditor.iOS.Xcode.PlistElementArray;

            foreach (var id in AD_IDs)
            {
                var skadId = skadnetworkItems.AddDict();
                skadId.SetString(SKADNETWORKID, id);
            }

            // 追加URL schemes
            var urlTypeArray = plist.root.CreateArray("CFBundleURLTypes");
            var urlTypeDict = urlTypeArray.AddDict();
            urlTypeDict.SetString("CFBundleURLName", "appsflyer-depplink");
            var urlSchemes = urlTypeDict.CreateArray("CFBundleURLSchemes");
            urlSchemes.AddString("blockguru");

            //保存plist
            plist.WriteToFile(plistPath);
            File.WriteAllText(projectPath, xcodeProject.WriteToString());

            AddxcFramework();
#endif
        }
    }

#if UNITY_IOS
    private static void AddCapability(PBXProject project, string pathToBuiltProject)
    {
        //    string target = project.TargetGuidByName(PBXProject.GetUnityTargetName());
        var target = project.GetUnityMainTargetGuid();

        // Add BackgroundModes And Need to modify info.plist
        project.AddCapability(target, PBXCapabilityType.BackgroundModes);

        //    project.AddCapability(target, PBXCapabilityType.InAppPurchase);

        // Need Create entitlements
        string relativeEntitlementFilePath = "Entitlements.entitlements";
        string absoluteEntitlementFilePath = pathToBuiltProject + "/" + relativeEntitlementFilePath;

        PlistDocument tempEntitlements = new PlistDocument();

        //    string key_KeychainSharing = "keychain-access-groups";
        //    var arr = (tempEntitlements.root[key_KeychainSharing] = new PlistElementArray()) as PlistElementArray;

        //    arr.values.Add(new PlistElementString("$(AppIdentifierPrefix)com.tencent.xxxx"));
        //    arr.values.Add(new PlistElementString("$(AppIdentifierPrefix)com.tencent.wsj.keystoregroup"));

        string key_PushNotifications = "aps-environment";
        tempEntitlements.root[key_PushNotifications] = new PlistElementString("production");

        project.AddCapability(target, PBXCapabilityType.PushNotifications, relativeEntitlementFilePath);
        //    project.AddCapability(target, PBXCapabilityType.KeychainSharing, relativeEntitlementFilePath);

        string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        File.WriteAllText(projPath, project.WriteToString());
        tempEntitlements.WriteToFile(absoluteEntitlementFilePath);

        ModifyEntitlementFile(absoluteEntitlementFilePath);
    }

    private static void ModifyEntitlementFile(string absoluteEntitlementFilePath)
    {
        Debug.Log(absoluteEntitlementFilePath);

        if (!File.Exists(absoluteEntitlementFilePath)) return;
        //    try
        {
            StreamReader reader = new StreamReader(absoluteEntitlementFilePath);
            var content = reader.ReadToEnd().Trim();
            reader.Close();

            var needFindString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            var changeString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + "\n" + "<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">";
            Debug.Log("Before: " + content);
            content = content.Replace(needFindString, changeString);
            Debug.Log("After: " + content);
            StreamWriter writer = new StreamWriter(new FileStream(absoluteEntitlementFilePath, FileMode.Create));
            writer.WriteLine(content);
            writer.Flush();
            writer.Close();
        }
        //    catch (Exception e)
        //    {
        //        Debug.Log("ModifyEntitlementFile - Failed: " + e.Message);
        //    }
    }

    [MenuItem("Build/iOS/AddxcFramework", priority = 2041)]
    private static void AddxcFramework()
    {
        var tBasePath = Directory.GetParent(Application.dataPath).FullName;
        var tBuildPath = Path.Combine(tBasePath, "Build/iOS");
        var tScriptPath = Path.Combine(tBasePath, "AutoBuilder");
        var tName = "AppLovinQualityServiceSetup-ios.rb";
        var sourceFileName = Path.Combine(tScriptPath, tName);
        var destFileName = Path.Combine(tBuildPath, tName);
        if (File.Exists(sourceFileName))
        {
            File.Copy(sourceFileName, destFileName, true);
            RunRubyScript(tName, tBuildPath);
        }
        else
        {
            Debug.LogError("原文件不存在！！" + sourceFileName);
        }

        RunRubyScript("addxcframework.rb", tScriptPath, tBuildPath);
    }

    private static void RunRubyScript(string scriptName, string scriptPath, string argument = null)
    {
        var scriptFilePath = Path.Combine(scriptPath, scriptName);
        if (!File.Exists(scriptFilePath))
        {
            Debug.LogError($"Ruby script '{scriptName}' not found at '{scriptFilePath}'");
            return;
        }

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "ruby";
        startInfo.Arguments = $"{scriptName} {argument}";
        startInfo.WorkingDirectory = scriptPath;
        startInfo.StandardOutputEncoding = Encoding.UTF8;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        using Process process = Process.Start(startInfo);
        using (StreamReader reader = process.StandardOutput)
        {
            string result = reader.ReadToEnd();
            Debug.Log(result);
        }

        using (StreamReader reader = process.StandardError)
        {
            string error = reader.ReadToEnd();
            if (!string.IsNullOrEmpty(error))
            {
                Debug.LogError(error);
            }
        }

        process.WaitForExit();
    }

#endif

    static string shellScript = @"#!/bin/sh
set -e
set -u
set -o pipefail

function on_error {
 echo ""$(realpath -mq ""${0}""):$1: error: Unexpected failure""
}
trap 'on_error $LINENO' ERR

if [ -z ${FRAMEWORKS_FOLDER_PATH+x} ]; then
# If FRAMEWORKS_FOLDER_PATH is not set, then there's nowhere for us to copy
# frameworks to, so exit 0 (signalling the script phase was successful).
 exit 0
fi

echo ""mkdir -p ${CONFIGURATION_BUILD_DIR}/${FRAMEWORKS_FOLDER_PATH}""
mkdir -p ""${CONFIGURATION_BUILD_DIR}/${FRAMEWORKS_FOLDER_PATH}""

COCOAPODS_PARALLEL_CODE_SIGN=""${COCOAPODS_PARALLEL_CODE_SIGN:-false}""
SWIFT_STDLIB_PATH=""${DT_TOOLCHAIN_DIR}/usr/lib/swift/${PLATFORM_NAME}""
BCSYMBOLMAP_DIR=""BCSymbolMaps""


# This protects against multiple targets copying the same framework dependency at the same time. The solution
# was originally proposed here: https://lists.samba.org/archive/rsync/2008-February/020158.html
RSYNC_PROTECT_TMP_FILES=(--filter ""P .*.??????"")

# Copies and strips a vendored framework
install_framework()
{
 if [ -r ""${BUILT_PRODUCTS_DIR}/$1"" ]; then
  local source=""${BUILT_PRODUCTS_DIR}/$1""
 elif [ -r ""${BUILT_PRODUCTS_DIR}/$(basename ""$1"")"" ]; then
  local source=""${BUILT_PRODUCTS_DIR}/$(basename ""$1"")""
 elif [ -r ""$1"" ]; then
  local source=""$1""
 fi

 local destination=""${TARGET_BUILD_DIR}/${FRAMEWORKS_FOLDER_PATH}""

 if [ -L ""${source}"" ]; then
  echo ""Symlinked...""
  source=""$(readlink ""${source}"")""
 fi

 if [ -d ""${source}/${BCSYMBOLMAP_DIR}"" ]; then
# Locate and install any .bcsymbolmaps if present, and remove them from the .framework before the framework is copied
  find ""${source}/${BCSYMBOLMAP_DIR}"" -name ""*.bcsymbolmap""|while read f; do
   echo ""Installing $f""
   install_bcsymbolmap ""$f"" ""$destination""
   rm ""$f""
  done
  rmdir ""${source}/${BCSYMBOLMAP_DIR}""
 fi

# Use filter instead of exclude so missing patterns don't throw errors.
 echo ""rsync --delete -av ""${RSYNC_PROTECT_TMP_FILES[@]}"" --links --filter \""- CVS/\"" --filter \""- .svn/\"" --filter \""- .git/\"" --filter \""- .hg/\"" --filter \""- Headers\"" --filter \""- PrivateHeaders\"" --filter \""- Modules\"" \""${source}\"" \""${destination}\""""
 rsync --delete -av ""${RSYNC_PROTECT_TMP_FILES[@]}"" --links --filter ""- CVS/"" --filter ""- .svn/"" --filter ""- .git/"" --filter ""- .hg/"" --filter ""- Headers"" --filter ""- PrivateHeaders"" --filter ""- Modules"" ""${source}"" ""${destination}""

 local basename
 basename=""$(basename -s .framework ""$1"")""
 binary=""${destination}/${basename}.framework/${basename}""

 if ! [ -r ""$binary"" ]; then
  binary=""${destination}/${basename}""
 elif [ -L ""${binary}"" ]; then
  echo ""Destination binary is symlinked...""
  dirname=""$(dirname ""${binary}"")""
  binary=""${dirname}/$(readlink ""${binary}"")""
 fi

# Strip invalid architectures so ""fat"" simulator / device frameworks work on device
 if [[ ""$(file ""$binary"")"" == *""dynamically linked shared library""* ]]; then
  strip_invalid_archs ""$binary""
 fi

# Resign the code if required by the build settings to avoid unstable apps
 code_sign_if_enabled ""${destination}/$(basename ""$1"")""

# Embed linked Swift runtime libraries. No longer necessary as of Xcode 7.
 if [ ""${XCODE_VERSION_MAJOR}"" -lt 7 ]; then
  local swift_runtime_libs
  swift_runtime_libs=$(xcrun otool -LX ""$binary"" | grep --color=never @rpath/libswift | sed -E s/@rpath\\/\(.+dylib\).*/\\1/g | uniq -u)
  for lib in $swift_runtime_libs; do
   echo ""rsync -auv \""${SWIFT_STDLIB_PATH}/${lib}\"" \""${destination}\""""
   rsync -auv ""${SWIFT_STDLIB_PATH}/${lib}"" ""${destination}""
   code_sign_if_enabled ""${destination}/${lib}""
  done
 fi
}
# Copies and strips a vendored dSYM
install_dsym() {
 local source=""$1""
 warn_missing_arch=${2:-true}
 if [ -r ""$source"" ]; then
# Copy the dSYM into the targets temp dir.
  echo ""rsync --delete -av ""${RSYNC_PROTECT_TMP_FILES[@]}"" --filter \""- CVS/\"" --filter \""- .svn/\"" --filter \""- .git/\"" --filter \""- .hg/\"" --filter \""- Headers\"" --filter \""- PrivateHeaders\"" --filter \""- Modules\"" \""${source}\"" \""${DERIVED_FILES_DIR}\""""
  rsync --delete -av ""${RSYNC_PROTECT_TMP_FILES[@]}"" --filter ""- CVS/"" --filter ""- .svn/"" --filter ""- .git/"" --filter ""- .hg/"" --filter ""- Headers"" --filter ""- PrivateHeaders"" --filter ""- Modules"" ""${source}"" ""${DERIVED_FILES_DIR}""

  local basename
  basename=""$(basename -s .dSYM ""$source"")""
  binary_name=""$(ls ""$source/Contents/Resources/DWARF"")""
  binary=""${DERIVED_FILES_DIR}/${basename}.dSYM/Contents/Resources/DWARF/${binary_name}""

# Strip invalid architectures from the dSYM.
  if [[ ""$(file ""$binary"")"" == *""Mach-O ""*""dSYM companion""* ]]; then
   strip_invalid_archs ""$binary"" ""$warn_missing_arch""
  fi
  if [[ $STRIP_BINARY_RETVAL == 0 ]]; then
# Move the stripped file into its final destination.
   echo ""rsync --delete -av ""${RSYNC_PROTECT_TMP_FILES[@]}"" --links --filter \""- CVS/\"" --filter \""- .svn/\"" --filter \""- .git/\"" --filter \""- .hg/\"" --filter \""- Headers\"" --filter \""- PrivateHeaders\"" --filter \""- Modules\"" \""${DERIVED_FILES_DIR}/${basename}.framework.dSYM\"" \""${DWARF_DSYM_FOLDER_PATH}\""""
   rsync --delete -av ""${RSYNC_PROTECT_TMP_FILES[@]}"" --links --filter ""- CVS/"" --filter ""- .svn/"" --filter ""- .git/"" --filter ""- .hg/"" --filter ""- Headers"" --filter ""- PrivateHeaders"" --filter ""- Modules"" ""${DERIVED_FILES_DIR}/${basename}.dSYM"" ""${DWARF_DSYM_FOLDER_PATH}""
  else
# The dSYM was not stripped at all, in this case touch a fake folder so the input/output paths from Xcode do not reexecute this script because the file is missing.
   mkdir -p ""${DWARF_DSYM_FOLDER_PATH}""
   touch ""${DWARF_DSYM_FOLDER_PATH}/${basename}.dSYM""
  fi
 fi
}

# Used as a return value for each invocation of `strip_invalid_archs` function.
STRIP_BINARY_RETVAL=0

# Strip invalid architectures
strip_invalid_archs() {
 binary=""$1""
 warn_missing_arch=${2:-true}
# Get architectures for current target binary
 binary_archs=""$(lipo -info ""$binary"" | rev | cut -d ':' -f1 | awk '{$1=$1;print}' | rev)""
# Intersect them with the architectures we are building for
 intersected_archs=""$(echo ${ARCHS[@]} ${binary_archs[@]} | tr ' ' '\n' | sort | uniq -d)""
# If there are no archs supported by this binary then warn the user
 if [[ -z ""$intersected_archs"" ]]; then
  if [[ ""$warn_missing_arch"" == ""true"" ]]; then
   echo ""warning: [CP] Vendored binary '$binary' contains architectures ($binary_archs) none of which match the current build architectures ($ARCHS).""
  fi
  STRIP_BINARY_RETVAL=1
  return
 fi
 stripped=""""
 for arch in $binary_archs; do
  if ! [[ ""${ARCHS}"" == *""$arch""* ]]; then
# Strip non-valid architectures in-place
   lipo -remove ""$arch"" -output ""$binary"" ""$binary""
   stripped=""$stripped $arch""
  fi
 done
 if [[ ""$stripped"" ]]; then
  echo ""Stripped $binary of architectures:$stripped""
 fi
 STRIP_BINARY_RETVAL=0
}

# Copies the bcsymbolmap files of a vendored framework
install_bcsymbolmap() {
  local bcsymbolmap_path=""$1""
  local destination=""${BUILT_PRODUCTS_DIR}""
  echo ""rsync --delete -av ""${RSYNC_PROTECT_TMP_FILES[@]}"" --filter ""- CVS/"" --filter ""- .svn/"" --filter ""- .git/"" --filter ""- .hg/"" --filter ""- Headers"" --filter ""- PrivateHeaders"" --filter ""- Modules"" ""${bcsymbolmap_path}"" ""${destination}""""
  rsync --delete -av ""${RSYNC_PROTECT_TMP_FILES[@]}"" --filter ""- CVS/"" --filter ""- .svn/"" --filter ""- .git/"" --filter ""- .hg/"" --filter ""- Headers"" --filter ""- PrivateHeaders"" --filter ""- Modules"" ""${bcsymbolmap_path}"" ""${destination}""
}

# Signs a framework with the provided identity
code_sign_if_enabled() {
 if [ -n ""${EXPANDED_CODE_SIGN_IDENTITY:-}"" -a ""${CODE_SIGNING_REQUIRED:-}"" != ""NO"" -a ""${CODE_SIGNING_ALLOWED}"" != ""NO"" ]; then
# Use the current code_sign_identity
  echo ""Code Signing $1 with Identity ${EXPANDED_CODE_SIGN_IDENTITY_NAME}""
  local code_sign_cmd=""/usr/bin/codesign --force --sign ${EXPANDED_CODE_SIGN_IDENTITY} ${OTHER_CODE_SIGN_FLAGS:-} --preserve-metadata=identifier,entitlements '$1'""

  if [ ""${COCOAPODS_PARALLEL_CODE_SIGN}"" == ""true"" ]; then
   code_sign_cmd=""$code_sign_cmd &""
  fi
  echo ""$code_sign_cmd""
  eval ""$code_sign_cmd""
 fi
}

";

    static string GetInstallShellStr(string dstPath)
    {
        return $@"
if [[ ""$CONFIGURATION"" == ""Debug"" ]]; then
 install_framework ""${{PODS_XCFRAMEWORKS_BUILD_DIR}}/{dstPath}""
fi
if [[ ""$CONFIGURATION"" == ""Release"" ]]; then
 install_framework ""${{PODS_XCFRAMEWORKS_BUILD_DIR}}/{dstPath}""
fi
if [[ ""$CONFIGURATION"" == ""ReleaseForProfiling"" ]]; then
 install_framework ""${{PODS_XCFRAMEWORKS_BUILD_DIR}}/{dstPath}""
fi
if [[ ""$CONFIGURATION"" == ""ReleaseForRunning"" ]]; then
 install_framework ""${{PODS_XCFRAMEWORKS_BUILD_DIR}}/{dstPath}""
fi
if [ ""${{COCOAPODS_PARALLEL_CODE_SIGN}}"" == ""true"" ]; then
 wait
fi
";
    }

    /*
     * 
     if [[ ""$CONFIGURATION"" == ""Debug"" ]]; then
     install_framework ""${PODS_XCFRAMEWORKS_BUILD_DIR}/OpenWrapSDK/OpenWrap/OMSDK_Pubmatic.framework""
    fi
    if [[ ""$CONFIGURATION"" == ""Release"" ]]; then
     install_framework ""${PODS_XCFRAMEWORKS_BUILD_DIR}/OpenWrapSDK/OpenWrap/OMSDK_Pubmatic.framework""
    fi
    if [[ ""$CONFIGURATION"" == ""ReleaseForProfiling"" ]]; then
     install_framework ""${PODS_XCFRAMEWORKS_BUILD_DIR}/OpenWrapSDK/OpenWrap/OMSDK_Pubmatic.framework""
    fi
    if [[ ""$CONFIGURATION"" == ""ReleaseForRunning"" ]]; then
     install_framework ""${PODS_XCFRAMEWORKS_BUILD_DIR}/OpenWrapSDK/OpenWrap/OMSDK_Pubmatic.framework""
    fi
    if [ ""${COCOAPODS_PARALLEL_CODE_SIGN}"" == ""true"" ]; then
     wait
    fi
     */

#if UNITY_IOS
    public static void Add_OMSDK_Pubmatic(PBXProject project)
    {
        string scriptName = "Add_OMSDK_Pubmatic";
        StringBuilder shellStr = new StringBuilder(shellScript);
        //缺失的sdk
        shellStr.Append(GetInstallShellStr("OpenWrapSDK/OpenWrap/OMSDK_Pubmatic.framework"));
        //shellStr.Append(GetInstallShellStr("AmazonPublisherServicesSDK/APS_iOS_SDK-4.7.7/DTBiOSSDK.xcframework"));
        //shellStr.Append(GetInstallShellStr("FBAEMKit/XCFrameworks/FBAEMKit.xcframework"));
        //shellStr.Append(GetInstallShellStr("FBSDKCoreKit_Basics/XCFrameworks/FBSDKCoreKit_Basics.xcframework"));
        //shellStr.Append(GetInstallShellStr("FBSDKCoreKit/XCFrameworks/FBSDKCoreKit.xcframework"));
        //shellStr.Append(GetInstallShellStr("FBSDKGamingServicesKit/XCFrameworks/FBSDKGamingServicesKit.xcframework"));
        //shellStr.Append(GetInstallShellStr("FBSDKLoginKit/XCFrameworks/FBSDKLoginKit.xcframework"));
        //shellStr.Append(GetInstallShellStr("FBSDKShareKit/XCFrameworks/FBSDKShareKit.xcframework"));
        //shellStr.Append(GetInstallShellStr("Fyber_Marketplace_SDK/IASDKCore/IASDKCore.xcframework"));
        //shellStr.Append(GetInstallShellStr("OpenWrapSDK/OpenWrapSDK/OMSDK_Pubmatic.xcframework"));
        //shellStr.Append(GetInstallShellStr("smaato-ios-sdk/vendor/OMSDK_Smaato.xcframework"));
        //shellStr.Append(GetInstallShellStr("Usercentrics/Usercentrics.xcframework"));
        //shellStr.Append(GetInstallShellStr("UsercentricsUI/UsercentricsUI.xcframework"));

        project.AddShellScriptBuildPhase(project.GetUnityMainTargetGuid(), scriptName, "/bin/sh", shellStr.ToString());
    }
#endif

    internal static void CopyAndReplaceDirectory(string srcPath, string dstPath)
    {
        if (Directory.Exists(dstPath))
            Directory.Delete(dstPath);
        if (File.Exists(dstPath))
            File.Delete(dstPath);

        Directory.CreateDirectory(dstPath);

        foreach (var file in Directory.GetFiles(srcPath))
            File.Copy(file, Path.Combine(dstPath, Path.GetFileName(file)));

        foreach (var dir in Directory.GetDirectories(srcPath))
            CopyAndReplaceDirectory(dir, Path.Combine(dstPath, Path.GetFileName(dir)));
    }
}

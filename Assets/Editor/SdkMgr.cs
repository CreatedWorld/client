using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class SdkMgr : EditorWindow
{
    private static SDKPlatform plat;
    /// <summary>
    /// 热更地址
    /// </summary>
    private static string webUrl = "";
    /// <summary>
    /// 登录ip
    /// </summary>
    private static string loginIP = "47.93.35.212";
    /// <summary>
    /// 登录端口
    /// </summary>
    private static string loginPort = "9009";
    /// <summary>
    /// 应用id
    /// </summary>
    private static string bundleIdentifier = "com.quangming.majiang";
    /// <summary>
    /// 版本号
    /// </summary>
    private static string serverVersion = "2.6.14.1";
    /// <summary>
    /// 编译目标
    /// </summary>
    private static int buildTarget = BuildTarget.StandaloneWindows64.GetHashCode();
    /// <summary>
    /// 是否开发版本
    /// </summary>
    private static bool isDevelopment = false;
    /// <summary>
    /// 是否开启性能分享
    /// </summary>
    private static bool isAutoConnect = false;
    /// <summary>
    /// 是否开启调试
    /// </summary>
    private static bool isDebug = false;
    /// <summary>
    /// 是否自动出包
    /// </summary>
    private static bool isAutoBuild = false;
    private delegate void ChangeCallBack();
    private static ChangeCallBack callBack;
    [MenuItem("恩赐方/选择平台/本地", false, 2)]
    public static void CopyLocal()
    {
        plat = SDKPlatform.LOCAL;
        GetConfig(SDKPlatform.LOCAL.GetHashCode());
        callBack = delegate ()
        {
            DirectoryInfo androidFolder = new DirectoryInfo("Assets/Plugins/Android");
            if (androidFolder.Exists)
            {
                androidFolder.Delete(true);
            }
            SetConfig(SDKPlatform.LOCAL.GetHashCode());
            PlayerSettings.keystorePass = "antiphon";
            PlayerSettings.keyaliasPass = "antiphon";
        };
        EditorWindow.GetWindow(typeof(SdkMgr), false, "配置本地SDK");
    }
    [MenuItem("恩赐方/选择平台/微信Android", false, 2)]
    public static void CopyWeiXinAndroid()
    {
        plat = SDKPlatform.ANDROID;
        GetConfig(SDKPlatform.ANDROID.GetHashCode());
        callBack = delegate ()
        {
            DirectoryInfo sdkFolder = new DirectoryInfo("Sdk/Weixin/Android");
            DirectoryInfo androidFolder = new DirectoryInfo("Assets/Plugins/Android");
            if (androidFolder.Exists)
            {
                androidFolder.Delete(true);
            }
            CopyFolder(sdkFolder.FullName, new DirectoryInfo("Assets/Plugins").FullName);
            SetConfig(SDKPlatform.ANDROID.GetHashCode());
            PlayerSettings.keystorePass = "antiphon";
            PlayerSettings.keyaliasPass = "antiphon";
        };
        EditorWindow.GetWindow(typeof(SdkMgr), false, "配置微信Android");
    }

    [MenuItem("恩赐方/选择平台/微信IOS", false, 2)]
    public static void CopyWeiXinIOS()
    {
        plat = SDKPlatform.IOS;
        GetConfig(SDKPlatform.IOS.GetHashCode());
        callBack = delegate ()
        {
            DirectoryInfo androidFolder = new DirectoryInfo("Assets/Plugins/Android");
            if (androidFolder.Exists)
            {
                androidFolder.Delete(true);
            }
            SetConfig(SDKPlatform.IOS.GetHashCode());
            PlayerSettings.keystorePass = "antiphon";
            PlayerSettings.keyaliasPass = "antiphon";
        };
        EditorWindow.GetWindow(typeof(SdkMgr));
        EditorWindow.GetWindow(typeof(SdkMgr), false, "配置微信IOS");
    }

    /// <summary>
    /// 复制文件夹到目标文件夹
    /// </summary>
    /// <param name="strFromPath"></param>
    /// <param name="strToPath"></param>
    private static void CopyFolder(string strFromPath, string strToPath)
    {
        //如果源文件夹不存在，则创建
        if (!Directory.Exists(strFromPath))
        {
            Directory.CreateDirectory(strFromPath);
        }
        //取得要拷贝的文件夹名
        string strFolderName = strFromPath.Substring(strFromPath.LastIndexOf("\\") +
          1, strFromPath.Length - strFromPath.LastIndexOf("\\") - 1);
        //如果目标文件夹中没有源文件夹则在目标文件夹中创建源文件夹
        if (!Directory.Exists(strToPath + "\\" + strFolderName))
        {
            Directory.CreateDirectory(strToPath + "\\" + strFolderName);
        }
        //创建数组保存源文件夹下的文件名
        string[] strFiles = Directory.GetFiles(strFromPath);
        //循环拷贝文件
        for (int i = 0; i < strFiles.Length; i++)
        {
            //取得拷贝的文件名，只取文件名，地址截掉。
            string strFileName = strFiles[i].Substring(strFiles[i].LastIndexOf("\\") + 1, strFiles[i].Length - strFiles[i].LastIndexOf("\\") - 1);
            //开始拷贝文件,true表示覆盖同名文件
            File.Copy(strFiles[i], strToPath + "\\" + strFolderName + "\\" + strFileName, true);
        }
        //创建DirectoryInfo实例
        DirectoryInfo dirInfo = new DirectoryInfo(strFromPath);
        //取得源文件夹下的所有子文件夹名称
        DirectoryInfo[] ZiPath = dirInfo.GetDirectories();
        for (int j = 0; j < ZiPath.Length; j++)
        {
            //获取所有子文件夹名
            string strZiPath = ZiPath[j].ToString();
            //把得到的子文件夹当成新的源文件夹，从头开始新一轮的拷贝
            CopyFolder(strZiPath, strToPath + "\\" + strFolderName);
        }
    }

    /// <summary>
    /// 显示平台设置界面
    /// </summary>
    void OnGUI()
    {
        EditorGUILayout.LabelField("SDK配置", EditorStyles.boldLabel);

        webUrl = EditorGUILayout.TextField("热更地址", webUrl);
        loginIP = EditorGUILayout.TextField("服务器IP", loginIP);
        loginPort = EditorGUILayout.TextField("服务器端口", loginPort);
        bundleIdentifier = EditorGUILayout.TextField("应用id", bundleIdentifier);
        serverVersion = EditorGUILayout.TextField("服务器版本号", serverVersion);
        string[] selectNames = { BuildTarget.StandaloneWindows.ToString(),
            BuildTarget.StandaloneWindows64.ToString(),
            BuildTarget.Android.ToString(),
            BuildTarget.iOS.ToString() };
        int[] selectValues = { BuildTarget.StandaloneWindows.GetHashCode(),
            BuildTarget.StandaloneWindows64.GetHashCode(),
            BuildTarget.Android.GetHashCode(),
            BuildTarget.iOS.GetHashCode() };
        buildTarget = EditorGUILayout.IntPopup("发布平台",buildTarget, selectNames, selectValues);
        isDevelopment = EditorGUILayout.Toggle("是否开发版本", isDevelopment);
        if (isDevelopment)
        {
            isAutoConnect = EditorGUILayout.Toggle("是否性能分析", isAutoConnect);
            isDebug = EditorGUILayout.Toggle("是否调试版本", isDebug);
        }
        else
        {
            isAutoConnect = false;
            isDebug = false;
        }
        isAutoBuild = EditorGUILayout.Toggle("是否自动出包", isAutoBuild);


        if (GUILayout.Button("设置", GUILayout.Width(50)))
        {
            FileInfo scriptFile = new FileInfo("Assets/Scripts/Platform/Global/GlobalData.cs");
            StreamReader reader = new StreamReader(scriptFile.FullName);
            string scriptStr = reader.ReadToEnd();
            reader.Close();
            string webUrlStr = string.Format("    public const string WebUrl = \"{0}\";", webUrl);
            string loginIPStr = string.Format("    public static string LoginServer = \"{0}\";", loginIP);
            string loginPortStr = string.Format("    public static int LoginPort = {0};", loginPort);
            string platformStr = string.Format("    public static SDKPlatform sdkPlatform = SDKPlatform.{0};", plat.ToString());
            string serverVersionStr = string.Format("    public static string VERSIONS = \"{0}\";", serverVersion);
            Regex reg1 = new Regex(@"    public const string WebUrl = .*");
            scriptStr = reg1.Replace(scriptStr, webUrlStr);
            Regex reg2 = new Regex(@"    public static string LoginServer = .*");
            scriptStr = reg2.Replace(scriptStr, loginIPStr);
            Regex reg3 = new Regex(@"    public static int LoginPort = .*");
            scriptStr = reg3.Replace(scriptStr, loginPortStr);
            Regex reg4 = new Regex(@"    public static SDKPlatform sdkPlatform = .*");
            scriptStr = reg4.Replace(scriptStr, platformStr);
            Regex reg5 = new Regex(@"    public static string VERSIONS = .*");
            scriptStr = reg5.Replace(scriptStr, serverVersionStr);
            TextEditor textEditor = new TextEditor();
            textEditor.text = scriptStr;
            textEditor.OnFocus();
            textEditor.Copy();
            FileStream fs = new FileStream("Assets/Scripts/Platform/Global/GlobalData.cs", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(textEditor.text);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
            PlayerSettings.bundleVersion = serverVersion;
            PlayerSettings.bundleIdentifier = bundleIdentifier;
            if (callBack != null)
            {
                callBack();
            }
            AssetDatabase.Refresh();
            string url = Application.dataPath.Replace("/Assets", "/");
            if (buildTarget == BuildTarget.StandaloneWindows.GetHashCode() || buildTarget == BuildTarget.StandaloneWindows64.GetHashCode())
            {
                url += "client/client.exe";
            }
            else if (buildTarget == BuildTarget.Android.GetHashCode() && plat == SDKPlatform.ANDROID)
            {
                url += string.Format("微信登录端{0}.apk",serverVersion);
            }
            else if (buildTarget == BuildTarget.Android.GetHashCode() && plat == SDKPlatform.LOCAL)
            {
                url += string.Format("客户端{0}.apk", serverVersion);
            }
            else if (buildTarget == BuildTarget.iOS.GetHashCode())
            {
                url += "XCode";
            }
            BuildOptions buildOptions = BuildOptions.None;
            if (isDevelopment)
            {
                buildOptions |= BuildOptions.Development;
            }
            if (isDebug)
            {
                buildOptions |= BuildOptions.AllowDebugging;
            }
            if (isAutoConnect)
            {
                buildOptions |= BuildOptions.ConnectWithProfiler;
            }
            if (isAutoBuild)
            {
                BuildPipeline.BuildPlayer(GetBuildScenes(), url, (BuildTarget)buildTarget, buildOptions);
                string folderUrl = url;
                while (folderUrl.IndexOf('/') != -1)
                {
                    folderUrl = folderUrl.Replace('/', '\\');
                }
                System.Diagnostics.Process.Start("Explorer.exe", "/select," + folderUrl);
            }
            Debug.LogError("版本设置完成");
        }
    }

    //在这里找出你当前工程所有的场景文件，假设你只想把部分的scene文件打包 那么这里可以写你的条件判断 总之返回一个字符串数组。
    static string[] GetBuildScenes()
    {
        List<string> names = new List<string>();

        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null)
                continue;
            if (e.enabled)
                names.Add(e.path);
        }
        return names.ToArray();
    }


    /// <summary>
    /// 获取平台设置
    /// </summary>
    /// <param name="platform"></param>
    public static void GetConfig(int platform)
    {
        if (PlayerPrefs.HasKey("PlatForm_" + platform + "_WebUrl"))
        {
            webUrl = PlayerPrefs.GetString("PlatForm_" + platform + "_WebUrl");
        }
        if (PlayerPrefs.HasKey("PlatForm_" + platform + "_LoginIP"))
        {
            loginIP = PlayerPrefs.GetString("PlatForm_" + platform + "_LoginIP");
        }
        if (PlayerPrefs.HasKey("PlatForm_" + platform + "_LoginPort"))
        {
            loginPort = PlayerPrefs.GetString("PlatForm_" + platform + "_LoginPort");
        }
        if (PlayerPrefs.HasKey("PlatForm_" + platform + "_BundleIdentifier"))
        {
            bundleIdentifier = PlayerPrefs.GetString("PlatForm_" + platform + "_BundleIdentifier");
        }
        if (PlayerPrefs.HasKey("PlatForm_" + platform.GetHashCode() + "_ServerVersion"))
        {
            serverVersion = PlayerPrefs.GetString("PlatForm_" + platform + "_ServerVersion");
        }
        if (PlayerPrefs.HasKey("PlatForm_" + platform.GetHashCode() + "_BuildTarget"))
        {
            buildTarget = PlayerPrefs.GetInt("PlatForm_" + platform + "_BuildTarget");
        }
        if (PlayerPrefs.HasKey("PlatForm_" + platform.GetHashCode() + "_IsDebug"))
        {
            isDebug = PlayerPrefs.GetInt("PlatForm_" + platform + "_IsDebug") == 1;
        }
    }

    /// <summary>
    /// 保存平台设置
    /// </summary>
    /// <param name="platform"></param>
    public static void SetConfig(int platform)
    {
        PlayerPrefs.SetString("PlatForm_" + platform + "_WebUrl", webUrl);
        PlayerPrefs.SetString("PlatForm_" + platform + "_LoginIP", loginIP);
        PlayerPrefs.SetString("PlatForm_" + platform + "_LoginPort", loginPort);
        PlayerPrefs.SetString("PlatForm_" + platform + "_BundleIdentifier", bundleIdentifier);
        PlayerPrefs.SetString("PlatForm_" + platform + "_ServerVersion", serverVersion);
        PlayerPrefs.SetInt("PlatForm_" + platform + "_BuildTarget", buildTarget);
        PlayerPrefs.SetInt("PlatForm_" + platform + "_IsDebug", isDebug ? 1 : 0);
    }
}

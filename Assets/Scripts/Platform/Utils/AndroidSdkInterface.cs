using System;
using UnityEngine;

public class AndroidSdkInterface
{
    /**获取udid*/
    public static string getUdid()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject currContent = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaObject teleMgr = currContent.Call<AndroidJavaObject>("getSystemService", "phone");
        return teleMgr.Call<string>("getDeviceId");
    }

    /**获取mac地址*/
    public static string getMac()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject currContent = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaObject wifiMgr = currContent.Call<AndroidJavaObject>("getSystemService", "wifi");
        AndroidJavaObject wifiInfo = wifiMgr.Call<AndroidJavaObject>("getConnectionInfo");
        return wifiInfo.Call<string>("getMacAddress");
    }

    /**获取wifi信号强度*/
    public static int getWifiLevel()
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject currContent = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaObject wifiMgr = currContent.Call<AndroidJavaObject>("getSystemService", "wifi");
        AndroidJavaObject wifiInfo = wifiMgr.Call<AndroidJavaObject>("getConnectionInfo");
        int level = wifiInfo.Call<int>("getRssi");
        return level;
    }

    /// <summary>
    /// 复制内容到剪切板
    /// </summary>
    /// <param name="copyStr"></param>
    public static void CopyToClip(string copyStr)
    {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject currContent = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaObject clipboardMgr = currContent.Call<AndroidJavaObject>("getSystemService", "clipboard");
        AndroidJavaClass clipData = new AndroidJavaClass("android.content.ClipData");
        AndroidJavaObject clipDataResult = clipData.CallStatic<AndroidJavaObject>("newPlainText","data",copyStr);
        clipboardMgr.Call("setPrimaryClip", clipDataResult);
    }

    /// <summary>
    /// 获取版本号
    /// </summary>
    /// <returns></returns>
    public static string GetVersion()
    {
        string versionName = "";
        AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject mainActivity = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject packageManager = mainActivity.Call<AndroidJavaObject>("getPackageManager");
        string packageName = mainActivity.Call<string>("getPackageName");
        AndroidJavaObject packageInfo = packageManager.Call<AndroidJavaObject>("getPackageInfo", packageName, 0);
        versionName = packageInfo.Get<string>("versionName");
        return versionName;
    }

    /// <summary>
    /// 获取手机电量
    /// </summary>
    /// <returns></returns>
    public static int GetElectricity()
    {
        int electricity = -1;
        try
        {
            string CapacityString = System.IO.File.ReadAllText("/sys/class/power_supply/battery/capacity");
            electricity = int.Parse(CapacityString);
        }
        catch (Exception)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject currContent = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
            AndroidJavaObject batteryMgr = currContent.Call<AndroidJavaObject>("getSystemService", "batterymanager");
            electricity = batteryMgr.Call<int>("getIntProperty", 4);
        }        
        return electricity;
    }

    /// <summary>
    /// 下载链接
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static void DownloadFile(string url)
    {
        AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject mainActivity = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass urlClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject urlObj =  urlClass.CallStatic<AndroidJavaObject>("parse",url);
        AndroidJavaObject intentObj = new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW", urlObj);
        mainActivity.Call("startActivity",intentObj);

    }

    /// <summary>
    /// 获取启动参数
    /// </summary>
    public static string GetStartParam()
    {
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            AndroidJavaObject uriData = intent.Call<AndroidJavaObject>("getData");
            string param = uriData.Call<string>("getQuery");
            return param;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// 打开URL地址
    /// </summary>
    /// <param name="url"></param>
    public static void OpenUrl(string url)
    {
        AndroidJavaClass urlClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject urlObj = urlClass.CallStatic<AndroidJavaObject>("parse", url);
        AndroidJavaObject intentObj = new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW", urlObj);
        AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject mainActivity = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("startActivity", intentObj);
    }

    /////////////////////////////////////////////
    //////////////sdk相关接口////////////////////
    //AndroidManifest.xml需要添加启动项目配置
    //<intent-filter>
    //    <action android:name="android.intent.action.VIEW" />

    //    <category android:name="android.intent.category.DEFAULT" />
    //    <category android:name="android.intent.category.BROWSABLE" />

    //    <data
    //        android:host="quanminmajiang"
    //        android:scheme="antiphon" />
    //</intent-filter>
    //
    //
    //onNewIntent时需要回调Unity发送启动参数
    //@Override
    //protected void onNewIntent(Intent intent)
    //{
    //    super.onNewIntent(intent);
    //    if (intent != null && intent.getData() != null)
    //    {
    //        if (intent.getData().getQuery() != null)
    //        {
    //            UnityPlayer.UnitySendMessage("GameMgr", "RespStartParam", intent.getData().getQuery());
    //        }
    //    }
    //}
    //
    //
    //onCreate时需要创建启动页面
    //@Override
    //protected void onCreate(Bundle savedInstanceState)
    //{
    //    super.onCreate(savedInstanceState);
    //    .....
    //
    //    bgView = new ImageView(this);
    //    bgView.setScaleType(ImageView.ScaleType.FIT_CENTER);
    //    bgView.setBackgroundResource(R.drawable.splash_image);
    //    mUnityPlayer.addView(bgView);
    //}
    //public void HideSplash()
    //{
    //    new Handler(Looper.getMainLooper()).post(new Runnable() {
    //        @Override
    //        public void run()
    //{
    //    mUnityPlayer.removeView(bgView);
    //    bgView = null;
    //}
    //});
    //}
    /////////////////////////////////////////////

    /// <summary>
    /// 微信支付
    /// <param name="payJson">微信支付信息JSON对象</param>
    /// 支付结果回调
    /// result 支付结果
    /// UnityPlayer.UnitySendMessage("GameMgr","onPayResult",result)
    /// </summary>
    public static void WeChatPay(string payJson)
    {
        AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject mainActivity = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("weChatPay", payJson);
    }

    /// <summary>
    /// 支付宝支付
    /// <param name="payJson">支付宝支付信息JSON字符串</param>
    /// 支付结果回调
    /// result 支付结果
    /// UnityPlayer.UnitySendMessage("GameMgr","onPayResult",result)
    /// </summary>
    public static void AliPay(string payJson)
    {
        AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject mainActivity = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("aliPay", payJson);
    }


    /// <summary>
    /// 第三方支付接口
    /// <param name="payJson">支付信息JSON字符串</param>
    /// 支付结果回调
    /// result 支付结果
    /// UnityPlayer.UnitySendMessage("GameMgr","onPayResult",result)
    /// </summary>
    public static void FWPay(string payJson)
    {
        AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject mainActivity = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("otherPay", payJson);
    }


    /// <summary>
    /// 隐藏启动画面，应用onCreate时创建一个ImageView作为启动画面，启动完成后调用该接口删除该ImageView
    /// </summary>
    public static void HidenSplash()
    {
        AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject mainActivity = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("HideSplash");
    }

    /// <summary>
    /// 微信登陆，
    /// 登录回调 UnityPlayer.UnitySendMessage("GameMgr","onWeChatLogin",weChatInfo)
    /// weChatInfo为 登录返回Json数据
    /// </summary>
    public static void SendWeiXinLogin()
    {
        Debug.Log("SendWeiXinLogin");
        AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject mainActivity = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("weChatLogin");
    }

    /// <summary>
    /// 微信分享
    /// <param name="text">分享内容:{0}|{1}|{2}形式传入,{0}:分享连接,{1}:分享标题,{2}:分享描述</param>
    /// <param name="isTimeline">分享至朋友圈/好友 true:朋友圈 false:好友</param>
    /// 微信分享结果回调
    /// resp为布尔值 true为分享成功 false为分享失败
    /// UnityPlayer.UnitySendMessage("GameMgr","onShareWeChatResult",resp)
    /// </summary>
    public static void WeiXinShare(string url, string title, string desc, bool isTimeline)
    {
        AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject mainActivity = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("shareWeChat", url, title, desc, isTimeline);
    }

    /// <summary>
    /// 微信分享截屏
    /// </summary>
    /// <param name="screenJpg">分享的图片字节数组</param>
    /// <param name="desc">分享描述</param>
    /// <param name="isTimeline">分享至朋友圈/好友 true:朋友圈 false:好友</param>
    public static void WeiXinShareScreen(string desc, bool isTimeline)
    {
        UIManager.Instance.StartSaveScreen((Texture2D screenShot) => {
            byte[] screenJpg = screenShot.EncodeToJPG();
            AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject mainActivity = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
            mainActivity.Call("shareBitmap", screenJpg, desc, isTimeline);
        });
    }

    /// <summary>
    /// 获取经纬度，
    /// 返回回调 
    /// lng_lat 经纬度 使用逗号分隔，如： 116°E,40°N
    /// UnityPlayer.UnitySendMessage("GameMgr","onGetLngAndLat",lng_lat)
    /// </summary>
    public static void GetLngAndLat()
    {
        AndroidJavaClass androidClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject mainActivity = androidClass.GetStatic<AndroidJavaObject>("currentActivity");
        mainActivity.Call("getLngAndLat");
    }
}

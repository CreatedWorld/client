using System.Runtime.InteropServices;

namespace Platform.Utils
{
    /// <summary>
    /// ios平台接口
    /// </summary>
    class IOSSdkInterface
    {
        /// <summary>
        /// 微信登陆，
        /// 登录回调 UnityPlayer.UnitySendMessage("GameMgr","onWeChatLogin",weChatInfo)
        /// weChatInfo为 登录返回Json数据
        /// </summary>
        [DllImport("__Internal")]
        public static extern void weChatLogin();

        /// <summary>
        /// 微信分享
        /// <param name="text">分享内容:{0}|{1}|{2}形式传入,{0}:分享连接,{1}:分享标题,{2}:分享描述</param>
        /// <param name="isTimeline">分享至朋友圈/好友 true:朋友圈 false:好友</param>
        /// 微信分享结果回调
        /// resp为布尔值 true为分享成功 false为分享失败
        /// UnityPlayer.UnitySendMessage("GameMgr","onShareWeChatResult",resp)
        /// </summary>
        [DllImport("__Internal")]
        public static extern void shareWeChat(string url, string title, string desc, bool isTimeline);

        /// <summary>
        /// 微信分享截屏
        /// </summary>
        /// <param name="imgBase64">图片的base64编码</param>
        /// <param name="isTimeline">分享至朋友圈/好友 true:朋友圈 false:好友</param>
        [DllImport("__Internal")]
        public static extern void shareBitmap(string imgBase64, bool isTimeline);

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        [DllImport("__Internal")]
        public static extern string GetVersion();

        /// <summary>
        /// 第三方支付接口
        /// <param name="payJson">支付信息JSON字符串</param>
        /// 支付结果回调
        /// result 支付结果
        /// UnityPlayer.UnitySendMessage("GameMgr","onPayResult",result)
        /// </summary>
        [DllImport("__Internal")]
        public static extern void otherPay(string payJson);

        /// <summary>
        /// 微信支付
        /// <param name="payJson">微信支付信息JSON对象</param>
        /// 支付结果回调
        /// result 支付结果
        /// UnityPlayer.UnitySendMessage("GameMgr","onPayResult",result)
        /// </summary>
        [DllImport("__Internal")]
        public static extern void weChatPay(string payJson);

        /// <summary>
        /// 支付宝支付
        /// <param name="payJson">支付宝支付信息JSON字符串</param>
        /// 支付结果回调
        /// result 支付结果
        /// UnityPlayer.UnitySendMessage("GameMgr","onPayResult",result)
        /// </summary>
        [DllImport("__Internal")]
        public static extern void aliPay(string payJson);

        /// <summary>
        /// 更新游戏
        /// <param name="url">更新的应用下载地址</param>
        /// </summary>
        [DllImport("__Internal")]
        public static extern void UpdateApp(string url);

        /// <summary>
        /// 获取经纬度，
        /// 返回回调 
        /// lng_lat 经纬度 使用逗号分隔，如： 116°E,40°N
        /// UnityPlayer.UnitySendMessage("GameMgr","onGetLngAndLat",lng_lat)
        /// </summary>
        [DllImport("__Internal")]
        public static extern void GetLngAndLat();
    }
}

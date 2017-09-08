using System.Collections.Generic;
using UnityEngine;
using Platform.Net;
using Platform.Utils;
using Platform.Model.Battle;
using System.Threading;
///
///                             _ooOoo_
///                            o8888888o
///                            88" . "88
///                            (| -_- |)
///                            O\  =  /O
///                         ____/`---'\____
///                       .'  \\|     |//  `.
///                      /  \\|||  :  |||//  \
///                     /  _||||| -:- |||||-  \
///                     |   | \\\  -  /// |   |
///                     | \_|  ''\---/''  |   |
///                     \  .-\__  `-`  ___/-. /
///                   ___`. .'  /--.--\  `. . __
///                ."" '<  `.___\_<|>_/___.'  >'"".
///               | | :  `- \`.;`\ _ /`;.`/ - ` : | |
///               \  \ `-.   \_ __\ /__ _/   .-` /  /
///          ======`-.____`-.___\_____/___.-`____.-'======
///                             `=---='
///          ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
///                     佛祖保佑        永无BUG
///            佛曰:
///                   写字楼里写字间，写字间里程序员；
///                   程序人员写程序，又拿程序换酒钱。
///                   酒醒只在网上坐，酒醉还来网下眠；
///                   酒醉酒醒日复日，网上网下年复年。
///                   但愿老死电脑间，不愿鞠躬老板前；
///                   奔驰宝马贵者趣，公交自行程序员。
///                   别人笑我忒疯癫，我笑自己命太贱；
///                   不见满街漂亮妹，哪个归得程序员？
///
public class GameMgr : MonoBehaviour
{
    private static GameMgr instance;
    public static GameMgr Instance
    {
        get
        {
            return instance;
        }
    }
    public GameObject splaObj;
    /// <summary>
    /// 接收消息池
    /// </summary>
    public ConcurrentLinkedQueue<ReciveMsgVO> ReciveMsgPool = new ConcurrentLinkedQueue<ReciveMsgVO>();
    /// <summary>
    /// 发送消息池
    /// </summary>
    public ConcurrentLinkedQueue<SendMsgVO> SendMsgPool = new ConcurrentLinkedQueue<SendMsgVO>();
    /// <summary>
    /// 消息响应方法代理
    /// </summary>
    /// <param name="bytes"></param>
    public delegate void MsgHandlerFun(byte[] bytes);
    /// <summary>
    /// 消息号映射回调方法字典
    /// </summary>
    private Dictionary<MsgNoS2C, List<MsgHandlerFun>> msgHandleDic;
    /// <summary>
    /// 消息响应方法代理
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="clientIndex"></param>
    public delegate void MsgWithIndexHandlerFun(byte[] bytes,int clientIndex);
    /// <summary>
    /// 消息号映射回调方法字典
    /// </summary>
    private Dictionary<MsgNoS2C, List<MsgWithIndexHandlerFun>> msgWithIndexHandleDic;
    /// <summary>
    /// gui样式
    /// </summary>
    private GUIStyle style;

    public GameMgr()
    {
    }
    void Awake()
    {
        splaObj.SetActive(true);
        style = new GUIStyle();
        style.fontSize = 30;
        style.normal.textColor = Color.yellow;
        this.GameMgrInit();
        Utils.Timer.Instance.Init();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    void Start()
    {
        Application.targetFrameRate = 30;
        ApplicationFacade.Instance.SendNotification(NotificationConstant.COMM_GAMEMGR_INIT);
        gameObject.AddComponent<ClientAIMgr>();
        if (GlobalData.sdkPlatform == SDKPlatform.ANDROID)
        {
            AndroidSdkInterface.HidenSplash();
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, Screen.height - 30, 200, 50), GlobalData.VERSIONS, style);
        if (UIManager.Instance.needSaveScreen)
        {
            UIManager.Instance.SaveScreenTexture();
        }
    }

    /// <summary>
    /// 点击的起始坐标
    /// </summary>
    Vector2 moveStart = Vector2.zero;
    /// <summary>
    /// 解锁步骤
    /// </summary>
    uint moveIndex = 0;
    void Update()
    {
        SendMsgHandler();
        ReciveMsgHandler();
        Utils.Timer.Instance.DoUpdate();
        UIManager.Instance.Update();
        if (Input.GetMouseButtonDown(0))
        {
            moveStart = Input.mousePosition;
            moveIndex = 0;
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                var curPos = Input.mousePosition;
                if (moveIndex == 0 && moveStart.y - curPos.y > 100)
                {
                    moveIndex++;
                    moveStart = curPos;
                }
                if (moveIndex == 1 && curPos.x - moveStart.x > 100)
                {
                    moveIndex++;
                    moveStart = curPos;
                }
                if (moveIndex == 2 && curPos.y - moveStart.y > 100)
                {
                    moveIndex++;
                    var logView = GameObject.Find("UIRoot").transform.Find("LogView").gameObject;
                    logView.SetActive(!logView.activeSelf);
                }
            }
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            DialogMsgVO dialogMsgVO = new DialogMsgVO();
            dialogMsgVO.title = "退出提示";
            dialogMsgVO.content = "是否退出游戏";
            dialogMsgVO.dialogType = DialogType.CONFIRM;
            dialogMsgVO.confirmCallBack = delegate { Application.Quit(); };
            DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
            dialogView.data = dialogMsgVO;
        }
    }

    void FixedUpdate()
    {
        Utils.Timer.Instance.DoFixUpdate();
        UIManager.Instance.FixedUpdate();
    }

    void OnDestroy()
    {
        ApplicationFacade.Instance.RemoveMediator(Mediators.GAMEMGR_MEDIATOR);
        NetMgr.Instance.OnDisable();
    }

    /// <summary>
    /// 发送消息池处理
    /// </summary>
    private void SendMsgHandler()
    {
        while (!SendMsgPool.IsEmpty)
        {
            SendMsgVO sendMsgVO = null;
            SendMsgPool.TryDequeue(out sendMsgVO);
            if (sendMsgVO == null)
            {
                continue;
            }
            if (GlobalData.LoginServer == "127.0.0.1")
            {
                ClientAIMgr.Instance.SendBuff(sendMsgVO.channel, sendMsgVO.msgType, sendMsgVO.tbuff);
            }
            else
            {
                if (NetMgr.Instance.ConnentionDic.ContainsKey(sendMsgVO.socketType))
                {
                    NetMgr.Instance.ConnentionDic[sendMsgVO.socketType].SendBuff(sendMsgVO.channel, sendMsgVO.msgType, sendMsgVO.tbuff, sendMsgVO.offCheckTimeOut);
                }
            }
            ClientAIMgr.Instance.ShowSendMsgLog((MsgNoC2S)sendMsgVO.channel, sendMsgVO.tbuff);
        }
    }

    /// <summary>
    /// 接收消息池处理方法,每帧调用一次
    /// </summary>
    private void ReciveMsgHandler()
    {
        while (!ReciveMsgPool.IsEmpty)
        {
            ReciveMsgVO msg = null;
            ReciveMsgPool.TryDequeue(out msg);
            if (msg == null)
            {
                continue;
            }
            MsgNoS2C msgNo = (MsgNoS2C)msg.channel;
            ClientAIMgr.Instance.ShowMsgLog(string.Format("消息:{0} 消息号:{1} 客户端：{2}", msgNo.ToString(), msg.channel, msg.clientIndex));
            if (msgHandleDic.ContainsKey(msgNo))
            {
                for (int i = 0; i < msgHandleDic[msgNo].Count; i++)
                {
                    try
                    {
                        msgHandleDic[msgNo][i](msg.bytes);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(string.Format("{0} {1}", e.Message, e.StackTrace));
                    }
                }
            }
            if (msgWithIndexHandleDic.ContainsKey(msgNo))
            {
                for (int i = 0; i < msgWithIndexHandleDic[msgNo].Count; i++)
                {
                    try
                    {
                        msgWithIndexHandleDic[msgNo][i](msg.bytes, msg.clientIndex);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(string.Format("{0} {1}", e.Message, e.StackTrace));
                    }
                }
            }
        }        
    }

    private void GameMgrInit()
    {
        instance = this;
        var audioSystem = GameObject.Find("AudioSystem").gameObject;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(Camera.main);
        DontDestroyOnLoad(UIManager.Instance.UiRoot);
        DontDestroyOnLoad(audioSystem);
        msgHandleDic = new Dictionary<MsgNoS2C, List<MsgHandlerFun>>();
        msgWithIndexHandleDic = new Dictionary<MsgNoS2C, List<MsgWithIndexHandlerFun>>();
        ApplicationFacade.Instance.RegisterMediator(new GameMgrMediator(Mediators.GAMEMGR_MEDIATOR, this));
        ResourcesMgr resMgr = ResourcesMgr.Instance;
        this.InitPrefsKey();
    }

    /// <summary>
    /// 添加消息映射响应
    /// </summary>
    /// <param PlayerName="msgNo">消息号</param>
    /// <param PlayerName="msgHandler">消息回调方法</param>
    public void AddMsgHandler(MsgNoS2C msgNo, MsgHandlerFun msgHandler)
    {
        if (!msgHandleDic.ContainsKey(msgNo))
        {
            msgHandleDic.Add(msgNo, new List<MsgHandlerFun>());
        }
        msgHandleDic[msgNo].Add(msgHandler);
    }

    /// <summary>
    /// 添加带序号的消息映射响应
    /// </summary>
    /// <param name="msgNo"></param>
    /// <param name="msgHandler"></param>
    public void AddMsgWithIndex(MsgNoS2C msgNo,MsgWithIndexHandlerFun msgHandler)
    {
        if (!msgWithIndexHandleDic.ContainsKey(msgNo))
        {
            msgWithIndexHandleDic.Add(msgNo, new List<MsgWithIndexHandlerFun>());
        }
        msgWithIndexHandleDic[msgNo].Add(msgHandler);
    }

    /// <summary>
    /// 移除消息映射
    /// </summary>
    /// <param PlayerName="msgNo">消息号</param>
    /// <param PlayerName="msgHandler">消息回调方法</param>
    public void RemoveMsgHandler(MsgNoS2C msgNo, MsgHandlerFun msgHandler)
    {
        if (!msgHandleDic.ContainsKey(msgNo))
        {
            return;
        }
        for (int i = 0; i < msgHandleDic[msgNo].Count; i++)
        {
            if (msgHandleDic[msgNo][i] == msgHandler)
            {
                msgHandleDic[msgNo].Remove(msgHandler);
                return;
            }
        }
    }

    /// <summary>
    /// 移除所有消息监听
    /// </summary>
    public void RemoveAllMsgHandler()
    {
        msgHandleDic.Clear();
    }

    /// <summary>
    /// 初始化本地设置
    /// </summary>
    private void InitPrefsKey()
    {
        if (PlayerPrefs.HasKey(PrefsKey.SERVERIP))
        {
            GlobalData.LoginServer = PlayerPrefs.GetString(PrefsKey.SERVERIP);
        }
        if (PlayerPrefs.HasKey(PrefsKey.SERVERPORT))
        {
            GlobalData.LoginPort = PlayerPrefs.GetInt(PrefsKey.SERVERPORT);
        }
        if (PlayerPrefs.HasKey(PrefsKey.USERNAME))
        {
            GlobalData.UserName = PlayerPrefs.GetString(PrefsKey.USERNAME);
        }
        if (PlayerPrefs.HasKey(PrefsKey.USERMAC))
        {
            GlobalData.UserMac = PlayerPrefs.GetString(PrefsKey.USERMAC);
        }
        if (PlayerPrefs.HasKey(PrefsKey.SOUNDSET))
        {
            GlobalData.AudioVolume = PlayerPrefs.GetFloat(PrefsKey.SOUNDSET);
        }
        if (PlayerPrefs.HasKey(PrefsKey.MUSICSET))
        {
            GlobalData.BGMVolume = PlayerPrefs.GetFloat(PrefsKey.MUSICSET);
        }
    }

    /// <summary>
    /// 返回启动参数
    /// </summary>
    /// <param name="str"></param>
    public void RespStartParam(string str)
    {
        GlobalData.StartUpParam = str;
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_RECIVE_STARTUP);
    }

    /// <summary>
    /// 微信登陆回调
    /// </summary>
    /// <param name="str"></param>
    public void onWeChatLogin(string str)
    {
        WXUserInfo wxUserInfo = JsonUtility.FromJson<WXUserInfo>(str);
        PlayerPrefs.SetString(PrefsKey.USERMAC, wxUserInfo.unionid);
        PlayerPrefs.SetString(PrefsKey.USERNAME, wxUserInfo.nickname);
        PlayerPrefs.SetString(PrefsKey.HEADURL, wxUserInfo.headimgurl);
        PlayerPrefs.SetInt(PrefsKey.SEX, int.Parse(wxUserInfo.sex));
        ApplicationFacade.Instance.SendNotification(NotificationConstant.MEDI_LOGIN_WXLOGINSUCCEED);
    }

    /// <summary>
    /// 充值返回
    /// </summary>
    /// <param name="result"></param>
    public void onPayResult(string result)
    {
        //result = result.ToLower();
        //DialogMsgVO dialogMsgVO = new DialogMsgVO();
        //dialogMsgVO.title = "充值提示";
        //dialogMsgVO.content = result == "true"?"充值成功": "充值失败";
        //dialogMsgVO.dialogType = DialogType.ALERT;
        //DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
        //dialogView.data = dialogMsgVO;
        //NetMgr.Instance.SendBuff(SocketType.HALL, MsgNoC2S.PAY_SUCCESS_C2S.GetHashCode(), 0, new PaySuccessC2S(),true);
    }

    /// <summary>
    /// 分享返回
    /// </summary>
    /// <param name="result"></param>
    public void onShareWeChatResult(string result)
    {
        //result = result.ToLower();
        //DialogMsgVO dialogMsgVO = new DialogMsgVO();
        //dialogMsgVO.title = "分享提示";
        //dialogMsgVO.content = result == "true" ? "分享成功" : "分享失败";
        //dialogMsgVO.dialogType = DialogType.ALERT;
        //DialogView dialogView = UIManager.Instance.ShowUI(UIViewID.DIALOG_VIEW) as DialogView;
        //dialogView.data = dialogMsgVO;
        //NetMgr.Instance.SendBuff(SocketType.HALL, MsgNoC2S.PAY_SUCCESS_C2S.GetHashCode(), 0, new PaySuccessC2S(), true);
    }

    /// <summary>
    /// 分享返回
    /// </summary>
    /// <param name="result"></param>
    public void onGetLngAndLat(string lng_lat)
    {
        //string [] position = lng_lat.Split(',');
        ////////////////////////////////////////////////
        //将自己的经纬度发送给服务器，各个项目自己处理//
        ////////////////////////////////////////////////
    }
}

/// <summary>
/// 消息缓存链表
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConcurrentLinkedQueue<T>
{
　　private class Node
　　{
　　　　internal T Item;
　　　　internal Node Next;

　　　　public Node(T item, Node next)
　　　　{
　　　　　　this.Item = item;
　　　　　　this.Next = next;
　　　　}
　　}

　　private Node _head;
　　private Node _tail;

　　public ConcurrentLinkedQueue()
　　{
　　　　_head = new Node(default(T), null);
　　　　_tail = _head;
　　}

　　public bool IsEmpty
　　{
　　　　get { return (_head.Next == null); }
　　}

　　public void Enqueue(T item)
　　{
　　　　Node newNode = new Node(item, null);
　　　　while (true)
　　　　{
　　　　　　Node curTail = _tail;
　　　　　　Node residue = curTail.Next;

　　　　　　//判断_tail是否被其他process改变
　　　　　　if (curTail == _tail)
　　　　　　{
　　　　　　　　//A 有其他rocess执行C成功，_tail应该指向新的节点
　　　　　　　　if (residue == null)
　　　　　　　　{
　　　　　　　　　　//C 其他process改变了tail节点，需要重新取tail节点
　　　　　　　　　　if (Interlocked.CompareExchange(
　　　　　　　　　　　　ref curTail.Next, newNode, residue) == residue)
　　　　　　　　　　{
　　　　　　　　　　　　//D 尝试修改tail
　　　　　　　　　　　　Interlocked.CompareExchange(ref _tail, newNode, curTail);
　　　　　　　　　　　　return;
　　　　　　　　　　}
　　　　　　　　}
　　　　　　　　else
　　　　　　　　{
　　　　　　　　　　//B 帮助其他线程完成D操作
　　　　　　　　　　Interlocked.CompareExchange(ref _tail, residue, curTail);
　　　　　　　　}
　　　　　　}
　　　　}
　　}

　　public bool TryDequeue(out T result)
　　{
        result = default(T);
        Node curHead;
　　　　Node curTail;
　　　　Node next;
　　　　do
　　　　{
　　　　　　curHead = _head;
　　　　　　curTail = _tail;
　　　　　　next = curHead.Next;
　　　　　　if (curHead == _head)
　　　　　　{
　　　　　　　　if (next == null)　//Queue为空
　　　　　　　　{
　　　　　　　　　　result = default(T);
　　　　　　　　　　return false;
　　　　　　　　}
　　　　　　　　if (curHead == curTail) //Queue处于Enqueue第一个node的过程中
　　　　　　　　{
　　　　　　　　　　//尝试帮助其他Process完成操作
　　　　　　　　　　Interlocked.CompareExchange(ref _tail, next, curTail);
　　　　　　　　}
　　　　　　　　else
　　　　　　　　{
　　　　　　　　　　//取next.Item必须放到CAS之前
　　　　　　　　　　result = next.Item;
　　　　　　　　　　//如果_head没有发生改变，则将_head指向next并退出
　　　　　　　　　　if (Interlocked.CompareExchange(ref _head,
　　　　　　　　　　　　next, curHead) == curHead)
　　　　　　　　　　　　break;
　　　　　　　　}
　　　　　　}
　　　　}
　　　　while (true);
　　　　return true;
　　}
}
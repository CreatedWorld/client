using LZR.Data.NetWork.Client;
using System.Collections.Generic;
using Utils;
/// <summary>
/// 压力测试
/// </summary>
public class PressDebug {
    /// <summary>
    /// 客户端
    /// </summary>
    public Dictionary<int, PressDebugClient> clientDic = new Dictionary<int, PressDebugClient>();
    /// <summary>
    /// 房主客户端
    /// </summary>
    private List<PressDebugClient> roomClientList = new List<PressDebugClient>();
    private static PressDebug mInstance = null;
    /// <summary>
    /// 消息管理类
    /// </summary>
    public static PressDebug Instance
    {
        get
        {
            if (null == mInstance)
            {
                mInstance = new PressDebug();
                return mInstance;
            }
            return mInstance;
        }
    }

    public PressDebug()
    {
        GlobalData.isDebugModel = true;
        Timer.Instance.AddTimer(0.5f, 0, 0.5f, JoinInRoomLoop);
    }

    /// <summary>
    /// 开始压力测试
    /// </summary>
    public void StartTest(int clientNum)
    {
        for (int i = 0; i < clientNum; i++)
        {
            var client = new PressDebugClient(i);
            clientDic.Add(i, client);
        }
    }

    /// <summary>
    /// 添加房间
    /// </summary>
    /// <param name="roomId"></param>
    public void AddRoom(PressDebugClient client)
    {
        roomClientList.Add(client);
    }

    /// <summary>
    /// 定时加入房间
    /// </summary>
    private void JoinInRoomLoop()
    {
        foreach (PressDebugClient client in roomClientList)
        {
            for (int i = 1; i < GlobalData.SIT_NUM; i++)
            {
                NSocket socket = null;
                if (clientDic[client.clientIndex + i].ConnentionDic.TryGetValue(SocketType.HALL,out socket))
                {
                    if (clientDic[client.clientIndex + i].roomCode == null && clientDic[client.clientIndex].roomCode != null)
                    {
                        clientDic[client.clientIndex + i].SendRoomIDMsg(clientDic[client.clientIndex].roomCode);
                    }
                }
            }
        }
    }
}

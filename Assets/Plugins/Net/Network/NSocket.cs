using System;
using System.IO;
using System.Net;
using ProtoBuf;
using UnityEngine;
using System.Collections.Generic;

namespace LZR.Data.NetWork.Client
{
    /// <summary>
    /// NSocket
    /// </summary>
    public sealed class NSocket
    {
        /// <summary>
        /// 客户端序号
        /// </summary>
        private int clientIndex;
        private IPEndPoint mServerAddress = null;
        /// <summary>
        /// 服务器地址
        /// </summary>
        public IPEndPoint ServerAddress
        {
            get
            {
                if (mServerAddress == null)
                    return mServerAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 65520);
                return mServerAddress;
            }
            set
            {
                if (IsConnected)
                    throw new Exception("非法操作!已建立连接,不允许修改服务器地址");
                else
                    mServerAddress = value;
            }
        }

        private Action<string> mConsole = Debug.Log;
        /// <summary>
        /// 输出
        /// </summary>
        public Action<string> Console
        {
            get { return mConsole; }
            set { mConsole = value; }
        }

        private Action<string> mLogError = Debug.LogError;
        /// <summary>
        /// 输出
        /// </summary>
        public Action<string> LogError
        {
            get { return mLogError; }
            set { mLogError = value; }
        }

        private NSocketModel mNSocketModel = null;
        /// <summary>
        /// 连接类
        /// </summary>
        private NSocketModel socketModel
        {
            get
            {
                if (mNSocketModel == null)
                {
                    return mNSocketModel = new NSocketModel(this, clientIndex);
                }
                return mNSocketModel;
            }
            set
            {
                mNSocketModel = value;
            }
        }

        private bool mIsConnected = false;
        /// <summary>
        /// 获取一个Boollean值是否已连接
        /// </summary>
        public bool IsConnected 
        {
            get
            {
                return mIsConnected;
            }
            internal set
            {
                mIsConnected = value;
            }
        }

        private int mMaxReConnectNums = 3;
        /// <summary>
        /// 连接断开时自动重连次数
        /// 为0则无限循环重连直至连接成功
        /// </summary>
        public int MaxReConnectNums
        {
            get
            {
                return mMaxReConnectNums;
            }
            set
            {
                if (IsConnected)
                    throw new Exception("非法操作!已建立连接,不允许修改服务器地址");
                else mMaxReConnectNums = value;
            }
        }

        private Action mOnConnectSuccessful = null;
        /// <summary>
        /// 当连接成功
        /// </summary>
        public Action OnConnectSuccessful
        {
            get { return mOnConnectSuccessful ?? (mOnConnectSuccessful = () => { }); }
            set
            {
                mOnConnectSuccessful = value;
            }
        }

        private Action mOnDisconnect = null;
        /// <summary>
        /// 当连接断开
        /// </summary>
        public Action OnDisconnect
        {
            get { return mOnDisconnect ?? (mOnDisconnect = () => { }); }
            set
            {
                mOnDisconnect = value;
            }
        }

        private Action mOnConnectFailed = null;
        /// <summary>
        /// 当重连失败
        /// </summary>
        public Action OnConnectFailed
        {
            get { return mOnConnectFailed ?? (mOnConnectFailed = () => { }); }
            set
            {
                mOnConnectFailed = value;
            }
        }

        private Action<int, int, byte[],int> mReceiveBuff = null;
        /// <summary>
        /// 接收到新消息
        /// </summary>
        public Action<int, int, byte[],int> OnReceiveBuff
        {
            get { return mReceiveBuff ?? (mReceiveBuff = (i, i1, arg3,clientIndex) => { }); }
            set
            {
                mReceiveBuff = value;
            }
        }

        private Action<int> mOnRquestTimeOut = null;
        /// <summary>
        /// 消息超时回调
        /// </summary>
        public Action<int> OnRquestTimeOut
        {
            get
            {
                if (mOnRquestTimeOut == null)
                    mOnRquestTimeOut = i => {Console("协议ID:"+i+",超时未回应"); };
                return mOnRquestTimeOut;
            }
            set
            {
                mOnRquestTimeOut = value;
            }
        }

        private int mRquestTimeOut = 13;
        /// <summary>
        /// 消息回应超时时间
        /// </summary>
        public int RquestTimeOut
        {
            get { return mRquestTimeOut; }
            set { mRquestTimeOut = value; }
        }

        /// <summary>
        /// 用户ID字典，用于消息编码解码
        /// </summary>
        public static Dictionary<int,int> userIdDic = new Dictionary<int, int>();

        /// <summary>
        /// 房间id，用于消息编码解码
        /// </summary>
        public static Dictionary<int, int> roomIdDic = new Dictionary<int, int>();

        /// <summary>
        /// 更新客户端房间号
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="clientIndex"></param>
        public static void UpdateRoomID(int roomId,int clientIndex=0)
        {
            if (roomIdDic.ContainsKey(clientIndex))
            {
                roomIdDic[clientIndex] = roomId;
            }
            else
            {
                roomIdDic.Add(clientIndex, roomId);
            }
        }

        /// <summary>
        /// 更新客户端房间号
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="clientIndex"></param>
        public static void UpdateUserID(int userId, int clientIndex = 0)
        {
            if (userIdDic.ContainsKey(clientIndex))
            {
                userIdDic[clientIndex] = userId;
            }
            else
            {
                userIdDic.Add(clientIndex, userId);
            }
        }

        /// <summary>
        /// 构造模块
        /// </summary>
        /// <param name="serverAddress">服务器地址</param>
        public NSocket(IPEndPoint serverAddress,int clientIndex)
        {
            if (serverAddress == null)
                throw new Exception("服务器地址为空,无法创建新连接");
            IsConnected = false;
            this.ServerAddress = serverAddress;
            this.clientIndex = clientIndex;
        }

        /// <summary>
        /// 启动该连接
        /// </summary>
        public void StartTcpConnection()
        {
            if (ServerAddress == null)
                throw new Exception("服务器地址为空,无法启动连接");
            if (IsConnected) throw new Exception("已连接,请断开后重试");
            socketModel.TcpConnecting();
        }

        /// <summary>
        /// 停止该连接
        /// </summary>
        public void StopTcpConnection()
        {
            socketModel.Close();
        }

        /// <summary>
        /// 热切换服务器连接
        /// </summary>
        public void HotChangeConnect(IPEndPoint serverAddress)
        {
            StopTcpConnection();
            this.ServerAddress = serverAddress;
            StartTcpConnection();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="channel">通信协议</param>
        /// <param name="type">数据包类型</param>
        /// <param name="tobject">要发送的内容</param>
        /// <param name="OffCheckTimeOut">是否忽略超时</param>
        /// <param name="clientIndex">客户端序号</param>
        public void SendBuff<T>(int channel, int type, T tobject,bool OffCheckTimeOut = false,int clientIndex = 0)
        {
            if (type < 0 || type > 1) throw new Exception("数据类型异常");
            socketModel.AddSendBuff(channel, type, tobject,OffCheckTimeOut, clientIndex);
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="buff">字节流</param>
        /// <returns>T类型</returns>
        public static T DeSerializes<T>(byte[] buff)
        {
            using (MemoryStream ms = new MemoryStream(buff))
            {
                T t = Serializer.Deserialize<T>(ms);
                return t;
            }
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static byte[] Serialize<T>(T t)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serializer.Serialize<T>(ms, t);
                return ms.ToArray();
            }
        }
    }
}

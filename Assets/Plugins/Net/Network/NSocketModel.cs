using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using LZR.Data.TypeConversion;

namespace LZR.Data.NetWork.Client
{
    /// <summary>
    /// 连接模块
    /// </summary>
    internal sealed class NSocketModel
    {
        /// <summary>
        /// 此连接类的Socket
        /// </summary>
        internal Socket socket = null;

        /// <summary>
        /// 此连接类的外部提供类
        /// </summary>
        internal NSocket NSocket = null;

        /// <summary>
        /// 已重连次数
        /// </summary>
        internal int ReConnectingNums = 0;

        /// <summary>
        /// 偏移量
        /// </summary>
        internal int packLength = 0;

        /// <summary>
        /// 包体最小长度
        /// </summary>
        internal const int ConstLength = 20;

        /// <summary>
        /// 临时数据包
        /// </summary>
        internal byte[] TempBuff = new byte[1024];

        /// <summary>
        /// 接收到的数据流缓存池
        /// </summary>
        internal List<byte> ReceiveCache = new List<byte>();

        /// <summary>
        /// 从数据流缓存池中
        /// 取出的数据包队列,
        /// 未进行协议ID与内容分离
        /// </summary>
        internal Queue<byte[]> ReceiveBuffQue = new Queue<byte[]>();

        /// <summary>
        /// 要发送的数据包队列
        /// </summary>
        internal Queue<KeyValuePair<int, byte[]>> SendBuffQue = new Queue<KeyValuePair<int, byte[]>>();

        /// <summary>
        /// 是否允许
        /// 进行重连操作
        /// </summary>
        internal bool DotCanReConnecting = false;

        /// <summary>
        /// 定时器
        /// </summary>
        private Timer timer = null;

        /// <summary>
        /// 已发送的数据包
        /// </summary>
        private Dictionary<int, DateTime> SendChanneled = new Dictionary<int, DateTime>();
        /// <summary>
        /// 客户端序号
        /// </summary>
        private int clientIndex;

        /// <summary>
        /// 构造函数
        /// </summary>
        internal NSocketModel(NSocket nSocket,int clientIndex)
        {
            this.NSocket = nSocket;
            this.clientIndex = clientIndex;
        }

        /// <summary>
        /// TCP连接
        /// </summary>
        internal void TcpConnecting()
        {
            DotCanReConnecting = false;
            socket = new Socket(NSocket.ServerAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            NSocket.Console("开始连接:" + NSocket.ServerAddress);
            socket.BeginConnect(NSocket.ServerAddress, OnConnectCompletion, null);
        }

        /// <summary>
        /// 异步连接回调
        /// </summary>
        private void OnConnectCompletion(IAsyncResult iar)
        {
            if (!socket.Connected)
            {
                NSocket.OnDisconnect();
                return;
            }
            try
            {
                socket.EndConnect(iar);
                NSocket.Console("连接服务器" + NSocket.ServerAddress + "成功");
                NSocket.IsConnected = true;
                timer = new Timer(CheckRquestTimeOut, null, 0, 1000);
                NSocket.OnConnectSuccessful();
                socket.BeginReceive(TempBuff, 0, TempBuff.Length, SocketFlags.None, OnReceiveComplete, null);
            }
            catch (Exception e)
            {
                NSocket.Console("连接服务器" + NSocket.ServerAddress + "失败" + e.Message + e.StackTrace);
                NSocket.IsConnected = false;
                ReConnecting();
            }
        }

        /// <summary>
        /// 重连操作
        /// </summary>
        internal void ReConnecting()
        {
            if (DotCanReConnecting) return;
            if (ReConnectingNums <= NSocket.MaxReConnectNums || NSocket.MaxReConnectNums == 0)
            {
                ++ReConnectingNums;
                NSocket.Console("重连服务器" + NSocket.ServerAddress + "中...");
                TcpConnecting();
                return;
            }
            NSocket.Console("重连服务器" + NSocket.ServerAddress + "失败");
            NSocket.OnConnectFailed();
        }

        /// <summary>
        /// 收到数据包回调
        /// </summary>
        /// <param name="arResult"></param>
        private void OnReceiveComplete(IAsyncResult arResult)
        {
            if (!socket.Connected)
            {
                NSocket.Console("连接断开");
                return;
            }
            try
            {
                int length = socket.EndReceive(arResult);
                //NSocket.Console(string.Format("接收消息长度 {0} 客户端 {1}", length, clientIndex));
                byte[] message = new byte[length];
                Buffer.BlockCopy(TempBuff, 0, message, 0, length);
                ReceiveCache.AddRange(message);
                Queue<byte[]> buffs = DecoderPack();
                while (buffs.Count > 0)
                {
                    ReceiveBuffQue.Enqueue(buffs.Dequeue());
                    msgList = new Queue<byte[]>();
                }
                OnreadMsgThread();
                socket.BeginReceive(TempBuff, 0, TempBuff.Length, SocketFlags.None, OnReceiveComplete, null);
            }
            catch (Exception e)
            {
                NSocket.Console("断开" + NSocket.ServerAddress + "连接" + e.Message + e.StackTrace);
                OnDisconnect();
            }
        }

        /// <summary>
        /// 读取消息线程
        /// </summary>
        private void OnreadMsgThread()
        {
            while (ReceiveBuffQue.Count > 0)
            {
                int channel = 0;
                int type = 0;
                int userId = 0;
                int roomId = 0;
                byte[] msgByte = ReceiveBuffQue.Dequeue();
                byte[] data = DeCoderMsg(msgByte, out channel, out type, out userId, out roomId);
                if (SendChanneled.ContainsKey(channel)) SendChanneled.Remove(channel);
                if (channel == 0)
                {
                    string result = "";
                    for (int i = 0; i < msgByte.Length; i++)
                    {
                        result += " " + msgByte[i];
                    }
                    NSocket.LogError("收到空消息" + result);
                }
                NSocket.OnReceiveBuff(channel, type, data, clientIndex);
            }
        }

        /// <summary>
        /// 将要发送的数据包存入队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">数据包类型0-Protobuf,1-Json</param>
        /// <param name="tobject"></param>
        /// <param name="channel"></param>
        /// <param name="clientIndex">客户端序号</param>
        internal void AddSendBuff<T>(int channel, int type, T tobject, bool OffCheckTimeOut,int clientIndex)
        {
            byte[] tbuff = null;
            switch (type)
            {
                case 0:
                    tbuff = NSocket.Serialize(tobject);
                    break;
                case 1:
                    tbuff = FromString.GetBytes(tobject as string, Encoding.Default);
                    break;
            }
            byte[] buff = Encoder(channel, type, tbuff, clientIndex);
            SendBuffQue.Enqueue(new KeyValuePair<int, byte[]>(OffCheckTimeOut == false ? channel : -1, buff));
            SendBuff();
        }

        /// <summary>
        /// 数据包超时检测
        /// </summary>
        /// <param name="sender"></param>
        private void CheckRquestTimeOut(object sender)
        {
            if (SendChanneled.Count > 0)
            {
                DateTime nowTime = DateTime.Now;
                lock (SendChanneled)
                {
                    List<int> WaitRemoveChannel = new List<int>();
                    foreach (KeyValuePair<int, DateTime> pair in SendChanneled)
                    {
                        TimeSpan timeSpan = nowTime - pair.Value;
                        if (timeSpan.Seconds >= NSocket.RquestTimeOut)
                        {
                            NSocket.OnRquestTimeOut(pair.Key);
                            WaitRemoveChannel.Add(pair.Key);
                        }
                    }
                    foreach (int t in WaitRemoveChannel)
                    {
                        SendChanneled.Remove(t);
                    }
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        private void SendBuff()
        {
            if (SendBuffQue.Count > 0)
            {
                KeyValuePair<int, byte[]> cache = SendBuffQue.Dequeue();
                byte[] data = cache.Value;
                if (data != null) socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, cache.Key > 0 ? cache.Key : -1);
            }
        }


        /// <summary>
        /// 发送数据包结果回调
        /// </summary>
        /// <param name="ar"></param>
        private void SendCallback(IAsyncResult ar)
        {
            ar.AsyncWaitHandle.WaitOne();
            if (ar.IsCompleted)
            {
                int channel = (int)ar.AsyncState;
                if (channel > 0) SendChanneled.Add(channel, DateTime.Now);
                if (SendBuffQue.Count > 0) SendBuff();
            }
            else
            {
                NSocket.Console("发送数据后断开" + NSocket.ServerAddress + "连接");
                OnDisconnect();
            }
        }

        /// <summary>
        /// 当连接断开
        /// </summary>
        private void OnDisconnect()
        {
            timer.Dispose();
            NSocket.IsConnected = false;
            NSocket.OnDisconnect();
        }

        /// <summary>
        /// 未解码完成的消息队列
        /// </summary>
        Queue<byte[]> msgList = new Queue<byte[]>();
        /// <summary>
        /// 消息解码
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        private Queue<byte[]> DecoderPack()
        {
            MemoryStream ms = new MemoryStream(ReceiveCache.ToArray());
            BinaryReader br = new BinaryReader(ms, Encoding.Default);
            ReceiveCache.Clear();
            if (packLength == 0)
            {
                packLength = ReadInt(br.ReadBytes(4)) + ConstLength - 4;
            }
            if (br.BaseStream.Length - br.BaseStream.Position >= packLength)
            {
                //消息
                var buff = br.ReadBytes(packLength);
                packLength = 0;
                msgList.Enqueue(buff);
                ReceiveCache.AddRange(br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position)));
                if (ReceiveCache.Count >= ConstLength)
                {
                    DecoderPack();
                }
            }
            else
            {
                br.BaseStream.Seek(0, SeekOrigin.Current);
                ReceiveCache.AddRange(br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position)));
            }
            br.Close();
            ms.Close();
            if (ms != null)
                ms.Dispose();
            return msgList;
        }

        /// <summary>
        /// 解包
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private byte[] DeCoderMsg(byte[] buff, out int id, out int type, out int userId, out int roomId)
        {
            try
            {
                MemoryStream ms = new MemoryStream(buff);
                BinaryReader br = new BinaryReader(ms);
                id = ReadInt(br.ReadBytes(4));
                type = ReadInt(br.ReadBytes(4));
                userId = ReadInt(br.ReadBytes(4));
                roomId = ReadInt(br.ReadBytes(4));
                byte[] data = br.ReadBytes(buff.Length);
                //NSocket.Console(string.Format("消息解包 {0} 客户端 {1}", id, clientIndex));
                return data;
            }
            catch (Exception e)
            {
                NSocket.Console("数据包写入标志与协议失败:" + e.Message + e.StackTrace);
                id = 0;
                type = 0;
                userId = 0;
                roomId = 0;
                return null;
            }
        }


        /// <summary>
        /// 在数据包头部加入长度标志与协议ID
        /// 前四字节为长度,后四字节为协议ID
        /// 其他后段数据为真实数据包
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="type"></param>
        /// <param name="buff"></param>
        /// <param name="clientIndex">客户端序号</param>
        /// <returns>Bye[]</returns>
        private byte[] Encoder(int channel, int type, byte[] buff,int clientIndex)
        {
            try
            {
                if (buff == null) return null;
                MemoryStream ms = new MemoryStream();
                BinaryWriter bw = new BinaryWriter(ms, Encoding.Default);
                bw.Write(WriterInt(buff.Length));
                bw.Write(WriterInt(channel));
                bw.Write(WriterInt(type));
                bw.Write(WriterInt(NSocket.userIdDic[clientIndex]));
                bw.Write(WriterInt(NSocket.roomIdDic[clientIndex]));
                bw.Write(buff);
                buff = ms.ToArray();
                bw.Close();
                ms.Dispose();
                return buff;
            }
            catch (Exception e)
            {
                NSocket.Console("数据包写入标志与协议失败:" + e);
                throw new Exception("数据包写入标志与协议失败");
            }
        }

        /// <summary>
        /// 读大端序
        /// </summary>
        /// <param name="intbytes"></param>
        /// <returns></returns>
        private int ReadInt(byte[] intbytes)
        {
            Array.Reverse(intbytes);
            return BitConverter.ToInt32(intbytes, 0);
        }

        /// <summary>
        /// 写大端序
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private byte[] WriterInt(int value)
        {
            byte[] bs = BitConverter.GetBytes(value);
            Array.Reverse(bs);
            return bs;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        internal void Close()
        {
            if (timer != null) timer.Dispose();
            DotCanReConnecting = true;
            ReConnectingNums = 0;
            ReceiveCache.Clear();
            ReceiveBuffQue.Clear();
            if (NSocket.IsConnected)
            {
                NSocket.IsConnected = false;
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                NSocket.Console("关闭与" + NSocket.ServerAddress + "的连接");
            }
            else
            {
                //Socket.Disconnect(false);
            }
        }
    }
}

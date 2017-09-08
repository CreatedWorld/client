namespace Platform.Net
{
    /// <summary>
    /// 接收的消息结构
    /// </summary>
    public class ReciveMsgVO
    {
        /// <summary>
        /// 消息号
        /// </summary>
        public int channel;
        /// <summary>
        /// 消息体
        /// </summary>
        public byte[] bytes;
        /// <summary>
        /// 消息体类型0-json,1-class
        /// </summary>
        public int type;
        /// <summary>
        /// 客户端序号
        /// </summary>
        public int clientIndex;
    }

    /// <summary>
    /// 发送消息体
    /// </summary>
    public class SendMsgVO
    {
        /// <summary>
        /// 消息号
        /// </summary>
        public int channel;
        /// <summary>
        /// 消息类型
        /// </summary>
        public int msgType;
        /// <summary>
        /// 消息内容
        /// </summary>
        public object tbuff;
        /// <summary>
        /// 是否标识消息超时
        /// </summary>
        public bool offCheckTimeOut;
        /// <summary>
        /// 客户端序号
        /// </summary>
        public int clientIndex = 0;
        /// <summary>
        /// 连接类型
        /// </summary>
        public SocketType socketType;
    }
}

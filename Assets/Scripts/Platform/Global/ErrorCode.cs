namespace Platform.Global
{
    class ErrorCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public const int SUCCESS = 1;
        /// <summary>
        /// 失败
        /// </summary>
        public const int FAILT = 0;

        /// <summary>
        /// 已经在房间内
        /// </summary>
        public const int ALREADY_IN_ROOM = 1001;
        /// <summary>
        /// 房间不存在
        /// </summary>
        public const int NO_ROOM = 1002;
        /// <summary>
        /// 不在该房间中
        /// </summary>
        public const int NOT_IN_ROOM = 1003;
        /// <summary>
        /// 玩家状态不匹配
        /// </summary>
        public const int USER_STATE_NOT_MATCH = 1004;
        /// <summary>
        /// 已经有玩家在该座位上，不能加入
        /// </summary>
        public const int NO_SEAT_IN_ROOM = 1005;
        /// <summary>
        /// 不允许解散房间
        /// </summary>
        public const int NOT_LIMIT_DISSOLVE_ROOM = 1006;
        /// <summary>
        /// 超过房间允许的局数
        /// </summary>
        public const int OVERFLOW_ROOM_ROUNDS = 1007;
        /// <summary>
        /// 房间已满
        /// </summary>
        public const int OVERFLOW_ROOM_PLAYERS = 1008;
        /// <summary>
        /// 不允许出的牌
        /// </summary>
        public const int FORBIDDEN_CARD = 9999;
    }
}

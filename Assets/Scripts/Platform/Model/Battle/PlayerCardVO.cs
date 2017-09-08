using System.Collections.Generic;

namespace Platform.Model.Battle
{
    /// <summary>
    /// 每步单人的牌面数据
    /// </summary>
    class PlayerCardVO
    {
        /// <summary>
        /// 玩家id
        /// </summary>
        public int userId;
        /// <summary>
        /// 碰杠的牌
        /// </summary>
        public List<PengGangCardVO> pengGangCards = new List<PengGangCardVO>();
        /// <summary>
        /// 手中的牌
        /// </summary>
        public List<int> handCards = new List<int>();
        /// <summary>
        /// 已打出的牌
        /// </summary>
        public List<int> putCards = new List<int>();
        /// <summary>
        /// 摸到的牌
        /// </summary>
        public int getCard;
        /// <summary>
        /// 记录的牌类型
        /// </summary>
        public PlayerCardType[] recordCardType;
    }

    /// <summary>
    /// 玩家手中的牌类型
    /// </summary>
    enum PlayerCardType
    {
        PENGGANG,
        HAND,
        GET,
        PUT
    }
}

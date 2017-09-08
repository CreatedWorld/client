using System.Collections.Generic;

namespace Platform.Model.Battle
{
    /// <summary>
    /// 牌面设置VO
    /// </summary>
    class CardSettingVO
    {
        /// <summary>
        /// 座位号
        /// </summary>
        public int sit = 1;
        /// <summary>
        /// 手牌
        /// </summary>
        public List<int> handCards = new List<int>();
        /// <summary>
        /// 摸到的牌
        /// </summary>
        public int getCard = 0;
        /// <summary>
        /// 玩家是否禁用
        /// </summary>
        public bool isForbit = false;
    }
}

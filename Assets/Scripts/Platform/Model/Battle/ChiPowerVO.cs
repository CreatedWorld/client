using System.Collections.Generic;

namespace Platform.Model.Battle
{
    /// <summary>
    /// 吃牌操作的权重
    /// </summary>
    class ChiPowerVO
    {
        /// <summary>
        /// 可以吃的牌
        /// </summary>
        public List<int> chiCards;
        /// <summary>
        /// 吃的牌
        /// </summary>
        public int chiCard;
        /// <summary>
        /// 禁止出的牌
        /// </summary>
        public List<int> forbitCards;
        /// <summary>
        /// 吃牌权重
        /// </summary>
        public float power;
        public ChiPowerVO(List<int> pChiCards, int pChiCard, List<int> pForbitCards,float pPower)
        {
            chiCards = pChiCards;
            chiCard = pChiCard;
            forbitCards = pForbitCards;
            power = pPower;
        }
    }
}

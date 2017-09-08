namespace Platform.Model.Battle
{
    /// <summary>
    /// 出牌权重VO
    /// </summary>
    class CardPowerVO
    {
        /// <summary>
        /// 牌面值
        /// </summary>
        public int cardValue;
        /// <summary>
        /// 出牌权重
        /// </summary>
        public float power;
        public CardPowerVO(int pCardValue,float pPower)
        {
            cardValue = pCardValue;
            power = pPower;
        }
    }
}

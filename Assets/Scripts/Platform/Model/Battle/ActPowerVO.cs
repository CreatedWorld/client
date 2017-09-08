namespace Platform.Model.Battle
{
    /// <summary>
    /// 动作权重VO
    /// </summary>
    class ActPowerVO
    {
        /// <summary>
        /// 动作
        /// </summary>
        public PlayerActType act;
        /// <summary>
        /// 动作操作的牌
        /// </summary>
        public int actCard;
        /// <summary>
        /// 动作权重
        /// </summary>
        public float power;
        public ActPowerVO(PlayerActType pAct,int pActCard,float pPower)
        {
            act = pAct;
            actCard = pActCard;
            power = pPower;
        }
    }
}

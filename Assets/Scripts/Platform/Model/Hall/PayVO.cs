namespace Platform.Model.Hall
{
    /// <summary>
    /// 支付VO
    /// </summary>
    class PayVO
    {
        /// <summary>
        /// 新用户注册时自动生成，商户id
        /// </summary>
        public string channelId = "1000100020001342";
        /// <summary>
        /// 新用户注册时自动生成，MD5私钥
        /// </summary>
        public string key = "B353A0D9D074451488E639B916EE04D0";
        /// <summary>
        /// 新用户注册完成后 自己添加应用，应用id
        /// </summary>
        public string appId = "3402";
        /// <summary>
        /// 金额
        /// </summary>
        public string money;
        /// <summary>
        /// 应用计费点对应的描述
        /// </summary>
        public string pricePointDec;
        /// <summary>
        /// 道具名称
        /// </summary>
        public string subject;
        /// <summary>
        ///  渠道号，由商户在后台选择
        /// </summary>
        public string qn = "zyap3402_56699_100";
        /// <summary>
        /// 自定义订单号,最大长度200,须保持唯一性
        /// </summary>
        public string outTradeNo;
        /// <summary>
        /// 回调地址
        /// </summary>
        public string return_url = "http://www.quanminyuepaiba.com/Home/Index/openapp";
    }
}

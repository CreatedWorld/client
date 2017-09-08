using Platform.Model;
using Platform.Net;
using Platform.Utils;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 商场列表单项内容
/// </summary>
public class ShopListItem : TableViewItem {
    private bool isInit;
    /// <summary>
    /// 标题
    /// </summary>
    private Text title;
    /// <summary>
    /// 充值金额描述
    /// </summary>
    private Text rechargeBtnTxt;
    /// <summary>
    /// 支付按钮
    /// </summary>
    private Button payBtn;
    /// <summary>
    /// 数据
    /// </summary>
    private Productinfo data;
    public bool IsInit
    {
        get
        {
            return isInit;
        }

        set
        {
            isInit = value;
        }
    }

    public Button PayBtn
    {
        get
        {
            return payBtn;
        }
    }

    public override void Updata(object data)
    {
        if (data == null)
        {
            return;
        }
        base.Updata(data);
        this.data = (Productinfo)data;
        this.title.text = this.data.name;
        this.rechargeBtnTxt.text = string.Format("充值{0}元",this.data.rmb);
    }

    public ShopListItem()
    {
        this.IsInit = false;
    }

    private void Awake()
    {
        if (!IsInit)
        {
            title = this.transform.FindChild("InfoText").GetComponent<Text>();
            rechargeBtnTxt = this.transform.FindChild("RechargeBtn/RechargeBtnTxt").GetComponent<Text>();
            payBtn = this.transform.FindChild("RechargeBtn").GetComponent<Button>();
            payBtn.onClick.AddListener(BuyRoomcard);
        }
    }

    /// <summary>
    /// 进入充值
    /// </summary>
    /// <param name="view"></param>
    private void BuyRoomcard()
    {
        var getOrderInfoC2S = new GetOrderInfoC2S();
        getOrderInfoC2S.id = this.data.id;
        NetMgr.Instance.SendBuff(SocketType.HALL, MsgNoC2S.C2S_CoreHall_OrederInfoC2S.GetHashCode(), 0, getOrderInfoC2S);
    }
}
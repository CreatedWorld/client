using System.Collections.Generic;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections;
using Platform.Net;
using Platform.Model;

public class ShoppingMediator : Mediator, IMediator
{
    public ShoppingMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {

    }

    public ShoppingView View
    {
        get
        {
            return (ShoppingView)ViewComponent;
        }
    }
    public override IList<string> ListNotificationInterests()
    {
        IList<string> list = new List<string>();
        list.Add(NotificationConstant.MEDI_HALL_PRODUCTUPDATE);
        return list;
    }
    public override void HandleNotification(INotification notification)
    {
        switch (notification.Name)
        {
            case NotificationConstant.MEDI_HALL_PRODUCTUPDATE:
                UpdateProductList();
                break;
            default:
                break;
        }
    }
    public override void OnRegister()
    {
        base.OnRegister();
        this.View.ButtonAddListening(this.View.CloseButton,
        () =>
        {
            UIManager.Instance.HideUI(UIViewID.SHOPPING_VIEW);
        });
        //NetMgr.Instance.SendBuff(SocketType.HALL, MsgNoC2S.GET_PRODUCTLIST_C2S.GetHashCode(), 0, new GetProductListC2S());
        
    }
    public override void OnRemove()
    {
        base.OnRemove();
    }

    /// <summary>
    /// 更新商品信息列表
    /// </summary>
    private void UpdateProductList()
    {
        var hallProxy = ApplicationFacade.Instance.RetrieveProxy(Proxys.HALL_PROXY) as HallProxy;
        ArrayList datas = new ArrayList();
        datas.AddRange(hallProxy.productList);
        View.TableView.DataProvider = datas;
    }
    
}


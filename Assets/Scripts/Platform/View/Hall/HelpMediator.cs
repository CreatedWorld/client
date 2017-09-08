using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;

public class HelpMediator : Mediator, IMediator
{
    public HelpMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {

    }

    public HelpView View
    {
        get
        {
            return (HelpView)ViewComponent;
        }
    }

    public override void OnRegister()
    {
        base.OnRegister();
        this.View.ButtonAddListening(this.View.CloseButton, () => 
        {
            UIManager.Instance.HideUI(UIViewID.HELP_VIEW);
        });
    }
    public override void OnRemove()
    {
        base.OnRemove();
    }
}


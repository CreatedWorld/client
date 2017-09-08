using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;

public class ActivityMediator : Mediator, IMediator
{

    public ActivityMediator(string mediatorName, object viewComponent) : base(mediatorName, viewComponent)
    {

    }

    public ActivityView View
    {
        get
        {
            return (ActivityView)ViewComponent;
        }
    }

    public override void OnRegister()
    {
        base.OnRegister();
        this.View.ButtonAddListening(this.View.CloseButton,
        () =>
        {
            UIManager.Instance.HideUI(UIViewID.ACTIVITY_VIEW);
        });
        //绑定事件
        this.View.Information.AddOnClickEvent(
            () =>
            {
                this.View.Information.Show();
                this.View.Contact.Hide();
                this.View.Announcement.Hide();
                this.View.Generalize.Hide();
            });
        this.View.Contact.AddOnClickEvent(
            () =>
            {
                this.View.Information.Hide();
                this.View.Contact.Show();
                this.View.Announcement.Hide();
                this.View.Generalize.Hide();
            });
        this.View.Announcement.AddOnClickEvent(
            () =>
            {
                this.View.Information.Hide();
                this.View.Contact.Hide();
                this.View.Announcement.Show();
                this.View.Generalize.Hide();
            });
        this.View.Generalize.AddOnClickEvent(
            () =>
            {
                this.View.Information.Hide();
                this.View.Contact.Hide();
                this.View.Announcement.Hide();
                this.View.Generalize.Show();
            });
    }
    public override void OnRemove()
    {
        base.OnRemove();
    }
}


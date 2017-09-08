using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using UnityEngine;


public class SettingProxy : Proxy, IProxy
{
    public SettingProxy(string NAME) : base(NAME)
    {
    }
    public override void OnRegister()
    {
    }

    public void HallConnectResponse(byte[] bytes)
    {
    }
}
package com.quangming.majiang.wxapi;


import android.app.Activity;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.support.v7.app.AlertDialog;
import android.util.Log;
import android.widget.Toast;

import com.quangming.majiang.MainActivity;
import com.tencent.mm.opensdk.constants.ConstantsAPI;
import com.tencent.mm.opensdk.modelbase.BaseReq;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.IWXAPIEventHandler;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.unit.sdkinterface.SdkInterface;

public class WXEntryActivity extends Activity implements IWXAPIEventHandler {
    private IWXAPI api;
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        api = WXAPIFactory.createWXAPI(this, MainActivity.APP_ID,false);
        api.registerApp(MainActivity.APP_ID);
        api.handleIntent(getIntent(), this);
        Toast.makeText(this.getApplicationContext(),"回调成功:", Toast.LENGTH_LONG).show();
    }
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        setIntent(intent);
        api.handleIntent(intent, this);//必须调用此句话
    }
    @Override
    public void onReq(BaseReq baseReq) {
        Toast.makeText(this.getApplicationContext(),"回调成功:", Toast.LENGTH_LONG).show();
    }
    @Override
    public void onResp(BaseResp baseResp) {
        if(baseResp.getType() == ConstantsAPI.COMMAND_SENDMESSAGE_TO_WX)
        {
            Toast.makeText(this.getApplicationContext(),"分享成功:", Toast.LENGTH_LONG).show();
        }
        else if(baseResp.getType()==ConstantsAPI.COMMAND_SENDAUTH)//登陆发送广播
        {
            SdkInterface.GetInstance().LoginResult(baseResp);
        }
        this.finish();
    }
}

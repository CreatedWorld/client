package com.unit.sdkinterface;

import android.app.Activity;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.widget.Toast;

import com.alibaba.fastjson.JSON;
import com.fanwei.jubaosdk.common.core.OnPayResultListener;
import com.fanwei.jubaosdk.shell.FWPay;
import com.fanwei.jubaosdk.shell.PayOrder;
import com.quangming.majiang.Bean.WXAccessTokenInfo;
import com.quangming.majiang.MainActivity;

import com.quangming.majiang.R;
import com.tencent.mm.opensdk.modelbase.BaseResp;
import com.tencent.mm.opensdk.modelmsg.SendAuth;
import com.tencent.mm.opensdk.modelmsg.SendMessageToWX;
import com.tencent.mm.opensdk.modelmsg.WXImageObject;
import com.tencent.mm.opensdk.modelmsg.WXMediaMessage;
import com.tencent.mm.opensdk.modelmsg.WXWebpageObject;
import com.tencent.mm.sdk.platformtools.Util;
import com.unity3d.player.UnityPlayer;
import com.zhy.http.okhttp.OkHttpUtils;
import com.zhy.http.okhttp.callback.StringCallback;

import java.io.IOException;

import okhttp3.Call;

//SDK接口类,单例
public class SdkInterface {
    private static SdkInterface instance = null;
    private SdkInterface()
    {
    }
    public static SdkInterface GetInstance()
    {
        if(instance == null)
        {
            instance = new SdkInterface();
        }
        return instance;
    }
    //微信登陆接口
    public void LoginWeiXin() {
        // 判断是否安装了微信客户端
        if (!MainActivity.api.isWXAppInstalled()) {
            Toast.makeText(MainActivity.mainActivity.getApplicationContext(), "您还未安装微信客户端！", Toast.LENGTH_SHORT).show();
            return;
        }
        final SendAuth.Req req = new SendAuth.Req();
        req.scope = "snsapi_userinfo";
        req.state = "antiphon";
        MainActivity.api.sendReq(req);
    }
    //登陆结果
    public void LoginResult(BaseResp baseResp)
    {
        SendAuth.Resp resp = (SendAuth.Resp)baseResp;
        switch (resp.errCode) {
            case BaseResp.ErrCode.ERR_OK:
                //Toast.makeText(MainActivity.mainActivity.getApplicationContext(),"ERR_OK", Toast.LENGTH_LONG).show();
                LoginWeiXinSuccess(resp);
                break;
            case BaseResp.ErrCode.ERR_USER_CANCEL:
                Toast.makeText(MainActivity.mainActivity.getApplicationContext(),"ERR_USER_CANCEL", Toast.LENGTH_LONG).show();
                break;
            case BaseResp.ErrCode.ERR_AUTH_DENIED:
                Toast.makeText(MainActivity.mainActivity.getApplicationContext(),"ERR_AUTH_DENIED", Toast.LENGTH_LONG).show();
                break;
            default:
                break;
        }
        //LoginWeiXinSuccess();
    }
    //用户同意登陆
    public void LoginWeiXinSuccess(SendAuth.Resp resp)
    {
        //发送登陆GET请求
        String url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + MainActivity.APP_ID +
                     "&secret=" + MainActivity.APP_KEY +
                     "&code=" + resp.code +
                     "&grant_type=authorization_code";
        OkHttpUtils.get().url(url).build().execute(new StringCallback() {
            @Override
            public void onError(Call call, Exception e, int i) {
                Toast.makeText(MainActivity.mainActivity.getApplicationContext(),"获取登陆信息失败", Toast.LENGTH_LONG).show();
            }
            //获取登陆信息成功后,发送获取用户信息GET请求
            @Override
            public void onResponse(String resp, int i) {
                GetUserInfo(resp);
            }
        });
    }

    //获取用户信息
    void GetUserInfo(String resp){
        WXAccessTokenInfo tokenInfo = JSON.parseObject(resp, WXAccessTokenInfo.class);
        String url = "https://api.weixin.qq.com/sns/userinfo?access_token=" + tokenInfo.getAccess_token() +
                     "&openid=" + tokenInfo.getOpenid();
        OkHttpUtils.get().url(url).build().execute(new StringCallback() {
            @Override
            public void onError(Call call, Exception e, int i) {
                Toast.makeText(MainActivity.mainActivity.getApplicationContext(),"获取用户信息失败", Toast.LENGTH_LONG).show();
            }
            //获取用户信息成功后回调unity函数
            @Override
            public void onResponse(String resp, int i) {
                UnityPlayer.UnitySendMessage("LoginMgr","RespLoginResult",resp);
            }
        });
    }
    //检测是否有错误
    boolean ValidateSuccess(String resp){
        return (!"errcode".contains(resp) && !"errmsg".contains(resp)) || ("errmsg".contains(resp) && "ok".contains(resp));
    }

    //分享微信朋友圈或好友
    public void Share(String url,String title,String desc,boolean isTimelineCb)
    {
        WXWebpageObject webpage = new WXWebpageObject();
        webpage.webpageUrl = url;
        WXMediaMessage msg = new WXMediaMessage(webpage);
        msg.title = title;
        msg.description = desc;
        Bitmap bmp = BitmapFactory.decodeResource(MainActivity.mainActivity.getResources(), R.drawable.app_icon);
        Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, 128, 128, true);
        bmp.recycle();
        msg.thumbData = Util.bmpToByteArray(thumbBmp, true);

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("webpage");
        req.message = msg;
        req.scene = isTimelineCb ? SendMessageToWX.Req.WXSceneTimeline : SendMessageToWX.Req.WXSceneSession;
        MainActivity.api.sendReq(req);
    }

    //分享图片
    public void ShareBitmap(byte[] imageByte,String desc,boolean isTimelineCb)
    {
        Bitmap bmp = BitmapFactory.decodeByteArray(imageByte, 0, imageByte.length);
        WXImageObject imgObj = new WXImageObject(bmp);

        WXMediaMessage msg = new WXMediaMessage();
        msg.mediaObject = imgObj;
        msg.description = desc;

        Bitmap thumbBmp = Bitmap.createScaledBitmap(bmp, 128, 128, true);
        bmp.recycle();
        msg.thumbData = Util.bmpToByteArray(thumbBmp, true);  //缩略图

        SendMessageToWX.Req req = new SendMessageToWX.Req();
        req.transaction = buildTransaction("img");
        req.message = msg;
        req.scene = isTimelineCb ? SendMessageToWX.Req.WXSceneTimeline : SendMessageToWX.Req.WXSceneSession;
        MainActivity.api.sendReq(req);
    }

    private String buildTransaction(final String type) {
        return (type == null) ? String.valueOf(System.currentTimeMillis()) : type + System.currentTimeMillis();
    }

    public void PayByFW(Activity activity, PayOrder payOrder)
    {
        FWPay.pay(activity,payOrder,0, new OnPayResultListener() {
            @Override
            public void onSuccess(Integer code, String message, String payId) {
                Toast.makeText(MainActivity.mainActivity.getApplicationContext(),"回调成功:" + code + ":" + message + ":" + payId, Toast.LENGTH_LONG).show();
                UnityPlayer.UnitySendMessage("GameMgr","RespPaySuccess","true");
            }
            @Override
            public void onFailed(Integer code, String message, String payId) {
                Toast.makeText(MainActivity.mainActivity.getApplicationContext()," :" + code + ":" + message + ":" + payId, Toast.LENGTH_LONG).show();
                UnityPlayer.UnitySendMessage("GameMgr","RespPayFailed","false");
            }
        });
    }
}

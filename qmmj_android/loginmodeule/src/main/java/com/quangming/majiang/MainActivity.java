package com.quangming.majiang;

import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.util.DisplayMetrics;
import android.view.LayoutInflater;
import android.view.View;
import android.widget.ImageView;

import com.fanwei.jubaosdk.common.util.CommonUtils;
import com.fanwei.jubaosdk.shell.PayOrder;
import com.tencent.mm.opensdk.openapi.IWXAPI;
import com.tencent.mm.opensdk.openapi.WXAPIFactory;
import com.unit.sdkinterface.SdkInterface;
import com.unity3d.player.UnityPlayer;
import com.unity3d.player.UnityPlayerActivity;
import com.zhy.http.okhttp.OkHttpUtils;
import com.zhy.http.okhttp.cookie.CookieJarImpl;
import com.zhy.http.okhttp.cookie.store.PersistentCookieStore;
import com.zhy.http.okhttp.https.HttpsUtils;
import com.zhy.http.okhttp.log.LoggerInterceptor;

import java.util.Timer;
import java.util.TimerTask;
import java.util.concurrent.TimeUnit;

import okhttp3.OkHttpClient;

//安卓启动项
public class MainActivity extends UnityPlayerActivity {
    //APP应用ID
    public static String APP_ID = "wx78b45158e185cb77";
    public static String APP_KEY = "8fef47b55ead83482420628907a1fac1";
    public static MainActivity mainActivity;
    //微信API
    public static IWXAPI api;
    //启动页面
    private ImageView bgView = null;

    //Activity启动项
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        this.InitOkHttp();
        mainActivity = this;
        //根据应用ID创建新的微信API用于与微信交互
        api = WXAPIFactory.createWXAPI(this, APP_ID,true);
        api.registerApp(APP_ID);

        bgView = new ImageView(this);
        bgView.setScaleType(ImageView.ScaleType.FIT_CENTER);
        String bgName="splash_bg";
        int splash_bg = getResources().getIdentifier(bgName, "drawable", getPackageName());
        bgView.setBackgroundResource(R.drawable.splash_image);
        mUnityPlayer.addView(bgView);
    }

    public void HideSplash()
    {
        new Handler(Looper.getMainLooper()).post(new Runnable()
        {
            @Override
            public void run()
            {
                mUnityPlayer.removeView(bgView);
                bgView = null;
            }
        });
    }

    @Override
    protected void onNewIntent(Intent intent)
    {
        super.onNewIntent(intent);
        if(intent != null && intent.getData() != null)
        {
            if(intent.getData().getQuery() != null)
            {
                UnityPlayer.UnitySendMessage("GameMgr","RespStartParam",intent.getData().getQuery());
            }
        }

    }
    //初始化网络模块
    private void InitOkHttp() {
        HttpsUtils.SSLParams sslParams = HttpsUtils.getSslSocketFactory(null, null, null);
        CookieJarImpl cookieJar = new CookieJarImpl(new PersistentCookieStore(getApplicationContext()));
        OkHttpClient okHttpClient = new OkHttpClient.Builder()
                .addInterceptor(new LoggerInterceptor("TAG"))
                .cookieJar(cookieJar)
                .connectTimeout(20000L, TimeUnit.MILLISECONDS)
                .readTimeout(20000L,TimeUnit.MILLISECONDS)
                .writeTimeout(20000L,TimeUnit.MILLISECONDS)
                .sslSocketFactory(sslParams.sSLSocketFactory, sslParams.trustManager)
                //其他配置
                .build();
        OkHttpUtils.initClient(okHttpClient);
    }
    //登陆微信,Unity中回调该函数
    public void OnLoginWeiXin() {
        SdkInterface.GetInstance().LoginWeiXin();
    }

    //微信分享
    public void OnShare(String url,String title,String desc,boolean isTimelineCb) {
        SdkInterface.GetInstance().Share(url,title,desc,isTimelineCb);
    }

    //分享图片
    public void OnShareBitmap(byte[] imageByte,String desc,boolean isTimelineCb)
    {
        SdkInterface.GetInstance().ShareBitmap(imageByte,desc,isTimelineCb);
    }
    //付款宝支付接口
    public void OnPayByFW(String amount,String goodsName,String playerID,String payid,String remark)
    {
        final PayOrder payOrder = new PayOrder()
                //金额(必需)
                .setAmount(amount)
                //商品名称(必需)
                .setGoodsName(goodsName)
                //商户订单号(必需)
                .setPayId(payid)
                //玩家Id(必需)
                .setPlayerId(playerID)
                //测试备注
                .setRemark(remark);
        // 主线程中调用 FWPay.pay 方法
        SdkInterface.GetInstance().PayByFW(this,payOrder);
    }
}

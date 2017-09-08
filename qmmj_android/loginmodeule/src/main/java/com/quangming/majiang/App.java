package com.quangming.majiang;

import android.app.Application;

import com.fanwei.jubaosdk.shell.FWPay;


public class App extends Application {
    public static App instrnce;
    @Override
    public void onCreate() {
        super.onCreate();
        instrnce = this;
        FWPay.init(this, "55265202", false);
    }
}

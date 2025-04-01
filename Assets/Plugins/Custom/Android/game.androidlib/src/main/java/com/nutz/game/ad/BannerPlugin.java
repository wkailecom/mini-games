package com.nutz.game.ad;

import android.app.Activity;
import android.os.Bundle;
import android.util.Log;
import android.view.ViewGroup;
import android.widget.RelativeLayout;

import com.amazon.device.ads.AdError;
import com.amazon.device.ads.DTBAdCallback;
import com.amazon.device.ads.DTBAdRequest;
import com.amazon.device.ads.DTBAdResponse;
import com.amazon.device.ads.DTBAdSize;
import com.applovin.mediation.MaxAd;
import com.applovin.mediation.MaxAdFormat;
import com.applovin.mediation.MaxAdRevenueListener;
import com.applovin.mediation.MaxAdViewAdListener;
import com.applovin.mediation.MaxError;
import com.applovin.mediation.ads.MaxAdView;
import com.applovin.sdk.AppLovinSdk;
import com.applovin.sdk.AppLovinSdkUtils;
import com.google.firebase.analytics.FirebaseAnalytics;

import java.util.Locale;

public class BannerPlugin {
    private static final String TAG = "admob_Banner";
    private final ViewGroup mParent;
    private MaxAdView mAdView;
    //private boolean mIsReady = false;
    private double mValue = 0;
    private String mUnitId = null;
    private final Activity activity;

    public BannerPlugin(Activity activity, ViewGroup parent) {
        mParent = parent;
        this.activity = activity;
        mAdView = new MaxAdView("69457da232a3b3b1", activity);
        //mAdView.setLocalExtraParameter(ALPubMaticOpenWrapConstants.ENABLE_TEST_MODE_KEY, true);
        // 加载成功后，banner会不停自动刷新
        MaxAdViewAdListener mAdListener = new MaxAdViewAdListener() {
            @Override
            public void onAdExpanded(MaxAd ad) {

            }

            @Override
            public void onAdCollapsed(MaxAd ad) {

            }

            @Override
            public void onAdLoaded(MaxAd ad) {
                // 加载成功后，banner会不停自动刷新
                //mIsReady = true;

                Log.d(TAG, "onAdLoaded(): " + ad.getAdUnitId() + " " + ad.getNetworkName());
            }

            @Override
            public void onAdDisplayed(MaxAd ad) {
                MessageChannel channel = new MessageChannel();
                channel.showAd("banner", ad.getNetworkName(), mUnitId, ad.getAdUnitId());
            }

            @Override
            public void onAdHidden(MaxAd ad) {
                MessageChannel channel = new MessageChannel();
                channel.closeAd("banner", ad.getNetworkName(), mUnitId, ad.getAdUnitId());
            }

            @Override
            public void onAdClicked(MaxAd ad) {

            }

            @Override
            public void onAdLoadFailed(String adUnitId, MaxError error) {
                Log.d(TAG, "onAdLoaded(): " + adUnitId + " " + error.getMessage());
            }

            @Override
            public void onAdDisplayFailed(MaxAd ad, MaxError error) {

            }
        };
        mAdView.setListener(mAdListener);

        MaxAdRevenueListener mAdRevenueListener = ad -> {
            Log.d(TAG, "MaxAdRevenueListener: " + ad.getAdUnitId() + " " + ad.getNetworkName());

            double revenue = ad.getRevenue();
            if (revenue < 0) revenue = 0.0;

            mValue = revenue * 1000;
            
            MessageChannel channel = new MessageChannel();
            channel.didPayRevenueForAd("banner", ad.getNetworkName(), mUnitId, ad.getAdUnitId(),
                    AppLovinSdk.getInstance(activity).getConfiguration().getCountryCode(),
                    revenue);
            ReportToFirebase(ad.getNetworkName(), ad.getFormat().toString(), ad.getAdUnitId(), ad.getRevenue());
        };
        mAdView.setRevenueListener(mAdRevenueListener);
    }

    public void load(String unitId) {
        Log.i(TAG, "load: " + unitId);
        mUnitId = unitId;

        int heightPx = AppLovinSdkUtils.dpToPx(activity, 50);
        Log.d(TAG, "load size: " + heightPx);
        RelativeLayout.LayoutParams params = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MATCH_PARENT,
                heightPx);
        params.addRule(RelativeLayout.ALIGN_PARENT_BOTTOM, RelativeLayout.TRUE);
        params.addRule(RelativeLayout.CENTER_HORIZONTAL);
        mAdView.setLayoutParams(params);
        mAdView.setExtraParameter("adaptive_banner", "true");
        mAdView.setExtraParameter("allow_pause_auto_refresh_immediately", "true");
        //mAdView.loadAd();

        loadAmazon();
    }

    private void loadAmazon() {
        String amazonAdSlotId = "4456d1de-63da-4278-bba9-dd6660f6fca5";
        MaxAdFormat adFormat = MaxAdFormat.BANNER;

        // Raw size will be 320x50 for BANNERs on phones, and 728x90 for LEADERs on tablets
        AppLovinSdkUtils.Size rawSize = adFormat.getSize();
        DTBAdSize size = new DTBAdSize( rawSize.getWidth(), rawSize.getHeight(), amazonAdSlotId );

        DTBAdRequest adLoader = new DTBAdRequest();
        adLoader.setSizes( size );
        adLoader.loadAd( new DTBAdCallback()
        {
            @Override
            public void onSuccess(final DTBAdResponse dtbAdResponse)
            {
                Log.d(TAG, "amazon load success");
                // 'adView' is your instance of MaxAdView
                mAdView.setLocalExtraParameter( "amazon_ad_response", dtbAdResponse );
                mAdView.loadAd();
            }

            @Override
            public void onFailure(final AdError adError)
            {
                Log.d(TAG, "amazon load error: " + adError.getMessage());
                // 'adView' is your instance of MaxAdView
                mAdView.setLocalExtraParameter( "amazon_ad_error", adError );
                mAdView.loadAd();
            }
        } );
    }

    public void onResume() {
    }

    public void onPause() {
    }

    public void onDestroy() {
        if (mAdView != null) {
            mAdView.destroy();
            mAdView = null;
        }
    }

    public boolean isReady() {
        return true;//mIsReady;
    }

    public void show() {
        if (mAdView != null && mAdView.getParent() == null) {
            Log.d(TAG, "show()");
            mParent.addView(mAdView);
            mAdView.startAutoRefresh();
        }
    }

    public void hide() {
        if (mAdView != null && mAdView.getParent() != null) {
            Log.d(TAG, "hide()");
            mParent.removeView(mAdView);
            mAdView.stopAutoRefresh();
        }
    }

    public synchronized String getEcpm() {
        return String.format(Locale.ENGLISH, "%.4f", mValue);
    }

    private void ReportToFirebase(String network, String adunit, String format, double revenue) {
        try {
            FirebaseAnalytics analytics = FirebaseAnalytics.getInstance(activity);
            Bundle bundle = new Bundle();
            bundle.putString(FirebaseAnalytics.Param.AD_PLATFORM, "AppLovin");
            bundle.putString(FirebaseAnalytics.Param.AD_SOURCE, network);
            bundle.putString(FirebaseAnalytics.Param.AD_FORMAT, format);
            bundle.putString(FirebaseAnalytics.Param.AD_UNIT_NAME, adunit);
            bundle.putString(FirebaseAnalytics.Param.CURRENCY, "USD");
            bundle.putDouble(FirebaseAnalytics.Param.VALUE, revenue);
            analytics.logEvent(FirebaseAnalytics.Event.AD_IMPRESSION, bundle);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}

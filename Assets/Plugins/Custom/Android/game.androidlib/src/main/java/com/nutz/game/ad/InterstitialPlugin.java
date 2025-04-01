package com.nutz.game.ad;

import android.app.Activity;
import android.os.Bundle;
import android.os.Handler;
import android.util.Log;

import com.amazon.device.ads.AdError;
import com.amazon.device.ads.DTBAdCallback;
import com.amazon.device.ads.DTBAdRequest;
import com.amazon.device.ads.DTBAdResponse;
import com.amazon.device.ads.DTBAdSize;
import com.applovin.mediation.MaxAd;
import com.applovin.mediation.MaxAdListener;
import com.applovin.mediation.MaxAdRevenueListener;
import com.applovin.mediation.MaxError;
import com.applovin.mediation.ads.MaxInterstitialAd;
import com.applovin.sdk.AppLovinSdk;
import com.google.firebase.analytics.FirebaseAnalytics;

import java.util.Locale;
import java.util.concurrent.TimeUnit;

public class InterstitialPlugin {
    private static final String TAG = "admob_Interstitial";

    private final MaxInterstitialAd mInterstitialAd;
    private String mUnitId = null;
    private double mValue = 0;
    private int retryAttempt;
    private Activity activity;

    public InterstitialPlugin(Activity activity) {
        this.activity = activity;
        mInterstitialAd = new MaxInterstitialAd("5558e84a43bd6938", activity);
        //mInterstitialAd.setLocalExtraParameter(ALPubMaticOpenWrapConstants.ENABLE_TEST_MODE_KEY,true);
        // Reset retry attempt
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        MaxAdListener mAdListener = new MaxAdListener() {
            @Override
            public void onAdLoaded(MaxAd ad) {
                Log.d(TAG, "onAdLoaded: " + ad.getAdUnitId() + " " + ad.getNetworkName());
                // Reset retry attempt
                retryAttempt = 0;

                MessageChannel channel = new MessageChannel();
                channel.loadedAd("interstitial", ad.getNetworkName(), mUnitId, ad.getAdUnitId());
            }

            @Override
            public void onAdDisplayed(MaxAd ad) {
                Log.d(TAG, "onAdDisplayed: " + ad.getAdUnitId() + " " + ad.getNetworkName());

                MessageChannel channel = new MessageChannel();
                channel.showAd("interstitial", ad.getNetworkName(), mUnitId, ad.getAdUnitId());
            }

            @Override
            public void onAdHidden(MaxAd ad) {
                Log.d(TAG, "onAdHidden: " + ad.getAdUnitId() + " " + ad.getNetworkName());

                mInterstitialAd.loadAd();

                MessageChannel channel = new MessageChannel();
                channel.closeAd("interstitial", ad.getNetworkName(), mUnitId, ad.getAdUnitId());
            }

            @Override
            public void onAdClicked(MaxAd ad) {

            }

            @Override
            public void onAdLoadFailed(String adUnitId, MaxError error) {
                Log.d(TAG, "onAdLoadFailed: " + adUnitId + " " + error.getMessage());

                retryAttempt++;
                long delayMillis = TimeUnit.SECONDS.toMillis((long) Math.pow(2, Math.min(6, retryAttempt)));
                new Handler().postDelayed(mInterstitialAd::loadAd, delayMillis);
            }

            @Override
            public void onAdDisplayFailed(MaxAd ad, MaxError error) {
                Log.d(TAG, "onAdDisplayFailed: " + ad.getAdUnitId() + " " + ad.getNetworkName() + " " + error.getMessage());

                // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
                mInterstitialAd.loadAd();

                MessageChannel channel = new MessageChannel();
                channel.showAdFail("interstitial", ad.getNetworkName(), mUnitId, ad.getAdUnitId());
            }
        };
        mInterstitialAd.setListener(mAdListener);

        MaxAdRevenueListener mAdRevenueListener = ad -> {
            Log.d(TAG, "MaxAdRevenueListener: " + ad.getAdUnitId() + " " + ad.getNetworkName());

            double revenue = ad.getRevenue();
            if (revenue < 0) revenue = 0.0;

            mValue = revenue * 1000;

            MessageChannel channel = new MessageChannel();
            channel.didPayRevenueForAd("interstitial", ad.getNetworkName(), mUnitId, ad.getAdUnitId(),
                    AppLovinSdk.getInstance(activity).getConfiguration().getCountryCode(),
                    revenue);

            ReportToFirebase(ad.getNetworkName(), ad.getFormat().toString(), ad.getAdUnitId(), ad.getRevenue());
        };
        mInterstitialAd.setRevenueListener(mAdRevenueListener);
    }

    public boolean isReady() {
        return mInterstitialAd.isReady();
    }

    public void load(final String unitId) {
        Log.i(TAG, "load: " + unitId);
        mUnitId = unitId;
        //mInterstitialAd.loadAd();

        loadAmazon();
    }

    private void loadAmazon() {
        DTBAdRequest adLoader = new DTBAdRequest();
        adLoader.setSizes( new DTBAdSize.DTBInterstitialAdSize( "9f05eb6d-5d83-4272-b4dc-21ef05ce9477" ) );
        adLoader.loadAd( new DTBAdCallback()
        {
            @Override
            public void onSuccess(final DTBAdResponse dtbAdResponse)
            {
                Log.d(TAG, "amazon load success");
                // 'interstitialAd' is your instance of MaxInterstitialAd
                mInterstitialAd.setLocalExtraParameter( "amazon_ad_response", dtbAdResponse );
                mInterstitialAd.loadAd();
            }

            @Override
            public void onFailure(final AdError adError)
            {
                Log.d(TAG, "amazon load error: " + adError.getMessage());
                // 'interstitialAd' is your instance of MaxInterstitialAd
                mInterstitialAd.setLocalExtraParameter( "amazon_ad_error", adError );
                mInterstitialAd.loadAd();
            }
        });
    }

    public void show() {
        if (mInterstitialAd.isReady()) {
            Log.i(TAG, "show");
            mInterstitialAd.showAd();
        } else {
            Log.e(TAG, "The interstitial ad wasn't ready yet.");
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

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
import com.applovin.mediation.MaxAdRevenueListener;
import com.applovin.mediation.MaxError;
import com.applovin.mediation.MaxReward;
import com.applovin.mediation.MaxRewardedAdListener;
import com.applovin.mediation.ads.MaxRewardedAd;
import com.applovin.sdk.AppLovinSdk;
import com.google.firebase.analytics.FirebaseAnalytics;

import java.util.Locale;
import java.util.concurrent.TimeUnit;

public class RewardVideoPlugin {
    private static final String TAG = "admob_RewardVideo";
    private static final String OBJ = "Main Camera";
    private String mUnitId = null;
    private final MaxRewardedAd mRewardedAd;
    private double mValue = 0;
    private int retryAttempt;
    private Activity activity;

    public RewardVideoPlugin(Activity activity) {
        this.activity = activity;
        mRewardedAd = MaxRewardedAd.getInstance("7f111652924bae46", activity);
        //mRewardedAd.setLocalExtraParameter(ALPubMaticOpenWrapConstants.ENABLE_TEST_MODE_KEY, true );
        // Reset retry attempt
        // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)
        MaxRewardedAdListener mAdListener = new MaxRewardedAdListener() {
            @Override
            public void onRewardedVideoStarted(MaxAd ad) {

            }

            @Override
            public void onRewardedVideoCompleted(MaxAd ad) {

            }

            @Override
            public void onUserRewarded(MaxAd ad, MaxReward reward) {
                MessageChannel channel = new MessageChannel();
                channel.earnedReward("reward_video", ad.getNetworkName(), mUnitId, ad.getAdUnitId());
            }

            @Override
            public void onAdLoaded(MaxAd ad) {
                Log.d(TAG, "onAdLoaded: " + ad.getAdUnitId() + " " + ad.getNetworkName());
                // Reset retry attempt
                retryAttempt = 0;

                MessageChannel channel = new MessageChannel();
                channel.loadedAd("reward_video", ad.getNetworkName(), mUnitId, ad.getAdUnitId());
            }

            @Override
            public void onAdDisplayed(MaxAd ad) {
                MessageChannel channel = new MessageChannel();
                channel.showAd("reward_video", ad.getNetworkName(), mUnitId, ad.getAdUnitId());
            }

            @Override
            public void onAdHidden(MaxAd ad) {
                Log.d(TAG, "onAdHidden: " + ad.getAdUnitId() + " " + ad.getNetworkName());

                mRewardedAd.loadAd();

                MessageChannel channel = new MessageChannel();
                channel.closeAd("reward_video", ad.getNetworkName(), mUnitId, ad.getAdUnitId());
            }

            @Override
            public void onAdClicked(MaxAd ad) {

            }

            @Override
            public void onAdLoadFailed(String adUnitId, MaxError error) {
                // We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds)
                Log.d(TAG, "onAdLoadFailed: " + adUnitId + " " + error.getMessage());

                retryAttempt++;
                long delayMillis = TimeUnit.SECONDS.toMillis((long) Math.pow(2, Math.min(6, retryAttempt)));
                new Handler().postDelayed(mRewardedAd::loadAd, delayMillis);
            }

            @Override
            public void onAdDisplayFailed(MaxAd ad, MaxError error) {
                Log.d(TAG, "onAdLoadFailed: " + ad.getAdUnitId() + " " + ad.getNetworkName() + " " + error.getMessage());

                mRewardedAd.loadAd();

                MessageChannel channel = new MessageChannel();
                channel.showAdFail("reward_video", ad.getNetworkName(), mUnitId, ad.getAdUnitId());
            }
        };
        mRewardedAd.setListener(mAdListener);

        MaxAdRevenueListener mAdRevenueListener = ad -> {
            Log.d(TAG, "MaxAdRevenueListener: " + ad.getAdUnitId() + " " + ad.getNetworkName());

            double revenue = ad.getRevenue();
            if (revenue < 0) revenue = 0.0;

            mValue = revenue * 1000;

            MessageChannel channel = new MessageChannel();
            channel.didPayRevenueForAd("reward_video", ad.getNetworkName(), mUnitId, ad.getAdUnitId(),
                    AppLovinSdk.getInstance(activity).getConfiguration().getCountryCode(),
                    revenue);

            ReportToFirebase(ad.getNetworkName(), ad.getFormat().toString(), ad.getAdUnitId(), ad.getRevenue());
        };
        mRewardedAd.setRevenueListener(mAdRevenueListener);
    }

    public void load(String unitId) {
        Log.i(TAG, "load: " + unitId);
        mUnitId = unitId;
        //mRewardedAd.loadAd();

        loadAmazon();
    }

    private void loadAmazon() {
        DTBAdRequest adLoader = new DTBAdRequest();

        // Switch video player width and height values(320, 480) depending on device orientation
        adLoader.setSizes( new DTBAdSize.DTBVideo( 320, 480, "faaab6a0-c5bc-44dd-94b0-ca4036a785d6" ) );
        adLoader.loadAd( new DTBAdCallback()
        {
            @Override
            public void onSuccess(final DTBAdResponse dtbAdResponse)
            {
                // 'rewardedAd' is your instance of MaxRewardedAd
                mRewardedAd.setLocalExtraParameter( "amazon_ad_response", dtbAdResponse );
                mRewardedAd.loadAd();
            }

            @Override
            public void onFailure(final AdError adError)
            {
                // 'rewardedAd' is your instance of MaxRewardedAd
                mRewardedAd.setLocalExtraParameter( "amazon_ad_error", adError );
                mRewardedAd.loadAd();
            }
        } );
    }

    public boolean isReady() {
        return mRewardedAd.isReady();
    }

    public void show() {
        if (mRewardedAd.isReady()) {
            Log.i(TAG, "show");
            mRewardedAd.showAd();
        } else {
            // 广告失效以后需要重新加载
            Log.e(TAG, "show: exception");
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

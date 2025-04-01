package com.nutz.game.ad;

import android.app.Activity;
import android.util.Log;

import com.google.android.ump.ConsentDebugSettings;
import com.google.android.ump.ConsentInformation;
import com.google.android.ump.ConsentRequestParameters;
import com.google.android.ump.UserMessagingPlatform;

import java.util.concurrent.atomic.AtomicBoolean;

//https://developers.google.com/admob/android/privacy?hl=zh-cn
public class UMP {
    public interface InitListener {
        void onInit();
    }

    private final String TAG = "UMP";
    private ConsentInformation consentInformation;
    // Use an atomic boolean to initialize the Google Mobile Ads SDK and load ads once.
    private final AtomicBoolean isMobileAdsInitializeCalled = new AtomicBoolean(false);

    public void start(Activity activity, InitListener listener) {
        ConsentDebugSettings debugSettings = new ConsentDebugSettings.Builder(activity)
                .setDebugGeography(ConsentDebugSettings.DebugGeography.DEBUG_GEOGRAPHY_EEA)
                .addTestDeviceHashedId("C0FB8EC6450B0229CB906AD2CE6AF1C0")
                .build();


        // Set tag for under age of consent. false means users are not under age
        // of consent.
        ConsentRequestParameters params = new ConsentRequestParameters
                .Builder()
                //.setConsentDebugSettings(debugSettings)
                //.setTagForUnderAgeOfConsent(false)
                .build();

        consentInformation = UserMessagingPlatform.getConsentInformation(activity);
        consentInformation.requestConsentInfoUpdate(
                activity,
                params,
                () -> {
                    UserMessagingPlatform.loadAndShowConsentFormIfRequired(activity, loadAndShowError -> {
                        if (loadAndShowError != null) {
                            Log.w(TAG, String.format("%s: %s",
                                    loadAndShowError.getErrorCode(),
                                    loadAndShowError.getMessage()));
                            LogStream.getInstance().sendLog("ump_load_err", String.format("%s: %s",
                                    loadAndShowError.getErrorCode(),
                                    loadAndShowError.getMessage()));
                        }

                        if (consentInformation.canRequestAds()) {
                            if (isMobileAdsInitializeCalled.getAndSet(true)) {
                                listener.onInit();
                            }
                        } else {
                            LogStream.getInstance().sendLog("ump_request_ad", "no consent");
                        }
                    });
                },
                requestConsentError -> {
                    // Consent gathering failed.
                    Log.w(TAG, String.format("%s: %s",
                            requestConsentError.getErrorCode(),
                            requestConsentError.getMessage()));
                    LogStream.getInstance().sendLog("ump_request_err", String.format("%s: %s",
                            requestConsentError.getErrorCode(),
                            requestConsentError.getMessage()));
                });

        // Check if you can initialize the Google Mobile Ads SDK in parallel
        // while checking for new consent information. Consent obtained in
        // the previous session can be used to request ads.
        if (consentInformation.canRequestAds()) {
            if (isMobileAdsInitializeCalled.getAndSet(true)) {
                listener.onInit();
            }
        } else {
            LogStream.getInstance().sendLog("ump_request_ad", "can init ad in parallel");
        }
    }

    public void reset() {
        if (consentInformation != null)
            consentInformation.reset();
    }
}


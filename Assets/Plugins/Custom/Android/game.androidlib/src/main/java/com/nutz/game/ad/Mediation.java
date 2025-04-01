package com.nutz.game.ad;

import android.app.Activity;
import android.content.Context;
import android.content.SharedPreferences;
import android.net.Uri;
import android.os.Build;
import android.os.LocaleList;
import android.provider.Settings;
import android.text.TextUtils;
import android.util.Log;
import android.view.ViewGroup;

import com.amazon.device.ads.AdRegistration;
import com.amazon.device.ads.DTBAdNetwork;
import com.amazon.device.ads.DTBAdNetworkInfo;
import com.amazon.device.ads.MRAIDPolicy;
import com.applovin.sdk.AppLovinMediationProvider;
import com.applovin.sdk.AppLovinSdk;
import com.applovin.sdk.AppLovinSdkSettings;
import com.google.android.gms.ads.identifier.AdvertisingIdClient;
import com.pubmatic.sdk.common.OpenWrapSDK;
import com.pubmatic.sdk.common.models.POBApplicationInfo;

import java.net.MalformedURLException;
import java.net.URL;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Locale;
import java.util.UUID;

// https://dev.mintegral.com/doc/index.html?file=sdk-m_sdk-android&lang=cn
// https://dev.mintegral.com/doc/index.html?file=sdk-m_sdk_admob-android&lang=cn
// https://developers.smaato.com//publishers/nextgen-sdk-android-getting-started/
public class Mediation {
    private static final String TAG = "admob_manger";

    private BannerPlugin banner;
    private RewardVideoPlugin rewardVideo;
    private InterstitialPlugin interstitial;
    private Activity activity;
    //private UMP ump = new UMP();

    private final static Mediation mManager = new Mediation();

    public static Mediation getInstance() {
        return mManager;
    }

    public void init(Activity activity, ViewGroup bannerParent) {
        this.activity = activity;

        // Amazon requires an 'Activity' instance
        AdRegistration.getInstance("21d50ba1-c04a-48cd-a7b9-2c00b677961d", activity);
        AdRegistration.setAdNetworkInfo(new DTBAdNetworkInfo(DTBAdNetwork.MAX));
        AdRegistration.setMRAIDSupportedVersions(new String[]{"1.0", "2.0", "3.0"});
        AdRegistration.setMRAIDPolicy(MRAIDPolicy.CUSTOM);

        //AdRegistration.enableTesting( true );
        //AdRegistration.enableLogging( true );

        // Required: a valid Play Store Url for your Android app.
        POBApplicationInfo appInfo = new POBApplicationInfo();
        try {
            appInfo.setStoreURL(new URL("https://play.google.com/store/apps/details?id=com.starfish.cryptogram.an"));
        } catch (MalformedURLException e) {
            // Handle exception in your own way
            e.printStackTrace();
        }

        // This app information is a global configuration.
        // You need not set this for every ad request (of any ad type).
        OpenWrapSDK.setApplicationInfo(appInfo);

        // Indicates whether or not the ad request is GDPR (General Data Protection Regulation) compliant.
        OpenWrapSDK.setGDPREnabled(true);
        // A valid Base64 encoded consent string as defined at, https://github.com/InteractiveAdvertisingBureau/GDPR-Transparency-and-Consent-Framework.
        OpenWrapSDK.setGDPRConsent("accepted");

        AppLovinSdk.getInstance(activity).setMediationProvider(AppLovinMediationProvider.MAX);
        AppLovinSdk.getInstance(activity).initializeSdk(configuration -> {
            // AppLovin SDK is initialized, start loading ads
            Log.d(TAG, "initializeSdk");

            banner = new BannerPlugin(activity, bannerParent);
            rewardVideo = new RewardVideoPlugin(activity);
            interstitial = new InterstitialPlugin(activity);

            banner.load("69457da232a3b3b1");
            interstitial.load("5558e84a43bd6938");
            rewardVideo.load("7f111652924bae46");
        });
        // AppLovinSdk.getInstance(activity).getSettings().setVerboseLogging(true);
        AppLovinSdk.getInstance(activity).setUserIdentifier(md5(getAndroidId()));
    }

    public void onResume() {
        try {
            if (banner != null)
                banner.onResume();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void onPause() {
        try {
            if (banner != null)
                banner.onPause();
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void onDestroy() {
        if (banner != null)
            banner.onDestroy();
    }

    public void onBackPressed() {

    }

    public void showInterstitial() {
        if (activity != null) {
            activity.runOnUiThread(() -> interstitial.show());
        }
    }

    public void showRewardVideo() {
        if (rewardVideo != null) {
            activity.runOnUiThread(() -> rewardVideo.show());
        }
    }

    public int isBannerReady() {
        if (banner != null) {
            return banner.isReady() ? 1 : 0;
        }

        return 0;
    }

    public int isInterstitialReady() {
        if (interstitial != null) {
            return interstitial.isReady() ? 1 : 0;
        }

        return 0;
    }

    public int isRewardVideoReady() {
        if (rewardVideo != null) {
            return rewardVideo.isReady() ? 1 : 0;
        }

        return 0;
    }

    public void showBanner() {
        if (banner != null) {
            activity.runOnUiThread(() -> banner.show());
        }
    }

    public void hideBanner() {
        if (banner != null) {
            activity.runOnUiThread(() -> banner.hide());
        }
    }

    public void showTestTool() {
        activity.runOnUiThread(() -> AppLovinSdk.getInstance(activity).showMediationDebugger());
    }

    public String getCurrentInterstitialEcpm() {
        String ecpm = interstitial.getEcpm();
        Log.d(TAG, "inter_ecpm: " + ecpm);
        return ecpm;
    }

    public String getCurrentRewardVideoEcpm() {
        String ecpm = rewardVideo.getEcpm();
        Log.d(TAG, "reward_ecpm: " + ecpm);
        return ecpm;
    }

    public String getCurrentBannerEcpm() {
        if (banner != null) {
            return banner.getEcpm();
        }
        return "0";
    }

    public String getCountry() {
        try {
            Locale locale;
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                locale = LocaleList.getDefault().get(0);
            } else {
                locale = Locale.getDefault();
            }
            if (locale != null) {
                return locale.getCountry();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

        return "US";
    }

    public String getLanguage() {
        try {
            Locale locale;
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N) {
                locale = LocaleList.getDefault().get(0);
            } else {
                locale = Locale.getDefault();
            }
            if (locale != null) {
                return locale.getLanguage() + "-" + locale.getCountry();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

        return "en-US";
    }

    public String getAdvertisingId() {
        try {
            AdvertisingIdClient.Info adInfo = AdvertisingIdClient.getAdvertisingIdInfo(activity);
            return adInfo.getId();
        } catch (Exception e) {
            e.printStackTrace();
        }

        return "Unknown";
    }

    public String getAndroidId() {
        String androidId = getAndroidIdOld();
        try {
            if (isZero(androidId)) {
                return getAndroidIdNew();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
        return androidId;
    }

    public String getAndroidIdOld() {
        SharedPreferences mPreferences = activity.getSharedPreferences("FOTO_WORD_CRUSH", Context.MODE_PRIVATE);
        String applicationUniqueID = mPreferences.getString("applicationUniqueID", "");
        if (TextUtils.isEmpty(applicationUniqueID)) {
            String androidId = Settings.Secure.getString(activity.getApplicationContext().getContentResolver(), Settings.Secure.ANDROID_ID);
            if (TextUtils.isEmpty(androidId)) {
                String uuid = "uuid" + UUID.randomUUID().toString();
                Log.d(TAG, "获取到uuid:" + uuid);
                writeCache(mPreferences, uuid);
                return uuid;
            } else {
                Log.d(TAG, "获取到androidId:" + androidId);
                writeCache(mPreferences, androidId);
                return androidId;
            }
        } else {
            Log.d(TAG, applicationUniqueID.contains("uuid") ? "获取的是uuid" : "获取的是androidid" + applicationUniqueID);
            return applicationUniqueID;
        }
    }

    private static boolean isZero(String androidId) {
        boolean isZero = true;
        for (int i = 0; i < androidId.length(); i++) {
            String str = androidId.charAt(i) + "";
            if (!str.equals("0")) {
                isZero = false;
                break;
            }
        }
        return isZero;
    }

    private String getAndroidIdNew() {
        SharedPreferences mPreferences = activity.getSharedPreferences("FOTO_WORD_CRUSH", Context.MODE_PRIVATE);
        String uuid = "uuid" + UUID.randomUUID().toString();
        Log.d(TAG, "获取到uuid:" + uuid);
        writeCache(mPreferences, uuid);
        return uuid;
    }

    private void writeCache(SharedPreferences mPreferences, String applicationUniqueID) {
        SharedPreferences.Editor editor = mPreferences.edit();
        editor.putString("applicationUniqueID", applicationUniqueID);
        editor.commit();
    }

    public static String md5(final String s) {
        final String MD5 = "MD5";
        try {
            // Create MD5 Hash
            MessageDigest digest = java.security.MessageDigest
                    .getInstance(MD5);
            digest.update(s.getBytes());
            byte messageDigest[] = digest.digest();

            // Create Hex String
            StringBuilder hexString = new StringBuilder();
            for (byte aMessageDigest : messageDigest) {
                String h = Integer.toHexString(0xFF & aMessageDigest);
                while (h.length() < 2)
                    h = "0" + h;
                hexString.append(h);
            }
            return hexString.toString();

        } catch (NoSuchAlgorithmException e) {
            e.printStackTrace();
        }
        return "";
    }
}

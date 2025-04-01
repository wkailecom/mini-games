//
//  iOSBridage.m
//  TESTSDK
//
//  Created by wanghaiqi on 2021/12/9.
//

#import "iOSBridge.h"


#import "YTNativeTools.h"
#import "CommonMacros.h"

#import "AdManagerInitOption.h"

#import "UnityAppController.h"


#import <UIKit/UIKit.h>
#import "EventLogUtil.h"
#import <CoreTelephony/CTTelephonyNetworkInfo.h>
#import <CoreTelephony/CTCarrier.h>
#import "YTNativeTools.h"
#import "AudioToolbox/AudioToolbox.h"
#import "AdManager.h"
#import "AdBanner.h"
#import "AdReward.h"
#import "AdInterstitial.h"
#import "BetaAdToolRootVC.h"
#import "SEvent.h"
#import "SEventPlugins.h"
#import "CommonMacros.h"
#import "EcpmPlugin.h"
#import "EventLogUtil.h"
#import "YTUserNotificationCenter.h"
#import "NotifyExtend.h"

#ifdef __cplusplus
extern "C" {
#endif
    
    extern void UnitySendMessage( const char *className, const char *methodName, const char *param );
    
#ifdef __cplusplus
}
#endif

@implementation iOSBridge
char *const AdEventsName = "AdEvents";

char *const CmpEventsName = "CmpEvents";

// Converts NSString to C style string by way of copy (Mono will free it)
#define MakeStringCopy( _x_ ) ( _x_ != NULL && [_x_ isKindOfClass:[NSString class]] ) ? strdup( [_x_ UTF8String] ) : NULL

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]

+ (iOSBridge *)share {
    static iOSBridge *instance;
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken,
                  ^{
        instance = [iOSBridge new];
    });
    
    return instance;
}
    
#pragma mark -- [AdManagerInitOption] callback start
- (void)getAds:(NSDictionary * _Nullable)data {
    NSLog(@"getAds");
}

- (void)interstitialAdClose:(nonnull NSDictionary *)data {
    NSLog(@"interstitialAdClose");
        UnitySendMessage(AdEventsName, "onAdClosed", MakeStringCopy([self getJsonFromDic:data]));
}

- (void)interstitialAdLoaded:(nonnull NSDictionary *)data {
    NSLog(@"interstitialAdLoaded");
        UnitySendMessage(AdEventsName, "onInterstitialAdLoaded", MakeStringCopy([self getJsonFromDic:data]));

}

- (void)rewardLoaded:(nonnull NSDictionary *)data {
    NSLog(@"rewardLoaded");
        UnitySendMessage(AdEventsName, "onRewardAdLoaded", MakeStringCopy([self getJsonFromDic:data]));

}

- (void)rewardUser:(nonnull NSDictionary *)data {
    NSLog(@"rewardUser");
        UnitySendMessage(AdEventsName, "onUserEarnedReward", MakeStringCopy([self getJsonFromDic:data]));

}

- (void)rewardVideoClose:(nonnull NSDictionary *)data {
    NSLog(@"rewardVideoClose");
        UnitySendMessage(AdEventsName, "onAdClosed", MakeStringCopy([self getJsonFromDic:data]));

}

- (void)shouldShowInterstitialAD:(nonnull NSDictionary *)data {
    NSLog(@"shouldShowInterstitialAD");
        UnitySendMessage(AdEventsName, "onAdDisplayed", MakeStringCopy([self getJsonFromDic:data]));
}

- (void)showInterstitialFailed:(nonnull NSDictionary *)data {
    NSLog(@"showInterstitialFailed");
        UnitySendMessage(AdEventsName, "onAdFailedToDisplay", MakeStringCopy([self getJsonFromDic:data]));

}

- (void)showRewardVideoFailed:(nonnull NSDictionary *)data {
    NSLog(@"showRewardVideoFailed");
        UnitySendMessage(AdEventsName, "onAdFailedToDisplay", MakeStringCopy([self getJsonFromDic:data]));

}

- (void)didPayRevenueForAd:(nonnull NSDictionary *)data {
    UnitySendMessage(AdEventsName, "didPayRevenueForAd", MakeStringCopy([self getJsonFromDic:data]));
}

- (void)cmpConsentFormOpened:(nonnull NSDictionary *)data {
    UnitySendMessage(CmpEventsName, "cmpConsentFormOpened", MakeStringCopy([self getJsonFromDic:data]));
}


#pragma mark -- [AdManagerInitOption]  callback end


#pragma mark C Section

#pragma clang diagnostic push
#pragma clang diagnostic ignored "-Wstrict-prototypes"
#ifdef __cplusplus
extern "C" {
#endif
    void ShakeIos(){
        //AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
        //3D Touch �� Peek �𶯷���
        //AudioServicesPlaySystemSound(1519);
        //3D Touch �� Pop �𶯷���
        //AudioServicesPlaySystemSound(1520);
        //����������
        //AudioServicesPlaySystemSound(1521);
        
        if (@available(iOS 10.0, *)) {
            UIImpactFeedbackGenerator *feedBackGenertor = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
            [feedBackGenertor impactOccurred];
        } else {
            // Fallback on earlier versions
        }
    }
    
    const char *CFGetDeviceId() {
        return MakeStringCopy([YTNativeTools getIdfaOrIdfvInSilence]);
    }
    
#pragma mark - ATT start
    int CFGetAuthAdTraceState() {
        return [YTNativeTools CFGetAuthAdTraceState];
    }
    
    const char *CFGetIdfa() {
        NSLog(@"original YTNativeTools CFRequireATTAuthInNativeSetting");
        return  MakeStringCopy([YTNativeTools getIDFA]);
        NSLog(@"original YTNativeTools end");
    }
#pragma mark - ATT end

#pragma mark - AD start
    
#pragma mark AD init
    void CFInitAdLib(const char *json, const char *afCallbackInfo) {
        //NSLog(@"original ad enter  CFInitAdLib init params str=%@, afCallbackInfo str= %@",GetStringParam(json),GetStringParam(afCallbackInfo));
        //        [[AdManager shared] getServerAds:nil];
        //        [AdManager initializeWithOptions:[iOSBridge share] and:[EventLogUtil dictionaryWithJsonString:GetStringParam(json)] and:[UIApplication sharedApplication].keyWindow.rootViewController and:[UIApplication sharedApplication].keyWindow];
        
        [AdManager initializeWithOptions:[iOSBridge share] and:[EventLogUtil dictionaryWithJsonString:GetStringParam(json)] and:UnityGetGLViewController() and:UnityGetGLView()];
        
    }

    void CFInitUmp() {
//        [AdManager shared].initOption = [iOSBridge share];
        
        
        [[AdManager shared] initUmp:[iOSBridge share]];
    }
    
    void showAdTest()
    {
        [[AdManager shared] showTestAdTool];
    }
    
#pragma mark banner start
    bool CFIsBannerReady() {
        return [AdBanner shared].bannerIsready;
    }
    
    void CFShowBanner()
    {
        [[AdBanner shared] showBannerAd];
    }
    
    void CFHideBanner()
    {
        [[AdBanner shared] hiddenBanner];
    }
    
    void CFRemoveBanner()
    {
        [[AdBanner shared] destroyBanner];
    }
    
#pragma mark banner end
    
#pragma mark intetstitital start
    bool CFIsInterstitialReady()
    {
        return [[AdInterstitial shared] isInterstitialAdReady];
    }
    
    void CFShowInterstitial()
    {
        [[AdInterstitial shared] showInterstitialAd];
    }
#pragma mark intetstitital end
    
#pragma mark reward start
    void CFShowRewardedVideo()
    {
        [[AdReward shared] showRewardAd];
    }
    
    bool CFIsRewardedVideoReady()
    {
        return [[AdReward shared] isRewardAdReady];
    }
#pragma mark reward end

#pragma mark - AD end
    
#pragma mark - ecpm start
    const char *CFFetchvideoadecpm() {
        NSString *ecpm = [AdReward shared]._ecpmValue;
        NSLog(@"original ad CFFetchvideoadecpm  %@",ecpm);
        return  MakeStringCopy(ecpm);
    }
    
    const char *CFFetchinteradecpm() {
        //        NSString *ecpm = [EcpmPlugin getInterstitialEcpm];
        NSString *ecpm = [AdInterstitial shared]._ecpmValue;
        NSLog(@"original ad CFFetchinteradecpm %@", ecpm);
        return  MakeStringCopy(ecpm);
        [[YTUserNotificationCenter getProperUserNotificationCenter] requestNofiticationAuthentication];
    }
    
    
    
    double CFFetchvideoadecpm_double() {
        NSString *ecpm = [AdReward shared]._ecpmValue;
        NSLog(@"original ad CFFetchvideoadecpm_double  %f",[ecpm doubleValue]);
        return [ecpm doubleValue];
    }
    
    double CFFetchinteradecpm_double() {
        NSString *ecpm = [AdInterstitial shared]._ecpmValue;
        NSLog(@"original ad CFFetchinteradecpm_double %f", [ecpm doubleValue]);
        return  [ecpm doubleValue];
    }

    const char *CFFetchBannerAdecpm() {
        NSString *ecpm = [AdBanner shared]._ecpmValue;
        NSLog(@"original ad CFFetchBannerAdecpm  %@",ecpm);
        return  MakeStringCopy(ecpm);
    }
    
    double CFFetchBannerAdecpm_double() {
        NSString *ecpm = [AdBanner shared]._ecpmValue;
        NSLog(@"original ad CFFetchbannerecpm_double %f", [ecpm doubleValue]);
        return  [ecpm doubleValue];
    }
    
#pragma mark - ecpm end
    
    
#pragma mark -Notify start
    
    void CFGetNotificationPermis()
    {
        [[YTUserNotificationCenter getProperUserNotificationCenter] requestNofiticationAuthentication];
    }

    void CFSetBadgeNumber(char *count)
    {
        int countValue = [GetStringParam(count) intValue];
        [UIApplication sharedApplication].applicationIconBadgeNumber = countValue;
    }
    
    void CFScheduleLocalNotificationWithID(char *notiID,char *title,char *body,double second) {
        NSLog(@"oripath from unity");
        int notiIDValue = [GetStringParam(notiID) intValue];
        NSString  *imagePath = [[NSBundle mainBundle] pathForResource:@"small" ofType:@"png"];
                
        [[YTUserNotificationCenter getProperUserNotificationCenter] scheduleLocalNotificationWithID:GetStringParam(notiID)
                                                                                        title:GetStringParam(title)
                                                                                    subTitle:@""
                                                                                        body:GetStringParam(body)
                                                                                    imagePath:imagePath
                                                                                        badge:1
                                                                                    fireTime:second
                                                                                        action:nil];
    }

    //取消指定id的推送
    void CFCancelLocalNotificationWithID(char *notiID) {
        [[YTUserNotificationCenter getProperUserNotificationCenter] cancelLocalNotificationWithID:GetStringParam(notiID)];
    }
    
    //取消未弹出来的推送
    void CFCancelAllLocalNotification() {
        [[YTUserNotificationCenter getProperUserNotificationCenter] cancelAllLocalNotification];
    }
    
    //获取当前推送权限的状态，
    bool CFPushNotificationState() {
        return  [[YTUserNotificationCenter getProperUserNotificationCenter] pushNotificationState];
    }
    
    //获取用户系统深浅色 默认 0 浅色
    int CFUIUserInterfaceStyle() {
        return [[YTUserNotificationCenter getProperUserNotificationCenter] getUserInterfaceStyle];
    }
    
#pragma mark -Notify end

    //打开应用详情页
    void CFUpdateAppInAppStore() {
        NSURL *url = [NSURL URLWithString:@"itms-apps://itunes.apple.com/app/id6572296568"];
        if (@available(iOS 10.0, *)) {
            [[UIApplication sharedApplication] openURL:url
                options:@{ UIApplicationOpenURLOptionsSourceApplicationKey : @YES }
                completionHandler:^(BOOL success) {
                  if (success) {
                      NSLog(@"10以后可以跳转url");
                  } else {
                      NSLog(@"10以后不可以跳转url");
                  }
                }];
        } else {
            BOOL success = [[UIApplication sharedApplication] openURL:url];
            if (success) {
                NSLog(@"10以前可以跳转url");
            } else {
                NSLog(@"10以前不可以跳转url");
            }
        }
    }
    
- (NSString *)getJsonFromDic:(NSDictionary *)dict {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];
    
    if (!jsonData) {
        NSLog(@"Got an error: %@", error);
        return @"";
    } else {
        NSString *jsonString = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
        return jsonString;
    }
}
    
#ifdef __cplusplus
}
#endif
#pragma clang diagnostic pop

@end

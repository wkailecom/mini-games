package com.nutz.game.message.notify;

import android.app.Activity;
import android.app.Application;
import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.text.TextUtils;
import android.util.Log;

import androidx.core.app.NotificationCompat;


import com.nutz.game.LaunchActivity;

import org.jetbrains.annotations.NotNull;
import org.json.JSONException;
import org.json.JSONObject;

import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Locale;
import java.util.Objects;

/**
 * Created by wang on 2019/5/16 11:53
 */
public class SFNotificationUtil {

    private static final String TAG = "SFNotificationUtil";

    private static final String CatchName = "NotifyCatch";
    static int activityCount = 0;
    private static Handler handlerService = null;
    private static final int aDayNotifyCount = 3;

    /**
     * @param activity  main
     * @param delayTime 延迟时间 秒
     * @param id        通知id
     */
    public static void sendNotification(final Activity activity, long delayTime, int id, String title, String content) {
        try {
            Log.i(TAG, "sendNotification: ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Log.i(TAG, "sendNotification: Unity set Msg delayTime=" + delayTime + " id=" + id);
            NotifyModel notifyModel = getNotifyModel(activity, delayTime, id, title, content);
            if (notifyModel == null) return;
            NotifyCatchModel notifyCatchModel = getNotifyCatchModel(activity);
            if (notifyCatchModel != null && !DateUtil.isSameDay(notifyCatchModel.time, notifyModel.firstTime)) {
                notifyCatchModel.aDayCount = 0;
                DataUtil.writeToFile(activity, CatchName, notifyCatchModel.toJson().toString());
                Log.i(TAG, "sendNotification: New Day so reset notify count" + Objects.requireNonNull(getNotifyCatchModel(activity)).aDayCount);
            }
            setSendNotifyService(activity, delayTime, id, notifyModel);
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private static NotifyCatchModel getNotifyCatchModel(Activity activity) throws JSONException {
        String catchStr = DataUtil.readFromFile(activity, CatchName);
        if (!TextUtils.isEmpty(catchStr))
            return NotifyCatchModel.fromJson(new JSONObject(catchStr));
        return null;
    }

    public static void cancelSendMsgHandlerService(int id) {
        Log.i(TAG, "cancelSendMsgHandlerService: ---------------------------------------------");
        Log.i(TAG, "cancelSendMsgHandlerService: Unity cancel notify id is " + id);
        if (handlerService != null) {
            handlerService.removeMessages(id);
        }
    }

    private static void setSendNotifyService(Activity activity, long delayTime, int id, NotifyModel notifyModel) {
        if (handlerService == null) {
            handlerService = new Handler(activity.getMainLooper()) {
                @Override
                public void handleMessage(@NotNull Message msg) {
                    super.handleMessage(msg);
                    try {
                        int notifyId = msg.what;
                        NotifyModel notifyMode = (NotifyModel) msg.obj;
                        Log.i(TAG, "handleMessage: service come msg is " + notifyMode.toJson().toString());
                        if (activityCount > 0) {
                            Log.i(TAG, "handleMessage: Application activityCount > 0 notify skip...");
                        } else {
                            if (notifyId == 101) {
                                notifyMsg(activity, notifyMode, notifyMode.id, System.currentTimeMillis(), (NotificationManager) activity.getSystemService(Context.NOTIFICATION_SERVICE));
                            } else {
                                NotifyCatchModel notifyCatchModeOld = getNotifyCatchModel(activity);
                                if (notifyCatchModeOld != null && notifyCatchModeOld.aDayCount > aDayNotifyCount - 1) {
                                    Log.i(TAG, "handleMessage: a day notify count >3 notify skip...");
                                } else {
                                    NotifyCatchModel notifyCatchModelNew = new NotifyCatchModel();
                                    notifyCatchModelNew.time = notifyMode.firstTime;
                                    if (notifyCatchModeOld != null) {
                                        notifyCatchModelNew.aDayCount = notifyCatchModeOld.aDayCount + 1;
                                    } else {
                                        notifyCatchModelNew.aDayCount = 1;
                                    }
                                    DataUtil.writeToFile(activity, CatchName, notifyCatchModelNew.toJson().toString());
                                    notifyMsg(activity, notifyMode, notifyMode.id, System.currentTimeMillis(), (NotificationManager) activity.getSystemService(Context.NOTIFICATION_SERVICE));
                                }
                            }
                        }
                    } catch (Exception e) {
                        e.printStackTrace();
                    }
                }
            };
        }
        handlerService.removeMessages(id);//remove old message before send new message
        Message msg = handlerService.obtainMessage(id, notifyModel);
        handlerService.sendMessageDelayed(msg, delayTime * 1000);
    }

    //消息通知
    private static void notifyMsg(Context context, NotifyModel obj, int nid, long time, NotificationManager mNotifyMgr) {
        try {
            if (context == null || obj == null) return;
            if (mNotifyMgr == null)
                mNotifyMgr = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);
            if (time <= 0) return;

            //准备intent
            Intent intent = new Intent(context, LaunchActivity.class);
            if (obj.param != null && obj.param.trim().length() > 0) {
                intent.putExtra("param", obj.param);
            }
            Notification notification = null;
            String contentText = obj.content;
            PendingIntent pi = null;
            if(Build.VERSION.SDK_INT >= Build.VERSION_CODES.M){
                pi = PendingIntent.getActivity(context, 1, intent,
                        PendingIntent.FLAG_UPDATE_CURRENT | PendingIntent.FLAG_IMMUTABLE);
            } else{
                pi = PendingIntent.getActivity(context, 1, intent, PendingIntent.FLAG_UPDATE_CURRENT);
            }

            int icon = context.getResources().getIdentifier(obj.icon.equals("") ? "app_icon" : obj.icon, "mipmap", context.getPackageName());
            //版本兼容
            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {//兼容Android8.0
                String id = "default";
                int importance = NotificationManager.IMPORTANCE_LOW;
                CharSequence name = "notice";
                NotificationChannel mChannel = new NotificationChannel(id, name, importance);
                mChannel.enableLights(true);
                mChannel.setDescription("word notification");
                mChannel.enableLights(true);
                mChannel.setLightColor(Color.GREEN);
                mChannel.enableVibration(true);
                mChannel.setVibrationPattern(new long[]{100, 200, 300, 400, 500, 400, 300, 200, 400});
                if (mNotifyMgr != null) {
                    mNotifyMgr.createNotificationChannel(mChannel);
                    Notification.Builder builder = new Notification.Builder(context, id);
                    builder.setAutoCancel(true)
                            .setContentIntent(pi)
                            .setContentTitle(obj.title)
                            .setContentText(obj.content)
                            .setOngoing(false)
                            .setStyle(new Notification.BigTextStyle())
                            .setSmallIcon(icon)
                            .setWhen(System.currentTimeMillis());
                    if (obj.subText != null && obj.subText.trim().length() > 0) {
                        builder.setSubText(obj.subText);
                    }
                    notification = builder.build();
                }
            } else if (Build.VERSION.SDK_INT >= 23) {
                NotificationCompat.Builder builder = new NotificationCompat.Builder(context);
                builder.setContentTitle(obj.title)
                        .setContentText(contentText)
                        .setSmallIcon(icon)
                        .setContentIntent(pi)
                        .setAutoCancel(true)
                        .setOngoing(false)
                        .setStyle(new NotificationCompat.BigTextStyle())
                        .setWhen(System.currentTimeMillis());
                if (obj.subText != null && obj.subText.trim().length() > 0) {
                    builder.setSubText(obj.subText);
                }
                notification = builder.build();
            }

            if (notification != null && mNotifyMgr != null) {
                Log.i(TAG, "notifyMsg: user get notify...");
                mNotifyMgr.notify(nid, notification);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private static NotifyModel getNotifyModel(Activity activity, long delayTime, int id, String title, String content) {
//        NotifyModel notifyModel = null;
//        switch (id) {
//            case 101:
//                notifyModel = getNotifyModel(activity, delayTime, id, MsgEnum.power.getTitle(), MsgEnum.power.getContent(), "");
//                break;
//            case 102:
//                notifyModel = getNotifyModel(activity, delayTime, id, MsgEnum.gift.getTitle(), MsgEnum.gift.getContent(), "");
//                break;
//            case 103:
//                notifyModel = getNotifyModel(activity, delayTime, id, MsgEnum.offlineRecall_1.getTitle(), MsgEnum.offlineRecall_1.getContent(), "");
//                break;
//            case 104:
//                notifyModel = getNotifyModel(activity, delayTime, id, MsgEnum.offlineRecall_2.getTitle(), MsgEnum.offlineRecall_2.getContent(), "");
//                break;
//            case 105:
//                notifyModel = getNotifyModel(activity, delayTime, id, MsgEnum.offlineRecall_3.getTitle(), MsgEnum.offlineRecall_3.getContent(), "");
//                break;
//            default:
//                break;

//        }

        return getNotifyModel(activity, delayTime, id, title, content, "");
    }

    /**
     * @param activity  main
     * @param delayTime 通知延时的时间 （单位：秒）
     * @param id        通知id
     * @param title     通知title
     * @param content   通知内容
     * @param icon      图标
     * @return 通知对象
     */
    private static NotifyModel getNotifyModel(Activity activity, long delayTime, int id, String title, String content, String icon) {
        long now = System.currentTimeMillis();
        SimpleDateFormat smf = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss.SSS", Locale.getDefault());
        Date date = new Date(now + delayTime * 1000);
        NotifyModel obj = new NotifyModel();
        Log.i(TAG, "getNotifyModel: notify will come time " + smf.format(date));
        obj.id = id;
        obj.title = title;
        obj.subText = "";
        obj.content = content;
        obj.firstTime = now + delayTime * 1000;         //通知的定时器
        obj.activityClass = activity.getClass();
        obj.icon = icon;
        return obj;
    }

    /**
     * 判断当前应用是否在前台
     *
     * @param context application
     */
    public static void checkToSendNotification(Application context) {
        if (context == null)
            return;
        context.registerActivityLifecycleCallbacks(new Application.ActivityLifecycleCallbacks() {
            @Override
            public void onActivityCreated(@NotNull Activity activity, Bundle savedInstanceState) {

            }

            @Override
            public void onActivityStarted(@NotNull Activity activity) {

            }

            @Override
            public void onActivityResumed(@NotNull Activity activity) {
                activityCount++;
            }

            @Override
            public void onActivityPaused(@NotNull Activity activity) {
                activityCount--;
            }

            @Override
            public void onActivityStopped(@NotNull Activity activity) {

            }

            @Override
            public void onActivitySaveInstanceState(@NotNull Activity activity, @NotNull Bundle outState) {

            }

            @Override
            public void onActivityDestroyed(@NotNull Activity activity) {

            }
        });
    }

}

package com.nutz.game.ad;

import com.unity3d.player.UnityPlayer;

import org.json.JSONObject;

public class MessageChannel {
    public void loadedAd(String type, String platform, String id, String subId) {
        sendToUnity("onAdLoaded", type, platform, id, subId);
    }

    public void showAd(String type, String platform, String id, String subId) {
        sendToUnity("onAdDisplayed", type, platform, id, subId);
    }

    public void closeAd(String type, String platform, String id, String subId) {
        sendToUnity("onAdClosed", type, platform, id, subId);
    }

    public void showAdFail(String type, String platform, String id, String subId) {
        sendToUnity("onAdFailedToDisplay", type, platform, id, subId);
    }

    public void earnedReward(String type, String platform, String id, String subId) {
        sendToUnity("onUserEarnedReward", type, platform, id, subId);
    }

    public void didPayRevenueForAd(String type, String platform, String id, String subId, String country, double revenue) {
        sendToUnity2("didPayRevenueForAd", type, platform, id, subId, country, revenue);
    }

    private void sendToUnity(String funcName, String type, String platform, String id, String subId) {
        try {
            JSONObject obj = new JSONObject();
            obj.put("type", type);
            obj.put("platform", platform == null ? "admob" : platform);
            obj.put("id", id == null ? "" : id);
            obj.put("sub_id", subId == null ? "" : subId);
            UnityPlayer.UnitySendMessage("AdEvents", funcName, obj.toString());
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    private void sendToUnity2(String funcName, String type, String platform, String id, String subId, String country, double revenue) {
        try {
            JSONObject obj = new JSONObject();
            obj.put("type", type);
            obj.put("platform", platform == null ? "admob" : platform);
            obj.put("id", id == null ? "" : id);
            obj.put("sub_id", subId == null ? "" : subId);
            obj.put("country", country);
            obj.put("revenue", revenue);
            UnityPlayer.UnitySendMessage("AdEvents", funcName, obj.toString());
        } catch (Exception e) {
            e.printStackTrace();
        }
    }
}

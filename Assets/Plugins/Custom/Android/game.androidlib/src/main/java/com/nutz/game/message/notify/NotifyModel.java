package com.nutz.game.message.notify;

import android.app.Activity;


import com.nutz.game.message.jsonarse.JSONUtils;

import org.json.JSONObject;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

/**
 * Created by wang on 2019/5/16 11:58
 */
public class NotifyModel implements Serializable {
    public Integer id;
    public String title;
    public String subText;
    public String content;
    public String param;
    public Long firstTime;
    public Class<? extends Activity> activityClass;
    public String icon;
    public List<Long> times = new ArrayList<>();    //一个通知重复通知的次数


    public static byte[] toBytes(NotifyModel obj) throws IOException {
        ByteArrayOutputStream bout = new ByteArrayOutputStream();
        ObjectOutputStream oos = null;
        String content = null;
        oos = new ObjectOutputStream(bout);
        oos.writeObject(obj);
        oos.close();
        byte[] bytes = bout.toByteArray();
        bout.close();
        return bytes;
    }

    public static NotifyModel from(String content) throws IOException, ClassNotFoundException {
        ByteArrayInputStream bin = new ByteArrayInputStream(content.getBytes("ISO-8859-1"));
        ObjectInputStream ois = null;
        NotifyModel obj = null;

        ois = new ObjectInputStream(bin);
        obj = (NotifyModel) ois.readObject();
        ois.close();
        bin.close();
        return obj;
    }

    public static String to(NotifyModel obj) throws IOException {
        ByteArrayOutputStream bout = new ByteArrayOutputStream();
        ObjectOutputStream oos = null;
        String content = null;
        oos = new ObjectOutputStream(bout);
        oos.writeObject(obj);
        oos.close();
        byte[] bytes = bout.toByteArray();
        content = new String(bytes, "ISO-8859-1");
        bout.close();
        return content;
    }


    public JSONObject toJson() {

        JSONObject json = new JSONObject();
        try {
            json.put("id", id);
            json.put("title", title);
            json.put("subText", subText);
            json.put("content", content);
            json.put("param", param);
            json.put("firstTime", firstTime);
            json.put("icon", icon);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return json;
    }

    public static NotifyModel fromJson(JSONObject json) {
        NotifyModel notifyModel = new NotifyModel();
        notifyModel.id = JSONUtils.getInt(json, "id", 1);
        notifyModel.title = JSONUtils.getString(json, "title", "");
        notifyModel.subText = JSONUtils.getString(json, "subText", "");
        notifyModel.content = JSONUtils.getString(json, "content", null);
        notifyModel.param = JSONUtils.getString(json, "param", null);
        notifyModel.icon = JSONUtils.getString(json, "icon", null);
        notifyModel.firstTime = JSONUtils.getLong(json, "firstTime", 0);
        return notifyModel;
    }

}

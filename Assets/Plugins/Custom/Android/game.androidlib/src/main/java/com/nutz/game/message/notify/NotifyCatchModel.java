package com.nutz.game.message.notify;

import com.nutz.game.message.jsonarse.JSONUtils;

import org.json.JSONObject;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.Serializable;

/**
 * Created by wang on 2019/5/16 11:58
 */
public class NotifyCatchModel implements Serializable {
    public Integer aDayCount;

    public Long time;

    public static byte[] toBytes(NotifyCatchModel obj) throws IOException {
        ByteArrayOutputStream bout = new ByteArrayOutputStream();
        ObjectOutputStream oos = null;
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

    public static String to(NotifyCatchModel obj) throws IOException {
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
            json.put("aDayCount", aDayCount);
            json.put("time", time);
        } catch (Exception e) {
            e.printStackTrace();
        }
        return json;
    }

    public static NotifyCatchModel fromJson(JSONObject json) {
        NotifyCatchModel notifyModel = new NotifyCatchModel();
        notifyModel.aDayCount = JSONUtils.getInt(json, "aDayCount", 1);
        notifyModel.time = JSONUtils.getLong(json, "time", 0);
        return notifyModel;
    }

}

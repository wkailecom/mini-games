package com.nutz.game.message.notify;

import android.content.Context;
import android.content.SharedPreferences;

/**
 * Created by wind on 2017/8/9.
 */

public class BaseCache {
    protected Context context = null;
    protected SharedPreferences sp;

    public BaseCache(Context context, String name) {
        this.context = context;
        sp = context.getSharedPreferences(name, Context.MODE_PRIVATE);
    }

    public long getLong(String key, long defaultValue) {
        return sp.getLong(key, defaultValue);
    }

    public int getInt(String key, int defaultValue) {
        return sp.getInt(key, defaultValue);
    }

    public float getFloat(String key, float defaultValue) {
        return sp.getFloat(key, defaultValue);
    }

    public String getString(String key, String defaultValue) {
        return sp.getString(key, defaultValue);
    }

    public boolean getBoolean(String key, boolean defaultValue) {
        return sp.getBoolean(key, defaultValue);
    }

    public boolean put(String key, long value) {
        return sp.edit().putLong(key, value).commit();
    }

    public boolean put(String key, int value) {
        return sp.edit().putInt(key, value).commit();
    }

    public boolean put(String key, float value) {
        return sp.edit().putFloat(key, value).commit();
    }

    public boolean put(String key, String value) {
        return sp.edit().putString(key, value).commit();
    }

    public boolean put(String key, boolean value) {
        return sp.edit().putBoolean(key, value).commit();
    }
}

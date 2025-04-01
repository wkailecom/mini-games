package com.nutz.game.message.notify;

import android.content.Context;

import java.io.ByteArrayOutputStream;
import java.io.FileInputStream;
import java.io.FileOutputStream;

/**
 * Created by wind on 2017/6/8.
 */

public class DataUtil {


    public static void writeToFile(Context context, String filename, String content) {
        if (context == null || filename == null || filename.isEmpty() || content == null)
            return;
        try{
            FileOutputStream fout = context.openFileOutput(filename, Context.MODE_PRIVATE);
            byte [] bytes = content.getBytes();
            fout.write(bytes);
            fout.close();
        } catch(Exception e){
            e.printStackTrace();
        }
    }

    public static String readFromFile(Context context, String filename) {
        if (context == null || filename == null || filename.isEmpty())
            return null;
        try {
            FileInputStream fin = context.openFileInput(filename);
            int length = fin.available();
            byte [] buffer = new byte[length];
            int read = fin.read(buffer);
            ByteArrayOutputStream os = new ByteArrayOutputStream();
            os.write(buffer);
            fin.close();
            return os.toString();
        } catch (Exception e) {
            e.printStackTrace();
        }
        return null;
    }
}

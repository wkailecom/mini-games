package com.nutz.game.ad;

import android.util.Log;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.URL;
import java.net.URLEncoder;
import java.nio.charset.StandardCharsets;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingDeque;

import javax.net.ssl.HttpsURLConnection;

public class LogStream {
    private final static LogStream instance = new LogStream();

    public static LogStream getInstance() { return instance; }

    private final BlockingQueue<String> requestQueue = new LinkedBlockingDeque<>();
    private final Thread sendThread = new Thread(this::runSend);
    private String app = "";
    private String uid = "";

    public LogStream() {
    }

    public void quit() {
        requestQueue.offer("QUIT");

        try {
            sendThread.join();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }

    public void init(String app, String uid) {
        this.app = app;
        this.uid = uid;

        sendThread.start();
    }

    public void sendLog(String event, String msg) {
        try {
            String url = "https://api.wordcrush.net/analysis/log?app=" + app + "&uid=" + uid
                    + "&event=" + event + "&msg=" + URLEncoder.encode(msg, "UTF-8");
            send(url);
        } catch (UnsupportedEncodingException e) {
            e.printStackTrace();
        }
    }

    public void send(String url) {
        requestQueue.offer(url);
    }

    private void runSend() {
        while (true) {
            String url = null;
            try {
                url = requestQueue.take();
            } catch (Exception e) {
                e.printStackTrace();
            }

            if (url == null || url.equals("QUIT")) break;
            get(url);
        }
    }

    private void get(String url) {
        HttpsURLConnection connection = null;
        BufferedReader reader = null;
        try {
            URL u = new URL(url);
            connection = (HttpsURLConnection) u.openConnection();
            connection.setReadTimeout(5000);
            connection.setConnectTimeout(5000);
            connection.setRequestMethod("GET");
            int responseCode = connection.getResponseCode();
            if (responseCode == HttpsURLConnection.HTTP_OK) {
                reader = new BufferedReader(new InputStreamReader(connection.getInputStream()));
                String line;
                StringBuilder builder = new StringBuilder();
                while ((line = reader.readLine()) != null) {
                    builder.append(line);
                }
            }

            Log.d("LogStream", "code: " + responseCode);
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            if (reader != null) {
                try {
                    reader.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
            if (connection != null) {
                connection.disconnect();
            }
        }
    }

    private void post(String url, String body, String contentType) {
        HttpsURLConnection connection = null;
        BufferedReader reader = null;
        OutputStream out = null;
        try {
            URL u = new URL(url);
            connection = (HttpsURLConnection) u.openConnection();
            connection.setReadTimeout(5000);
            connection.setConnectTimeout(5000);
            connection.setDoOutput(true);
            connection.setDoInput(true);
            connection.setUseCaches(false);
            connection.setRequestMethod("POST");
            connection.setRequestProperty("Accept", "*/*");
            connection.setRequestProperty("Content-Type", "application/json");

            out = connection.getOutputStream();
            out.write(body.getBytes(StandardCharsets.UTF_8));
            out.flush();

            int responseCode = connection.getResponseCode();
            if (responseCode == HttpsURLConnection.HTTP_OK) {
                reader = new BufferedReader(new InputStreamReader(connection.getInputStream()));
                String line;
                StringBuilder builder = new StringBuilder();
                while ((line = reader.readLine()) != null) {
                    builder.append(line);
                }
            }

            Log.d("LogStream", "code: " + responseCode);
        } catch (Exception e) {
            e.printStackTrace();
        } finally {
            if (reader != null) {
                try {
                    reader.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
            if (out != null) {
                try {
                    out.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
            if (connection != null) {
                connection.disconnect();
            }
        }
    }
}

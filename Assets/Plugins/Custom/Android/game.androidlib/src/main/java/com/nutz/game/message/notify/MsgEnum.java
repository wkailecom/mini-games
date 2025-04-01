package com.nutz.game.message.notify;

public enum MsgEnum {

    power(101,  "❤️ All lives refilled! ❤️", "Ready to unleash the fun? Let's do it! \uD83E\uDD73"),
    gift(102, "⏰ Tic tac! ", "\uD83D\uDD25Your Special Offer's gonna expire! Don't miss it! \uD83C\uDF81"),
    offlineRecall_1(103,  "New day, new gift! \uD83C\uDF81", "\uD83C\uDF39Cuckoo, your Daily Prize has been delivered! \uD83C\uDF89"),

    offlineRecall_2(104,  "\uD83E\uDD29 Aha! Here you are! \uD83E\uDD29", "\uD83C\uDF40 New challenges are waiting for you! Play now!"),

    offlineRecall_3(105,  "✨ Time to unwind and relax! ✨", "\uD83C\uDF52Ready to make some matches? Let's smash it!");


    private int id;
    private String title;
    private String content;


    MsgEnum(int id, String title, String content) {
        this.id = id;
        this.title = title;
        this.content = content;
    }

    public int getId() {
        return id;
    }

    public String getTitle() {
        return title;
    }

    public String getContent() {
        return content;
    }
}

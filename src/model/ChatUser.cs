using LiteDB;

namespace soufBot.src.model;

public class ChatUser {

    public ObjectId _id { get; set; } = ObjectId.NewObjectId();
    public string? username { get; set; } = "INVALID";
    public int? timeLastMessageAwarded { get; set; } = -1;
    public int? score { get; set; } = -1;
    public int? messagesSent { get; set; } = -1;

    public ChatUser(string username = "", int timeLastMessageAwarded = 0, int score = 0, int messagesSent = 0) {
        this.username = username;
        this.timeLastMessageAwarded = timeLastMessageAwarded;
        this.score = score;
        this.messagesSent = messagesSent;
    }

    public ChatUser() { }

    public override string ToString() {
        return $"{username}\t{timeLastMessageAwarded}\t{score}\t{messagesSent}";
    }

    public NetworkChatUser ToNetworkObject() {
        return new NetworkChatUser(username ?? "INVALID", score ?? -1, messagesSent ?? -1);
    }


}
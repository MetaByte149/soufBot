using LiteDB;

namespace soufBot.src.model;

public class ChatUser
{

    public ObjectId _id { get; set; } = ObjectId.NewObjectId();
    public string? username { get; set; } = "INVALID";
    public int? timeLastMessageAwarded { get; set; } = -1;
    public int? score { get; set; } = -1;

    public ChatUser(string? username, int? timeLastMessageAwarded, int? score)
    {
        this.username = username;
        this.timeLastMessageAwarded = timeLastMessageAwarded;
        this.score = score;
    }

    public ChatUser() { }

    public override string ToString()
    {
        return $"{username}\t{timeLastMessageAwarded}\t{score}";
    }


}
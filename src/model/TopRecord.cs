
using soufBot.src.model.network;

namespace soufBot.src.model;

public class TopRecord {
    public readonly ChatUser[] topUsers;
    public readonly string channel;

    public TopRecord(ChatUser[] topUsers, string channel) {
        this.topUsers = topUsers;
        this.channel = channel;
    }

    public NetworkTopRecord ToNetworkObject() {

        NetworkChatUser[] networkUsers = topUsers.Select((a) => a.ToNetworkObject()).ToArray();
        return new NetworkTopRecord(networkUsers);
    }
}
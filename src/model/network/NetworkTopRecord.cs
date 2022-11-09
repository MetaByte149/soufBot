using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soufBot.src.model.network;

public class NetworkTopRecord {
    public NetworkChatUser[] topUsers;

    public NetworkTopRecord(NetworkChatUser[] topUsers) {
        this.topUsers = topUsers;
    }
}
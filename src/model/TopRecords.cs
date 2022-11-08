using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace soufBot.src.model;

public class TopRecord {
    public readonly ChatUser[] topUsers;
    public readonly string channel;

    public TopRecord(ChatUser[] topUsers, string channel) {
        this.topUsers = topUsers;
        this.channel = channel;
    }
}
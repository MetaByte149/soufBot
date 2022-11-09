using LiteDB;
using soufBot.src.model;
using soufBot.src.tools;

namespace soufBot.src;

class DatabaseConnection {

    LiteDatabase db;

    public DatabaseConnection() {

        db = new LiteDatabase(
            new ConnectionString() {
                Filename = "chatUsers.db",
                Connection = ConnectionType.Shared
            }
        );

    }

    public ChatUser? getUserFromChannel(string name, string channel) {
        using (db) {
            var collection = db.GetCollection<ChatUser>(channel);

            var query = collection.Query().Where(x => x.username == name);

            if (query.Exists()) {
                var obj = query.First();

                return obj;
            }

            return null;
        }
    }

    public bool UpdateUserFromChannel(ChatUser user, string channel) {

        using (db) {
            var collection = db.GetCollection<ChatUser>(channel);

            bool updated = collection.Update(user);


            if (updated) return updated;

            collection.Insert(user);
            return false;

        }
    }

    public List<ChatUser> GetTopTenOfChannel(string channel) {

        using (db) {
            var collection = db.GetCollection<ChatUser>(channel);

            var originalFind = collection.Find(Query.All("score", Query.Descending), 0, 10);

            List<ChatUser> userList = new();

            foreach (var user in originalFind)
                if (user != null) {
                    userList.Add(user);
                }

            return userList;

        }

    }

    public TopRecord[] GetTopRecords(bool activeUsersOnly, string[] channels) {

        TopRecord[] topRecords = new TopRecord[channels.Length];

        int currentTime = Time.CurrentTimeSeconds();

        if (activeUsersOnly) {


            using (db) {
                for (int i = 0; i < channels.Length; i++) {

                    var collection = db.GetCollection<ChatUser>(channels[i]);

                    var originalFind = collection.Find(Query.All("score", Query.Descending), 0, 10).Where((a) => currentTime - a.timeLastMessageAwarded < Time.MINUTES_TO_SECONDS_RATIO);

                    List<ChatUser> userList = new();

                    foreach (var user in originalFind)
                        if (user != null) {
                            userList.Add(user);
                        }

                    topRecords[i] = new(userList.ToArray(), channels[i]);

                }

            }
        } else {
            throw new NotImplementedException();
        }
        return topRecords;
    }
}
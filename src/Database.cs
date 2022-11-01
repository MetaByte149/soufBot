using LiteDB;
using soufBot.src.model;

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

    public List<ChatUser> getTopTenOfChannel(string channel) {

        using (db) {
            var collection = db.GetCollection<ChatUser>(channel);

            var originalFind = collection.Find(Query.All("score", Query.Descending), 0, 10);

            List<ChatUser> userList = new List<ChatUser>();

            foreach (var user in originalFind)
                if (user != null) {
                    Console.WriteLine(user.ToString());
                    userList.Add(user);
                }

            return userList;

        }

    }
}
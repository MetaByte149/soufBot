using LiteDB;
using soufBot.src.model;

namespace soufBot.src;

class DatabaseConnection
{

    LiteDatabase db;

    public DatabaseConnection()
    {

        db = new LiteDatabase(
            new ConnectionString()
            {
                Filename = "chatUsers.db",
                Connection = ConnectionType.Shared
            }
        );

    }

    public void NewMessageFound(string channel, string username)
    {
        // Open database (or create if doesn't exist)
        using (db)
        {
            // Get a collection (or create, if doesn't exist)
            var collection = db.GetCollection<ChatUser>(channel);

            List<ChatUser> chatUsers = collection.Query().Where(x => x.username == username).ToList();
            ChatUser chatUser;
            if (chatUsers.Count > 0)
                chatUser = chatUsers[0];
            else
                chatUser = new ChatUser(null, null, null) { username = username };

            int unixTime = (int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();


            if (unixTime - chatUser.timeLastMessageAwarded > 5000)
            {
                chatUser.score++;
                chatUser.timeLastMessageAwarded = unixTime;
            }
            // Insert new customer document (Id will be auto-incremented)
            collection.Insert(chatUser);


            // Index document using document Name property
            collection.EnsureIndex(chatUser => chatUser.username);

            // // Use LINQ to query documents (filter, sort, transform)
            // var results = collection.Query()
            //     .Where(x => x.Name.StartsWith("J"))
            //     .OrderBy(x => x.Name)
            //     .Select(x => new { x.Name, NameUpper = x.Name.ToUpper() })
            //     .Limit(10)
            //     .ToList();
        }
    }

    public ChatUser? getUserFromChannel(string name, string channel)
    {
        using (db)
        {
            var collection = db.GetCollection<ChatUser>(channel);

            var query = collection.Query().Where(x => x.username == name);

            if (query.Exists())
            {
                var obj = query.First();

                return obj;
            }

            return null;
        }
    }

    public bool UpdateUserFromChannel(ChatUser user, string channel)
    {

        using (db)
        {
            var collection = db.GetCollection<ChatUser>(channel);

            bool updated = collection.Update(user);


            if (updated) return updated;

            collection.Insert(user);
            return false;

        }
    }

}
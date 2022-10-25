using LiteDB;
using Model;
using SoufBot.Model;

namespace SoufBot
{
    class DatabaseConnection
    {

        LiteDatabase db;

        public DatabaseConnection()
        {

            this.db = new LiteDatabase("testDB.db");

        }

        public void NewMessageFound(String channel, String username)
        {
            // Open database (or create if doesn't exist)
            using (this.db)
            {
                // Get a collection (or create, if doesn't exist)
                var collection = db.GetCollection<ChatUser>(channel);

                List<ChatUser> chatUsers = collection.Query().Where(x => x.username == username).ToList();
                ChatUser chatUser;
                if (chatUsers.Count > 0)
                    chatUser = chatUsers[0];
                else
                    chatUser = new ChatUser() { username = username };

                int unixTime = ((int)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());


                if ((unixTime - chatUser.timeLastMessageAwarded) > 5000) {
                    chatUser.score++;
                    chatUser.timeLastMessageAwarded = unixTime;
                }
                // TODO: check how update works
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

    }
}
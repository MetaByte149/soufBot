using LiteDB;
using soufBot.src;

namespace SoufBot
{
    class Program
    {
        static void Main(string[] args)
        {
            TwitchBot soufBot = new TwitchBot();
            String? text = "";
            while (true)
            {
                text = Console.ReadLine();
                soufBot.SendMessage("metabyte149", text ?? "ERROR");
            }

            // var faker = new Faker();
            // using (var db = new LiteDatabase("testDB.db"))
            // {

            //     var collection = db.GetCollection<TestClass>("testCollection");

            //     var query = collection.Query().Where(x => x.name == "orac");

            //     if (query.Exists())
            //     {
            //         // var result = query.FirstOrDefault();
            //         // Console.WriteLine(result.ToString());

            //         // result.alive = !result.alive;

            //         // collection.Update(result);

            //         // Console.WriteLine(result.ToString());

            //         Console.WriteLine(collection.Update(new TestClass() { alive = true, name = "souf" }));

            //     }
            //     else
            //     {
            //         Console.WriteLine("Not foudn1");
            //     }


            //     // for (int i = -5; i < 19; i++)
            //     // {
            //     //     var testClass = new TestClass() { alive = faker.Random.Bool(), name = faker.Name.FullName().Trim().Substring(1, 4).ToLower() };
            //     //     collection.Insert(testClass);
            //     // }
        }

    }

    public class TestClass
    {
        public ObjectId? _id { get; set; } = ObjectId.NewObjectId();

        public String name { get; set; } = "NO_NAME";
        public bool alive { get; set; } = false;

        public override string ToString()
        {
            return $"{_id}\t{name}\t{alive}";
        }
    }
}



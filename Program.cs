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
        }

    }
}



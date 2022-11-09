using soufBot.src;

namespace SoufBot {
    class Program {
        static void Main(string[] _) {
            var soufBot = new TwitchBot();
            string? text;
            while (true) {
                text = Console.ReadLine();
                soufBot.SendMessage("undeadaragon", text ?? "ERROR");
            }
        }

    }
}



using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using System.Text.Json;
using Newtonsoft.Json;
using LiteDB;

namespace SoufBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // SoufBot soufBot = new SoufBot();
            // String? text = "";
            // while (true)
            // {
            //     text = Console.ReadLine();
            //     soufBot.SendMessage("metabyte149", text ?? "ERROR");
            // }

            var db = new LiteDatabase("testDB.db");

            var collection = db.GetCollection("testCollection");


        }

        public class testClass {
            
        } 
    }

}

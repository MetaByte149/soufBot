using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Newtonsoft.Json;
using SoufBot.Model;

namespace SoufBot {
    class SoufBot
    {
        TwitchClient client;
        public string OAUTH_TOKEN = "";
        public string USERNAME = "";
        public string[] CHANNEL_LIST = new String[0];

        public SoufBot()
        {
            try
            {
                String secretFileText = File.ReadAllText("secret/secret.json");
                Secrets? secrets = JsonConvert.DeserializeObject<Secrets>(secretFileText);

                this.USERNAME = secrets?.USERNAME ?? "";

                this.OAUTH_TOKEN = secrets?.OAUTH_TOKEN ?? "";
                this.CHANNEL_LIST = secrets?.CHANNEL_LIST ?? new String[0];

            }
            catch
            {
                this.printError("Could not read secrets and assign");
            }


            ConnectionCredentials credentials = new ConnectionCredentials(
                this.USERNAME,
                this.OAUTH_TOKEN
            );
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);

            foreach (String channel in this.CHANNEL_LIST!)
                client.Initialize(credentials, channel);

            client.OnLog += Client_OnLog!;
            client.OnJoinedChannel += Client_OnJoinedChannel!;
            client.OnMessageReceived += Client_OnMessageReceived!;
            client.OnWhisperReceived += Client_OnWhisperReceived!;
            client.OnNewSubscriber += Client_OnNewSubscriber!;
            client.OnConnected += Client_OnConnected!;

            client.Connect();
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            // Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            this.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {

            this.SendMessage(
                "metabyte149",
                "Hello there " + e.ChatMessage.DisplayName
            );

            string filePath = "test/test.json";

            String originalFileText = "";
            try
            {
                originalFileText = File.ReadAllText(filePath);
            }
            catch
            {
                printError($"File: '{filePath}' not found.");
            }
            List<ChatRecord>? records = JsonConvert.DeserializeObject<List<ChatRecord>>(originalFileText);

            ChatRecord newChatRecord = new ChatRecord() { user = e.ChatMessage.DisplayName, msg = e.ChatMessage.Message };

            if (records == null)
                records = new List<ChatRecord>() { newChatRecord };
            else
                records.Add(newChatRecord);



            string jsonString = JsonConvert.SerializeObject(records);
            Console.WriteLine(e.ChatMessage.DisplayName + " : " + e.ChatMessage.Message);
            Console.WriteLine(jsonString);
            File.WriteAllText(filePath, jsonString);

        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e) { }

        public void SendMessage(String channel, String message)
        {
            this.client.SendMessage(channel, message);
            Console.WriteLine($"{this.USERNAME} : {message}");
        }

        private void printError(String? msg)
        {
            Console.WriteLine($"[ERROR]: {msg}");
        }
    }

}
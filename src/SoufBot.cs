using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using Newtonsoft.Json;
using soufBot.src.model;
using soufBot.src.tools;

namespace soufBot.src;

class TwitchBot
{
    TwitchClient client;
    public DatabaseConnection db;

    public string OAUTH_TOKEN = "";
    public string USERNAME = "";
    public string[] CHANNEL_LIST = new string[0];



    public TwitchBot()
    {
        try
        {

            printLog("Reading secrets file...");
            string secretFileText = File.ReadAllText("secret/secret.json");
            Secrets? secrets = JsonConvert.DeserializeObject<Secrets>(secretFileText);

            USERNAME = secrets?.USERNAME ?? "";

            OAUTH_TOKEN = secrets?.OAUTH_TOKEN ?? "";
            CHANNEL_LIST = secrets?.CHANNEL_LIST ?? new string[0];
            printLog("Finished reading secrets file!");

        }
        catch
        {
            printError("Could not read secrets and assign");
        }

        printLog("Connecting to twitch...");
        db = new DatabaseConnection();


        ConnectionCredentials credentials = new ConnectionCredentials(
            USERNAME,
            OAUTH_TOKEN
        );
        var clientOptions = new ClientOptions
        {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };
        WebSocketClient customClient = new WebSocketClient(clientOptions);
        client = new TwitchClient(customClient);

        foreach (string channel in CHANNEL_LIST!)
            client.Initialize(credentials, channel);

        client.OnLog += Client_OnLog!;
        client.OnJoinedChannel += Client_OnJoinedChannel!;
        client.OnMessageReceived += Client_OnMessageReceived!;
        client.OnWhisperReceived += Client_OnWhisperReceived!;
        client.OnNewSubscriber += Client_OnNewSubscriber!;
        client.OnConnected += Client_OnConnected!;

        client.Connect();

        printLog("Connected to twitch!");
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
        SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
    }

    private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
    {
        ChatMessage message = e.ChatMessage;
        int currentTime = Time.CurrentTimeSeconds();
        printLog($"Received message {message.Message} in {message.Channel} from {message.DisplayName}");

        ChatUser? chatUser = db.getUserFromChannel(message.DisplayName, message.Channel);

        printLog($"Attempted to pick up chatuser: {chatUser?.ToString()}");

        if (chatUser == null)
        {
            printLog($"{message.DisplayName} is new!");
            chatUser = new ChatUser(message.DisplayName, currentTime, Constants.SCORE_PER_MESSAGE);
        }
        else if (currentTime - chatUser.timeLastMessageAwarded > Constants.TIME_BETWEEN_MESSAGES)
        {
            printLog($"{chatUser.username} deserves some points");

            chatUser.score += Constants.SCORE_PER_MESSAGE;
            chatUser.timeLastMessageAwarded = currentTime;
        }
        else
        {
            printLog($"{chatUser.username} chatted recently already");
            return;
        }

        printLog($"Updating/inserting {chatUser.username}");
        bool userExisted = db.UpdateUserFromChannel(chatUser, message.Channel);

        printLog(userExisted ? "User has been found and is updated" : "User has not been found and is inserted");



    }

    private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
    {
    }

    private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e) { }

    public void SendMessage(string channel, string message)
    {
        client.SendMessage(channel, message);
        Console.WriteLine($"{USERNAME} : {message}");
    }

    private void printError(string? msg)
    {
        Console.WriteLine($"[ERROR]: {msg}");
    }

    private void printLog(string? msg)
    {
        Console.WriteLine($"[LOG]: {msg}");
    }
}
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

class TwitchBot {
    readonly TwitchClient client;
    readonly private DatabaseConnection db;

    readonly private string OAUTH_TOKEN = "";
    readonly private string USERNAME = "";
    readonly private string[] CHANNEL_LIST = Array.Empty<string>();

    private int indexOfChannelsJoined = 0;

    public TwitchBot() {
        try {
            PrintLog("Reading secrets file...");
            string secretFileText = File.ReadAllText("secret/secret.json");
            Secrets? secrets = JsonConvert.DeserializeObject<Secrets>(secretFileText);

            USERNAME = secrets?.USERNAME ?? "";

            OAUTH_TOKEN = secrets?.OAUTH_TOKEN ?? "";
            CHANNEL_LIST = secrets?.CHANNEL_LIST ?? Array.Empty<string>();
            PrintLog("Finished reading secrets file!");
        } catch {
            PrintError("Could not read secrets and assign");
        }

        PrintLog("Connecting to twitch...");
        db = new DatabaseConnection();

        var credentials = new ConnectionCredentials(USERNAME, OAUTH_TOKEN);
        var clientOptions = new ClientOptions {
            MessagesAllowedInPeriod = 750,
            ThrottlingPeriod = TimeSpan.FromSeconds(30)
        };
        var customClient = new WebSocketClient(clientOptions);
        client = new TwitchClient(customClient);

        client.Initialize(credentials, CHANNEL_LIST[0]);
        indexOfChannelsJoined++;

        client.OnLog += Client_OnLog!;
        client.OnJoinedChannel += Client_OnJoinedChannel!;
        client.OnMessageReceived += Client_OnMessageReceived!;
        client.OnWhisperReceived += Client_OnWhisperReceived!;
        client.OnNewSubscriber += Client_OnNewSubscriber!;
        client.OnConnected += Client_OnConnected!;

        client.Connect();

        PrintLog("Connected to twitch!");
    }

    private void Client_OnLog(object sender, OnLogArgs e) {
        // printLog($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data} ");
    }

    private void Client_OnConnected(object sender, OnConnectedArgs e) {
        PrintLog($"Connected to {e.AutoJoinChannel}");
    }

    private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e) {
        SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");

        if (indexOfChannelsJoined >= CHANNEL_LIST.Length)
            return;

        client.JoinChannel(CHANNEL_LIST[indexOfChannelsJoined]);
        indexOfChannelsJoined++;
    }

    private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e) {
        ChatMessage message = e.ChatMessage;
        int currentTime = Time.CurrentTimeSeconds();
        PrintLog(
            $"Received message {message.Message} in {message.Channel} from {message.DisplayName}"
        );

        ChatUser? chatUser = db.getUserFromChannel(message.DisplayName, message.Channel);
        PrintLog($"Attempted to pick up chatuser: {chatUser?.ToString()}");

        if (message.Message.StartsWith("soufbot "))
            try {
                HandleCommand(message.Message.Split(" ").Skip(1).ToArray(), message.Channel);

            } catch (Exception exception) {
                PrintError($"HandleCommand: {exception.Message}");

            }

        try {
            UpdateScoreFromUser(chatUser, message, currentTime);
        } catch (Exception exception) {
            PrintError($"UpdateScoreFromUser: {exception.Message}");
        }

    }

    private void HandleCommand(string[] args, string channel) {

        switch (args[0]) {
            case "leaderboard":
                PrintLeaderboard(channel);
                break;

            default:
                SendMessage(channel, "KonCha");
                break;
        }
    }

    private void PrintLeaderboard(string channel) {
        List<ChatUser> topUsers = db.getTopTenOfChannel(channel);
        int i = 0;
        string txt = string.Join(", ", topUsers.Select(e => $"{++i}). {e.username} "));
        SendMessage(channel, txt);
    }

    private void UpdateScoreFromUser(ChatUser? chatUser, ChatMessage message, int currentTime) {
        if (chatUser == null) {
            PrintLog($"{message.DisplayName} is new!");
            chatUser = new ChatUser(
                message.DisplayName,
                currentTime,
                Constants.SCORE_PER_MESSAGE,
                1
            );
        } else if (currentTime - chatUser.timeLastMessageAwarded > Constants.TIME_BETWEEN_MESSAGES) {
            PrintLog($"{chatUser.username} deserves some points");

            chatUser.score += Constants.SCORE_PER_MESSAGE;
            chatUser.timeLastMessageAwarded = currentTime;
            chatUser.messagesSent++;
        } else {
            chatUser.messagesSent++;
            PrintLog($"{chatUser.username} chatted recently already");
        }

        PrintLog($"Updating/inserting {chatUser.username}");
        bool userExisted = db.UpdateUserFromChannel(chatUser, message.Channel);

        PrintLog(
            userExisted
                ? "User has been found and is updated"
                : "User has not been found and is inserted"
        );
    }

    private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e) { }

    private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e) { }

    public void SendMessage(string channel, string message) {
        client.SendMessage(channel, message);
        PrintLog($"{USERNAME} : {message}");
    }

    private static void PrintError(string? msg) {
        PrintLog($"[ERROR]: {msg}");
    }

    private static void PrintLog(string? msg) {
        Console.WriteLine($"[LOG]: {msg}");
    }
}

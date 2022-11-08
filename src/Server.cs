using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using soufBot.src.model;

namespace soufBot.src;

public class Server {
    public const int port = 19727;

    public List<ConnectedClient> clients = new();

    public void Start() {

        TcpListener listener = new(IPAddress.Any, port);
        listener.Start();


        while (true) {
            TcpClient client = listener.AcceptTcpClient();
            PrintLog("Found new client!");
            Thread thread = new(() => HandleNewClient(client));
            thread.Start(client);

        }
    }

    public void HandleNewClient(TcpClient tcpClient) {

        // Buffer for reading data
        byte[] bytes = new byte[256];
        int streamLength;

        try {
            // Get a stream object for reading and writing
            NetworkStream stream = tcpClient.GetStream();


            streamLength = stream.Read(bytes, 0, bytes.Length);
            string channelName = System.Text.Encoding.ASCII.GetString(bytes, 0, streamLength);
            PrintLog($"RECEIVED: {channelName}");

            ConnectedClient client = new(tcpClient, channelName, stream);
            clients.Add(client);


        } catch (SocketException e) {
            PrintLog($"SocketException: {e}");
        } catch (IOException e) {
            PrintLog($"IOException: {e}");
        } catch (Exception e) {
            PrintLog($"Exception: {e}");
        }
    }


    public void SendDataToAll(TopRecord[] topRecords) {

        foreach (ConnectedClient client in clients) {
            var topRecord = topRecords.First((el) => el.channel == client.channel);
            if (topRecord == null) continue;

            var jsonText = JsonConvert.SerializeObject(topRecord);
            client.Send(jsonText);
        }


    }

    private static void PrintLog(string? msg) {
        Console.WriteLine($"[SERVER]: {msg}");
    }
}
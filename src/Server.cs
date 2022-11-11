using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using soufBot.src.model;
using soufBot.src.model.network;

namespace soufBot.src;

public class Server
{
    public const int port = 19727;

    public List<ConnectedClient> clients;

    public Server()
    {
        clients = new();
    }

    public void Start()
    {

        TcpListener listener = new(IPAddress.Any, port);
        listener.Start();


        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            PrintLog("Found new client!");
            Thread thread = new(() => HandleNewClient(client));
            thread.Start();

        }
    }

    public void HandleNewClient(TcpClient tcpClient)
    {

        // Buffer for reading data
        byte[] bytes = new byte[256];
        int streamLength;

        try
        {
            // Get a stream object for reading and writing
            NetworkStream stream = tcpClient.GetStream();


            streamLength = stream.Read(bytes, 0, bytes.Length);
            string channelName = Encoding.ASCII.GetString(bytes, 0, streamLength);
            PrintLog($"RECEIVED: {channelName}");

            ConnectedClient client = new(tcpClient, channelName, stream);
            clients.Add(client);


        }
        catch (SocketException e)
        {
            PrintLog($"SocketException: {e}");
        }
        catch (IOException e)
        {
            PrintLog($"IOException: {e}");
        }
        catch (Exception e)
        {
            PrintLog($"Exception: {e}");
        }
    }


    public void SendDataToAll(TopRecord[] topRecords)
    {

        for (int i = clients.Count - 1; i >= 0; i--)
        {
            ConnectedClient client = clients[i];

            TopRecord? topRecord = topRecords.FirstOrDefault((el) => el?.channel == client.channel, null);
            if (topRecord == null) continue;

            NetworkTopRecord networkObject = topRecord.ToNetworkObject();

            var jsonText = JsonConvert.SerializeObject(networkObject);
            try
            {
                client.Send(jsonText);
            }
            catch (IOException e)
            {
                PrintLog($"IOException (probably because they quit): {e}");
                client.Dispose();
                clients.Remove(client);

            }
        }


    }

    private static void PrintLog(string? msg)
    {
        Console.WriteLine($"[SERVER]: {msg}");
    }
}
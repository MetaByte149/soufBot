using System.Net.Sockets;
using System.Text;

namespace soufBot.src.model;

public class ConnectedClient {
    private readonly TcpClient client;
    public readonly string channel;
    private readonly NetworkStream stream;

    public ConnectedClient(TcpClient client, string channel, NetworkStream stream) {
        this.client = client;
        this.channel = channel;
        this.stream = stream;
    }

    public void Send(string msg) {

        byte[] msgBytes = Encoding.ASCII.GetBytes(msg);

        stream.Write(msgBytes, 0, msgBytes.Length);
        Console.WriteLine($"[{channel}] Sent: {msg}");
    }
}

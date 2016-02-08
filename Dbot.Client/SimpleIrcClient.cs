using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Dbot.Utility;

namespace Dbot.Client {
  public class SimpleIrcClient : IDisposable {
    private readonly string _server;
    private readonly int _port;
    private readonly string _nick;
    private readonly string _pass;
    private TcpClient _tcpClient;
    private NetworkStream _networkStream;
    private StreamReader _streamReader;
    private StreamWriter _streamWriter;

    public SimpleIrcClient(string server, int port, string nick, string pass = null) {
      _server = server;
      _port = port;
      _nick = nick;
      _pass = pass;
    }

    public void Connect() {
      try {
        _tcpClient = new TcpClient(_server, _port);
      } catch {
        Logger.Write("Connection Error");
        throw;
      }

      try {
        _networkStream = _tcpClient.GetStream();
        _streamReader = new StreamReader(_networkStream);
        _streamWriter = new StreamWriter(_networkStream);
        if (_pass != null)
          Send($"PASS {_pass}");
        Send($"NICK {_nick}");
        Send($"USER {_nick} {_nick} {_nick} :{_nick}");
      } catch {
        Logger.Write("Communication error");
        throw;
      }
    }

    public void Send(string data) {
      _streamWriter.WriteLine(data);
      _streamWriter.Flush();
      Logger.Write($"< {data}");
    }

    public void Run() {
      while (true) {
        var data = _streamReader.ReadLine();
        Logger.Write($"> {data}");
        if (data == null) continue;
        var charSeparator = new [] { ' ' };
        var ex = data.Split(charSeparator, 5);

        if (ex[0] == "PING") {
          Send($"PONG {ex[1]}");
        }
      }
    }

    public void Dispose() {
      _streamReader?.Close();
      _streamWriter?.Close();
      _networkStream?.Close();
      _tcpClient?.Close();
    }
  }
}

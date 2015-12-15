using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dbot.Common;
using Dbot.CommonModels;

namespace Dbot.Client {
  public class SampleClient : ConsolePrintClient {
    private IProcessor _processor;
    private readonly List<PublicMessage> _messageList = new List<PublicMessage>();

    public override async void Run(IProcessor processor) {
      _processor = processor;
      ParseRawIrc();
      foreach (var message in _messageList) {
        processor.ProcessMessage(message);
        await Task.Run(() => Task.Delay(100));
      }
    }

    public override void Forward(PublicMessage message) {
      _processor.ProcessMessage(message);
    }

    private void ParseRawIrc() {
      var regex = new Regex(@"<II> <(\S+)> (.*)");
      var lines = RawIrc.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
      foreach (var line in lines) {
        var match = regex.Match(line);
        if (match.Success) {
          var nick = match.Groups[1].Value;
          var text = match.Groups[2].Value;
          if (nick == "Bot") {
            //_messageList.Add(new ModPublicMessage(text));
          } else {
            _messageList.Add(new PublicMessage(nick, text));
          }
        }
      }
    }

    private const string RawIrc = @"

";
  }
}

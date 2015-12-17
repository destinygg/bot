namespace Dbot.CommonModels {
  public interface IClient {
    void Run(IProcessor processor);
    void Forward(PublicMessage message);
    void Send(PrivateMessage privateMessage);
    void Send(PublicMessage publicMessage);
    void Send(Mute mute);
    void Send(UnMuteBan unMuteBan);
    void Send(Subonly subonly);
    void Send(Ban ban);
  }
}
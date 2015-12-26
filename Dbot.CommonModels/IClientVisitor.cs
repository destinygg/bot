namespace Dbot.CommonModels {
  public interface IClientVisitor {
    void Run(IProcessor processor);
    void Forward(PublicMessage message);
    void Visit(PrivateMessage privateMessage);
    void Visit(PublicMessage publicMessage);
    void Visit(Mute mute);
    void Visit(UnMuteBan unMuteBan);
    void Visit(Subonly subonly);
    void Visit(Ban ban);

  }
}
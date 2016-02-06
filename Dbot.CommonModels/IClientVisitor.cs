// http://code.logos.com/blog/2010/03/the_visitor_pattern_and_dynamic_in_c_4.html
// Considered using `dynamic` to eliminate visitor pattern, but rejected due to the potential for runtime errors

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
    void Visit(Broadcast broadcast);
  }
}
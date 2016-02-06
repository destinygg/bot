using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class UnMuteBan : ISendable, ISendableVisitable {
    public UnMuteBan(string beneficiary) {
      this.Beneficiary = beneficiary;
    }

    public string Beneficiary { get; set; }

    public User Sender { get; set; }

    public string SenderName => Sender.Nick;


    public void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
    }

    public override string ToString() {
      return "Unbanned " + Beneficiary;
    }
  }
}
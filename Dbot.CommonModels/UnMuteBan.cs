using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class UnMuteBan : ISent, ISendableVisitable {
    public UnMuteBan(string beneficiary) {
      this.Beneficiary = beneficiary;
    }

    public string Beneficiary { get; set; }

    public void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
    }

    public override string ToString() {
      return "Unbanned " + Beneficiary;
    }
  }
}
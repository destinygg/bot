using Dbot.WebSocketModels;
using Newtonsoft.Json;

namespace Dbot.CommonModels {
  public class UnMuteBan : ISendable, ISendableVisitable {
    public UnMuteBan(string beneficiary) {
      this.Beneficiary = beneficiary;
    }

    public string Beneficiary { get; set; }

    private string _sender;
    public string Sender {
      get { return _sender; }
      set { _sender = value.ToLower(); }
    }


    public void Accept(IClientVisitor visitor) {
      visitor.Visit(this);
    }

    public override string ToString() {
      return "Unbanned " + Beneficiary;
    }
  }
}
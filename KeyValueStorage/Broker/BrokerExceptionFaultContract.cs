using System.Runtime.Serialization;

namespace Broker
{
    [DataContractAttribute]
    public class BrokerExceptionFaultContract
    {
        public BrokerExceptionFaultContract(string message)
        {
            this.Message = message;
        }

        [DataMemberAttribute]
        public string Message { get; set; }
    }
}

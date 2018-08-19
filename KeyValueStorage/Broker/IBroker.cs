using System.ServiceModel;

namespace Broker
{
    [ServiceContract]
    public interface IBroker
    {
        [OperationContract]
        [FaultContract(typeof(BrokerExceptionFaultContract))]
        string StoreData(byte[] value);

        [OperationContract]
        [FaultContract(typeof(BrokerExceptionFaultContract))]
        byte[] RetrieveData(string key);

        [OperationContract]
        [FaultContract(typeof(BrokerExceptionFaultContract))]
        void DeleteData(string key);
    }
}

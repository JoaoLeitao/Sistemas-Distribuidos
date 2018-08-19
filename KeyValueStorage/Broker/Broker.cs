using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Configuration;
using StorageInterface;

namespace Broker
{
    class Broker : IBroker
    {
        private readonly List<IKV> storages = new List<IKV>();

        public Broker()
        {
            ClientSection section = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");

            foreach (ChannelEndpointElement elem in section.Endpoints)
                storages.Add((IKV)Activator.GetObject(typeof(IKV), elem.Address.ToString()));
        }

        public string StoreData(byte[] value)
        {
            try
            {
                IKV storage = GetStorageWithLessPairs();
                string key = storage.StoreData(value);
                Console.WriteLine("\nValue stored with success, with the key - {0}!", key);
                return key;
            } catch(FaultException<BrokerExceptionFaultContract> e)
            {
                Console.WriteLine("\n" + e.Detail.Message);
                throw new FaultException<BrokerExceptionFaultContract>(new BrokerExceptionFaultContract(e.Message), new FaultReason(e.Message));
            }
        }

        public byte[] RetrieveData(string key)
        {
            if (key == null)
            {
                Console.WriteLine("\nCouldn't retrieve the value with a null key!");
                throw new FaultException<BrokerExceptionFaultContract>(new BrokerExceptionFaultContract("Couldn't retrieve the value with a null key!"), new FaultReason("Couldn't retrieve the value with a null key!"));
            }

            IEnumerable<byte> toReturn = Enumerable.Empty<byte>();

            //retrieve all values associated with the given key
            foreach (IKV storage in storages)
            {
                byte[] value = storage.RetrieveData(key);
                if (value != null) toReturn = toReturn.Concat(value);
            }

            if(toReturn.Count() > 0)
            {
                Console.WriteLine("\nValue retrieved with success, with the key - {0}!", key);

                return toReturn.ToArray();
            }

            Console.WriteLine("\nThe provided key doesn't exist in storage!");
            throw new FaultException<BrokerExceptionFaultContract>(new BrokerExceptionFaultContract("The provided key doesn't exist in storage!"), new FaultReason("The provided key doesn't exist in storage!"));
        }

        public void DeleteData(string key)
        {
            if (key == null)
            {
                Console.WriteLine("\nCouldn't retrieve the value with a null key!");
                throw new FaultException<BrokerExceptionFaultContract>(new BrokerExceptionFaultContract("Couldn't retrieve the value with a null key!"), new FaultReason("Couldn't retrieve the value with a null key!"));
            }

            int deleted = 0;

            //delete all values associated with the given key
            foreach (IKV storage in storages)
            {
                byte[] value = storage.DeleteData(key);
                if (value != null) ++deleted;
            }

            if (deleted == storages.Count())
            {
                Console.WriteLine("\nValue deleted with success, with the key - {0}!", key);
                return;
            }

            Console.WriteLine("\nCouldn't delete the value with success!");
            throw new FaultException<BrokerExceptionFaultContract>(new BrokerExceptionFaultContract("Couldn't delete the value with success!"), new FaultReason("Couldn't delete the value with success!"));
        }

        private IKV GetStorageWithLessPairs()
        {
            try
            {
                return storages.OrderBy(s => s.Count()).First();
            } catch(OverflowException e)
            {
                Console.WriteLine("\n" + e.Message);
                throw new FaultException<BrokerExceptionFaultContract>(new BrokerExceptionFaultContract(e.Message), new FaultReason(e.Message));
            }
        }
    }
}

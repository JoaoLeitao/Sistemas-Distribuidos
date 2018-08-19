using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using App.Broker1;

namespace App
{
    class Program
    {
        private static readonly List<BrokerClient> brokers = new List<BrokerClient>();
        private static readonly Dictionary<string, Action> methods = new Dictionary<string, Action>();
        private static readonly Random random = new Random();
        private static bool exit = false;
        private static BrokerClient currentBroker;

        static void Main(string[] args)
        {
            InitMethodsDictionary();

            currentBroker = GetRandomBroker();

            do
            {
                Console.WriteLine("\nCommands of the application:\n" +
                    "Insert s to store data.\n" +
                    "Insert r to retrieve data.\n" +
                    "Insert d to delete data.\n" +
                    "Insert e to exit the application.\n");

                string command = Console.ReadLine().ToLower();

                if (methods.TryGetValue(command, out var method))
                    RunMethod(method);

            } while (!exit);
        }

        private static void InitMethodsDictionary()
        {
            methods.Add("s", StoreData);
            methods.Add("r", RetrieveData);
            methods.Add("d", DeleteData);
            methods.Add("e", Exit);
        }

        private static BrokerClient GetRandomBroker()
        {
            ClientSection section = (ClientSection)ConfigurationManager.GetSection("system.serviceModel/client");

            foreach (ChannelEndpointElement elem in section.Endpoints)
                brokers.Add(new BrokerClient(elem.Name));

            return brokers.ElementAt(random.Next(brokers.Count()));
        }

        private static void RunMethod(Action method)
        {
            try
            {
                method();
            }
            catch (Exception)   //in case of broker failure, another broker is chosen
            {
                BrokerFailureRecovery(method);
            }
        }

        private static void BrokerFailureRecovery(Action method)
        {
            foreach (BrokerClient broker in brokers.Where(elem => currentBroker != elem))
            {
                try
                {
                    currentBroker = broker;
                    method();
                    return;     //if the broker has success, there's no need to chose another
                }
                catch (Exception)
                {
                    //do nothing
                }
            }
            Console.WriteLine("No brokers available!");
        }

        private static void StoreData()
        {
            Console.Write("\nInsert the data to be stored: ");

            byte[] data = Encoding.UTF8.GetBytes(Console.ReadLine());

            try
            {
                string key = currentBroker.StoreData(data);
                Console.WriteLine("\nValue stored with success, with the key - {0}!", key);
            } catch(FaultException<BrokerExceptionFaultContract> e)
            {
                Console.WriteLine("\n" + e.Detail.Message);
            }
        }

        private static void RetrieveData()
        {
            Console.Write("\nInsert the key to retrieve the data: ");

            try
            {
                byte[] data = currentBroker.RetrieveData(Console.ReadLine());
                string stringData = Encoding.UTF8.GetString(data);
                Console.WriteLine("\nRetrieved data - {0}!", stringData);
            } catch(FaultException<BrokerExceptionFaultContract> e)
            {
                Console.WriteLine("\n" + e.Detail.Message);
            }
        }

        private static void DeleteData()
        {
            Console.Write("\nInsert the key to delete the data: ");

            try
            {
                string key = Console.ReadLine();
                currentBroker.DeleteData(key);
                Console.WriteLine("\nDeleted data with the key - {0}!", key);
            } catch (FaultException<BrokerExceptionFaultContract> e)
            {
                Console.WriteLine("\n" + e.Detail.Message);
            }
        }

        private static void Exit()
        {
            exit = true;
        }
    }
}

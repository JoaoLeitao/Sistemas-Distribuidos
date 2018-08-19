using System;
using System.ServiceModel;

namespace Broker
{
    class Program
    {
        static void Main(string[] args)
        {
            //using is to ensure that the object is disposed as soon as it goes out of scope
            using(ServiceHost service = new ServiceHost(typeof(Broker)))
            {
                service.Open();

                Console.WriteLine("Broker initiated!\nWaiting requests!\nClick Enter to terminate!");
                Console.ReadLine();
            }
        }
    }
}

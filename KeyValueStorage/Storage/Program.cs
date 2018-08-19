using System;
using System.Runtime.Remoting;

namespace Storage
{
    class Program
    {
        static void Main(string[] args)
        {
            string configFile = "Storage.exe.config";
            RemotingConfiguration.Configure(configFile, false);
            Console.WriteLine("Storage server initiated!\nWaiting requests!\nClick Enter to terminate!");
            Console.ReadLine();
        }
    }
}

using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using StorageInterface;

namespace Storage
{
    class KV : MarshalByRefObject, IKV
    {
        private readonly ConcurrentDictionary<string, byte[]> keyValuePairs = new ConcurrentDictionary<string, byte[]>();

        private long key = 0;   //key used to pair with the values stored

        public int Count()
        {
            try
            {
                return keyValuePairs.Count();
            } catch(OverflowException)
            {
                Console.WriteLine("\nStorage is full!");
                throw new OverflowException("Storage is full!");
            }
        }

        public string StoreData(byte[] value)
        {
            Interlocked.Increment(ref key);     //thread safe increment of the key

            string k = $"{key}";

            try
            {
                if (keyValuePairs.TryAdd(k, value))
                {
                    Console.WriteLine("\nValue stored with success, with the key - {0}!", k);
                    return k;
                }
            } catch(OverflowException)
            {
                Console.WriteLine("\nStorage is full!");
                throw new OverflowException("Storage is full!");
            }

            Console.WriteLine("\nCouldn't store the value, because the key already exists!");

            return null;
        }

        public byte[] RetrieveData(string key)
        {
            if(key != null)
            {
                if (keyValuePairs.TryGetValue(key, out byte[] val))
                {
                    Console.WriteLine("\nValue retrieved with success, with the key - {0}!", key);
                    return val;
                }

                Console.WriteLine("\nThe provided key doesn't exist in storage!");

                return null;
            }

            Console.WriteLine("\nCouldn't retrieve the value with a null key!");

            return null;
        }

        public byte[] DeleteData(string key)
        {
            if(key != null)
            {
                if (keyValuePairs.TryRemove(key, out byte[] val))
                {
                    Console.WriteLine("\nValue deleted with success, with the key - {0}!", key);
                    return val;
                }

                Console.WriteLine("\nCouldn't delete the value with success!");

                return null;
            }

            Console.WriteLine("\nCouldn't retrieve the value with a null key!");

            return null;
        }
    }
}

namespace StorageInterface
{
    public interface IKV
    {
        int Count();

        string StoreData(byte[] value);

        byte[] RetrieveData(string key);

        byte[] DeleteData(string key);
    }
}

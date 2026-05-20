namespace Zaykov.SimpleStore
{
    public interface ISimpleStore
    {
        void Set(string key, byte[] value);
        byte[] Get(string key);
        void Delete(string key);
    }

    public class SimpleStore : ISimpleStore
    {
        private Dictionary<string, byte[]> _store;
        public SimpleStore()
        {
            _store = new Dictionary<string, byte[]>();
        }
        public void Delete(string key)
        {
            _store.Remove(key);
        }

        public byte[] Get(string key)
        {
            return _store[key];
        }

        public void Set(string key, byte[] value)
        {
            _store[key] = value;
        }
    }
}

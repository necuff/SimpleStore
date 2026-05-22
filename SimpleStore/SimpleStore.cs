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
            if(key is null)
                throw new ArgumentNullException(nameof(key));

            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be empty or whitespace", nameof(key));

            _store.Remove(key);
        }

        public byte[] Get(string key)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if (!_store.TryGetValue(key, out var value))
                throw new KeyNotFoundException($"Key '{key}' not found");

            return value;
        }

        public void Set(string key, byte[] value)
        {
            if (key is null)
                throw new ArgumentNullException(nameof(key));

            if(value is null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length == 0)
                throw new ArgumentException("Value cannot be empty", nameof(value));
            
            _store[key] = value;
        }
    }
}

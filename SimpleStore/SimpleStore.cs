namespace Zaykov.SimpleStore
{
    public interface ISimpleStore
    {
        void Set(string key, byte[] value);
        byte[] Get(string key);
        void Delete(string key);
    }

    public class SimpleStore : ISimpleStore, IDisposable
    {
        private Dictionary<string, byte[]> _store;
        private readonly ReaderWriterLockSlim _lock;

        //statistic
        private long _getCount = 0;
        private long _setCount = 0;
        private long _deleteCount = 0;
        

        public SimpleStore()
        {
            _store = new Dictionary<string, byte[]>();
            _lock = new ReaderWriterLockSlim();
        }
        public void Delete(string key)
        {
            _lock.EnterWriteLock();
            try
            {
                if (key is null)
                    throw new ArgumentNullException(nameof(key));

                if (string.IsNullOrEmpty(key))
                    throw new ArgumentException("Key cannot be empty or whitespace", nameof(key));

                Interlocked.Increment(ref _deleteCount);
                _store.Remove(key);
            }
            finally 
            { 
                _lock.ExitWriteLock(); 
            }
        }

        public byte[] Get(string key)
        {
            _lock.EnterReadLock();
            try
            {
                if (key is null)
                    throw new ArgumentNullException(nameof(key));

                if (!_store.TryGetValue(key, out var value))
                    throw new KeyNotFoundException($"Key '{key}' not found");

                Interlocked.Increment(ref _getCount);
                return value;
            }
            finally 
            { 
                _lock.ExitReadLock();
            }
            
        }

        public void Set(string key, byte[] value)
        {
            _lock.EnterWriteLock();
            try
            {
                if (key is null)
                    throw new ArgumentNullException(nameof(key));

                if (value is null)
                    throw new ArgumentNullException(nameof(value));

                if (value.Length == 0)
                    throw new ArgumentException("Value cannot be empty", nameof(value));

                Interlocked.Increment(ref _setCount);
                _store[key] = value;
            }
            finally 
            { 
                _lock.ExitWriteLock(); 
            }
            
        }

        public (long, long, long) GetStatistic()
        {
            return new(_getCount, _setCount, _deleteCount);
        }

        public void Dispose()
        {
            _lock?.Dispose();
        }
    }
}

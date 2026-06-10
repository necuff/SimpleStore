

using System.Text;

namespace Zaykov.SimpleStore.Tests
{
    public class SimpleStoreTests
    {
        int threadCount = 10;
        int operationsCount = 100;

        [Fact]
        public async Task ParallelSetAndGet_ShouldMaintainDataIntegrityAndCounters()
        {
            //arrange
            var store = new SimpleStore();
            long _localGetCounter = 0;
            long _localSetCounter = 0;
            int exceptionCounter = 0;

            //Act
            var tasks = new List<Task>();
            
            for(int i = 0; i< threadCount; i++)
            {
                int threadId = i;
                tasks.Add(Task.Run(() =>
                {
                    for(int j = 0; j < operationsCount; j++)
                    {
                        string key = $"key_threadId_{i}_operation_{j}";
                        store.Set(key, Encoding.UTF8.GetBytes(key));
                        _localSetCounter++;
                    }
                }));
            }

            for (int i = 0; i < threadCount; i++)
            {
                int threadId = i;
                tasks.Add(Task.Run(() =>
                {
                    for (int j = 0; j < operationsCount; j++)
                    {
                        string key = $"key_threadId_{i}_operation_{j}";                        

                        try
                        {
                            var value = store.Get(key);
                            _localGetCounter++;
                        }
                        catch (Exception ex) 
                        {
                            exceptionCounter++;
                        }

                    }
                }));
            }

            await Task.WhenAll(tasks);

            var statistic = store.GetStatistic();

            //Assert            
            Assert.Equal(statistic.Item2, _localSetCounter);
            Assert.Equal(statistic.Item1, _localGetCounter);
            Assert.Equal(0, exceptionCounter);
        }
    }
}

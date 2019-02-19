using Newtonsoft.Json;
using odec.Framework.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemExchange.Interop.Workers;

namespace Exchange.Runner.TestRealization
{
    public class ExampleProcessWorker : IProcessPackageWorker
    {
        protected readonly ConcurrentQueue<IList<TestFakeObject>> Queue = new ConcurrentQueue<IList<TestFakeObject>>();
        public ExampleProcessWorker()
        {

        }
        public ExampleProcessWorker(int test)
        {
            LogEventManager.Logger.Info("Called parametrized ctor:" + test);
        }

        public bool IsOperationCompleted { get; set; }
        public bool Process<T>(T dataSource) where T : IProcessDataSource
        {
            LogEventManager.Logger.Info("Processing started");
            IList<TestFakeObject> fakePackage;
            var dirName = "dump";
            var fileName = "dumb.txt";
            Directory.CreateDirectory(dirName);
            if (!File.Exists(dirName + fileName))
            {
                File.Create(dirName + fileName);
            }
            File.Create(dirName + fileName);
            var packageLines = new List<string>();
            while (Queue.TryDequeue(out fakePackage))
            {
                foreach (var fake in fakePackage)
                    fake.SortOrder += fake.Id;

                packageLines.Add(JsonConvert.SerializeObject(fakePackage));
            }
            File.AppendAllLines(dirName + fileName, packageLines);




            IsOperationCompleted = true;
            return true;

            //var source = (AuctionDataSource)(dataSource as IProcessDataSource);

        }

        public bool IsBufferEmpty => Queue.IsEmpty;

        public void GetProcessPackage<T>(T dataSource, IExchangeSettings settings) where T : IProcessDataSource
        {
            LogEventManager.Logger.Info("Formation of packages started");
            var rawData = TestFakeObject.GenerateFakeObjects(1000000);
            var forQueueing = rawData.Select((it, i) => new { Index = i, Value = it })
                .GroupBy(it => it.Index / settings.PackageSize)
                .Select(it => it.Select(v => v.Value).ToList())
                .ToList();

            foreach (var item in forQueueing)
                Queue.Enqueue(item);
        }
    }
}
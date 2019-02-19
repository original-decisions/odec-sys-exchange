using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SystemExchange.Interop.Workers;
using Newtonsoft.Json;
using odec.Framework.Logging;

namespace Exchange.Runner.TestRealization
{
    public class ExampleExportWorker : ISendWorker
    {
        protected readonly ConcurrentQueue<IList<TestFakeObject>> Queue = new ConcurrentQueue<IList<TestFakeObject>>();
        public ExampleExportWorker()
        {

        }
        public ExampleExportWorker(int test)
        {
            LogEventManager.Logger.Info("Called parametrized ctor:" + test);
        }

        public bool IsBufferEmpty => Queue.IsEmpty;


        #region Implementation of ISendWorker

        /// <inheritdoc />
        public bool IsSendQueueEmpty => IsBufferEmpty;

        /// <inheritdoc />
        public void Send()
        {
            LogEventManager.Logger.Info("Import started");
            var dirName = "dump";
            var fileName = "dumb.txt";
            Directory.CreateDirectory(dirName);
            if (!File.Exists(dirName + fileName))
            {
                File.Create(dirName + fileName);
            }
            File.Create(dirName + fileName);
            var packageLines = new List<string>();
            while (Queue.TryDequeue(out var fakePackage))
            {
                foreach (var fake in fakePackage)
                    fake.SortOrder += fake.Id;

                packageLines.Add(JsonConvert.SerializeObject(fakePackage));
            }
            File.AppendAllLines(dirName + fileName, packageLines);
        }

        /// <inheritdoc />
        public void RefreshSendCollection(IExchangeSettings settings)
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

        #endregion
    }
}
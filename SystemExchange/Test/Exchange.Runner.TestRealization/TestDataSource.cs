using odec.Framework.Infrastructure;
using odec.Framework.Logging;
using System;
using SystemExchange.Interop.Workers;
using odec.Framework.SysExchange.Interop.Workers;

namespace Exchange.Runner.TestRealization
{
    public class TestDataSource : IProcessDataSource,IReceiveDataSource, ISenderDataSource
    {
        public TestDataSource()
        {

        }

        public TestDataSource(int test)
        {
            LogEventManager.Logger.Info("Passed parameter to Source:" + test);
        }
        public string DbConnection
        {
            get
            {
                if (ConnectionManager.MainConnection == null)
                    throw new InvalidOperationException("В файле конфигураций отсуствует строка подключения к БД");

                return ConnectionManager.MainConnection;
            }
        }
    }
}

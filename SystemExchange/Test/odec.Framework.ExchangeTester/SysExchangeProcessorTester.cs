using Autofac;
using NUnit.Framework;
using odec.Framework.SysExchange.DataProcessors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace odec.Framework.ExchangeTester
{
    public class SysExchangeProcessorTester : Tester
    {
        public SysExchangeProcessorTester()
        {

        }

        [Test]
        public void StartProcessor()
        {

            var svc = new ProcessDataService<Task>();
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());

        }

        [Test]
        public void StopProcessor()
        {
            var svc = new ProcessDataService<Task>();
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.Stop(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
        }

        [Test]
        public void SetWorkerParamsAndStartProcessor()
        {
            var svc = new ProcessDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassWorkerParams(new List<TypedParameter>
            {
                new TypedParameter(typeof(int),5)
            }));
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));
        }

        [Test]
        public void SetWorkerParamsNullAndStartProcessor()
        {
            var svc = new ProcessDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassWorkerParams(null));
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));
        }

        [Test]
        public void SetSourceParamsAndStartProcessor()
        {
            var svc = new ProcessDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassSourceParams(new List<TypedParameter>
            {
                new TypedParameter(typeof(int),5)
            }));
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));
        }

        [Test]
        public void SetSourceParamsNullAndStartProcessor()
        {
            var svc = new ProcessDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassSourceParams(null));
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));
        }

    }
}

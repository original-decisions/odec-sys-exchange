using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using NUnit.Framework;
using odec.Framework.SysExchange.DataProcessors;

namespace odec.Framework.ExchangeTester
{
    public class SysExchangeExporterTester : Tester
    {
        [Test]
        public void StartExporter()
        {
            
            var svc = new ExportDataService<Task>();
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
        }

        [Test]
        public void StopExporter()
        {
            var svc = new ExportDataService<Task>();
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));
        }

        [Test]
        public void SetWorkerParamsAndStartExporter()
        {
            var svc = new ExportDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassWorkerParams(new List<TypedParameter>
            {
                new TypedParameter(typeof(int),5)
            }));
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));
        }

        [Test]
        public void SetSourceParamsAndStartExporter()
        {
            var svc = new ExportDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassSourceParams(new List<TypedParameter>
            {
                new TypedParameter(typeof(int),5)
            }));
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));
        }

        [Test]
        public void SetWorkerParamsNullAndStartExporter()
        {
            var svc = new ExportDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassWorkerParams(null));
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));
        }

        [Test]
        public void SetSourceParamsNullAndStartExporter()
        {
            var svc = new ExportDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassSourceParams(null));
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));
        }
    }
}

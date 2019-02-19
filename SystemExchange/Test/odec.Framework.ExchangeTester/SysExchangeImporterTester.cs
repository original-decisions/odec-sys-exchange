
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;
using odec.Framework.SysExchange.DataProcessors;

namespace odec.Framework.ExchangeTester
{
    public class SysExchangeImporterTester : Tester
    {
        public SysExchangeImporterTester()
        {
            
        }

        [Test]
        public void StartImport()
        {
            
            var svc = new ImportDataService<Task>();
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
        }

        [Test]
        public void StopImport()
        {
            var svc = new ImportDataService<Task>();
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.Stop(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
        }

        [Test]
        public void SetWorkerParamsAndStartImporter()
        {
            var svc = new ImportDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassWorkerParams(new List<TypedParameter>
            {
                new TypedParameter(typeof(int),5)
            }));
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.Stop(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
        }

        [Test]
        public void SetWorkerParamsNullAndStartImporter()
        {
            var svc = new ImportDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassWorkerParams(null));
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));
        }

        [Test]
        public void SetSourceParamsAndStartImporter()
        {
            var svc = new ImportDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassSourceParams(new List<TypedParameter>
            {
                new TypedParameter(typeof(int),5)
            }));
            Assert.DoesNotThrow(() => svc.Start(null));
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));

        }
        [Test]
        public void SetSourceParamsNullAndStartImporter()
        {
            var svc = new ImportDataService<Task>();
            Assert.DoesNotThrow(() => svc.PassSourceParams(null));
            Assert.DoesNotThrow(() => svc.Start(null));
            
            Assert.DoesNotThrow(() => svc.ForceWait());
            Assert.DoesNotThrow(() => svc.Stop(null));
        }
#if NETCOREAPP2_1 || NETCOREAPP2_0
        
        [Ignore("Just for the test purposes")]
        public void TestRealService()
        {
            
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                Console.Title = "PMS API";

                var host = BuildWebHost(null);
                // InitializeDatabase(host.Services);
                host.Run();
                //var host = new WebHostBuilder()
                //    .UseKestrel()
                //    .UseUrls("http://localhost:58783/")
                //    .UseContentRoot(Directory.GetCurrentDirectory())
                //    .UseIISIntegration()
                //    .UseStartup<Startup>()
                //    .Build();

                //host.Run(); 
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }
#endif
    }
}

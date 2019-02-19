using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Web;
using NUnit.Framework;
using odec.Framework.SysExchange.DataProcessors;
using odec.Framework.SysExchange.Helpers;
using odec.Framework.SysExchange.Interop;
using PMS.Service;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace odec.Framework.ExchangeTester
{
    public class Tester
    {
        [OneTimeSetUp]
        public virtual void Init()
        {
#if NET452
            Assembly assembly = Assembly.GetCallingAssembly();

            AppDomainManager manager = new AppDomainManager();
            FieldInfo entryAssemblyfield = manager.GetType().GetField("m_entryAssembly", BindingFlags.Instance | BindingFlags.NonPublic);
            entryAssemblyfield.SetValue(manager, assembly);

            AppDomain domain = AppDomain.CurrentDomain;
            FieldInfo domainManagerField = domain.GetType().GetField("_domainManager", BindingFlags.Instance | BindingFlags.NonPublic);
            domainManagerField.SetValue(domain, manager);
#endif
            var config = CfgUtils.LookupDefaultConfigs();
            var serviceOptions = new ServiceOptions();
            var exchangeSection = config.GetSection("Exchange");
            var serviceOptionSection = exchangeSection.GetSection("Service");
            serviceOptionSection?.Bind(serviceOptions);

            CfgUtils.SetupIoC(config);
            ProcessDataService<Task>.Configuration = config;
            ImportDataService<Task>.Configuration = config;
            ExportDataService<Task>.Configuration = config;
        }

#if NETCOREAPP2_0 || NETCOREAPP2_1


        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseUrls("http://localhost:58785/")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    // delete all default configuration providers
                    //config.Sources.Clear();
                    config.AddJsonFile("lookupAssemblies.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile("exchangeCfg.json", optional: true, reloadOnChange: true);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
                .UseNLog()
                .UseStartup<Startup>()
                .UseIISIntegration()
                .UseApplicationInsights()
                .Build();

#endif
    }
}

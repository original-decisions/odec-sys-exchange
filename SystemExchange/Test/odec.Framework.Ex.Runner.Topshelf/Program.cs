using Autofac;
using Microsoft.Extensions.Configuration;
using odec.Framework.Ex.Runner.Topshelf.DataProcessors;
using odec.Framework.Infrastructure.Autofac;
using odec.Framework.SysExchange.DataProcessors;
using System.IO;
using odec.Framework.SysExchange.Helpers;
using odec.Framework.SysExchange.Interop;
using Topshelf;

namespace WinServices.AuctionProcessing
{
    public class Program
    {
        public static IConfiguration Configuration { get; set; }


        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("AutofacCfg.json", optional: true, reloadOnChange: true)
                .AddJsonFile("lookupAssemblies.json", optional: true, reloadOnChange: true)
                .AddJsonFile("exchangeCfg.json", optional: true, reloadOnChange: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            // Configuration = CfgUtils.LookupDefaultConfigs();
            var serviceOptions = new ServiceOptions();
            var exchangeSection = Configuration.GetSection("Exchange");
            var serviceOptionSection = exchangeSection.GetSection("Service");
            serviceOptionSection?.Bind(serviceOptions);
            //var registrantSection = exchangeSection.GetSection("Registrant");
            //var registrantOptions = new RegistrantOptions();

            //if (registrantSection != null)
            //    registrantSection.Bind(registrantOptions);
            //else
            //    registrantOptions.InitDefault();

            //var ioCBuilder = new ContainerBuilder();
            //ioCBuilder.RegisterModule(new ModuleRegistrant(Configuration, registrantOptions));
            //IoCHelper.Container = ioCBuilder.Build();
            CfgUtils.SetupIoC(Configuration);

            ProcessDataService<HostControl>.Configuration = Configuration;
            //инициализация сервиса
            switch (serviceOptions.Type)
            {
                case "ProcessService":
                    HostFactory.Run(x =>
                    {
                        x.Service<ProcessDataService>();//Как описание использовать ConverterService
                        x.RunAsLocalSystem();//Запускать под учетной записью System
                        x.StartManually();//Запуск вручную
                        x.SetServiceName(serviceOptions.Name);
                        x.SetDescription(string.Format("{0} - {1} ", serviceOptions.Name, serviceOptions.Description));
                        x.SetDisplayName(serviceOptions.Name);
                    });
                    break;
                case "ExportService":
                    HostFactory.Run(x =>
                    {
                        x.Service<ExportDataService>();//Как описание использовать ConverterService
                        x.RunAsLocalSystem();//Запускать под учетной записью System
                        x.StartManually();//Запуск вручную
                        x.SetServiceName(serviceOptions.Name);
                        x.SetDescription(string.Format("{0} - {1} ", serviceOptions.Name, serviceOptions.Description));
                        x.SetDisplayName(serviceOptions.Name);
                    });
                    break;
                case "ImportService":
                    HostFactory.Run(x =>
                    {
                        x.Service<ImportDataService>();//Как описание использовать ConverterService
                        x.RunAsLocalSystem();//Запускать под учетной записью System
                        x.StartManually();//Запуск вручную
                        x.SetServiceName(serviceOptions.Name);
                        x.SetDescription(string.Format("{0} - {1} ", serviceOptions.Name, serviceOptions.Description));
                        x.SetDisplayName(serviceOptions.Name);
                    });
                    break;
            }

        }
    }
}


#if !NETCOREAPP1_0


using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using odec.Framework.Ex.Runner.Topshelf.DataProcessors;
using odec.Framework.SysExchange.DataProcessors;
using odec.Framework.SysExchange.Helpers;
using odec.Framework.SysExchange.Interop;
using Topshelf;

namespace odec.Framework.ExchangeTester
{
    public class TopshelfSysExchangeTester : Tester
    {

        public TopshelfSysExchangeTester()
        {
            
        }

        [Test]
        public void Start()
        {
            var config =CfgUtils.LookupDefaultConfigs();
            var serviceOptions = new ServiceOptions();
            var exchangeSection = config.GetSection("Exchange");
            var serviceOptionSection = exchangeSection.GetSection("Service");
            serviceOptionSection?.Bind(serviceOptions);

            CfgUtils.SetupIoC(config);

            ImportDataService<HostControl>.Configuration = config;
            //RedmineImportWorker.Configuration = Configuration;

            //инициализация сервиса
            switch (serviceOptions.Type)
            {
                case "ProcessService":
                    HostFactory.Run(x =>
                    {
                        x.Service<ProcessDataService>();//Как описание использовать ConverterService
                        x.RunAsLocalSystem();//Запускать под учетной записью System
                        x.StartManually();//Запуск вручную
                        //x.UseNLog();
                        x.SetServiceName(serviceOptions.Name);
                        x.SetDescription($"{serviceOptions.Name} - {serviceOptions.Description} ");
                        x.SetDisplayName(serviceOptions.Name);
                    });
                    break;
                case "ExportService":
                    HostFactory.Run(x =>
                    {
                        x.Service<ExportDataService>();//Как описание использовать ConverterService
                        x.RunAsLocalSystem();//Запускать под учетной записью System
                        x.StartManually();//Запуск вручную
                       // x.UseNLog();
                        x.SetServiceName(serviceOptions.Name);
                        x.SetDescription($"{serviceOptions.Name} - {serviceOptions.Description} ");
                        x.SetDisplayName(serviceOptions.Name);
                    });
                    break;
                case "ImportService":
                    HostFactory.Run(x =>
                    {
                        x.Service<ImportDataService>();//Как описание использовать ConverterService
                        x.RunAsLocalSystem();//Запускать под учетной записью System
                        x.StartManually();//Запуск вручную
                       // x.UseNLog();
                        x.SetServiceName(serviceOptions.Name);
                        x.SetDescription($"{serviceOptions.Name} - {serviceOptions.Description} ");
                        x.SetDisplayName(serviceOptions.Name);
                    });
                    break;
            }
        }

        [Test]
        public void Stop()
        {

        }

    }
}
#endif
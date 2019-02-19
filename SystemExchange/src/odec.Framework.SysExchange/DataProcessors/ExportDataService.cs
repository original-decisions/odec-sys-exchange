using Autofac;
using Microsoft.Extensions.Configuration;
using odec.Framework.Infrastructure.Autofac;
using odec.Framework.Logging;
using odec.Framework.SysExchange.Interop.Workers;
using odec.Framework.SysExchange.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SystemExchange.Interop.Workers;

namespace odec.Framework.SysExchange.DataProcessors
{
    public class ExportDataService<T>
    {

        protected IEnumerable<TypedParameter> WorkerParameters = new List<TypedParameter>();
        protected IEnumerable<TypedParameter> SourceParameters = new List<TypedParameter>();
        private Task _task;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        public static IConfiguration Configuration { get; set; }

        public ExportDataService()
        {

        }

        public ExportDataService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void PassWorkerParams(IEnumerable<TypedParameter> parameters)
        {
            WorkerParameters = parameters;
        }

        public void PassSourceParams(IEnumerable<TypedParameter> parameters)
        {
            SourceParameters = parameters;
        }

        protected void InitCfg()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("exchangeCfg.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
        }
        public bool Start(T hostControl)
        {
            try
            {
                LogEventManager.Logger.Info("Begin start ExportDataService");
                if (Configuration == null)
                {
                    InitCfg();
                }
                var settings = new ExchangeSendSettings();
                if (Configuration != null)
                {
                    var settingsCfg = Configuration.GetSection("Exchange:Settings");
                    settingsCfg?.Bind(settings);
                }
                settings.Validate();
                //TODO:It will be nice to make it work with named convention
                //var worker = IoCHelper.Container.ResolveNamed<ISendWorker>(settings.WorkerTypeName);
                var worker = WorkerParameters != null && WorkerParameters.Any()
                    ? IoCHelper.Container.Resolve<ISendWorker>(WorkerParameters)
                    : IoCHelper.Container.Resolve<ISendWorker>();
                //TODO: Sender logic might be the same as for Importer/Processor. To be checked. Investigation required.
                var dataSource = SourceParameters!= null && SourceParameters.Any()
                    ? IoCHelper.Container.Resolve<ISenderDataSource>(SourceParameters)
                    : IoCHelper.Container.Resolve<ISenderDataSource>();

                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
                _task =
                    Task.Factory.StartNew(
                        () =>
                            Send(worker, settings),
                        _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                //Логируем
                LogEventManager.Logger.Error(ex);
                throw;
            }
            finally
            {
                LogEventManager.Logger.Info("End start ExportDataService");
            }

            return true;
        }

        public bool Stop(T hostControl)
        {
            try
            {
                LogEventManager.Logger.Info("Begin stop ExportDataService");

                _cancellationTokenSource.Cancel();
                _task.Wait(_cancellationToken);
            }
            catch (AggregateException aggrEx)
            {
                //Логируем
                LogEventManager.Logger.Error(aggrEx);

                if (!_task.IsCanceled)
                    throw;
            }
            catch (Exception ex)
            {
                //Логируем
                LogEventManager.Logger.Error(ex);
            }
            finally
            {
                LogEventManager.Logger.Info("End stop ExportDataService");
            }

            return true;
        }

        /// <summary>
        /// Sends the data
        /// </summary>
        /// <param name="worker">sender</param>
        /// <param name="settings">sender settings</param>
        private async void Send(ISendWorker worker, IExchangeSettings settings)
        {
            do
            {
                if (!settings.SilentCancel) _cancellationToken.ThrowIfCancellationRequested();

                worker.RefreshSendCollection(settings);
                var lastDateUpdated = DateTime.Now;
                if (!worker.IsSendQueueEmpty)
                {
                    var tasks = new List<Task>();
                    for (var i = 0; i < settings.CountThreads; i++)
                        tasks.Add(SendAction(worker));

                    while (tasks.Count > 0)
                    {
                        var finishedTask = await Task.WhenAny(tasks);
                        tasks.Remove(finishedTask);
                    }
                }
                else
                {
                    var timeOut = (int) (DateTime.Now - lastDateUpdated).TotalMilliseconds;
                    if (timeOut < settings.Timeout)
                        Thread.Sleep(settings.Timeout - timeOut);
                }
            } while (!settings.StopServiceAfterFirstRun);
        }

        /// <summary>
        /// Получает задачу отправки
        /// </summary>
        /// <param name="worker">Отправитель</param>
        /// <returns>Задача отправки</returns>
        private async Task SendAction(ISendWorker worker)
        {
            await Task.Run(() => worker.Send(), _cancellationToken);
        }
        public void ForceWait()
        {
            try
            {
                _task?.Wait(_cancellationToken);
            }
            catch (OperationCanceledException ce)
            {
            }
        }
    }
}
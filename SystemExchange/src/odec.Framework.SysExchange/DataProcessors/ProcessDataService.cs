using Autofac;
using Microsoft.Extensions.Configuration;
using odec.Framework.Infrastructure.Autofac;
using odec.Framework.Logging;
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
    public class ProcessDataService<T>
    {
        protected IEnumerable<TypedParameter> WorkerParameters = new List<TypedParameter>();
        protected IEnumerable<TypedParameter> SourceParameters = new List<TypedParameter>();
        public static IConfiguration Configuration { get; set; }
        private Task _task;
        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public ProcessDataService()
        {

        }

        public ProcessDataService(IConfiguration configuration)
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
                LogEventManager.Logger.Info("Begin start ProcessDataService");
                if (Configuration == null)
                {
                    InitCfg();
                }
                var settings = new ExchangeProcessSettings();
                if (Configuration != null)
                {
                    var settingsCfg = Configuration.GetSection("Exchange:Settings");
                    settingsCfg?.Bind(settings);
                }

                settings.Validate();
                //TODO:It will be nice to make it work with named convention
                //var worker = IoCHelper.Container.ResolveNamed<IProcessPackageWorker>(settings.WorkerTypeName);
                //var source = IoCHelper.Container.ResolveNamed<IProcessDataSource>(settings.DataSource);
                var worker = WorkerParameters != null && WorkerParameters.Any()
                    ? IoCHelper.Container.Resolve<IProcessPackageWorker>(WorkerParameters)
                    : IoCHelper.Container.Resolve<IProcessPackageWorker>();

                var source = SourceParameters != null && SourceParameters.Any()
                    ? IoCHelper.Container.Resolve<IProcessDataSource>(SourceParameters)
                    : IoCHelper.Container.Resolve<IProcessDataSource>();

                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
                _task = Task.Factory.StartNew(() => Process(worker, source, settings),
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
                LogEventManager.Logger.Info("End start ImportDataService");
            }

            return true;
        }

        public bool Stop(T hostControl)
        {
            try
            {
                LogEventManager.Logger.Info("Begin stop ImportDataService");

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
                LogEventManager.Logger.Info("End stop ImportDataService");
            }

            return true;
        }

        /// <summary>
        /// Отправляет данные
        /// </summary>
        /// <param name="worker">Отправитель</param>
        /// <param name="source"></param>
        /// <param name="settings"></param>
        private async void Process(IProcessPackageWorker worker, IProcessDataSource source, IExchangeSettings settings)
        {
            do
            {
                if (!settings.SilentCancel) _cancellationToken.ThrowIfCancellationRequested();

                worker.GetProcessPackage(source, settings);
                var lastDateUpdated = DateTime.Now;
                if (!worker.IsBufferEmpty)
                {
                    var tasks = new List<Task>();
                    for (var i = 0; i < settings.CountThreads; i++)
                        tasks.Add(ProcessAction(worker, source));

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
        private async Task ProcessAction(IProcessPackageWorker worker, IProcessDataSource source)
        {
            await Task.Run(() => worker.Process(source), _cancellationToken);
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

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
    public class ImportDataService<T>
    {
        protected IEnumerable<TypedParameter> WorkerParameters = new List<TypedParameter>();
        protected IEnumerable<TypedParameter> SourceParameters = new List<TypedParameter>();

        public ImportDataService()
        {

        }

        public ImportDataService(IConfiguration configuration)
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

        private Task _task;

        private CancellationTokenSource _cancellationTokenSource;
        private CancellationToken _cancellationToken;

        public static IConfiguration Configuration { get; set; }

        /// <summary>
        /// Initializes configuration
        /// </summary>
        protected void InitCfg()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("exchangeCfg.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
        }

        /// <summary>
        /// Starts Service
        /// </summary>
        /// <param name="hostControl"></param>
        /// <returns></returns>
        public virtual bool Start(T hostControl)
        {
            try
            {
                LogEventManager.Logger.Info("Begin start ImportDataService");
                if (Configuration == null)
                {
                    InitCfg();
                }
                var settings = new ExchangeReceiveSettings();
                if (Configuration != null)
                {
                    var settingsCfg = Configuration.GetSection("Exchange:Settings");
                    settingsCfg?.Bind(settings);
                }
                settings.Validate();
                //TODO:It will be nice to make it work with named convention
                //var worker = IoCHelper.Container.ResolveNamed<IPackageReceiveWorker>(settings.WorkerTypeName);
                //var source = IoCHelper.Container.ResolveNamed<IReceiveDataSource>(settings.DataSource);

                var worker = WorkerParameters != null && WorkerParameters.Any()
                    ? IoCHelper.Container.Resolve<IPackageReceiveWorker>(WorkerParameters)
                    : IoCHelper.Container.Resolve<IPackageReceiveWorker>();

                var source = SourceParameters != null && SourceParameters.Any()
                    ? IoCHelper.Container.Resolve<IReceiveDataSource>(SourceParameters)
                    : IoCHelper.Container.Resolve<IReceiveDataSource>();

                _cancellationTokenSource = new CancellationTokenSource();
                _cancellationToken = _cancellationTokenSource.Token;
                _task = Task.Factory.StartNew(() => Receive(worker, source, settings),
                    _cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
            catch (Exception ex)
            {
                LogEventManager.Logger.Error(ex);
                throw;
            }
            finally
            {
                LogEventManager.Logger.Info("End start ImportDataService");
            }

            return true;
        }

        public virtual bool Stop(T hostControl)
        {
            try
            {
                LogEventManager.Logger.Info("Begin stop ImportDataService");

                _cancellationTokenSource.Cancel();
                _task.Wait(_cancellationToken);
            }
            catch (AggregateException aggrEx)
            {
                LogEventManager.Logger.Error(aggrEx);

                if (!_task.IsCanceled)
                    throw;
            }
            catch (Exception ex)
            {
                LogEventManager.Logger.Error(ex);
            }
            finally
            {
                LogEventManager.Logger.Info("End stop ImportDataService");
            }

            return true;
        }

        /// <summary>
        /// Receives the data 
        /// </summary>
        /// <param name="worker">receiver</param>
        /// <param name="source">data source from where we receive the data</param>
        /// <param name="settings">settings</param>
        private async void Receive(IPackageReceiveWorker worker, IReceiveDataSource source, IExchangeSettings settings)
        {

            do
            {
                if (!settings.SilentCancel) _cancellationToken.ThrowIfCancellationRequested();
                // Gets receive package. It should fill in the Buffer with packages
                worker.GetReceivePackage(source, settings);
                var lastDateUpdated = DateTime.Now;
                if (!worker.IsBufferEmpty)
                {
                    var tasks = new List<Task>();
                    for (var i = 0; i < settings.CountThreads; i++)
                        tasks.Add(ReceiveAction(worker, source));

                    while (tasks.Count > 0)
                    {
                        var finishedTask = await Task.WhenAny(tasks);
                        tasks.Remove(finishedTask);
                    }
                }
                else
                {
                    var timeOut = (int)(DateTime.Now - lastDateUpdated).TotalMilliseconds;
                    if (timeOut < settings.Timeout)
                        Thread.Sleep(settings.Timeout - timeOut);
                }
            } while (!settings.StopServiceAfterFirstRun);

            
                
            
        }

        /// <summary>
        /// Starts receive task
        /// </summary>
        /// <param name="worker">Receiver/Importer</param>
        /// <returns>Receive/Import task</returns>
        private async Task ReceiveAction(IReceiveWorker worker, IReceiveDataSource source)
        {
            await Task.Run(() => worker.Receive(source), _cancellationToken);
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

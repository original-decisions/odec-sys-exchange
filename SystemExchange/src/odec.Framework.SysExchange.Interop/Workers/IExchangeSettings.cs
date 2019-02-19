using System.Collections.Generic;

namespace SystemExchange.Interop.Workers
{
    public interface IExchangeSettings
    {
        /// <summary>
        /// How much failures is acceptable for the particular package
        /// </summary>
        int RepeatLimit { get; }
        /// <summary>
        /// Unique code of the worker
        /// </summary>
        string Code { get; }
        /// <summary>
        /// Number of parallel threads
        /// </summary>
        int CountThreads { get; }
        /// <summary>
        /// Time
        /// </summary>
        int Timeout { get; }
        /// <summary>
        /// Maximum package size
        /// </summary>
        int PackageSize { get; }

        /// <summary>
        /// Flag which indicates if we should stop service after first run.
        /// Example case: if we use importer/exporter in another background process,
        /// which should shut down and not continue to run as win svc.
        /// </summary>
        bool StopServiceAfterFirstRun { get; }

        /// <summary>
        /// Validates the exchange settings.
        /// </summary>
        /// <returns>if settings are valid.</returns>
        bool Validate();

        /// <summary>
        /// Worker Typed parameters
        /// </summary>
        Dictionary<string,string> WorkerParams { get; }
        /// <summary>
        /// Data Source Typed Parameters
        /// </summary>
        Dictionary<string, string> SourceParams { get; set; }

        /// <summary>
        /// If in case of cancel exceptions should be thrown.
        /// </summary>
        bool SilentCancel { get; set; }
    }
}
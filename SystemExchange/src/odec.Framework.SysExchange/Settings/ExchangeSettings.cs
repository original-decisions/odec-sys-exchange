using System;
using System.Collections.Generic;
using SystemExchange.Interop.Workers;

namespace odec.Framework.SysExchange.Settings
{
    public abstract class ExchangeSettings : IExchangeSettings
    {
        #region Implementation of IExchangeSettings

        /// <inheritdoc />
        public int RepeatLimit { get; set; }

        /// <inheritdoc />
        public string Code { get; set; }

        /// <inheritdoc />
        public int CountThreads { get; set; }

        /// <inheritdoc />
        public int Timeout { get; set; }

        /// <inheritdoc />
        public int PackageSize { get; set; }

        /// <inheritdoc />
        public bool StopServiceAfterFirstRun { get; set; }

        /// <inheritdoc />
        public virtual bool Validate()
        {
            if (string.IsNullOrEmpty(Code))
                throw new InvalidOperationException("В файле конфигураций не задан уникальный код службы");

            if (CountThreads == 0)
                throw new InvalidOperationException("В файле конфигураций не задано количество потоков отправки");

            if (Timeout == 0)
                throw new InvalidOperationException("В файле конфигураций не задан таймаут повторной отправки");
            if (PackageSize == 0)
                throw new InvalidOperationException("Не указан размер пакета");
            return true;
        }

        /// <inheritdoc />
        public Dictionary<string, string> WorkerParams { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> SourceParams { get; set; }

        /// <inheritdoc />
        public bool SilentCancel { get; set; }

        #endregion
    }
}

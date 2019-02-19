using System;

namespace odec.Framework.SysExchange.Settings
{
    public class ExchangeProcessSettings : ExchangeSettings
    {
        /// <summary>
        /// Worker Type Name
        /// </summary>
        public string WorkerTypeName { get; set; }

        /// <summary>
        /// Data source from where we receive the data
        /// </summary>
        public string DataSource { get; set; }

        /// <inheritdoc />
        public override bool Validate()
        {
            if (string.IsNullOrEmpty(WorkerTypeName))
                throw new InvalidOperationException("В файле конфигураций не задан типа обработчика");
            if (string.IsNullOrEmpty(DataSource))
                throw new InvalidOperationException("В файле конфигураций не указан источник обработчика");
            return base.Validate();
        }
    }
}
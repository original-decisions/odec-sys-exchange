using System;

namespace odec.Framework.SysExchange.Settings
{
    public class ExchangeSendSettings : ExchangeSettings
    {
        /// <summary>
        /// Worker Type Name
        /// </summary>
        public string WorkerTypeName { get; set; }
        /// <inheritdoc />
        public override bool Validate()
        {
            if (string.IsNullOrEmpty(WorkerTypeName) )
                throw new InvalidOperationException("В файле конфигураций не задан типа отправителя");
            return base.Validate();
        }
    }
}
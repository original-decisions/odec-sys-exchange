using System.Collections.Concurrent;
using SystemExchange.Interop.Workers;

namespace odec.Framework.SysExchange.Workers
{
    public abstract class SendWorker<T> : ISendWorker where T : class
    {
        /// <summary>
        /// Send queue.
        /// </summary>
        protected readonly ConcurrentQueue<T> SendQueue;
        /// <summary>
        /// Default ctor
        /// </summary>
        protected SendWorker()
        {
            SendQueue = new ConcurrentQueue<T>();
        }


        #region Implementation of ISendWorker

        /// <inheritdoc />
        public bool IsSendQueueEmpty => SendQueue.IsEmpty;

        /// <inheritdoc />
        public abstract void Send();

        
        /// <inheritdoc />
        public abstract void RefreshSendCollection(IExchangeSettings settings);

        #endregion
    }
}
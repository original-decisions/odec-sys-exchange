namespace SystemExchange.Interop.Workers
{
    public interface ISendWorker
    {

        /// <summary>
        /// Flag if the send queue empty
        /// </summary>
        bool IsSendQueueEmpty { get; }

        /// <summary>
        /// Sends data
        /// </summary>
        void Send();
        /// <summary>
        /// Refreshes data queue for the send
        /// </summary>
        void RefreshSendCollection(IExchangeSettings settings);
    }
}
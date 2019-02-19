namespace SystemExchange.Interop.Workers
{
    /// <summary>
    /// Interface describes how a receive package worker should work.
    /// </summary>
    public interface IPackageReceiveWorker: IReceiveWorker
    {
        /// <summary>
        /// Property describes if the internal receive collection is empty.
        /// </summary>
        bool IsBufferEmpty { get; }
        /// <summary>
        /// Get Receive package based on the settings.
        /// </summary>
        /// <param name="settings"></param>
        void GetReceivePackage(IExchangeSettings settings);
        /// <summary>
        /// Get Receive package based on the receive data source and exchange settings
        /// </summary>
        /// <typeparam name="T">Type of the data source</typeparam>
        /// <param name="dataSource">receive data source</param>
        /// <param name="settings">exchange settings</param>
        void GetReceivePackage<T>(T dataSource, IExchangeSettings settings) where T : IReceiveDataSource;
    }
}
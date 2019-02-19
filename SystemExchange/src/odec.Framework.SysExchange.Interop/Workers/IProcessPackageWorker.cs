namespace SystemExchange.Interop.Workers
{
    /// <summary>
    /// Interface describes how a process package worker should work.
    /// </summary>
    public interface IProcessPackageWorker:IProcessWorker
    {
        /// <summary>
        /// Property describes if the internal process collection is empty.
        /// </summary>
        bool IsBufferEmpty { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <param name="settings"></param>
        void GetProcessPackage<T>(T dataSource, IExchangeSettings settings) where T : IProcessDataSource;
    }
}
namespace SystemExchange.Interop.Workers
{
    /// <summary>
    /// The interface that describes 
    /// </summary>
    public interface IProcessWorker
    {
        /// <summary>
        /// Indicates if the operation was completed
        /// </summary>
        bool IsOperationCompleted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        /// <returns></returns>
        bool Process<T>(T dataSource) where T : IProcessDataSource;
    }
}
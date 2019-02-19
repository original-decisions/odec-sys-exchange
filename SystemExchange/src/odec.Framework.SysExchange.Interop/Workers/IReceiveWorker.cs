namespace SystemExchange.Interop.Workers
{
    public interface IReceiveWorker
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsSourceDataImported { get; }
        
        bool Receive<T>(T dataSource) where T: IReceiveDataSource;
    }
}

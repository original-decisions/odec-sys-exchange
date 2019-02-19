namespace odec.Framework.SysExchange.Interop
{
    public class ServiceOptions
    {
        public ServiceOptions()
        {
            Name = "Default Service";
            Type = "ProcessService";
            Description = "Default Description";
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}
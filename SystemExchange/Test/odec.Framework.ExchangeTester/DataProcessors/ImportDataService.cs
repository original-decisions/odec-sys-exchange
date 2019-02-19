#if !NETCOREAPP1_0
using odec.Framework.SysExchange.DataProcessors;
using Topshelf;

namespace odec.Framework.Ex.Runner.Topshelf.DataProcessors
{
    public class ImportDataService : ImportDataService<HostControl>, ServiceControl
    {
        
    }
}
#endif
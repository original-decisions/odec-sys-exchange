#if NETCOREAPP2_0 || NETCOREAPP2_1

using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using odec.Framework.Infrastructure.ORM.EF;
using PMS.Service.Helpers;
using ConnectionType = odec.Framework.Infrastructure.ORM.EF.ConnectionType;
using EFOptions = odec.Framework.Infrastructure.ORM.EF.EFOptions;

namespace PMS.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
        {

            Configuration = configuration;

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            // Adding background task handler to the pipeline
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            
        }

        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();
            app.UseCors("default");

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}


#endif
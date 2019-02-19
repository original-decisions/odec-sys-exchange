
#if NETCOREAPP2_0 || NETCOREAPP2_1 
using Autofac;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using odec.Framework.Infrastructure.Autofac;
using odec.Framework.SysExchange.DataProcessors;
using PMS.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PMS.Service.Controllers
{

    /// <summary>
    /// Controller to manage user integrations
    /// </summary>
    [Route("api/[controller]/[action]")]
    public class IntegrationController : Controller
    {
        public IBackgroundTaskQueue Queue { get; }
        
        /// <summary>
        /// Constructor with db context.
        /// </summary>
        public IntegrationController(IBackgroundTaskQueue queue, ILogger<IntegrationController> logger)
        {
            Queue = queue;
            _logger = logger;
        }

        private readonly ILogger<IntegrationController> _logger;

        /// <summary>
        /// Starts synchronization for the part
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult StartSync()
        {
            try
            {
                async Task WorkItem(CancellationToken token)
                {
                    var guid = Guid.NewGuid().ToString();
                    var svc = new ImportDataService<Task>();
                    try
                    {
                        svc.PassWorkerParams(new List<TypedParameter> { new TypedParameter(typeof(int), 50) });
                        svc.PassSourceParams(new List<TypedParameter> { new TypedParameter(typeof(int), 500) });
                        svc.Start(null);
                        svc.ForceWait();
                        _logger.LogInformation($"Queued Background Task {guid} is complete. 3/3");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical($"Error in Redmine import service {nameof(svc)}. Query guid is: {guid}");
                        _logger.LogCritical(ex.Message, ex);
                    }


                    //for (int delayLoop = 0; delayLoop < 3; delayLoop++)
                    //{
                    //    _logger.LogInformation(
                    //        $"Queued Background Task {guid} is running. {delayLoop}/3");
                    //    await Task.Delay(TimeSpan.FromSeconds(5), token);
                    //}

                    //
                }

                Queue.QueueBackgroundWorkItem(WorkItem);


                //Repository.TryBeginSync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = ex.Message,
                    ex = ex,
                    displayText = "Unexpected error happened."
                });
            }
        }


    }
}


#endif
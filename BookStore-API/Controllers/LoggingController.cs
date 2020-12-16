using BookStore_API.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BookStore_API.Controllers
{
    public class LoggingController : ControllerBase
    {
        private readonly ILoggerService logger;

        protected LoggingController(ILoggerService logger)
        {
            this.logger = logger;
        }
        
        protected void LogInfo(string message)
        {
            var location = this.GetControllerActionNames();
            this.logger.LogInfo($"{location}: {message}");
        }
        
        protected void LogWarn(string message)
        {
            var location = this.GetControllerActionNames();
            this.logger.LogWarn($"{location}: {message}");
        }
        
        protected void LogError(string message)
        {
            var location = this.GetControllerActionNames();
            this.logger.LogError($"{location}: {message}");
        }

        protected ObjectResult InternalError(Exception e)
        {
            var location = this.GetControllerActionNames();
            this.logger.LogError($"{location}: {e}");
            
            return StatusCode(500, $"something went wrong. Please contact someone.");
        }
        
        protected ObjectResult InternalError(string message)
        {
            var location = this.GetControllerActionNames();
            this.logger.LogError($"{location}: {message}");
            
            return StatusCode(500, $"something went wrong. Please contact someone.");
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_Example.Filters.ResourceFilters
{
    public class FeatureDisableResourceFilter : IAsyncResourceFilter
    {

        private readonly ILogger<FeatureDisableResourceFilter> _logger;
        private readonly bool _isDisabled;

        public FeatureDisableResourceFilter(ILogger<FeatureDisableResourceFilter> logger, bool isDisabled = true)
        {
            _logger = logger;
            _isDisabled = isDisabled;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            //Before Logic
            _logger.LogInformation("{FilterName}.{MethodName} - before", nameof(FeatureDisableResourceFilter), nameof(OnResourceExecutionAsync));

            if (_isDisabled)
            {
                //context.Result = new NotFoundResult(); //404 - NOT FOUND
                context.Result = new StatusCodeResult(501); //501 - NOT IMPLEMENTED
            }
            else 
            {
                await next();
            }

            //After Logic
            _logger.LogInformation("{FilterName}.{MethodName} - after", nameof(FeatureDisableResourceFilter), nameof(OnResourceExecutionAsync));
        }
    }
}

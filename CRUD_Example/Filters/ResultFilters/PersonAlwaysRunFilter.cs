using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_Example.Filters.ResultFilters
{
    public class PersonAlwaysRunFilter : IAlwaysRunResultFilter
    {
        public void OnResultExecuted(ResultExecutedContext context)
        {
            
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            
        }
    }
}

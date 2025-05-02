using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class ApiKeyAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //exemplo inicial -> context.HttpContext.Request.Query.TryGetValue
            if (!context.HttpContext.Request.Headers.TryGetValue(Configuration.ApiKeyName, out var extractApiKeyName))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 401,
                    Content = "Api key não encontrada."
                };

                return;
            }

            if (!Configuration.ApiKeyValue.Equals(extractApiKeyName))
            {
                context.Result = new ContentResult()
                {
                    StatusCode = 403,
                    Content = "Acesso não autorizado."
                };

                return;
            }

            await next();
        }
    }
}

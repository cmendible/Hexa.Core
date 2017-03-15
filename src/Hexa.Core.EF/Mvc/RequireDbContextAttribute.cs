namespace Hexa.Core.Web.Mvc
{
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.EntityFrameworkCore;

    public class RequireDbContextAttribute : ActionFilterAttribute
    {
        private readonly DbContext context;

        public RequireDbContextAttribute(DbContext context)
        {
            this.context = context;
        }
        
        public override void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            try
            {
                this.context?.SaveChanges();
            }
            finally
            {
                this.context?.Dispose();
            }
        }
    }
}
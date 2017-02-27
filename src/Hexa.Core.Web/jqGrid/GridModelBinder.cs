namespace Hexa.Core.Web.Mvc.Helpers
{
    using System.Threading.Tasks;
    using Hexa.Core.Domain.Specification;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class GridModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {

            var request = bindingContext.HttpContext.Request;
            return Task.FromResult(new GridSettings
            {
                IsSearch = bool.Parse(request.Form["_search"].ToString() ?? "false"),
                PageIndex = int.Parse(request.Form["page"].ToString() ?? "1"),
                PageSize = int.Parse(request.Form["rows"].ToString() ?? "10"),
                SortColumn = request.Form["sidx"].ToString() ?? "",
                SortOrder = request.Form["sord"].ToString() ?? "asc",
                Where = Hexa.Core.Domain.Specification.Filter.Create(request.Form["filters"].ToString() ?? ""),
                Field = request.Form["searchField"].ToString() ?? "",
                SearchString = request.Form["searchString"].ToString() ?? "",
                Operator = request.Form["searchOper"].ToString() ?? ""
            });
        }

        [ModelBinder(BinderType = typeof(GridModelBinder))]
        public class GridSettings : SpecificationModel
        {
        }
    }
}
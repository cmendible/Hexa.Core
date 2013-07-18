namespace Hexa.Core.Web.Mvc.Helpers
{
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Web.Mvc;

    using Hexa.Core.Domain.Specification;

    public class GridModelBinder : IModelBinder
    {
        #region Methods

        public object BindModel(ControllerContext controllerContext,
                                ModelBindingContext bindingContext)
        {
            try
            {
                var request = controllerContext.HttpContext.Request;
                return new GridSettings
                {
                    IsSearch = bool.Parse(request["_search"] ?? "false"),
                    PageIndex = int.Parse(request["page"] ?? "1"),
                    PageSize = int.Parse(request["rows"] ?? "10"),
                    SortColumn = request["sidx"] ?? "",
                    SortOrder = request["sord"] ?? "asc",
                    Where = Hexa.Core.Domain.Specification.Filter.Create(request["filters"] ?? ""),
                    Field = request["searchField"] ?? "",
                    SearchString = request["searchString"] ?? "",
                    Operator = request["searchOper"] ?? ""
                };
            }
            catch
            {
                return null;
            }
        }

        #endregion Methods
    }

    [ModelBinder(typeof(GridModelBinder))]
    public class GridSettings : SpecificationModel
    {
    }
}
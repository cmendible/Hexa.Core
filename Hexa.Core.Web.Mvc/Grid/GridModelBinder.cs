namespace Hexa.Core.Web.Mvc.Helpers
{
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Web.Mvc;

    [DataContract]
    public class Filter
    {
        #region Properties

        [DataMember]
        public string groupOp
        {
            get;
            set;
        }

        [DataMember]
        public Rule[] rules
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public static Filter Create(string jsonData)
        {
            try
            {
                var serializer =
                    new DataContractJsonSerializer(typeof(Filter));
                System.IO.StringReader reader =
                    new System.IO.StringReader(jsonData);
                System.IO.MemoryStream ms =
                    new System.IO.MemoryStream(
                    Encoding.Default.GetBytes(jsonData));
                return serializer.ReadObject(ms) as Filter;
            }
            catch
            {
                return null;
            }
        }

        #endregion Methods
    }

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
                    Where = Filter.Create(request["filters"] ?? ""),
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
    public class GridSettings
    {
        #region Properties

        public string Field
        {
            get;
            set;
        }

        public bool IsSearch
        {
            get;
            set;
        }

        public string Operator
        {
            get;
            set;
        }

        public int PageIndex
        {
            get;
            set;
        }

        public int PageSize
        {
            get;
            set;
        }

        public string SearchString
        {
            get;
            set;
        }

        public string SortColumn
        {
            get;
            set;
        }

        public string SortOrder
        {
            get;
            set;
        }

        public Filter Where
        {
            get;
            set;
        }

        #endregion Properties
    }

    [DataContract]
    public class Rule
    {
        #region Properties

        [DataMember]
        public string data
        {
            get;
            set;
        }

        [DataMember]
        public string field
        {
            get;
            set;
        }

        [DataMember]
        public string op
        {
            get;
            set;
        }

        #endregion Properties
    }
}
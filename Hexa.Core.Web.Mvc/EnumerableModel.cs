namespace Hexa.Core.Web.Mvc.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class EnumerableModel<T>
    {
        public EnumerableModel(IEnumerable<T> list)
        {
            this.Items = list;
        }

        public IEnumerable<T> Items { get; set; }
    }
}

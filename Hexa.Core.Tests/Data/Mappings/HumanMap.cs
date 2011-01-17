using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Hexa.Core.Domain;
using Hexa.Core.Tests.Domain;

namespace Hexa.Core.Tests.Data
{
    public class HumanMap : RootEntityMap<Human>
    {
        public HumanMap()
        {
            Map(h => h.Name);
        }
    }
}

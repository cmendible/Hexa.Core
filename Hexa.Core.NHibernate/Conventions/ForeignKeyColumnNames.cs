using System;

using FluentNHibernate;
using FluentNHibernate.Conventions;

namespace Hexa.Core.Domain
{
	public class ForeignKeyColumnNames : ForeignKeyConvention
	{
		protected override string  GetKeyName(Member property, Type type)
		{
			// many-to-many, one-to-many, join
			if (property == null)
			{
				if (type.GetProperty("UniqueId") != null)
					return type.Name + "UniqueId"; 

				return type.Name + "Id";
			}

			// else -- many-to-one

			if (property.PropertyType.GetProperty("UniqueId") != null)
				return property.Name + "UniqueId"; 

			return property.Name + "Id";
		}
	}
}

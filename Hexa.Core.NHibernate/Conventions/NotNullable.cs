using System;
using System.ComponentModel.DataAnnotations;

using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Hexa.Core.Domain
{
	// TODO: Implement a "Check(LEN(str) > 0)" on NotNullNotEmpty.
	public class NotNullable : IPropertyConvention
	{
		#region IConvention<PropertyPart> Members
		public void Apply(IPropertyInstance target)
		{
            var att = Attribute.GetCustomAttribute(target.Property.MemberInfo, typeof(RequiredAttribute)) as RequiredAttribute;

			if (att != null)
				target.Not.Nullable();
		}

		#endregion
	}
}

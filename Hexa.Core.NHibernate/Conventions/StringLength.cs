using System;
using System.ComponentModel.DataAnnotations;

using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Hexa.Core.Domain
{
    public class StringLengthConvention : IPropertyConvention
	{
		#region IConvention<IProperty> Members
		public void Apply(IPropertyInstance target)
		{
			var att1 = Attribute.GetCustomAttribute(target.Property.MemberInfo, typeof(StringLengthAttribute)) as StringLengthAttribute;

			if (att1 != null)
				target.Length(att1.MaximumLength);
		}
		#endregion
	}
}

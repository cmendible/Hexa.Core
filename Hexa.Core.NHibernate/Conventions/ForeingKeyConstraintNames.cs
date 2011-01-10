using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Hexa.Core.Domain
{
	public class ForeingKeyConstraintNames : IReferenceConvention, IHasManyConvention
	{
		#region IReferenceConvention

		public void Apply(IManyToOneInstance instance)
		{
			var entity = instance.EntityType.Name;
			var member = instance.Property.Name;

			instance.ForeignKey(string.Format("FK_{0}_{1}", entity, member));
		}

		#endregion

		public void Apply(IOneToManyCollectionInstance instance)
		{
			string entity = instance.EntityType.Name;
			string member = instance.Member.Name;
			string child = instance.ChildType.Name;

			instance.Key.ForeignKey(string.Format("FK_{0}{1}_{2}", entity, member, child));
		}
	}
}

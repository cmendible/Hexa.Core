namespace Hexa.Core.Domain
{
    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.AcceptanceCriteria;
    using FluentNHibernate.Conventions.Inspections;
    using FluentNHibernate.Conventions.Instances;

    public class ManyToManyTableName : ManyToManyTableNameConvention
    {
        protected override string GetBiDirectionalTableName(
            IManyToManyCollectionInspector collection,
            IManyToManyCollectionInspector otherSide)
        {
            return Inflector.Underscore(collection.EntityType.Name + "_" + otherSide.EntityType.Name).ToUpper();
        }

        protected override string GetUniDirectionalTableName(IManyToManyCollectionInspector collection)
        {
            return Inflector.Underscore(collection.EntityType.Name + "_" + collection.ChildType.Name).ToUpper();
        }
    }

    public class TableNameConvention : IClassConvention
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table("`" + Inflector.Underscore(instance.EntityType.Name).ToUpper() + "´");
        }
    }
}
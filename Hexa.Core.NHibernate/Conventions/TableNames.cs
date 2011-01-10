using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.AcceptanceCriteria;

namespace Hexa.Core.Domain
{

    //public class PluralizeTableNames : IClassConvention
    //{
    //    private static EnglishInflector _inflector = new EnglishInflector();

    //    public void Apply(IClassInstance instance)
    //    {
    //        instance.Table(_inflector.Pluralize(instance.EntityType.Name));
    //    }
    //}

    public class TableNameConvention : IClassConvention, IClassConventionAcceptance
    {
        public void Apply(IClassInstance instance)
        {
            instance.Table("`" + Inflector.Underscore(instance.EntityType.Name).ToUpper() + "´");
        }

        public void Accept(FluentNHibernate.Conventions.AcceptanceCriteria.IAcceptanceCriteria<FluentNHibernate.Conventions.Inspections.IClassInspector> criteria)
        {
            criteria.Expect(x => x.TableName, Is.Not.Set);
        }
    }

    public class ManyToManyTableName : ManyToManyTableNameConvention
    {
        protected override string GetBiDirectionalTableName(IManyToManyCollectionInspector collection, IManyToManyCollectionInspector otherSide)
        {
            return Inflector.Underscore(collection.EntityType.Name + "_" + otherSide.EntityType.Name).ToUpper();
        }

        protected override string GetUniDirectionalTableName(IManyToManyCollectionInspector collection)
        {
            return Inflector.Underscore(collection.EntityType.Name + "_" + collection.ChildType.Name).ToUpper();
        }
    }

}

namespace Hexa.Core.Domain
{
    using FluentNHibernate.Conventions;
    using FluentNHibernate.Conventions.AcceptanceCriteria;
    using FluentNHibernate.Conventions.Inspections;
    using FluentNHibernate.Conventions.Instances;

    public class ManyToManyTableName : ManyToManyTableNameConvention
    {
        #region Methods

        protected override string GetBiDirectionalTableName(IManyToManyCollectionInspector collection,
            IManyToManyCollectionInspector otherSide)
        {
            return Inflector.Underscore(collection.EntityType.Name + "_" + otherSide.EntityType.Name).ToUpper();
        }

        protected override string GetUniDirectionalTableName(IManyToManyCollectionInspector collection)
        {
            return Inflector.Underscore(collection.EntityType.Name + "_" + collection.ChildType.Name).ToUpper();
        }

        #endregion Methods
    }

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
        #region Methods

        public void Accept(
            IAcceptanceCriteria<IClassInspector> criteria)
        {
            // Nothing
        }

        public void Apply(IClassInstance instance)
        {
            instance.Table("`" + Inflector.Underscore(instance.EntityType.Name).ToUpper() + "´");
        }

        #endregion Methods
    }
}
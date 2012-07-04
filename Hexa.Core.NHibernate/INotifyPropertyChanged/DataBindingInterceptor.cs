namespace Hexa.Core.Domain
{
    using System;

    using NHibernate;

    public class DataBindingInterceptor : EmptyInterceptor
    {
        #region Properties

        public ISessionFactory SessionFactory
        {
            set;
            get;
        }

        #endregion Properties

        #region Methods

        public override string GetEntityName(object entity)
        {
            var markerInterface = entity as DataBindingFactory.IMarkerInterface;
            if (markerInterface != null)
            {
                return markerInterface.TypeName;
            }
            return base.GetEntityName(entity);
        }

        public override object Instantiate(string clazz, EntityMode entityMode, object id)
        {
            if (entityMode == EntityMode.Poco)
            {
                Type type = Type.GetType(clazz);
                if (type != null)
                {
                    object instance = DataBindingFactory.Create(type);
                    SessionFactory.GetClassMetadata(clazz).SetIdentifier(instance, id, entityMode);
                    return instance;
                }
            }
            return base.Instantiate(clazz, entityMode, id);
        }

        #endregion Methods
    }
}
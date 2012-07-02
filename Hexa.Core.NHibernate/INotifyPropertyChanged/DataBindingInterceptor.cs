namespace Hexa.Core.Domain
{
    using System;
    using NHibernate;

    public class DataBindingInterceptor : EmptyInterceptor
    {
        public ISessionFactory SessionFactory { set; get; }

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

        public override string GetEntityName(object entity)
        {
            var markerInterface = entity as DataBindingFactory.IMarkerInterface;
            if (markerInterface != null)
                return markerInterface.TypeName;
            return base.GetEntityName(entity);
        }
    }
}
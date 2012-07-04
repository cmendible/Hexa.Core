namespace Hexa.Core.Domain
{
    using System;
    using System.ComponentModel;

    using Castle.DynamicProxy;

    public static class DataBindingFactory
    {
        #region Fields

        private static readonly ProxyGenerator _ProxyGenerator = new ProxyGenerator();

        #endregion Fields

        #region Nested Interfaces

        public interface IMarkerInterface
        {
            #region Properties

            string TypeName
            {
                get;
            }

            #endregion Properties
        }

        #endregion Nested Interfaces

        #region Methods

        public static T Create<T>()
        {
            return (T) Create(typeof(T));
        }

        public static object Create(Type type)
        {
            return _ProxyGenerator.CreateClassProxy(
                       type,
                       new[]
            {
                typeof(INotifyPropertyChanged),
                typeof(IMarkerInterface)
            },
            new NotifyPropertyChangedInterceptor(type.FullName));
        }

        #endregion Methods

        #region Nested Types

        public class NotifyPropertyChangedInterceptor : IInterceptor
        {
            #region Fields

            private readonly string typeName;

            private PropertyChangedEventHandler subscribers = delegate { };

            #endregion Fields

            #region Constructors

            public NotifyPropertyChangedInterceptor(string typeName)
            {
                this.typeName = typeName;
            }

            #endregion Constructors

            #region Methods

            public void Intercept(IInvocation invocation)
            {
                if (invocation.Method.DeclaringType == typeof(IMarkerInterface))
                {
                    invocation.ReturnValue = typeName;
                    return;
                }
                if (invocation.Method.DeclaringType == typeof(INotifyPropertyChanged))
                {
                    var propertyChangedEventHandler = (PropertyChangedEventHandler) invocation.Arguments[0];
                    if (invocation.Method.Name.StartsWith("add_"))
                    {
                        subscribers += propertyChangedEventHandler;
                    }
                    else
                    {
                        subscribers -= propertyChangedEventHandler;
                    }
                    return;
                }

                invocation.Proceed();

                if (invocation.Method.Name.StartsWith("set_"))
                {
                    string propertyName = invocation.Method.Name.Substring(4);
                    subscribers(invocation.InvocationTarget, new PropertyChangedEventArgs(propertyName));
                }
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}
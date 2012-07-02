using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Hexa.Core.ServiceModel
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ErrorBehaviorAttribute : Attribute, IServiceBehavior
    {
        Type errorHandlerType;

        public ErrorBehaviorAttribute(Type errorHandlerType)
        {
            this.errorHandlerType = errorHandlerType;
        }

        public Type ErrorHandlerType
        {
            get
                {
                    return this.errorHandlerType;
                }
        }

        void IServiceBehavior.Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        void IServiceBehavior.AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
                Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        void IServiceBehavior.ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler;

            try
                {
                    errorHandler = (IErrorHandler)Activator.CreateInstance(errorHandlerType);
                }
            catch (MissingMethodException e)
                {
                    throw new ArgumentException("The errorHandlerType specified in the ErrorBehaviorAttribute constructor must have a public empty constructor.", e);
                }
            catch (InvalidCastException e)
                {
                    throw new ArgumentException("The errorHandlerType specified in the ErrorBehaviorAttribute constructor must implement System.ServiceModel.Dispatcher.IErrorHandler.", e);
                }

            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
                {
                    ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                    channelDispatcher.ErrorHandlers.Add(errorHandler);
                }
        }
    }
}

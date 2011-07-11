using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using log4net;

namespace Hexa.Core.WebServices
{
	public class InstanceProvider : IInstanceProvider
	{
		private static readonly ILog _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IoCContainer Container { set; get; }
		protected Type ServiceType { set; get; }

        public InstanceProvider(IoCContainer container, Type type)
		{
			_Log.DebugFormat("New instance provider for '{0}'", type);

			ServiceType = type;
			Container = container;
		}

		#region IInstanceProvider Members

		public object GetInstance(InstanceContext instanceContext, Message message)
		{
			_Log.DebugFormat("Creating new '{0}' service instance..", ServiceType);

			return ServiceLocator.GetInstance(ServiceType);
		}

		public object GetInstance(InstanceContext instanceContext)
		{
			return GetInstance(instanceContext, null);
		}

		public void ReleaseInstance(InstanceContext instanceContext, object instance)
		{
			_Log.DebugFormat("Releasing '{0}' service instance..", ServiceType);
		}

		#endregion
	}

	public class InstantProviderServiceBehavior : IServiceBehavior
	{
        private IoCContainer Container;

        public InstantProviderServiceBehavior(IoCContainer container)
		{
			Container = container;
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
			{
				ChannelDispatcher cd = cdb as ChannelDispatcher;

				if (cd == null)
					continue;

				foreach (EndpointDispatcher ed in cd.Endpoints)
				{
					IInstanceProvider i = new InstanceProvider(Container, serviceDescription.ServiceType);
					ed.DispatchRuntime.InstanceProvider = i;
				}
			}

		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

		public void AddBindingParameters(
			ServiceDescription serviceDescription,
			ServiceHostBase serviceHostBase,
			Collection<ServiceEndpoint> endpoints,
			BindingParameterCollection bindingParameters)
		{
		}
	}
}
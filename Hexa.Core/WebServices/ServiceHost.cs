using System;

using log4net;

namespace Hexa.Core.WebServices
{
	/// <summary>
	/// ServiceLayer specific ServiceHost which has a reference to a ICoreContainer.
	/// </summary>
	public class ServiceHost : System.ServiceModel.ServiceHost
	{
		private static readonly ILog _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IoCContainer _Container = null;

        public ServiceHost(IoCContainer container, Type serviceType, params Uri[] baseAddresses)
			: base(serviceType, baseAddresses)
		{
			if (_Log.IsDebugEnabled)
				_Log.DebugFormat("New ServiceHost for '{0}'", serviceType);

			_Container = container;
		}

		protected override void OnOpening()
		{
			if (this.Description.Behaviors.Find<InstantProviderServiceBehavior>() == null)
				this.Description.Behaviors.Add(new InstantProviderServiceBehavior(_Container));

			base.OnOpening();
		}
	}
}
#region License

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion

using System;
using System.ServiceModel;

namespace Hexa.Core.ServiceModel
{

	/// <summary>
    /// Generic helper class for a WCF service proxy.
    /// </summary>
    /// <typeparam name="TProxy">The type of WCF service proxy to wrap.</typeparam>
    /// <typeparam name="TChannel">The type of WCF service interface to wrap.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
	public class ServiceProxyHelper<TProxy, TChannel>: IDisposable
        where TProxy : ClientBase<TChannel>, new()
        where TChannel : class
    {
        /// <summary>
        /// Private instance of the WCF service proxy.
        /// </summary>
        private TProxy _proxy;

        /// <summary>
        /// Gets the WCF service proxy wrapped by this instance.
        /// </summary>
        public TProxy Proxy
        {
            get
            {
                if (_proxy != null)
                    return _proxy;
                else
                    throw new ObjectDisposedException("ServiceProxyHelper");
            }
        }

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        public ServiceProxyHelper(TProxy proxy)
        {
			_proxy = proxy;
        }

        /// <summary>
        /// Disposes of this instance.
        /// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
		public void Dispose()
        {
            try
            {
                if (_proxy != null)
                {
                    if (_proxy.State != CommunicationState.Faulted)
                    {
                        _proxy.Close();
                    }
                    else
                    {
                        _proxy.Abort();
                    }
                }
            }
            catch (CommunicationException)
            {
                _proxy.Abort();
            }
            catch (TimeoutException)
            {
                _proxy.Abort();
            }
            catch (Exception)
            {
                _proxy.Abort();
                throw;
            }
            finally
            {
                _proxy = null;
            }
        }
    }

}
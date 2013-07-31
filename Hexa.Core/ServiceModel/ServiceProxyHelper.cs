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
namespace Hexa.Core.ServiceModel
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.ServiceModel;

    /// <summary>
    /// Generic helper class for a WCF service proxy.
    /// </summary>
    /// <typeparam name="TProxy">The type of WCF service proxy to wrap.</typeparam>
    /// <typeparam name="TChannel">The type of WCF service interface to wrap.</typeparam>
    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public class ServiceProxyHelper<TProxy, TChannel> : IDisposable
        where TProxy : ClientBase<TChannel>, new()
        where TChannel : class
    {
        /// <summary>
        /// Private instance of the WCF service proxy.
        /// </summary>
        private TProxy _proxy;

        /// <summary>
        /// Constructs an instance.
        /// </summary>
        public ServiceProxyHelper(TProxy proxy)
        {
            this._proxy = proxy;
        }

        /// <summary>
        /// Gets the WCF service proxy wrapped by this instance.
        /// </summary>
        public TProxy Proxy
        {
            get
            {
                if (this._proxy != null)
                {
                    return this._proxy;
                }
                else
                {
                    throw new ObjectDisposedException("ServiceProxyHelper");
                }
            }
        }

        /// <summary>
        /// Disposes of this instance.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly"),
         SuppressMessage("Microsoft.Usage", "CA1816:CallGCSuppressFinalizeCorrectly")]
        public void Dispose()
        {
            try
            {
                if (this._proxy != null)
                {
                    if (this._proxy.State != CommunicationState.Faulted)
                    {
                        this._proxy.Close();
                    }
                    else
                    {
                        this._proxy.Abort();
                    }
                }
            }
            catch (CommunicationException)
            {
                this._proxy.Abort();
            }
            catch (TimeoutException)
            {
                this._proxy.Abort();
            }
            catch (Exception)
            {
                this._proxy.Abort();
                throw;
            }
            finally
            {
                this._proxy = null;
            }
        }
    }
}
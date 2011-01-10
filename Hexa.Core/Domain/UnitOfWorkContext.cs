#region License

//===================================================================================
//Copyright 2010 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================

#endregion

using System.Web;
using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Remoting.Messaging;

namespace Hexa.Core.Domain
{
    public class UnitOfWorkContext
    {
        public static IUnitOfWork Current
        {
            get
            {
                if (RunningScopes.Count > 0)
                {
                    var unitOfWork = RunningScopes.Peek();
                    return unitOfWork;
                }
                else
                    return null;
            }
            private set
            {
                if (value == null)
                {
                    if (RunningScopes.Count > 0)
                        RunningScopes.Pop();
                }
                else
                    RunningScopes.Push(value);
            }
        }

        public static IUnitOfWork Start()
        {
            Current = ServiceLocator.GetInstance<IUnitOfWorkFactory>().Create();
            return Current;
        }

        public static void RemoveCurrent()
        {
            Current = null;
        }

        #region Nested

        /// <summary>
        /// Custom extension for OperationContext scope
        /// </summary>
        class ContainerExtension : IExtension<OperationContext>
        {
            #region Members

            public object Value { get; set; }

            #endregion

            #region IExtension<OperationContext> Members

            public void Attach(OperationContext owner)
            {

            }

            public void Detach(OperationContext owner)
            {

            }

            #endregion
        }

        #endregion

        private static string _key = "Hexa.Core.Domain.RunningContexts.Key";

        private static Stack<IUnitOfWork> RunningScopes
        {
            get
            {
                //Get object depending on  execution environment ( WCF without HttpContext,HttpContext or CallContext)
                if (OperationContext.Current != null)
                {
                    //WCF without HttpContext environment
                    var containerExtension = OperationContext.Current.Extensions.Find<ContainerExtension>();

                    if (containerExtension == null)
                    {
                        containerExtension = new ContainerExtension()
                        {
                            Value = new Stack<IUnitOfWork>()
                        };

                        OperationContext.Current.Extensions.Add(containerExtension);
                    }

                    return OperationContext.Current.Extensions.Find<ContainerExtension>().Value as Stack<IUnitOfWork>;
                }
                else if (HttpContext.Current != null)
                {
                    //HttpContext avaiable ( ASP.NET ..)
                    if (HttpContext.Current.Items[_key.ToString()] == null)
                        HttpContext.Current.Items[_key.ToString()] = new Stack<IUnitOfWork>();

                    return HttpContext.Current.Items[_key.ToString()] as Stack<IUnitOfWork>;
                }
                else
                {
                    if (CallContext.GetData(_key.ToString()) == null)
                        CallContext.SetData(_key.ToString(), new Stack<IUnitOfWork>());

                    //Not in WCF or ASP.NET Environment, UnitTesting, WinForms, WPF etc.
                    return CallContext.GetData(_key.ToString()) as Stack<IUnitOfWork>;
                }
            }
        }
    }

}

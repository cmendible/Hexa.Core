namespace Hexa.Core
{
    using System.Collections.Generic;
    using System.Runtime.Remoting.Messaging;
    using System.ServiceModel;
    using System.Web;

    /// <summary>
    /// Context State Helper
    /// </summary>
    public static class ContextState
    {
        /// <summary>
        /// Gets an item by the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the item to get</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>
        /// An item of type T
        /// </returns>
        public static T Get<T>(string key)
        {
            if (OperationContext.Current != null)
            {
                if (OperationContextState.ContainsKey(key))
                {
                    return (T)OperationContextState[key];
                }
                else
                {
                    return default(T);
                }
            }
            else if (HttpContext.Current != null)
            {
                return (T)HttpContext.Current.Items[key];
            }
            else
            {
                return (T)CallContext.GetData(key);
            }
        }

        /// <summary>
        /// Stores the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public static void Store(string key, object value)
        {
            if (OperationContext.Current != null)
            {
                OperationContextState[key] = value;
            }
            else if (HttpContext.Current != null)
            {
                HttpContext.Current.Items[key] = value;
            }
            else
            {
                CallContext.SetData(key, value);
            }
        }

        /// <summary>
        /// Gets the state of the operation context.
        /// </summary>
        /// <value>
        /// The state of the operation context.
        /// </value>
        private static IDictionary<string, object> OperationContextState
        {
            get
            {
                OperationContextExtension extension = OperationContext.Current.Extensions.Find<OperationContextExtension>();

                if (extension == null)
                {
                    extension = new OperationContextExtension();
                    OperationContext.Current.Extensions.Add(extension);
                }

                return extension.State;
            }
        }
    }
}
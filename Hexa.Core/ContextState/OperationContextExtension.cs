namespace Hexa.Core
{
    using System.Collections.Generic;
    using System.ServiceModel;

    /// <summary>
    /// OperationContext Extension
    /// </summary>
    public class OperationContextExtension : IExtension<OperationContext>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationContextExtension"/> class.
        /// </summary>
        public OperationContextExtension()
        {
            State = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>
        /// The state.
        /// </value>
        public IDictionary<string, object> State { get; private set; }

        /// <summary>
        /// Attaches the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Attach(OperationContext owner)
        {
        }

        /// <summary>
        /// Detaches the specified owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public void Detach(OperationContext owner)
        {
        }
    }
}
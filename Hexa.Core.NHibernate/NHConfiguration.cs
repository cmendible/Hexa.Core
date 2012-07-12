// -----------------------------------------------------------------------
// <copyright file="NHConfiguration.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Hexa.Core
{
    using System.ComponentModel.Composition;
    using NHibernate.Cfg;

    [Export(typeof(NHConfiguration))]
    public class NHConfiguration
    {
        static Configuration _configuration;

        public NHConfiguration()
        {
        }

        public NHConfiguration(Configuration configuration)
        {
            _configuration = configuration;
        }

        public Configuration Value { get { return _configuration; } }
    }
}

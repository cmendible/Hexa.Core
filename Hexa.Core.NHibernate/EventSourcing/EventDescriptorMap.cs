// -----------------------------------------------------------------------
// <copyright file="EventDescriptorMap.cs" company="I+3 Televisión">
//     Copyright (c) 2012. I+3 Televisión. All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using FluentNHibernate.Mapping;

    /// <summary>
    /// EventDescriptor Map
    /// </summary>
    public class EventDescriptorMap : ClassMap<EventDescriptor>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventDescriptorMap"/> class.
        /// </summary>
        public EventDescriptorMap()
        {
            this.Table("Events");

            this.CompositeId()
                .KeyProperty(c => c.Id)
                .KeyProperty(c => c.Version);

            this.Map(c => c.EventData)
                .CustomType<JsonType>()
                .Columns.Add("Type")
                .Columns.Add("Data");
        }
    }
}
//----------------------------------------------------------------------------------------------
// <copyright file="FixedDefaultFlushEventListener.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using NHibernate;
    using NHibernate.Event;
    using NHibernate.Event.Default;

    /// <summary>
    /// Fix for issue: https://hibernate.onjira.com/browse/HHH-2763
    /// http://stackoverflow.com/questions/3090733/an-nhibernate-audit-trail-that-doesnt-cause-collection-was-not-processed-by-fl
    /// </summary>
    /// <param name="session">The session.</param>
    public class FixedDefaultFlushEventListener : DefaultFlushEventListener
    {
        /// <summary>
        /// Fix for issue: https://hibernate.onjira.com/browse/HHH-2763
        /// http://stackoverflow.com/questions/3090733/an-nhibernate-audit-trail-that-doesnt-cause-collection-was-not-processed-by-fl
        /// </summary>
        /// <param name="session">The session.</param>
        protected override void PerformExecutions(IEventSource session)
        {
            try
            {
                session.ConnectionManager.FlushBeginning();
                session.PersistenceContext.Flushing = true;
                session.ActionQueue.PrepareActions();
                session.ActionQueue.ExecuteActions();
            }
            finally
            {
                session.PersistenceContext.Flushing = false;
                session.ConnectionManager.FlushEnding();
            }
        }
    }
}
//----------------------------------------------------------------------------------------------
// <copyright file="Log4NetLogger.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Logging
{
    using System;
    using System.ComponentModel.Composition;
    using System.Globalization;

    using log4net;

    [Export(typeof(ILogger))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class Log4NetLogger : ILogger
    {
        private readonly ILog log;

        public Log4NetLogger(Type type)
        {
            this.log = LogManager.GetLogger(type.FullName);
        }

        public Log4NetLogger(string typeName)
        {
            this.log = LogManager.GetLogger(typeName);
        }

        public void Debug(object message)
        {
            this.log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            this.log.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            this.log.DebugFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.log.DebugFormat(provider, format, args);
        }

        public void Error(object message)
        {
            this.log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            this.log.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            this.log.ErrorFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.log.ErrorFormat(provider, format, args);
        }

        public void Fatal(object message)
        {
            this.log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            this.log.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            this.log.FatalFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.log.FatalFormat(provider, format, args);
        }

        public void Info(object message)
        {
            this.log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            this.log.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            this.log.InfoFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.log.InfoFormat(provider, format, args);
        }

        public void Warn(object message)
        {
            this.log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            this.log.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            this.log.WarnFormat(CultureInfo.InvariantCulture, format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.log.WarnFormat(provider, format, args);
        }
    }
}
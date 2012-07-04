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

namespace Hexa.Core.Logging
{
    using System;
    using System.Globalization;
    using log4net;

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

        #region ILogger Members

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

        #endregion
    }
}
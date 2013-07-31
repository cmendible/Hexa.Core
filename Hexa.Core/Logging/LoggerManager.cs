//----------------------------------------------------------------------------------------------
// <copyright file="LoggerManager.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public static class LoggerManager
    {
        public static Func<Type, ILogger> GetLogger = type => new EmptyLogger();
    }
}
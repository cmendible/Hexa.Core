//----------------------------------------------------------------------------------------------
// <copyright file="ReflectionExtensions.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------

namespace Hexa.Core
{
    using System;
    using System.Reflection;

    public static class ReflectionExtensions
    {
        public static bool IsSubclassOfGeneric(this Type source, Type generic)
        {
            while (source != null && source != typeof(object))
            {
                Type cur = source.GetTypeInfo().IsGenericType ? source.GetGenericTypeDefinition() : source;
                if (generic == cur)
                {
                    return true;
                }

                source = source.GetTypeInfo().BaseType;
            }

            return false;
        }
    }
}
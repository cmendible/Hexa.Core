//----------------------------------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core
{
    using System;
    using System.Runtime.Serialization;

    public static class EnumExtensions
    {
        public static string GetEnumMemberValue(this Enum value)
        {
            var attributes
            = value.GetType().GetField(value.ToString())
              .GetCustomAttributes(typeof(EnumMemberAttribute), false)
              as EnumMemberAttribute[];

            return attributes.Length > 0 ? attributes[0].Value : string.Empty;
        }
    }
}